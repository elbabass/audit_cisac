import itertools
from datetime import timedelta, timezone, datetime
from pyspark.sql import SparkSession
from pyspark.sql.dataframe import DataFrame
from pyspark.sql.types import *
from pyspark.sql.functions import col, explode, to_json, from_json, when, lit, row_number
from pyspark.sql.window import Window
from generic_job.services.cosmos_service import CosmosService
from generic_job.services.delta_service import DeltaService
from generic_job.schemas.audit_request_schema import AuditRequestSchema
from generic_job.schemas.iswc_schema import IswcSchema
from generic_job.services.key_vault_configuration import KeyVaultConfiguration

class AuditRequestService:
    cosmos_database = "ISWC"
    cosmos_container = "AuditRequest"
    delta_database = "iswc"
    audit_delta_table = "auditrequest"
    iswc_delta_table = "iswc"

    def __init__(self, spark: SparkSession, config: KeyVaultConfiguration, interval_minutes: int):
        self.cosmos_connection_string = config.get_secret('AzureKeyVaultSecret-ISWC-ConnectionString-ISWCCosmosDb', True)
        self.delta_service = DeltaService(spark)
        self.cosmos_service = CosmosService(
            spark, self.cosmos_connection_string, self.cosmos_database, self.cosmos_container, AuditRequestSchema.audit_request_schema())
        self.spark = spark
        self.interval_minutes = interval_minutes

    def _convert_json_columns(self, df, schema: StructType):
        schema_dict = {field.name: field.dataType for field in schema.fields}

        for column_name, column_type in schema_dict.items():
            if isinstance(column_type, ArrayType) and isinstance(column_type.elementType, StructType):
                if dict(df.dtypes).get(column_name) == "string":
                    df = df.withColumn(column_name, from_json(col(column_name), column_type))

        return df

    def process_iswc_data_sql(self):
        iswc_data_df = (self.spark.read
                            .parquet('/mnt/reporting/IswcCreator/Batches/'))

        df_transformed = self._convert_json_columns(iswc_data_df, IswcSchema.iswc_schema())
        
        self.delta_service.write_delta_table(df_transformed, self.delta_database, self.iswc_delta_table, 'append', merge=True, schema=IswcSchema.iswc_schema())

    def get_audit_requests_change_feed(self, highwatermark: datetime):
        table_history_timestamp = (highwatermark - timedelta(minutes=self.interval_minutes)).strftime('%Y-%m-%dT%H:%M:%S') + "Z"
        cosmos_changes_df = self.cosmos_service.read_change_feed(table_history_timestamp)
        if (cosmos_changes_df.head() is None):
            print("No changes in cosmos since last execution")
            return (cosmos_changes_df, 0)
        
        total_cosmos_changes = cosmos_changes_df.count()
        print("Total changes read from cosmos: ", total_cosmos_changes)

        schema_fields = cosmos_changes_df.schema.fields
        df_select_list = []

        for field in schema_fields:
            field_name = field.name
            
            if isinstance(field.dataType, StructType):
                for subfield in field.dataType.fields:
                    subfield_name = subfield.name
                    full_field_name = f"{field_name}.{subfield_name}"
                    df_select_list.append(
                        col(full_field_name).alias(full_field_name.replace(".", "_"))
                        if subfield_name in [f.name for f in field.dataType.fields]
                        else lit(None).cast(str(subfield.dataType)).alias(full_field_name.replace(".", "_"))
                    )

            elif isinstance(field.dataType, ArrayType):
                element_type = str(field.dataType.elementType)
                df_select_list.append(
                    col(field_name).alias(field_name)
                    if field_name in [f.name for f in schema_fields]
                    else lit(None).cast(f"array<{element_type}>").alias(field_name)
                )

            else:
                df_select_list.append(
                    col(field_name).alias(field_name)
                    if field_name in [f.name for f in schema_fields]
                    else lit(None).cast(str(field.dataType)).alias(field_name)
                )

        if "Work" in [f.name for f in schema_fields]:
            work_schema = cosmos_changes_df.schema["Work"].dataType

            df_select_list.append(
                col("Work.PreferredIswc").alias("PreferredIswc") if "PreferredIswc" in [f.name for f in work_schema.fields] 
                else lit(None).cast("string").alias("PreferredIswc")
            )

            if "WorkNumber" in [f.name for f in work_schema.fields]:
                worknumber_schema = work_schema["WorkNumber"].dataType
                df_select_list.append(
                    col("Work.WorkNumber.Number").alias("AgencyWorkCode") if "WorkNumber" in [f.name for f in work_schema.fields] and "Number" in [f.name for f in worknumber_schema.fields] 
                    else lit(None).cast("string").alias("AgencyWorkCode")
                )

        if "TransactionError" in [f.name for f in schema_fields]:
            transaction_error_schema = cosmos_changes_df.schema["TransactionError"].dataType

            df_select_list.append(
                col("TransactionError.Code").alias("Code") if "Code" in [f.name for f in transaction_error_schema.fields] 
                else lit(None).cast("string").alias("Code")
            )

            df_select_list.append(
                col("TransactionError.Message").alias("Message") if "Message" in [f.name for f in transaction_error_schema.fields] 
                else lit(None).cast("string").alias("Message")
            )

        cosmos_changes_df_flattened = cosmos_changes_df.select(*df_select_list).dropDuplicates(["id"])

        return (cosmos_changes_df_flattened, total_cosmos_changes) 
    
    def update_audit_request_with_cosmos_changes(self, cosmos_changes_df: DataFrame, total_cosmos_changes: int):
        self.delta_service.merge_delta(cosmos_changes_df, self.delta_database, self.audit_delta_table)
        return (cosmos_changes_df, total_cosmos_changes) 

    def update_iswc_with_cosmos_changes(self, cosmos_changes_df: DataFrame):
        if (cosmos_changes_df.head() is None):
            print("No new changes detected in Cosmos since the last execution. No updates were made to the Iswc delta table")
            return 0 
        
        cosmos_changes_df = cosmos_changes_df.filter(
            (col("IsProcessingFinished") == True) & (col("IsProcessingError") == False)
        )

        if (cosmos_changes_df.head() is None):
            print("No valid changes detected in Cosmos since the last execution. No updates were made to the Iswc delta table")
            return 0
        
        window_spec = Window.partitionBy("AgencyWorkCode", "AgencyCode", "PreferredIswc").orderBy(col("CreatedDate").desc_nulls_last())

        ordered_audit_df = cosmos_changes_df.withColumn("update_rank", row_number().over(window_spec))

        latest_audit_updates = (
            ordered_audit_df
            .filter((col("update_rank") == 1))
            .withColumn("Status", when(col("TransactionType") == "CDR", False).otherwise(True))
            .drop("update_rank")
        )

        latest_audit_updates = latest_audit_updates.select(
            col("WorkIdAfter").alias("WorkId"),
            col("PreferredIswc").alias("PreferredIswc"),
            col("IswcStatus"),
            col("AgencyCode"),
            col("Work_SourceDb").alias("SourceDb"),
            col("AgencyWorkCode").alias("AgencyWorkCode"),
            col("CreatedDate").alias("CreatedDate"),  
            col("CreatedDate").alias("LastModifiedDate"),
            col("Work_CisnetCreatedDate").alias("CisnetCreatedDate"),
            col("Work_CisnetLastModifiedDate").alias("CisnetLastModifiedDate"),
            col("Work_BVLTR").alias("BVLTR"),
            col("Status"),
            col("Work_IsReplaced").alias("IsReplaced"),
            col("IsEligible"),
            col("Work_Titles").alias("Titles"),
            col("Work_InterestedParties").alias("InterestedParties"),
            col("Work_Performers").alias("Performers"),
            col("Work_Disambiguation").alias("Disambiguation"),
            col("Work_DisambiguationReason").alias("DisambiguationReason"),
            col("Work_DisambiguateFrom").alias("DisambiguateFrom"),
            col("Work_DerivedWorkType").alias("DerivedWorkType"),
            col("Work_DerivedFrom").alias("DerivedFrom"),
            col("Work_AdditionalIdentifiers").alias("AdditionalIdentifiers")
        )

        total_iswc_changes = latest_audit_updates.count()
        print(f"Processing {total_iswc_changes} latest changes for Iswc merge")

        latest_audit_updates.createOrReplaceTempView("audit_updates")

        self.spark.sql("""
            MERGE INTO iswc.iswc AS iswc
            USING audit_updates AS audit
            ON iswc.AgencyWorkCode = audit.AgencyWorkCode
            AND iswc.AgencyCode = audit.AgencyCode
            AND iswc.PreferredIswc = audit.PreferredIswc
            WHEN MATCHED THEN
                UPDATE SET
                    iswc.PreferredIswc = audit.PreferredIswc,
                    iswc.IswcStatus = audit.IswcStatus,
                    iswc.AgencyCode = audit.AgencyCode,
                    iswc.SourceDb = audit.SourceDb,
                    iswc.LastModifiedDate = audit.LastModifiedDate,
                    iswc.CisnetCreatedDate = audit.CisnetCreatedDate,
                    iswc.CisnetLastModifiedDate = audit.CisnetLastModifiedDate,
                    iswc.BVLTR = audit.BVLTR,
                    iswc.Status = audit.Status,
                    iswc.IsReplaced = audit.IsReplaced,
                    iswc.IsEligible = audit.IsEligible,
                    iswc.Titles = audit.Titles,
                    iswc.InterestedParties = audit.InterestedParties,
                    iswc.Performers = audit.Performers,
                    iswc.Disambiguation = audit.Disambiguation,
                    iswc.DisambiguationReason = audit.DisambiguationReason,
                    iswc.DisambiguateFrom = audit.DisambiguateFrom,
                    iswc.DerivedWorkType = audit.DerivedWorkType,
                    iswc.DerivedFrom = audit.DerivedFrom,
                    iswc.AdditionalIdentifiers = audit.AdditionalIdentifiers
            WHEN NOT MATCHED THEN
                INSERT (
                    WorkId, PreferredIswc, IswcStatus, AgencyCode, SourceDb, AgencyWorkCode, 
                    CreatedDate, LastModifiedDate, CisnetCreatedDate, CisnetLastModifiedDate, 
                    BVLTR, Status, IsReplaced, IsEligible, Titles, InterestedParties, Performers, 
                    Disambiguation, DisambiguationReason, DisambiguateFrom, DerivedWorkType, 
                    DerivedFrom, AdditionalIdentifiers
                )
                VALUES (
                    audit.WorkId, audit.PreferredIswc, audit.IswcStatus, audit.AgencyCode, 
                    audit.SourceDb, audit.AgencyWorkCode, audit.CreatedDate, audit.LastModifiedDate, 
                    audit.CisnetCreatedDate, audit.CisnetLastModifiedDate, audit.BVLTR, audit.Status, 
                    audit.IsReplaced, audit.IsEligible, audit.Titles, audit.InterestedParties, 
                    audit.Performers, audit.Disambiguation, audit.DisambiguationReason, 
                    audit.DisambiguateFrom, audit.DerivedWorkType, audit.DerivedFrom, 
                    audit.AdditionalIdentifiers
                )
        """)
        
        return total_iswc_changes
