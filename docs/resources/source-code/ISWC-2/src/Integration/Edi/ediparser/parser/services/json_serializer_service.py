import json
from datetime import datetime
from typing import Dict, Sequence

from ediparser.parser.models.api_response import ApiResponse
from ediparser.parser.services.logger_service import LoggerService

from ediparser.parser.models.output.json.hdr import HDR
from ediparser.parser.models.output.json.ack import ACK_JSON
from ediparser.parser.models.output.json.csn import CSN_JSON
from ediparser.parser.models.edi_file import EdiFile


class JsonSerializerService:
    FILE_EXTENSION = '.json'

    def __init__(self, file, api_responses: Dict[str, ApiResponse], response_file_type: str, logger: LoggerService):
        self.file = file
        self.api_responses = api_responses
        self.response_file_type = response_file_type
        self.current_datetime = datetime.utcnow()
        self.logger = logger

    def serialize_file(self):
        records = {}

        v2 = self.file.file_name != None and ('iswcp2' in self.file.file_name.lower() or 'iswc2' in self.file.file_name.lower())

        records['fileHeader'] = (HDR(self.file.header).get_record())
        records_section = []
        if self.response_file_type == 'ACK_JSON':
            records['acknowledgements'] = []
            records_section = records.get('acknowledgements')
        else:
            records['notifications'] = []
            records_section = records.get('notifications')

        for group_id, responses in self.api_responses.items():
            acks = []
            for response in responses:
                if response.status_code == 207:
                    self.get_responses_from_array(group_id, response, acks, v2)
                elif response.status_code >= 200 and response.status_code < 300:
                    acks.append(globals()[self.response_file_type](group_id, self.file.header, response.original_transaction[0],
                                                                   response.output, True, response.status_code, self.current_datetime, v2))
                elif response.status_code > 300:
                    if self.response_file_type.__contains__('CSN') and response.original_transaction[0]['OriginalTransactionType'] != 'CDR':  
                        continue

                    error = {"statusCode": response.status_code, 
                             "message": response.output}
    
                    response_ok = False

                    for original_transaction in response.original_transaction:
                        acks.append(globals()[self.response_file_type](group_id, self.file.header, original_transaction,
                                                                       error, response_ok, response.status_code, self.current_datetime))

            for ack in self.__serialize_group(group_id, acks):
                records_section.append(ack)

            responses = [elem[0] for elem in self.api_responses.values()]

            if self.response_file_type.__contains__('CSN'):
                submitting_agency = responses[0].original_transaction[0]['ReceivingAgencyCode']
                self.ack_filename = self.file.get_ack_file_name(
                    submitting_agency, self.FILE_EXTENSION, 'CSN_JSON')

            else:
                submitting_agency = self.file.header.get('submittingAgency')
                self.ack_filename = self.file.get_ack_file_name(
                    submitting_agency, self.FILE_EXTENSION, 'ACK_JSON')
                self.file.set_publisher_name_number('ACK_JSON', responses)

        return (self.ack_filename, records, self.file)

    def __serialize_group(self, group_id: int, acks: Sequence[ACK_JSON]):
        records = []

        for idx, ack in enumerate(acks):
            for child in ack.get_record(idx):
                records.append(child)

        return records

    def get_responses_from_array(self, group_id: int, api_response: ApiResponse, acks: list, v2: bool = False):
        for idx, item in enumerate(api_response.output):
            response_ok = 'rejection' not in item

            if response_ok and item.get('searchId') is not None and item.get('searchResult') is None and item.get('searchResults') is None:
                response_ok = False
                item['rejection'] = {'code': '404',
                                     'message': 'ISWC Not Found'}

            if not response_ok and self.response_file_type.__contains__('CSN'):
                continue

            if not self.response_file_type.__contains__('CSN') and response_ok and api_response.original_transaction[idx]['OriginalTransactionType'] in ['CAR', 'CUR', 'FSQ']:
                response_ok = 'submission' in item and 'verifiedSubmission' in item['submission']

                if not response_ok:
                    item['rejection'] = {
                        'code': '100', 'message': 'Internal Server Error'}
                    self.logger.log_submission_details(
                        api_response.original_transaction[idx], self.file.file_name)

            acks.append(globals()[self.response_file_type](group_id, self.file.header, api_response.original_transaction[idx],
                                                           item, response_ok, api_response.status_code, self.current_datetime, v2))
