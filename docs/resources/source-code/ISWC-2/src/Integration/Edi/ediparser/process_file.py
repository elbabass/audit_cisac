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
import datetime
import uuid

from pyspark.sql import SparkSession

from ediparser.parser.batch_parser import BatchParser
from ediparser.parser.services.key_vault_configuration import \
    KeyVaultConfiguration
from ediparser.parser.services.logger_service import LoggerService
from ediparser.parser.services.sftp_service import SftpService
from ediparser.parser.services.soap_service import SoapService

spark = SparkSession.builder.getOrCreate()
setting = spark.conf.get("spark.master")
if "local" in setting:
    from pyspark.dbutils import DBUtils
    dbutils = DBUtils(spark)
    dbutils.secrets.setToken('dkea1d3268751e9a007c7dfc5541d9013510')
    # file_type: ACK or ACK_JSON or 'ACK_TSV' or 'ACK_CSV'
    file_type = 'ACK_JSON'
    file_name = '312/In/iswcp2_nk_alloc_test_29082024.json'
else:
    dbutils = dbutils
    file_type = getArgument("file_type")
    file_name = getArgument("file_name")

config = KeyVaultConfiguration(dbutils)
ai_connection_string = config.get_secret(
    'ConnectionString-ApplicationInsights')
api_base_address = config.get_secret('BaseAddress-ApiManagement')
api_subscription_key = config.get_secret('Secret-ApiManagement')
hostname = config.get_secret('SFTP-Hostname')
username = config.get_secret('SFTP-Username')
password = config.get_secret('SFTP-Password')
soap_service_url = config.get_secret('SOAP-Service-Url')
soap_service_binding = config.get_secret('SOAP-Service-Binding')
soap_wsdl_url = config.get_secret('SOAP-Wsdl-Url')
soap_client_name = config.get_secret('SOAP-Client-Name')
soap_client_version = config.get_secret('SOAP-Client-Version')
soap_client_location_name = config.get_secret('SOAP-Client-Location-Name')
soap_client_api_key = config.get_secret('SOAP-Client-Api-Key')

partition_count = 32

cosmos_connection_string = config.get_secret(
    'AzureKeyVaultSecret-ISWC-ConnectionString-ISWCCosmosDb', True)

if config.get_secret('AzureKeyVaultSecret-ISWC-MaintenanceModeEnabled', exclude_prefix=True) == 'true':
    raise Exception('Maintenance mode is enabled.')

# COMMAND ----------

# DBTITLE 1,Run ediparser for a single file.
try:

    logger = LoggerService(ai_connection_string, cosmos_connection_string)
    sftp = SftpService(hostname, username, password)
    batchparser = BatchParser(api_base_address, api_subscription_key,
                              logger, spark, partition_count)
    soap = SoapService(logger, soap_service_url, soap_service_binding, soap_wsdl_url, soap_client_name, 
                       soap_client_version, soap_client_location_name, soap_client_api_key)

    try:
        start_date = datetime.datetime.utcnow()
        cosmos_id = uuid.uuid1()

        (file_name, file_type, file_contents) = sftp.get_file(file_name, file_type)

        logger.log_file_audit_cosmos(
            file_name, None, start_date, None, str(cosmos_id), None, 'In Progress')

        (ack_file_name, ack_file_contents, publisher_name_number, agency_code) = batchparser.run_batch(
            file_name, file_type, file_contents)

        if file_type == 'ACK_CSV':
            soap.send_file(ack_file_name, ack_file_contents, agency_code)

        sftp.archive_input_file(file_name)

        ack_file_name = sftp.put_ack_file(
             file_name, ack_file_name, ack_file_contents)

        end_date = datetime.datetime.utcnow()
        logger.log_file_audit_cosmos(
            file_name, ack_file_name, start_date, end_date, str(cosmos_id), publisher_name_number, 'Finished OK')
    except:
        end_date = datetime.datetime.utcnow()
        logger.log_file_audit_cosmos(
            file_name, None, start_date, None, str(cosmos_id), None, 'Finished Error')
        sftp.error_input_file(file_name)
        raise

    finally:
        sftp.close()

except:
    logger.log_exception()
    raise
