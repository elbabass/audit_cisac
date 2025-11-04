import requests

from ediparser.parser.models.input.input_transaction import InputTransaction
from ediparser.parser.models.output.output_row_definition import \
    OutputRowDefinition
from ediparser.parser.models.output.output_transaction import OutputTransaction


class PER(OutputTransaction):
    def __init__(self, perfomer: dict):
        super().__init__(None, None)
        self.perfomer = perfomer
        self.fields = OutputRowDefinition({
            'RecordType': (1, 3),
            'TransactionSequence#': (4, 8),
            'RecordSequence#': (12, 8),
            'FirstName': (20, 50),
            'LastName': (70, 50)
        })

    def get_record(self):
        record = {
            'RecordType': self.__class__.__name__,
            'FirstName': self.perfomer.get('firstName', ''),
            'LastName': self.perfomer.get('lastName', '')
        }
        return self.serialize_record(record)

    def get_child_records(self):
        pass
