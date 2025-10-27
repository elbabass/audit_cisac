from ediparser.parser.models.edi_file import EdiFileVersion
from ediparser.parser.models.input.input_row_definition import InputRowDefinition
from ediparser.parser.models.input.input_transaction import InputTransaction
from ediparser.parser.mixins.transaction_mixin import TransactionMixin


class MER(TransactionMixin, InputTransaction):
    def __init__(self, transaction, header):
        self.transaction = transaction
        self.header = header

    def get_http_verb(self):
        return 'POST'

    def get_url(self):
        return '/iswc/merge'

    def get_submission(self):
        return {
            'submissionId': self.transaction.get('submissionId'),
            'parameters': {
                'preferredIswc': self.transaction.get('preferredIswc'),
                'agency': self.transaction.get('agency')
            },
            'body': {
                'iswcs': self.transaction.get('mergeIswcs'),
            },
        }

    def get_body(self):
        return self.transaction.get('submission').get('body')

    def get_parameters(self):
        return self.transaction.get('submission').get('parameters')
