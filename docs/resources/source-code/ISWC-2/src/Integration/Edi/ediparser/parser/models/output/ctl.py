import requests

from ediparser.parser.models.input.input_transaction import InputTransaction
from ediparser.parser.models.output.output_row_definition import \
    OutputRowDefinition
from ediparser.parser.models.output.output_transaction import OutputTransaction
from ediparser.parser.models.edi_file import EdiFileVersion


class CTL(OutputTransaction):
    def __init__(self, tl: dict, header: dict):
        super().__init__(None, None)
        self.tl = tl
        self.header = header
        self.fields = OutputRowDefinition({
            'RecordType': (1, 3),
            'TransactionSequence#': (4, 8),
            'RecordSequence#': (12, 8),
            'WorkTitle': (20, 60),
            'TitleType': (80, 2)
        } if header['EdiFileVersion'] == EdiFileVersion.TWO else {
            'RecordType': (1, 3),
            'TransactionSequence#': (4, 8),
            'RecordSequence#': (12, 8),
            'WorkTitle': (20, 100),
            'TitleType': (120, 2)
        })

    def get_record(self):
        record = {
            'RecordType': self.__class__.__name__,
            'WorkTitle': self.tl['title'][0:60],
            'TitleType': self.tl['type']
        }
        return self.serialize_record(record)

    def get_child_records(self):
        pass
