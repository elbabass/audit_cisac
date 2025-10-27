import requests

from ediparser.parser.models.input.input_transaction import InputTransaction
from ediparser.parser.models.output.output_row_definition import \
    OutputRowDefinition
from ediparser.parser.models.output.output_transaction import OutputTransaction


class DIS(OutputTransaction):
    def __init__(self, ds: dict):
        super().__init__(None, None)
        self.ds = ds
        self.fields = OutputRowDefinition({
            'RecordType': (1, 3),
            'TransactionSequence#': (4, 8),
            'RecordSequence#': (12, 8),
            'ISWC': (20, 11),
        })

    def get_record(self):
        record = {
            'RecordType': self.__class__.__name__,
            'ISWC': self.ds['iswc'],
        }
        return self.serialize_record(record)

    def get_child_records(self):
        pass
