from reporting.reports.report_output import ReportOutput
from reporting.services.delta_service import DeltaService
from pyspark.sql import DataFrame
from pyspark.sql.functions import explode, col, expr, to_json, collect_list, struct
import json
from io import StringIO

class CreatorReport:
    def __init__(self, dependencies: dict):
        self.spark = dependencies['spark']
        self.report_output = ReportOutput()
        self.dbutils = dependencies['dbutils']
        self.delta_service = dependencies['delta_service']


    def create_extract(self, parameters: dict):
        pass

    def execute_report(self, parameters: dict):
        creator_base_number = parameters['CreatorBaseNumber'].strip()
        creator_name_number = parameters['CreatorNameNumber'].strip()

        audit_request_df = self.delta_service.get_table_as_df('iswc.iswc')
        
        (key, value) = self.__get_ip_number_type(creator_base_number, creator_name_number)
        audit_request_df_filtered = audit_request_df.filter(
            expr(f"EXISTS(InterestedParties, x -> x.{key} = '{value}')")
        )

        audit_request_df_grouped = audit_request_df_filtered.groupBy("PreferredIswc").agg(
            collect_list(
                struct(
                    "IswcStatus", "AgencyCode", "SourceDb", "AgencyWorkCode",
                    "CreatedDate", "LastModifiedDate", "CisnetCreatedDate", "CisnetLastModifiedDate",
                    "Category", "Status", "IsReplaced", "IsEligible",
                    "Titles", "InterestedParties", "Performers", "AdditionalIdentifiers"
                )
            ).alias("Works")
        )

        parameters['IpNumber'] = value
        file_name = self.report_output.get_output_file_name(parameters)
        return (parameters['SubmittingAgencyCode'], file_name, StringIO(self.to_json_string(audit_request_df_grouped)))

    def __get_ip_number_type(self, creator_base_number, creator_name_number):
        if creator_base_number:
            return ('IpBaseNumber', creator_base_number)
        elif creator_name_number:
            return ('IPNameNumber', creator_name_number)
    
    def to_json(self, df: DataFrame):
        df = df.withColumn("Titles", to_json(col("Titles"))) \
            .withColumn("InterestedParties", to_json(col("InterestedParties"))) \
            .withColumn("Performers", to_json(col("Performers"))) \
            .withColumn("AdditionalIdentifiers", to_json(col("AdditionalIdentifiers")))            

        pandas_df = df.toPandas()
        extract_data_json = pandas_df.to_json(orient='records')
        return extract_data_json
    
    def to_json_string(self, df: DataFrame):
        json_list = df.toJSON().collect()

        json_string = json.dumps([json.loads(record) for record in json_list], indent=4)

        return json_string