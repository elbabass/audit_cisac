class FSQ():
    def __init__(self, api_response: dict):
        self.work = api_response.get('submission').get('verifiedSubmission')

    def get_record(self, v2 = False):
        return {
            'originalTransactionType': 'FindSubmission',
            'originalTitle': self.work.get('originalTitle'),
            'agency': self.work.get('agency'),
            'sourcedb': self.work.get('sourcedb'),
            'preferredIswc': self.work.get('iswc'),
            'interestedParties': self.work.get('interestedParties'),
            'workcode': self.work.get('workcode'),
            'otherTitles': self.work.get('otherTitles')
        }
