# Databricks notebook source
# DBTITLE 1,Install dependencies on cluster
# MAGIC %python
# MAGIC %pip install paramiko==2.11.0
# MAGIC %pip install opencensus-ext-azure==1.0.2
# MAGIC %pip install azure-cosmos==3.1.2
# MAGIC %pip install cryptography==2.8.0
# MAGIC %pip install pyrsistent==0.16.0
# MAGIC %pip install jsonschema==3.2.0
# MAGIC %pip install python-magic==0.4.18
# MAGIC %pip install zeep==4.2.1
# MAGIC %pip install azure-mgmt-datafactory==0.13.0

# COMMAND ----------

# DBTITLE 1,Get configuration
from pyspark.sql import SparkSession
from pyspark.sql.functions import col, substring, trim, when

from generic_job.services.key_vault_configuration import KeyVaultConfiguration
from generic_job.services.logger_service import LoggerService
from generic_job.services.data_factory_service import DataFactoryService

spark = SparkSession.builder.getOrCreate()
setting = spark.conf.get("spark.master")
spark.conf.set("spark.databricks.delta.schema.autoMerge.enabled", "true")
if "local" in setting:
    from pyspark.dbutils import DBUtils
    dbutils = DBUtils(spark)
    # dbutils.secrets.setToken('')
    folder_name = '20250301'
else:
    dbutils = dbutils
    folder_name = getArgument('folder_name')

config = KeyVaultConfiguration(dbutils)
subscription_id = config.get_secret('SubscriptionID')
aad_client_id = config.get_secret('AAD-ClientID')
aad_secret = config.get_secret('AAD-Secret')
aad_tenant = config.get_secret('AAD-Tenant')
resource_group_name = config.get_secret('ResourceGroupName')
data_factory_name = config.get_secret('DataFactoryName')

missing_ipi_pipeline_name = 'Missing IPI Sync'
base_path = "dbfs:/mnt/ipi-integration/Missing IPI Sync/"
temp_output_path = base_path + "missing_ipi_output_temp"

if config.get_secret('AzureKeyVaultSecret-ISWC-MaintenanceModeEnabled', exclude_prefix=True) == 'true':
    raise Exception('Maintenance mode is enabled.')

# COMMAND ----------

# DBTITLE 1,Run audit job
logger = LoggerService()
data_factory_service = DataFactoryService(
            subscription_id, aad_client_id, aad_secret, aad_tenant, 
            resource_group_name, data_factory_name)

try:
    parameters = {
            'FolderName': folder_name
        }
    run_id = data_factory_service.new_pipeline_run(missing_ipi_pipeline_name, parameters)
    pipeline_status = data_factory_service.wait_for_pipeline_run(run_id)
    if pipeline_status != "Succeeded":
        raise Exception(f"Pipeline run {run_id} did not succeed. Status: {pipeline_status}")

    edi_df = spark.read.text(f"{base_path}IPI Unzip Files/*/*.EDI")

    ipa_df = edi_df.filter(col("value").startswith("IPA"))

    parsed_df = ipa_df.withColumn("IpBaseNumberRaw", trim(substring(col("value"), 57, 13)))

    valid_pattern = r"^I-\d{9}-\d$"

    valid_sftp_data_df = parsed_df.filter(col("IpBaseNumberRaw").rlike(valid_pattern)).select(col("IpBaseNumberRaw").alias("IpBaseNumber")).distinct()
    invalid_sftp_data_df = parsed_df.filter(~col("IpBaseNumberRaw").rlike(valid_pattern)).select(col("IpBaseNumberRaw").alias("IpBaseNumber")).distinct()

    valid_sftp_data_df.write.format("delta").mode("overwrite").saveAsTable("ipi.interestedpartysftp")

    invalid_sftp_data_df.write.mode("overwrite").format("delta").saveAsTable("ipi.invalidiprecordslog")
    
    invalid_sftp_data_count = invalid_sftp_data_df.count()
    if invalid_sftp_data_count > 0:
        logger.log_warning(f"Invalid IPI records found: {invalid_sftp_data_count} records. Check the 'ipi.invalidiprecordslog' delta table for more details.")

    iswc_ipi_data_df = (
        spark.read
        .option("header", True)
        .parquet(f"{base_path}IpiDataIswcDatabase.parquet")
        .withColumnRenamed("IpBaseNumber", "IpBaseNumber")
    )
    iswc_ipi_data_df.write.format("delta").mode("overwrite").saveAsTable("ipi.interestedpartydatabase")

    missing_ipi_df = valid_sftp_data_df.join(iswc_ipi_data_df, on="IpBaseNumber", how="left_anti")
    missing_ipi_df.write.format("delta").mode("overwrite").saveAsTable("ipi.missinginterestedparty")
    missing_ipi_df.coalesce(1).write.format("csv").option("header", "true").mode("overwrite").save(temp_output_path)
    files = dbutils.fs.ls(temp_output_path)
    part_files = [file for file in files if 'part-' in file.name]
    if len(part_files) == 1:
        part_file_path = part_files[0].path
        dbutils.fs.mv(part_file_path, f"{base_path}missing_ipi_output.csv")
        dbutils.fs.rm(temp_output_path, recurse=True)
    else:
        raise Exception(f"Unexpected number of part files found: {len(part_files)}. There should be exactly one part file.")

    missing_ipi_df_count = missing_ipi_df.count()
    if missing_ipi_df_count > 0:
        logger.log_warning(f"Missing Interested Parties detected: {missing_ipi_df_count} records. Check the 'ipi.missinginterestedparty' delta table for more details.")
    else:
        logger.log_info("No missing Interested Parties detected.")
except:
    logger.log_exception()
    raise
