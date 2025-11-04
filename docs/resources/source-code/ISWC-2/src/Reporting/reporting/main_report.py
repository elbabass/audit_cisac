# Databricks notebook source
# DBTITLE 1,Install dependencies on cluster
# MAGIC %python
# MAGIC %pip install paramiko==2.11.0
# MAGIC %pip install opencensus-ext-azure==1.0.2
# MAGIC %pip install azure-cosmos==3.1.2
# MAGIC %pip install cryptography==2.8.0
# MAGIC %pip install pyrsistent==0.16.0
# MAGIC %pip install jsonschema==3.2.0
# MAGIC %pip install azure-mgmt-datafactory==0.13.0

# COMMAND ----------

# DBTITLE 1,Get configuration
from reporting.services.mail_service import MailService
from pyspark.sql import SparkSession
from datetime import datetime
from reporting.services.logger_service import LoggerService
from reporting.services.key_vault_configuration import KeyVaultConfiguration
from reporting.services.sftp_service import SftpService
from reporting.services.delta_service import DeltaService
from reporting.reports.submission_audit import SubmissionAudit
from reporting.reports.agency_interest_extract import AgencyInterestExtract
from reporting.reports.iswc_full_extract import IswcFullExtract
from reporting.reports.publisher_iswc_tracking import PublisherIswcTracking
from reporting.reports.potential_duplicates import PotentialDuplicates
from reporting.reports.agency_work_list import AgencyWorkList
from reporting.reports.creator_report import CreatorReport

spark = SparkSession.builder.getOrCreate()
setting = spark.conf.get("spark.master")
if "local" in setting:
    from pyspark.dbutils import DBUtils
    dbutils = DBUtils(spark)
    # dbutils.secrets.setToken('')

    # report_type = 'SubmissionAudit',c
    #               'AgencyInterestExtract',
    #               'IswcFullExtract',
    #               'PublisherIswcTracking',
    #               'PotentialDuplicates', 
    #               'AgencyWorkList',
    #               'CreatorReport'
    report_type = 'CreatorReport'

    #Status='Error' or 'Succeeded'
    submitting_agency_code = '128'
    agency_code = '128'
    agency_work_code = ''
    agreement_from_date = '01/01/1980 12:00:00 AM'
    agreement_to_date = '01/01/2021 12:00:00 AM'
    status = ''
    from_date = '01/20/2021 12:00:00 AM'
    to_date = '01/21/2021 12:00:00 AM'
    transaction_source = ''
    most_recent_version = 'True'
    email = 'test@test.com'
    consider_original_titles_only = 'True'
    potential_duplicates_create_extract_mode = 'True'
    creator_base_number = 'I-002985150-9'
    creator_name_number = '589238793'

else:
    dbutils = dbutils
    report_type = getArgument("report_type")
    submitting_agency_code = getArgument("submitting_agency_code")
    agency_code = getArgument("agency_code")
    agency_work_code = getArgument("agency_work_code")
    agreement_from_date = getArgument("agreement_from_date")
    agreement_to_date = getArgument("agreement_to_date")
    status = getArgument("status")
    from_date = getArgument("from_date")
    to_date = getArgument("to_date")
    transaction_source = getArgument("transaction_source")
    most_recent_version = getArgument("most_recent_version")
    email = getArgument("email")
    consider_original_titles_only = getArgument(
        "consider_original_titles_only")
    potential_duplicates_create_extract_mode = getArgument(
        "potential_duplicates_create_extract_mode")
    creator_base_number = getArgument("creator_base_number")
    creator_name_number = getArgument("creator_name_number")

config = KeyVaultConfiguration(dbutils)
ai_connection_string = config.get_secret(
    'ConnectionString-ApplicationInsights')
db_connection_string = config.get_secret(
    'AzureKeyVaultSecret-ISWC-ConnectionString-ISWCAzureSqlDatabase', True)
hostname = config.get_secret('SFTP-Hostname')
username = config.get_secret('SFTP-Username')
password = config.get_secret('SFTP-Password')
subscription_id = config.get_secret('SubscriptionID')
aad_client_id = config.get_secret('AAD-ClientID')
aad_secret = config.get_secret('AAD-Secret')
aad_tenant = config.get_secret('AAD-Tenant')
resource_group_name = config.get_secret('ResourceGroupName')
data_factory_name = config.get_secret('DataFactoryName')
mail_service_uri = config.get_secret('MailService-Uri')
mail_service_key = config.get_secret('MailService-Key')

partition_count = 8

if config.get_secret('AzureKeyVaultSecret-ISWC-MaintenanceModeEnabled', exclude_prefix=True) == 'true':
    raise Exception('Maintenance mode is enabled.')

date_format = '%m/%d/%Y %I:%M:%S %p'

# COMMAND ----------

# DBTITLE 1,Run report
logger = LoggerService(ai_connection_string)

try:
    sftp = SftpService(hostname, username, password)
    mail = MailService(mail_service_uri, mail_service_key)
    delta_service = DeltaService(spark)

    parameters = {
        'ReportType': report_type,
        'SubmittingAgencyCode': submitting_agency_code,
        'AgencyCode': agency_code,
        'AgencyWorkCode': agency_work_code,
        'Status': status,
        'FromDate': datetime.strptime(from_date, date_format) if from_date != '' else '',
        'ToDate': datetime.strptime(to_date, date_format) if to_date != '' else '',
        'AgreementFromDate': datetime.strptime(agreement_from_date, date_format) if agreement_from_date != '' else '',
        'AgreementToDate': datetime.strptime(agreement_to_date, date_format) if agreement_to_date != '' else '',
        'TransactionSource': transaction_source,
        'MostRecentVersion': most_recent_version,
        'Email': email,
        'ConsiderOriginalTitlesOnly': consider_original_titles_only,
        'PotentialDuplicatesCreateExtractMode': potential_duplicates_create_extract_mode,
        'CreatorBaseNumber': creator_base_number,
        'CreatorNameNumber': creator_name_number
    }

    ReportingClassName = {
        'SubmissionAudit': SubmissionAudit,
        'AgencyInterestExtract': AgencyInterestExtract,
        'IswcFullExtract': IswcFullExtract,
        'PublisherIswcTracking': PublisherIswcTracking,
        'PotentialDuplicates': PotentialDuplicates,
        'AgencyWorkList' : AgencyWorkList,
        'CreatorReport': CreatorReport
    }.get(report_type)

    dependencies = {
        'connection_string': db_connection_string,
        'spark': spark,
        'dbutils': dbutils,
        'subscription_id': subscription_id,
        'aad_client_id': aad_client_id,
        'aad_secret': aad_secret,
        'aad_tenant': aad_tenant,
        'resource_group_name': resource_group_name,
        'data_factory_name': data_factory_name,
        'delta_service': delta_service
    }

    report = ReportingClassName(dependencies)

    (customer_code, file_name, extract_data) = report.execute_report(parameters)

    potential_duplicates_create_extract_mode = parameters[
        'PotentialDuplicatesCreateExtractMode'] == 'True'
    
    if (report_type != 'IswcFullExtract' and report_type != 'AgencyWorkList') and not (report_type == 'PotentialDuplicates' and not potential_duplicates_create_extract_mode):
        sftp.put_report(customer_code, file_name, extract_data)
        mail.send(email, report_type)

    sftp.close()

except:
    logger.log_exception()
    raise
