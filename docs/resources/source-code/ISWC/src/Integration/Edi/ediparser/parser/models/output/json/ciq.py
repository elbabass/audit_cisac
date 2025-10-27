class CIQ():
    def __init__(self, api_response: dict):
        self.iswc = api_response.get('searchResults')[0]

    def get_record(self, v2 = False):
        record = {
            'originalTransactionType': 'SearchByAgencyWorkCodeSubmission',
            'originalTitle': self.iswc.get('originalTitle'),
            'agency': self.iswc.get('agency'),
            'preferredIswc': self.iswc.get('iswc'),
            'interestedParties': self.iswc.get('interestedParties'),
            'otherTitles': self.iswc.get('otherTitles'),
            'workInfo': [],
            'linkedISWC': self.iswc.get('linkedISWC'),
            'parentISWC': self.iswc.get('parentISWC'),
            'overallParentISWC': self.iswc.get('overallParentISWC'),
        }

        if self.iswc.get('works') is not None:
            for w in self.iswc.get('works'):
                work = {
                    'agency': w.get('agency'),
                    'sourcedb': w.get('sourcedb'),
                    'workcode': w.get('workcode')
                }

                record['workInfo'].append(work)

        return record
