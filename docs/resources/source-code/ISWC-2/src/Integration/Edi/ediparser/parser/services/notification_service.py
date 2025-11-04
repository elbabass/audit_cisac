import calendar
import itertools
import json
from datetime import datetime
from distutils import util

import pandas as pd
from pyspark.sql import SparkSession

from ediparser.parser.models.api_response import ApiResponse
from ediparser.parser.models.edi_file import EdiFile, EdiFileVersion
from ediparser.parser.models.group import Group
from ediparser.parser.models.output.json.hdr import HDR as HDRJson
from ediparser.parser.services.cosmos_service import CosmosService
from ediparser.parser.services.sftp_service import SftpService


class NotificationService:
    database = "ISWC"
    container = "CsnNotifications"

    days_of_the_week_dict = dict(zip(calendar.day_name, range(7)))

    def __init__(self, cosmos_connection_string: str, sftp: SftpService, spark: SparkSession):
        self.cosmos_service = CosmosService(
            cosmos_connection_string, self.database, self.container)
        self.sftp = sftp
        self.spark = spark

    def get_agencies(self):
        for agency in self.spark.sql("select AgencyID, NotificationType, Frequency, IncludeOwnSubmissions from iswc.agencynotificationtype").rdd.collect():

            if not self.sftp.customer_directory_exists(agency.AgencyID, raise_exception=False):
                continue

            if not self.job_should_run(agency.Frequency):
                continue

            csn_records = self.peek(
                self.cosmos_service.get_csn_records_unprocessed(agency.AgencyID))

            if csn_records is None:
                continue
            else:
                file_type = 'CSN' if agency.NotificationType is None else agency.NotificationType

                include_own_submissions = bool(
                    util.strtobool(agency.IncludeOwnSubmissions))
                
                for records in self.chunks(csn_records, max_length=100_000_000):
                    (file_contents, file) = self.__get_file_contents(
                        file_type, records, agency.AgencyID, include_own_submissions)

                    yield (agency.AgencyID, file_type, file_contents, file, records)

    def peek(self, iterable):
        try:
            first = next(iterable)
        except StopIteration:
            return None
        return itertools.chain([first], iterable)

    def chunks(self, iterable, max_length):
        chunk = []
        current_length = 0

        for item in iterable:
            item_length = len(item['HttpResponse'])
            if current_length + item_length > max_length:
                yield chunk
                chunk = []
                current_length = 0
            chunk.append(item)
            current_length += item_length

        if chunk:
            yield chunk

    def __get_file_contents(self, file_type: str, csn_records: list, agency_id: str, includeOwnSubmissions: bool):
        if file_type.__contains__('JSON'):
            return self.__get_file_contents_json(csn_records, agency_id, includeOwnSubmissions), self.__get_file_header_json(agency_id, csn_records)
        else:
            return self.__get_file_contents_ack(csn_records, agency_id, includeOwnSubmissions, file_type), self.__get_file_header_ack(csn_records, file_type)

    def __get_file_header_ack(self, csn_records: list, file_type: str):
        now = datetime.now()
        hdr = {
            'RecordType': 'HDR',
            'SenderType': 'SO',
            'SenderID': '315'.rjust(9, '0'),
            'SenderName': 'CSI CENTER'.ljust(45),
            'EdiFileVersion': EdiFileVersion.TWO if file_type == 'CSN_CLASSIC' else EdiFileVersion.THREE,
            'CharacterSet': '',
            'OriginalFile': '',
            'SenderIPNameNumber': '',
            'CreationDate': now.strftime('%Y%m%d'),
            'CreationTime': now.strftime('%H%M%S'),
            'StandardVersionNumber': '02.00' if file_type == 'CSN_CLASSIC' else '03.00',
            'SenderID': '315'.rjust(9, '0')
        }
        groups = [Group('CMQ', '1', [pd.DataFrame(csn_records)])]

        return EdiFile(None, hdr, groups)

    def __get_file_contents_ack(self,  csn_records: list, agency_id: str, includeOwnSubmissions: bool, file_type: str):
        api_responses = {}
        for csn in csn_records:
            if not includeOwnSubmissions and csn['SubmittingAgencyCode'] == agency_id:
                continue

            wft_id = csn['WorkflowTaskID']
            wft_status = csn['WorkflowStatus']

            if wft_id == None or file_type == 'CSN_CLASSIC':
                wft_id = ''
                wft_status = ''

            response_output = {
                'searchResult': json.loads(csn['HttpResponse'])
            }
            original_transaction = {
                'RecordType': 'CMQ',
                'WorkflowTaskID': wft_id,
                'WorkflowStatus': wft_status,
                'OriginalTransactionType': csn['TransactionType'],
                'AgencyCode': csn['SubmittingAgencyCode'],
                'TransactionSequence#': ''.rjust(8, '0'),
                'RecordSequence': ''.rjust(8, '0'),
                'ReceivingAgencyCode': csn['ReceivingAgencyCode'],
                'ParentIswc': csn['ToIswc'] if file_type in ['CSN', 'CSN_CLASSIC'] and csn['TransactionType'] in ['MER', 'CDR'] else csn['FromIswc'] if file_type in ['CSN', 'CSN_CLASSIC'] and csn['TransactionType'] == 'DMR' else '',
                'ReceivingAgencyWorkCode': csn['ReceivingAgencyWorkCode']
            }

            status_code = 404 if csn['HttpResponse'] == 'null' else 200

            api_responses.setdefault('00001', []).append(
                ApiResponse(response_output, [original_transaction], status_code))

        return api_responses

    def __get_file_header_json(self, agency_id: str, csn_records: list):
        groups = [Group('CMQ', '1', [pd.DataFrame(csn_records)])]
        return EdiFile(None, HDRJson(None, agency_id).get_record(), groups)

    def __get_file_contents_json(self, csn_records: list, agency_id: str, includeOwnSubmissions: bool):
        api_responses = {}
        submission_id = 1

        for csn in csn_records:
            if not includeOwnSubmissions and csn['SubmittingAgencyCode'] == agency_id:
                continue

            wft_id = csn['WorkflowTaskID']
            wft_status = csn['WorkflowStatus']

            try:
                wft_message = csn['WorkflowMessage']
            except:
                wft_message = ''

            response_output = {
                'searchResult': json.loads(csn['HttpResponse'])
            }
            original_transaction = {
                'OriginalSubmissionId': submission_id,
                'TransactionType': 'CMQ',
                'WorkflowTaskID': wft_id,
                'WorkflowStatus': wft_status,
                'OriginalTransactionType': csn['TransactionType'],
                'AgencyCode': csn['SubmittingAgencyCode'],
                'TransactionSequence#': ''.rjust(8, '0'),
                'RecordSequence': ''.rjust(8, '0'),
                'ReceivingAgencyCode': csn['ReceivingAgencyCode'],
                'ParentIswc': csn['ToIswc'] if csn['TransactionType'] in ['MER', 'CDR'] else csn['FromIswc'] if csn['TransactionType'] == 'DMR' else None,
                'WorkflowMessage': wft_message
            }
            submission_id += 1

            status_code = 404 if csn['HttpResponse'] == 'null' else 200

            api_responses.setdefault('00001', []).append(
                ApiResponse(response_output, [original_transaction], status_code))

        return api_responses

    def set_processed_notifications(self, csn_records: list):
        for record in csn_records:
            record['ProcessedOnDate'] = datetime.now().isoformat()
            self.cosmos_service.set_processed_items(record)

    def job_should_run(self, frequency: str):
        if frequency == 'Daily':
            return True
        elif frequency == 'Weekly' and datetime.today().weekday() != self.days_of_the_week_dict['Wednesday']:
            return False
        elif frequency == 'Weekly' and datetime.today().weekday() == self.days_of_the_week_dict['Wednesday']:
            return True
        elif datetime.today().weekday() == self.days_of_the_week_dict[frequency]:
            return True
        else:
            return False

    def update_records_with_null_HttpResponse(self):
        records = self.cosmos_service.get_records_with_null_HttpResponse()
        for idx, record in enumerate(records):
            record['HttpResponse'] = None
            record['ProcessedOnDate'] = None
            self.cosmos_service.set_processed_items(record)
