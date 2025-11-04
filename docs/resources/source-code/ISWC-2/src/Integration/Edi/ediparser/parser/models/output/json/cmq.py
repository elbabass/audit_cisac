class CMQ():
    def __init__(self, api_response: dict):
        search_results = api_response.get('searchResults')
        search_result = search_results[0] if search_results else api_response.get('searchResult')
        self.iswc = search_result
        if self.iswc is None:
            self.iswc = api_response

    def get_record(self, v2 = False):
        record = {
            'originalTransactionType': 'SearchByIswcSubmission',
            'originalTitle': self.iswc.get('originalTitle'),
            'agency': self.iswc.get('agency'),
            'iswc': self.iswc.get('iswc'),
            'iswcStatus': self.iswc.get('iswcStatus'),
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
