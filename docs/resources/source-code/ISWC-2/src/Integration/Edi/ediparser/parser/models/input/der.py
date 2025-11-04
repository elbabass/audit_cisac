from ediparser.parser.models.input.input_row_definition import InputRowDefinition
from ediparser.parser.models.input.input_transaction import InputTransaction


class DER(InputTransaction):
    def __init__(self, transaction: object, header: dict):
        super().__init__()
        self.header = header
        self.fields = InputRowDefinition(transaction, self.__class__.__name__, {
            'RecordType': (1, 3),
            'TransactionSequence#': (4, 8),
            'RecordSequence#': (12, 8),
            'ISWC': (20, 11),
            'Title': (31, 100)
        })

    def get_http_verb(self):
        return None

    def get_url(self):
        return None

    def get_parameters(self):
        return None

    def get_body(self):
        return {
            'ISWC': self.fields.get_field('ISWC'),
            'Title': self.fields.get_field('Title')
        }
