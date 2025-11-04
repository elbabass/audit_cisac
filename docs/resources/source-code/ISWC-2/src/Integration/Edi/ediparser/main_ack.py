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

# COMMAND ----------

# DBTITLE 1,Get configuration
from pyspark.sql import SparkSession

from ediparser.parser.parser import Parser
from ediparser.parser.services.job_service import JobService
from ediparser.parser.services.key_vault_configuration import \
    KeyVaultConfiguration
from ediparser.parser.services.logger_service import LoggerService
from ediparser.parser.services.sftp_service import SftpService

spark = SparkSession.builder.getOrCreate()
setting = spark.conf.get("spark.master")
if "local" in setting:
    from pyspark.dbutils import DBUtils
    dbutils = DBUtils(spark)
    # dbutils.secrets.setToken('')
else:
    dbutils = dbutils

config = KeyVaultConfiguration(dbutils)
ai_connection_string = config.get_secret(
    'ConnectionString-ApplicationInsights')
api_base_address = config.get_secret('BaseAddress-ApiManagement')
api_subscription_key = config.get_secret('Secret-ApiManagement')
hostname = config.get_secret('SFTP-Hostname')
username = config.get_secret('SFTP-Username')
password = config.get_secret('SFTP-Password')
databricks_job_id = config.get_secret('Databricks-JobID')
databricks_allocation_job_id = config.get_secret('Databricks-Allocation-JobID')
databricks_resolution_job_id = config.get_secret('Databricks-Resolution-JobID')
databricks_thirdparty_job_id = config.get_secret('Databricks-ThirdParty-JobID')
databricks_api_bearer_token = config.get_secret('Databricks-JobApiBearerToken')
partition_count = 8
cosmos_connection_string = config.get_secret(
    'AzureKeyVaultSecret-ISWC-ConnectionString-ISWCCosmosDb', True)

if config.get_secret('AzureKeyVaultSecret-ISWC-MaintenanceModeEnabled', exclude_prefix=True) == 'true':
    raise Exception('Maintenance mode is enabled.')

# COMMAND ----------

# DBTITLE 1,Get files and create ediparser job per file.
try:

    logger = LoggerService(ai_connection_string, cosmos_connection_string)
    sftp = SftpService(hostname, username, password)
    parser = Parser(api_base_address, api_subscription_key,
                    logger, spark, partition_count)

    jobs = JobService(databricks_job_id, databricks_api_bearer_token, logger)
    allocationJobs = JobService(databricks_allocation_job_id, databricks_api_bearer_token, logger)
    resolutionJobs = JobService(databricks_resolution_job_id, databricks_api_bearer_token, logger)
    thirdPartyJobs = JobService(databricks_thirdparty_job_id, databricks_api_bearer_token, logger)

    for (file_name, file_type) in sftp.get_list_of_files():
        try:
            if file_name.__contains__('/Allocation/'):
                allocationJobs.create_job(file_name, file_type)
            elif file_name.__contains__('/Resolution/'):
                resolutionJobs.create_job(file_name, file_type)
            elif len(file_name.split('/')[0]) > 3:
                thirdPartyJobs.create_job(file_name, file_type)
            else:
                jobs.create_job(file_name, file_type)
        except:
            logger.log_exception('Error:' + file_name)
            sftp.error_input_file(file_name)

    sftp.close()

except:
    logger.log_exception()
    raise
