class OutputTransactionMixinTsv:
    def __init__(self, api_response: dict):
        self.api_response = api_response

    def get_tsv_field(self, field_name):
        return {"recordType": 0,
                "submittingAgency": 1,
                "submittingSourcedb": 2,
                "submittingPublisher / name": 3,
                "submittingPublisher / nameNumber": 4,
                "submittingPublisher / role": 5,
                "submittingPublisher / email": 6,
                "originalSubmissionId": 7,
                "preferredIswc": 8,
                "workcode": 9,
                "originalTitle": 10,
                "processingDateTime": 11,
                "transactionStatus": 12,
                "errorMessages": 13,
                "workInfo1 / agency": 14,
                "workInfo1 / sourcedb": 15,
                "workInfo1 / workcode ": 16
                }.get(field_name)
