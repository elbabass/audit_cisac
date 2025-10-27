from ediparser.parser.models.input.input_transaction import InputTransaction
from ediparser.parser.models.input.rejection import get_rejection_message


class CUR(InputTransaction):
    def __init__(self, transaction, header):
        self.transaction = transaction
        self.header = header

    def get_http_verb(self):
        return 'PUT'

    def get_url(self):
        return '/submission/batch'

    def get_submission(self):
        if 'multipleAgencyWorkCodes' in self.transaction.keys():
            return {
                'body': {
                    'submissionId': self.transaction.get('submissionId'),
                    'submission': self.transaction,
                    'multipleAgencyWorkCodes': self.transaction('multipleAgencyWorkCodes')
                }
            }
        else:
            return {
                'body': {
                    'submissionId': self.transaction.get('submissionId'),
                    'submission': self.transaction
                }
            }

    def get_parameters(self):
        return None

    def get_body(self):
        return self.transaction.get('submission').get('body')

    def get_children(self, transaction_type: str):
        def isNaN(num):
            return num != num

        children = self.transaction.get(transaction_type)

        if children == None or isNaN(children):
            return []

        results = []

        for child in children:
            results.append(globals()[transaction_type](
                {transaction_type: child}, self.header).get_body())

        return results

    def get_rejection(self):
        rejection = get_rejection_message(
            self.transaction.get('submission').get('body').get('submission'), 'JSON')
        return rejection
