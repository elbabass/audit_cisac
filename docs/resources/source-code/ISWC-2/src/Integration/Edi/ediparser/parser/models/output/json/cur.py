class CUR():
    def __init__(self, api_response: dict):
        self.work = api_response.get('submission').get('verifiedSubmission')
        self.alternateIswcMatches = api_response.get('submission').get('alternateIswcMatches')

    def get_record(self, v2 = False):
        if v2:
            record = {
                'originalTransactionType': 'UpdateSubmission',
                'originalTitle': self.work.get('originalTitle'),
                'agency': self.work.get('agency'),
                'sourcedb': self.work.get('sourcedb'),
                'iswc': self.work.get('iswc'),
                'iswcStatus': self.work.get('iswcStatus'),
                'interestedParties': self.work.get('interestedParties'),
                'performers': self.work.get('performers'),
                'additionalIdentifiers': self.work.get('additionalIdentifiers'),
                'workcode': self.work.get('workcode'),
                'alternateIswcMatches': self.alternateIswcMatches
            }
        else:
            record = {
                'originalTransactionType': 'UpdateSubmission',
                'originalTitle': self.work.get('originalTitle'),
                'agency': self.work.get('agency'),
                'sourcedb': self.work.get('sourcedb'),
                'preferredIswc': self.work.get('iswc'),
                'interestedParties': self.work.get('interestedParties'),
                'workcode': self.work.get('workcode')
            }

        if self.work.get('otherTitles') is not None:
            record['otherTitles'] = self.work.get('otherTitles')

        return record
