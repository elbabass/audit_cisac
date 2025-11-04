from ediparser.parser.models.input.input_transaction import InputTransaction
from ediparser.parser.models.input.rejection import get_rejection_message


class CDR(InputTransaction):
    def __init__(self, transaction, header):
        self.transaction = transaction
        self.header = header

    def get_http_verb(self):
        return 'DELETE'

    def get_url(self):
        return '/submission'

    def get_submission(self):
        return {
            'submissionId': self.transaction.get('submissionId'),
            'parameters': {
                'preferredIswc': self.transaction.get('preferredIswc'),
                'agency': self.transaction.get('agency'),
                'workcode': self.transaction.get('workcode'),
                'sourceDb': self.transaction.get('sourcedb'),
                'reasonCode': self.transaction.get('reasonCode')
            }
        }

    def get_body(self):
        return None

    def get_parameters(self):
        return self.transaction.get('submission').get('parameters')
