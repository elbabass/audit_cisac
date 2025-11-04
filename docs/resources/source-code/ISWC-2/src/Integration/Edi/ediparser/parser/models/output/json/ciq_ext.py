class CIQEXT():
    def __init__(self, api_response: dict):
        self.iswcs = api_response.get('searchResults')

    def get_record(self, v2 = False):
        record = {
            'originalTransactionType': 'PublisherContextSearch',
            'searchResults': self.iswcs
        }

        return record
