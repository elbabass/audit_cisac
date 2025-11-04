from ediparser.parser.mixins.transaction_mixin import TransactionMixin
from ediparser.parser.models.edi_file import EdiFileVersion
from ediparser.parser.models.input.input_row_definition import InputRowDefinition
from ediparser.parser.models.input.input_transaction import InputTransaction


class CDR(TransactionMixin, InputTransaction):
    def __init__(self, transaction, header):
        super().__init__(transaction, header)

    def get_http_verb(self):
        return 'DELETE'

    def get_url(self):
        return '/submission'

    def get_parameters(self):
        parameters = {
            'preferredIswc': self.fields.get_field('PreferredISWC'),
            'agency': self.fields.get_field('AgencyCode'),
            'workcode': self.fields.get_field('AgencyWorkCode'),
            'sourceDb': self.fields.get_field('SourceDBCode'),
            'reasonCode': 'EdiV1'
        }

        if parameters['sourceDb'] == '':
            parameters['sourceDb'] = 0

        if self.header['EdiFileVersion'] == EdiFileVersion.THREE:
            parameters['reasonCode'] = self.fields.get_field('DeletionReason')

        return parameters

    def get_body(self):
        return None
