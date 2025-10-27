import json
from datetime import datetime
from typing import Dict, Sequence

from ediparser.parser.models.api_response import ApiResponse
from ediparser.parser.services.logger_service import LoggerService

from ediparser.parser.models.output.tsv.ack import ACK_TSV
from ediparser.parser.models.edi_file import EdiFile


class TsvSerializerService:
    FILE_EXTENSION = '.txt'

    def __init__(self, file, api_responses: Dict[str, ApiResponse], response_file_type: str, logger: LoggerService):
        self.file = file
        self.api_responses = api_responses
        self.response_file_type = response_file_type
        self.current_datetime = datetime.utcnow()
        self.logger = logger

    def serialize_file(self):
        records_section = []
        for group_id, responses in self.api_responses.items():
            acks = []
            for response in responses:
                if response.status_code == 207:
                    self.get_responses_from_array(group_id, response, acks)
                elif response.status_code >= 200 and response.status_code < 300:
                    acks.append(globals()[self.response_file_type](group_id, self.file.header, response.original_transaction[0],
                                                                   response.output, True, response.status_code, self.current_datetime))
                elif response.status_code > 300:
                    error = {"statusCode": response.status_code,
                             "message": response.output}

                    for original_transaction in response.original_transaction:
                        acks.append(globals()[self.response_file_type](group_id, self.file.header, original_transaction,
                                                                       error, False, response.status_code, self.current_datetime))

            for ack in self.__serialize_group(group_id, acks):
                records_section.append(ack)

            submitting_agency = responses[0].original_transaction[0].get(
                'submittingAgency')
            self.ack_filename = self.file.get_ack_file_name(
                submitting_agency, self.FILE_EXTENSION, 'ACK_TSV')
            self.file.set_publisher_name_number('ACK_TSV', responses)

            return (self.ack_filename, records_section, self.file)

    def __serialize_group(self, group_id: int, acks: Sequence[ACK_TSV]):
        records = []

        for idx, ack in enumerate(acks):
            try:
                for child in ack.get_record(idx):
                    records.append(child)
            except:
                self.logger.log_warning(
                    "Serialization failure: " + str(ack.original_transaction))
                raise

        return records

    def get_responses_from_array(self, group_id: int, api_response: ApiResponse, acks: list):
        for idx, item in enumerate(api_response.output):
            response_ok = 'rejection' not in item

            if response_ok and item.get('searchId') is not None and item.get('searchResults') is None:
                response_ok = False
                item['rejection'] = {'code': '404',
                                     'message': 'ISWC Not Found'}

            if response_ok and api_response.original_transaction[idx]['OriginalTransactionType'] in ['CAR', 'FSQ']:
                response_ok = 'submission' in item and 'verifiedSubmission' in item['submission']

                if not response_ok:
                    item['rejection'] = {
                        'code': '100', 'message': 'Internal Server Error'}
                    self.logger.log_submission_details(
                        api_response.original_transaction[idx], self.file.file_name)

            acks.append(globals()[self.response_file_type](group_id, self.file.header, api_response.original_transaction[idx],
                                                           item, response_ok, api_response.status_code, self.current_datetime))
