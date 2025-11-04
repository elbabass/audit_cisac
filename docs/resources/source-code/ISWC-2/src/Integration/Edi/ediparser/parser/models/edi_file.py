from enum import Enum
from typing import Dict, Sequence
import datetime

from ediparser.parser.models.group import Group


class EdiFile:
    def __init__(self, file_name, header: Dict, groups: Sequence[Group]):
        self.file_name = file_name
        self.header = header
        self.groups = groups
        self.publisher_name_number = None
        self.agency_code = None

    def __get_name(self, name_format: str, submitting_agency: str, file_extension: str, type: str = None):
        current_datetime = datetime.datetime.utcnow()
        submitter_code = ''

        if type != None and not type.__contains__('CSN'):
            if 'Allocation' in self.file_name or 'Resolution' in self.file_name:
                submitter_code = self.file_name.split('/')[2]
                submitting_agency = (
                    submitting_agency or self.file_name.split('/')[0])
            elif type == 'ACK_JSON':
                submitting_agency = self.header.get('submittingAgency', '')

        if name_format == 'UPResults':
            return 'UPResults{}_315_{}_{}{}'.format(
                current_datetime.replace(
                    microsecond=0).isoformat().replace(':', '-'),
                submitter_code,
                submitting_agency,
                file_extension
            )

        if name_format == 'ISWCP':
            return 'ISWCP_{}_315_{}_{}{}'.format(
                current_datetime.replace(
                    microsecond=0).isoformat().replace(':', '-'),
                submitter_code,
                submitting_agency,
                file_extension
            )
        else:
            return 'ISWC{}{}315{}'.format(
                current_datetime.strftime('%Y%m%d%H%M%S'),
                submitting_agency,
                file_extension
            )

    def get_ack_file_name(self, submitting_agency: str, file_extension: str, type: str = None):
        if type == 'ACK_TSV':
            file_name_format = 'ISWCP'
        elif type == 'ACK_JSON' and 'iswcp' in self.file_name.lower():
            file_name_format = 'ISWCP'
        elif type == 'ACK_CSV':
            file_name_format = 'UPResults'    
        else:
            file_name_format = 'ISWC'
        return self.__get_name(file_name_format, submitting_agency, file_extension, type)

    def set_publisher_name_number(self, type: str = None, api_response=None):
        if type == 'ACK_JSON':
            if self.header.get('submittingPublisher') != None:
                self.publisher_name_number = self.header['submittingPublisher']['nameNumber']
        elif type == 'ACK_TSV':
            if api_response != None and api_response[0] != None:
                self.publisher_name_number = api_response[0].original_transaction[0].get(
                    'submittingPublishernameNumber')
                if self.publisher_name_number != '':
                    self.publisher_name_number = int(
                        self.publisher_name_number)

    def set_agency_code(self, type: str = None, api_response=None):
        self.agency_code = self.header.get('SenderID').lstrip('0').rjust(3, '0')

class EdiFileVersion(Enum):
    ONE = 1
    TWO = 2
    THREE = 3
