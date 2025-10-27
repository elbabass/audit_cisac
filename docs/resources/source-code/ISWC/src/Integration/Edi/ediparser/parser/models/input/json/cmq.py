from ediparser.parser.models.input.input_transaction import InputTransaction


class CMQ(InputTransaction):
    def __init__(self, transaction, header):
        self.transaction = transaction
        self.header = header

    def get_http_verb(self):
        return 'POST'

    def get_url(self):
        return '/iswc/searchByIswc/batch'
    
    def get_thirdParty_url(self, group_id):
        return '/thirdParty/searchByIswc/batch'

    def get_submission(self):
        return {
            'submissionId': self.transaction.get('submissionId'),
            'body': {
                'iswc': self.transaction.get('iswc')
            }
        }

    def get_parameters(self):
        return None

    def get_body(self):
        return self.transaction.get('submission').get('body')

    def get_csn_data(self):
        return {
            'workflowStatus': self.transaction.get('workflowStatus'),
            'workflowTaskId': self.transaction.get('workflowTaskId'),
            'originalTransactionType': self.transaction.get('originalTransactionType'),
            'agency': self.transaction.get('agency'),
            'receivingAgency': self.transaction.get('receivingAgency'),
            'parentIswc': self.transaction.get('parentIswc')
        }
