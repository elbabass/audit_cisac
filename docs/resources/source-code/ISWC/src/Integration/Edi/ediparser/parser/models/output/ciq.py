from ediparser.parser.models.input.input_transaction import InputTransaction
from ediparser.parser.models.output.cwi import CWI
from ediparser.parser.models.output.output_row_definition import \
    OutputRowDefinition
from ediparser.parser.models.output.output_transaction import OutputTransaction


class CIQ(OutputTransaction):

    def __init__(self, api_response: dict, original_transaction: InputTransaction, header: dict):
        super().__init__(api_response, original_transaction)
        self.header = header
        self.iswc = self.get_field('searchResults')[0]

    def get_record(self):
        return {
            'WorkTitle': self.iswc.get('originalTitle'),
            'AgencyCode': self.iswc.get('agency').rjust(3, '0'),
            'SourceDBCode': self.iswc.get('agency').rjust(3, '0'),
            'PreferredISWC': self.iswc.get('iswc'),
        }

    def get_child_records(self):
        records = []
        for work in self.iswc.get('works'):
            cwi = CWI(work, self.header)
            records.append(cwi.get_record())
            for child in cwi.get_child_records():
                records.append(child)

        return records
