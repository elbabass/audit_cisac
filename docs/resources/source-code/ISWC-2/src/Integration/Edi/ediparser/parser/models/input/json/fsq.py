from ediparser.parser.mixins.transaction_mixin import TransactionMixin
from ediparser.parser.models.edi_file import EdiFileVersion
from ediparser.parser.models.input.input_row_definition import InputRowDefinition
from ediparser.parser.models.input.input_transaction import InputTransaction


class FSQ(TransactionMixin, InputTransaction):
    def __init__(self, transaction, header):
        self.transaction = transaction
        self.header = header

    def get_http_verb(self):
        return 'POST'

    def get_url(self):
        return '/resolution/batch'

    def get_submission(self):
        self.transaction['agency'] = self.header.get('submittingAgency')
        self.transaction['sourcedb'] = self.header.get('submittingSourcedb')
        self.transaction['category'] = 'DOM'
        self.transaction['previewDisambiguation'] = True

        if 'additionalIdentifiers' in self.transaction.keys():
            if 'publisherIdentifiers' in self.transaction['additionalIdentifiers'].keys() and self.transaction['additionalIdentifiers']['publisherIdentifiers'] != []:
                self.transaction['additionalIdentifiers']['publisherIdentifiers'].append({
                    "nameNumber": self.header.get('submittingPublisher').get('nameNumber'),
                    "workCode": [self.transaction.get('workcode')]})
            else:
                self.transaction['additionalIdentifiers'].update({"publisherIdentifiers": [{
                    "nameNumber": self.header.get('submittingPublisher').get('nameNumber'),
                    "workCode": [self.transaction.get('workcode')]}]})

        else:
            self.transaction['additionalIdentifiers'] = {"publisherIdentifiers": [{
                "nameNumber":  self.header.get('submittingPublisher').get('nameNumber'),
                "workCode": [self.transaction.get('workcode')]}]}

        if 'agencyWorkCodes' in self.transaction.keys():
            self.transaction['additionalIdentifiers'].update(
                {"agencyWorkCodes": self.transaction.get('agencyWorkCodes')})
            self.transaction.pop('agencyWorkCodes')

        return {
            'body': {
                'submissionId': self.transaction.get('submissionId'),
                'submission': self.transaction
            }
        }

    def get_parameters(self):
        return None

    def get_body(self):
        body = self.transaction.get('submission').get('body')
        body.get('submission')['workcode'] = self.generate_workcode(
            body.get('submission')['workcode'], body)
        return body
