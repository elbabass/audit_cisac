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
from io import StringIO

from generic_job.services.key_vault_configuration import KeyVaultConfiguration
from generic_job.services.logger_service import LoggerService
from generic_job.services.high_watermark_service import HighWatermarkService
from generic_job.services.change_tracker_service import ChangeTrackerService
from generic_job.services.sftp_service import SftpService
from generic_job.models.high_watermark_type import HighWatermarkType
from reporting.services.audit_reader_service import AuditReaderService

spark = SparkSession.builder.getOrCreate()
setting = spark.conf.get("spark.master")
if "local" in setting:
    from pyspark.dbutils import DBUtils
    dbutils = DBUtils(spark)
    # dbutils.secrets.setToken('')
    submitting_party_id = 'All'
    high_watermark = ''
else:
    dbutils = dbutils
    submitting_party_id = getArgument('submitting_party_id') or 'All'
    high_watermark = getArgument('high_watermark')

config = KeyVaultConfiguration(dbutils)
hostname = config.get_secret('SFTP-Hostname')
username = config.get_secret('SFTP-Username')
password = config.get_secret('SFTP-Password')

if config.get_secret('AzureKeyVaultSecret-ISWC-MaintenanceModeEnabled', exclude_prefix=True) == 'true':
    raise Exception('Maintenance mode is enabled.')

# COMMAND ----------

# DBTITLE 1,Run audit job
logger = LoggerService()
audit_reader = AuditReaderService(spark)
high_watermark_service = HighWatermarkService(spark)
change_tracker_service = ChangeTrackerService(spark)
sftp = SftpService(hostname, username, password)

try:
    if not high_watermark:
        hwm = high_watermark_service.get_high_watermark(HighWatermarkType.CHANGETRACKERWATERMARK.value)
    else:
        hwm = high_watermark
        
    audit_data_df = audit_reader.get(start_date=hwm)
    
    (file_name, extract_data, iswc_changes) = change_tracker_service.create_extract(audit_data_df)

    for third_party in change_tracker_service.get_third_parties(submitting_party_id):
        try:
            sftp.put_file(third_party['SubmittingPartyId'], file_name, StringIO(extract_data))
        except Exception as e:
            logger.log_exception(
                'Error: Creating Recent Iswc Changes file for {}'.format(third_party['SubmittingPartyId'])
            )
            logger.log_exception(str(e))
    sftp.close()

    if not high_watermark:
        new_hwm = high_watermark_service.set_high_watermark(HighWatermarkType.CHANGETRACKERWATERMARK.value)
    
    logger.log_change_tracker_job_details(iswc_changes, new_hwm)

except:
    logger.log_exception()
    raise
