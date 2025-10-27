class CIQEXT():
    def __init__(self, api_response: dict):
        self.iswcs = api_response.get('searchResult')

    def get_record(self, v2 = False):
        record = {
            'originalTransactionType': 'PublisherContextSearch',
            'searchResult': self.iswcs
        }

        return record
