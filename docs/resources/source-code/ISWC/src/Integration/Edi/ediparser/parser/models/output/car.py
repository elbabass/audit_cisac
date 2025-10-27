import requests

from ediparser.parser.models.input.input_transaction import InputTransaction
from ediparser.parser.models.output.cip import CIP
from ediparser.parser.models.output.cwi import CWI
from ediparser.parser.models.output.output_row_definition import \
    OutputRowDefinition
from ediparser.parser.models.output.output_transaction import OutputTransaction


class CAR(OutputTransaction):
    def __init__(self, api_response: dict, original_transaction: InputTransaction, header: dict):
        super().__init__(api_response, original_transaction)
        self.work = self.get_field('submission').get('verifiedSubmission')
        self.header = header

    def get_record(self):
        return {
            'WorkTitle': self.work['originalTitle'],
            'AgencyCode': self.work['agency'].rjust(3, '0'),
            'AgencyWorkCode': self.work.get('workcode'),
            'SourceDBCode': str(self.work.get('sourcedb')).rjust(3, '0'),
            'PreferredISWC': self.work['iswc'],
        }

    def get_child_records(self):
        records = []

        cwi = CWI(self.work, self.header)
        records.append(cwi.get_record())
        for child in cwi.get_child_records():
            records.append(child)

        return records
