import requests

from ediparser.parser.models.input.input_transaction import InputTransaction
from ediparser.parser.models.output.output_row_definition import \
    OutputRowDefinition
from ediparser.parser.models.output.output_transaction import OutputTransaction


class CIP(OutputTransaction):
    def __init__(self, ip: dict):
        super().__init__(None, None)
        self.ip = ip
        self.fields = OutputRowDefinition({
            'RecordType': (1, 3),
            'TransactionSequence#': (4, 8),
            'RecordSequence#': (12, 8),
            'IPNameNumber': (20, 11),
            'IPBaseNumber': (31, 13),
            'IPRole': (44, 2)
        })

    def get_record(self):
        record = {
            'RecordType': self.__class__.__name__,
            'IPNameNumber': self.ip['nameNumber'] if 'nameNumber' in self.ip else ''.ljust(11),
            'IPBaseNumber': self.ip['baseNumber'] if 'baseNumber' in self.ip else ''.ljust(13),
            'IPRole': self.ip['role']
        }
        return self.serialize_record(record)

    def get_child_records(self):
        pass
