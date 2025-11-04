import requests

from ediparser.parser.models.input.input_transaction import InputTransaction
from ediparser.parser.models.output.output_row_definition import \
    OutputRowDefinition
from ediparser.parser.models.output.output_transaction import OutputTransaction


class INS(OutputTransaction):
    def __init__(self, instrumentation: dict):
        super().__init__(None, None)
        self.instrumentation = instrumentation
        self.fields = OutputRowDefinition({
            'RecordType': (1, 3),
            'TransactionSequence#': (4, 8),
            'RecordSequence#': (12, 8),
            'Code': (20, 3)
        })

    def get_record(self):
        record = {
            'RecordType': self.__class__.__name__,
            'Code': self.instrumentation['code']
        }
        return self.serialize_record(record)

    def get_child_records(self):
        pass
