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
from ediparser.parser.services.key_vault_configuration import \
    KeyVaultConfiguration
from ediparser.parser.services.logger_service import LoggerService
from ediparser.parser.services.notification_service import NotificationService
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
partition_count = 8

cosmos_connection_string = config.get_secret(
    'AzureKeyVaultSecret-ISWC-ConnectionString-ISWCCosmosDb', True)

if config.get_secret('AzureKeyVaultSecret-ISWC-MaintenanceModeEnabled', exclude_prefix=True) == 'true':
    raise Exception('Maintenance mode is enabled.')

# COMMAND ----------

# DBTITLE 1,Run ediparser
logger = LoggerService(ai_connection_string, cosmos_connection_string)

try:
    sftp = SftpService(hostname, username, password)
    parser = Parser(api_base_address, api_subscription_key,
                    logger, spark, partition_count)
    notification_service = NotificationService(cosmos_connection_string, sftp, spark)

    for (agency_id, file_type, file_contents, file, csn_records) in notification_service.get_agencies():
        try:
            if file_contents:
                (csn_file_name, csn_file_contents, csn_file_count) = parser.run_csn(
                    file_type, file, file_contents)
                sftp.put_csn_file(agency_id, csn_file_name, csn_file_contents)
                logger.log_parsed_file(
                    file, csn_file_name, None, csn_file_count)

            notification_service.set_processed_notifications(csn_records)
        except:
            logger.log_exception(
                'Error: Creating CSN files for {}'.format(agency_id))
    sftp.close()

except:
    logger.log_exception()
    raise
