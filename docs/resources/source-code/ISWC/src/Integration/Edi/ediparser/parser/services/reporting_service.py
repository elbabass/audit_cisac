import calendar
from datetime import datetime, timedelta

from ediparser.parser.models.api_response import ApiResponse
from ediparser.parser.models.edi_file import EdiFileVersion
from pyspark.sql import SparkSession
from pyspark.sql.functions import col

from reporting.services.audit_reader_service import AuditReaderService
from ediparser.parser.services.logger_service import LoggerService
from ediparser.parser.services.sftp_service import SftpService


class ReportingService:
    container = '/mnt/submission-audit/'
    notification_type = 'CSE'

    days_of_the_week_dict = dict(zip(calendar.day_name, range(7)))

    def __init__(self, 
                 spark: SparkSession,
                 logger: LoggerService,
                 sftp: SftpService):
        self.spark = spark
        self.logger = logger
        self.sftp = sftp

    def get_agencies(self):
        for agency in self.spark.sql("select AgencyID, Frequency from iswc.agencynotificationtype").rdd.collect():
            if not self.sftp.customer_directory_exists(agency.AgencyID, False):
                self.logger.log_warning(f"Customer folder not found for agency {agency.AgencyID}. Skipping CSE generation.")
                continue

            if not self.job_should_run(agency.Frequency):
                continue

            (from_date, to_date) = self.__get_date_range(agency.Frequency)

            audit_reader = AuditReaderService(self.spark)
            data = audit_reader.get(from_date, to_date)

            data = data.filter(
                (col('AgencyCode') == agency.AgencyID) &
                (col('IsProcessingError') == True) &
                (col('Code') != 100)
            )

            cse_records = self.__get_file_contents(data)

            if not cse_records:
                continue

            yield (agency.AgencyID, self.notification_type, self.__get_file_header(), cse_records)

    def __get_file_header(self):
        now = datetime.now()
        hdr = {
            'RecordType': 'HDR',
            'SenderType': 'SO',
            'SenderID': '315'.rjust(9, '0'),
            'SenderName': 'CSI CENTER'.ljust(45),
            'EdiFileVersion': EdiFileVersion.TWO,
            'CharacterSet': '',
            'OriginalFile': '',
            'SenderIPNameNumber': '',
            'CreationDate': now.strftime('%Y%m%d'),
            'CreationTime': now.strftime('%H%M%S'),
            'StandardVersionNumber': '02.00',
            'SenderID': '315'.rjust(9, '0')
        }

        return hdr

    def __get_file_contents(self, df):
        api_responses = {}
        for row in df.collect():
            response_output = {
                'searchResults': [{
                    'works': [{
                        'agency': row.AgencyCode,
                        'workcode': row.AgencyWorkCode,
                        'sourcedb': row.SourceDb,
                        'originalTitle': row.OriginalTitle,
                        'iswc': row.PreferredIswc if row.PreferredIswc is not None else ''
                    }]
                }],
                'rejection': {
                    'code': row.Code,
                    'message': row.Message
                }
            }
            original_transaction = {
                'RecordType': row.TransactionType,
                'WorkflowTaskID': '',
                'WorkflowStatus': '',
                'OriginalTransactionType': row.TransactionType,
                'AgencyCode': row.AgencyCode,
                'TransactionSequence#': ''.rjust(8, '0'),
                'RecordSequence': ''.rjust(8, '0'),
                'ReceivingAgencyCode': row.AgencyCode
            }

            status_code = 207

            api_responses.setdefault('00001', []).append(
                ApiResponse([response_output], [original_transaction], status_code))

        return api_responses

    def __get_date_range(self, frequency):
        to_date = datetime.now().today()
        if frequency == 'Daily':
            from_date = to_date - timedelta(days=1)
        else: 
            from_date = to_date - timedelta(days=7)
        return (from_date, to_date)

    def job_should_run(self, frequency: str):
        weekday = datetime.today().weekday()
        if frequency == 'Daily':
            return True
        elif frequency == 'Weekly':
            return weekday == self.days_of_the_week_dict['Wednesday']
        elif frequency in self.days_of_the_week_dict:
            return weekday == self.days_of_the_week_dict[frequency]
        return False
