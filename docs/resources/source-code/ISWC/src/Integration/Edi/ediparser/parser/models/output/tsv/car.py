from ediparser.parser.mixins.output_transaction_mixin_tsv import OutputTransactionMixinTsv


class CAR:
    def __init__(self, api_response: dict):
        self.publisher = api_response.get('submittingPublisher')
        self.work = api_response.get('submission')
        self.record_key = OutputTransactionMixinTsv(api_response)

    def get_record(self):
        record = {}

        def get_tsv_key(field, api_response_field):
            response_field_value = self.work.get(
                'verifiedSubmission').get(api_response_field)
            record[self.record_key.get_tsv_field(
                field)] = str(response_field_value) if response_field_value else ''

        get_tsv_key('preferredIswc', 'iswc')
        # get_tsv_key('workInfo1 / agency', '')
        # get_tsv_key('workInfo1 / sourcedb', '')
        # get_tsv_key('workInfo1 / workcode ', '')

        return record
