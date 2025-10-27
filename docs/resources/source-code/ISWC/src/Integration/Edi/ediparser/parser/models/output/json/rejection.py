
class Rejection():
    def __init__(self, api_response, original_transaction, api_response_code):
        self.api_response = api_response
        self.original_transaction = original_transaction
        self.api_response_code = api_response_code

    def get_record(self):
        rejection = {}
        if self.api_response_code == 207:
            error = self.api_response.get('rejection')
            rejection['code'] = error.get('code', '')
            rejection['message'] = error.get('message', '')
        elif self.api_response_code == 500:
            rejection['code'] = self.api_response_code
            rejection['message'] = 'Internal Server Error'
        else:
            error = self.api_response
            if error.get('code') is not None:
                rejection['code'] = error.get('code', '')
                rejection['message'] = error.get('message', '')
            elif error.get('statusCode') is not None:
                rejection['code'] = error.get('statusCode', '')
                rejection['message'] = error.get('message', '')
            else:
                rejection['code'] = self.api_response_code
                rejection['message'] = error

        return rejection

    def get_original_transaction_type(self):
        return {
            'CAR': 'AddSubmission',
            'CUR': 'UpdateSubmission',
            'CDR': 'DeleteSubmission',
            'CMQ': 'SearchByIswcSubmission',
            'CIQ': 'SearchByAgencyWorkCodeSubmission',
            'MER': 'MergeSubmission',
            'WFT': 'UpdateWorkflowTask',
            'CIQEXT': 'PublisherContextSearch'
        }.get(self.original_transaction['OriginalTransactionType'])
