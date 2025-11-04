from ediparser.parser.models.input.input_row_definition import InputRowDefinition
from ediparser.parser.models.input.input_transaction import InputTransaction


class NAT(InputTransaction):
    def __init__(self, transaction: object, header: dict):
        super().__init__()
        self.header = header
        self.fields = InputRowDefinition(transaction, self.__class__.__name__, {
            'RecordType': (1, 3),
            'TransactionSequence#': (4, 8),
            'RecordSequence#': (12, 8),
            'TitleType': (20, 2),
            'LanguageCode': (22, 2),
            'WorkTitle': (24, 100)
        })

    def get_http_verb(self):
        return None

    def get_url(self):
        return None

    def get_parameters(self):
        return None

    def get_body(self):
        return {
            'title': self.fields.get_field('WorkTitle'),
            'type': self.fields.get_field('TitleType')
        }
