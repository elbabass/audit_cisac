class CAR():
    def __init__(self, api_response: dict):
        self.work = api_response.get('submission').get('verifiedSubmission')
        self.alternateIswcMatches = api_response.get('submission').get('alternateIswcMatches')

    def get_record(self, v2 = False):
        if v2:
            return {
                'originalTransactionType': 'AddSubmission',
                'originalTitle': self.work.get('originalTitle'),
                'agency': self.work.get('agency'),
                'sourcedb': self.work.get('sourcedb'),
                'iswc': self.work.get('iswc'),
                'iswcStatus': self.work.get('iswcStatus'),
                'interestedParties': self.work.get('interestedParties'),
                'performers': self.work.get('performers'),
                'additionalIdentifiers': self.work.get('additionalIdentifiers'),
                'workcode': self.work.get('workcode'),
                'otherTitles': self.work.get('otherTitles'),
                'archivedIswc': self.work.get('archivedIswc'),
                'alternateIswcMatches': self.alternateIswcMatches
            }
        else:
            return {
                'originalTransactionType': 'AddSubmission',
                'originalTitle': self.work.get('originalTitle'),
                'agency': self.work.get('agency'),
                'sourcedb': self.work.get('sourcedb'),
                'preferredIswc': self.work.get('iswc'),
                'interestedParties': self.work.get('interestedParties'),
                'workcode': self.work.get('workcode'),
                'otherTitles': self.work.get('otherTitles'),
                'archivedIswc': self.work.get('archivedIswc')
            }
