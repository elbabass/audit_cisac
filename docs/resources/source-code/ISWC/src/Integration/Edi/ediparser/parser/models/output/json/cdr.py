import requests


class CDR():
    def __init__(self, api_response: dict):
        self.api_response = api_response

    def get_record(self, v2 = False):
        return {
            'originalTransactionType': 'DeleteSubmission'
        }

    def get_child_records(self):
        return None
