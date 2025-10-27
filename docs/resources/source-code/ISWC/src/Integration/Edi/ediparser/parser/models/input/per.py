from ediparser.parser.models.input.input_row_definition import InputRowDefinition
from ediparser.parser.models.input.input_transaction import InputTransaction


class PER(InputTransaction):
    def __init__(self, transaction: object, header: dict):
        super().__init__()
        self.header = header
        self.fields = InputRowDefinition(transaction, self.__class__.__name__, {
            'RecordType': (1, 3),
            'TransactionSequence#': (4, 8),
            'RecordSequence#': (12, 8),
            'FirstName': (20, 50),
            'LastName': (70, 50)
        })

    def get_http_verb(self):
        return None

    def get_url(self):
        return None

    def get_parameters(self):
        return None

    def get_body(self):
        return {
            'firstName': self.fields.get_field('FirstName'),
            'lastName': self.fields.get_field('LastName')
        }
