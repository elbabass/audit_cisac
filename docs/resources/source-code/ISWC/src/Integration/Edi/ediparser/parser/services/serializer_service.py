import json
from datetime import datetime
from typing import Dict, Sequence

from ediparser.parser.models.api_response import ApiResponse
from ediparser.parser.models.edi_file import EdiFile
from ediparser.parser.models.group import Group
from ediparser.parser.models.input.input_transaction import InputTransaction
from ediparser.parser.services.logger_service import LoggerService
from ediparser.parser.models.output.ack import ACK
from ediparser.parser.models.output.csn import CSN
from ediparser.parser.models.output.hdr import HDR
from ediparser.parser.models.output.cse import CSE


class SerializerService:
    OLD_FILE_EXTENSION = '.010'
    MODERN_FILE_EXTENSION = '.030'
    CSN_FILE_EXTENSION = '.csn'

    def __init__(self, file: EdiFile, api_responses: Dict[str, ApiResponse], response_file_type: str, logger: LoggerService):
        self.file = file
        self.api_responses = api_responses
        self.response_file_type = 'CSN' if response_file_type == 'CSN_CLASSIC' else response_file_type
        self.current_datetime = datetime.utcnow()
        self.creation_datetime = datetime.strptime(
            file.header['CreationDate'] + file.header['CreationTime'], '%Y%m%d%H%M%S')
        self.logger = logger

        ack_file_extension = self.OLD_FILE_EXTENSION if file.header[
            'StandardVersionNumber'][0:2] == '02' else self.MODERN_FILE_EXTENSION

        sender_id = file.header['SenderID'][-3:]
        if response_file_type.__contains__('CSN') or response_file_type.__contains__('CSE'):
            responses = [elem[0] for elem in self.api_responses.values()]
            csn_agency = responses[0].original_transaction[0]['ReceivingAgencyCode']
        else:
            csn_agency = None

        if sender_id == '300':
            self.ack_filename = "CSI{}{}{}315{}".format(
                self.file.header['CreationDate'],
                self.file.header['CreationTime'],
                sender_id,
                self.CSN_FILE_EXTENSION if
                response_file_type.__contains__('CSN') else ack_file_extension
            )
        else:
            if csn_agency != None or response_file_type.__contains__('CSE'):
                self.ack_filename = self.file.get_ack_file_name(
                    csn_agency, self.CSN_FILE_EXTENSION, 'CSN')
            else:
                self.ack_filename = self.file.get_ack_file_name(
                    sender_id, ack_file_extension, 'ACK')

    def serialize_file(self):
        records = []
        total_transactions = 0
        records.append(
            HDR(self.file.header, self.current_datetime).get_record())

        for group_id, responses in self.api_responses.items():
            acks = []
            for response in responses:
                if response.status_code == 207:
                    self.get_responses_from_array(group_id, response, acks)
                elif response.status_code >= 200 and response.status_code < 300:
                    acks.append(globals()[self.response_file_type](group_id, self.file.header, response.original_transaction[0],
                                                                   response.output, True, response.status_code, self.creation_datetime, self.current_datetime))
                elif response.status_code > 300:
                    if self.response_file_type.__contains__('CSN') and response.original_transaction[0].get('OriginalTransactionType') != 'CDR':
                        continue
                    error = {"statusCode": response.status_code,
                             "message": response.output}
                    response_ok = False if response.original_transaction[0].get('OriginalTransactionType') != 'CDR' else True
                    acks.append(globals()[self.response_file_type](group_id, self.file.header, response.original_transaction[0],
                                                                   error, response_ok, response.status_code, self.creation_datetime, self.current_datetime))
            total_transactions += len(acks)

            for ack in self.__serialize_group(group_id, acks):
                records.append(ack)

        records.append('TRL' +
                       self.__zero_fill(len(self.api_responses), 5) +
                       self.__zero_fill(total_transactions, 8) +
                       self.__zero_fill(len(records) + 1, 8))

        return (self.ack_filename, records, self.file)

    def __serialize_group(self, group_id: int, acks: Sequence[ACK]):
        records = []

        records.append('GRH' + self.response_file_type + group_id + '01.00')

        for idx, ack in enumerate(acks):
            for child in ack.get_record(idx):
                records.append(child)

        records.append('GRT' + group_id +
                       self.__zero_fill(len(acks), 8) +
                       self.__zero_fill(len(records) + 1, 8))

        return records

    def __zero_fill(self, record, length):
        return str(record).rjust(length, '0')

    def get_responses_from_array(self, group_id: int, api_response: ApiResponse, acks: list):
        for idx, item in enumerate(api_response.output):
            response_ok = 'rejection' not in item

            if response_ok and item.get('searchId') is not None and item.get('searchResults') is None:
                response_ok = False
                item['rejection'] = {'code': '404',
                                     'message': 'ISWC Not Found'}

            if not response_ok and self.response_file_type.__contains__('CSN'):
                continue

            if response_ok and api_response.original_transaction[idx]['RecordType'] in ['CAR', 'CUR']:
                response_ok = 'submission' in item and 'verifiedSubmission' in item['submission']

                if not response_ok:
                    item['rejection'] = {
                        'code': '100', 'message': 'Internal Server Error'}
                    self.logger.log_submission_details(
                        api_response.original_transaction[idx], self.file.file_name)

            acks.append(globals()[self.response_file_type](group_id, self.file.header, api_response.original_transaction[idx],
                                                           item, response_ok, api_response.status_code, self.creation_datetime, self.current_datetime))
