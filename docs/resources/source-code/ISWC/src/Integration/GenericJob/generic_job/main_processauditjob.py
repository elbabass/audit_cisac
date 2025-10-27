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
from pyspark.sql import SparkSession

from generic_job.services.key_vault_configuration import KeyVaultConfiguration
from generic_job.services.logger_service import LoggerService
from generic_job.services.audit_request_service import AuditRequestService
from generic_job.services.high_watermark_service import HighWatermarkService
from generic_job.models.high_watermark_type import HighWatermarkType

spark = SparkSession.builder.getOrCreate()
setting = spark.conf.get("spark.master")
if "local" in setting:
    from pyspark.dbutils import DBUtils
    dbutils = DBUtils(spark)
    # dbutils.secrets.setToken('')
    load_iswc_data_sql_server = 'False'
    interval_minutes = 60
else:
    dbutils = dbutils
    load_iswc_data_sql_server = getArgument("load_iswc_data_sql_server") or "False"
    interval_minutes = int(getArgument('interval_minutes') or 60)

config = KeyVaultConfiguration(dbutils)

if config.get_secret('AzureKeyVaultSecret-ISWC-MaintenanceModeEnabled', exclude_prefix=True) == 'true':
    raise Exception('Maintenance mode is enabled.')

# COMMAND ----------

# DBTITLE 1,Run audit job
logger = LoggerService()
high_watermark_service = HighWatermarkService(spark)
audit_request_service = AuditRequestService(spark, config, interval_minutes)

try:
    if load_iswc_data_sql_server == 'True':
        audit_request_service.process_iswc_data_sql()

    else:
        hwm = high_watermark_service.get_high_watermark(HighWatermarkType.AUDITHIGHWATERMARK.value)
        
        (cosmos_changes_df, total_cosmos_changes) = audit_request_service.get_audit_requests_change_feed(hwm)
        audit_request_service.update_audit_request_with_cosmos_changes(cosmos_changes_df, total_cosmos_changes)

        total_iswc_changes = audit_request_service.update_iswc_with_cosmos_changes(cosmos_changes_df)

        new_hwm = high_watermark_service.set_high_watermark(HighWatermarkType.AUDITHIGHWATERMARK.value)

        logger.log_audit_job_details(total_cosmos_changes, total_iswc_changes, new_hwm)

except:
    logger.log_exception()
    raise
