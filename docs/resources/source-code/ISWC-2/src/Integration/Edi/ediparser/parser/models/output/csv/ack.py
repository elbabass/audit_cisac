class ACK_CSV():
    def __init__(self,
                 original_group_id: str,
                 header: dict,
                 original_transaction: dict,
                 api_response: dict,
                 api_response_ok: bool,
                 api_response_code: int):

        self.original_group_id = original_group_id
        self.original_transaction = original_transaction
        self.api_response = api_response
        self.api_response_ok = api_response_ok
        self.api_response_code = api_response_code
        self.header = header

        self.iswc = None
        if api_response.get('searchResults') is not None and len(api_response.get('searchResults')) > 0:
            self.iswc = api_response.get('searchResults')[0]

        self.record = {
            'SourceDatabase': '',
            'SocietyCode': '',
            'WorkNumber': '',
            'ISWC': '',
            'ReferenceNumber': self.original_transaction['ReferenceNumber'],
            'Comment': '',
            'PerformDCI': '',
            'WorkCategory': ''
        }

    def get_record(self, sequence_number: int):
        if self.api_response_ok and self.iswc is not None:
            self.record['SocietyCode'] = self.iswc.get('agency')

            if self.iswc.get('works') is not None and len(self.iswc.get('works')) > 0:
                works = self.iswc.get('works')
                eligible_works = list(filter(lambda x: x['iswcEligible'] == True, works))
                original_work = sorted(eligible_works, key = lambda x : x['createdDate'])[0]

                self.record['SourceDatabase'] = str(original_work.get('sourcedb'))
                self.record['SocietyCode'] = original_work.get('agency')
                self.record['WorkNumber'] = original_work.get('workcode')
                self.record['ISWC'] = original_work.get('iswc')

        return self.record
