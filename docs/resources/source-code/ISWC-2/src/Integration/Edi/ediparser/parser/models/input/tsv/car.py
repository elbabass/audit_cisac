from ediparser.parser.models.input.input_transaction import InputTransaction
from ediparser.parser.models.input.rejection import get_rejection_message
from ediparser.parser.mixins.transaction_mixin_tsv import TransactionMixinTsv


class CAR(InputTransaction):
    def __init__(self, transaction, header):
        self.transaction = transaction
        self.header = header
        self.TransactionMixinTsv_obj = TransactionMixinTsv(self.transaction)

    def get_http_verb(self):
        return 'POST'

    def get_url(self):
        return '/allocation/batch'

    def get_submission(self):
        submission_body = self.TransactionMixinTsv_obj.get_tsv_body()
        submission_body['submission']['workcode'] = self.generate_workcode(submission_body['submission']['workcode'], submission_body['submission'])
        return {'body': submission_body}

    def get_parameters(self):
        return None

    def get_body(self):
        body = self.transaction.get('submission').get('body')
        return body

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
            self.transaction.get('submission').get('body').get('submission'), 'TSV')
        return rejection
