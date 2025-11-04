import csv
from datetime import datetime
from typing import Dict, Sequence

from ediparser.parser.models.api_response import ApiResponse
from ediparser.parser.models.edi_file import EdiFile
from ediparser.parser.models.group import Group
from ediparser.parser.models.input.input_transaction import InputTransaction
from ediparser.parser.services.logger_service import LoggerService
from ediparser.parser.models.output.csv.ack import ACK_CSV

class CsvSerializerService:
    FILE_EXTENSION = '.csv'

    def __init__(self, file: EdiFile, api_responses: Dict[str, ApiResponse], response_file_type: str, logger: LoggerService):
        self.file = file
        self.api_responses = api_responses
        self.response_file_type = response_file_type
        self.current_datetime = datetime.utcnow()
        self.creation_datetime = datetime.strptime(
            file.header['CreationDate'] + file.header['CreationTime'], '%Y%m%d%H%M%S')
        sender_id = file.header['SenderID'][-3:]
        self.ack_filename = self.file.get_ack_file_name(
                    sender_id, self.FILE_EXTENSION, self.response_file_type)
        self.logger = logger

    def serialize_file(self):
        records = []
        total_transactions = 0
        
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
                                                                   error, response_ok, response.status_code))
            total_transactions += len(acks)

            for ack in self.__serialize_group(group_id, acks):
                records.append(ack)

            self.file.set_agency_code('ACK_JSON', responses)

        return (self.ack_filename, records, self.file)

    def __serialize_group(self, group_id: int, acks: Sequence[ACK_CSV]):
        records = []

        for idx, ack in enumerate(acks):
            records.append((','.join(ack.get_record(idx).values())))

        return records

    def get_responses_from_array(self, group_id: int, api_response: ApiResponse, acks: list):
        for idx, item in enumerate(api_response.output):
            response_ok = 'rejection' not in item

            if response_ok and item.get('searchId') is not None and item.get('searchResults') is not None:
                acks.append(globals()[self.response_file_type](group_id, self.file.header, api_response.original_transaction[idx],
                                                           item, response_ok, api_response.status_code))
