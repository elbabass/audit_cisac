from ediparser.parser.mixins.transaction_mixin import TransactionMixin
from ediparser.parser.models.edi_file import EdiFileVersion
from ediparser.parser.models.input.input_row_definition import InputRowDefinition
from ediparser.parser.models.input.input_transaction import InputTransaction


class CIQ(TransactionMixin, InputTransaction):
    def __init__(self, transaction, header):
        super().__init__(transaction, header)

    def get_http_verb(self):
        return 'POST'

    def get_url(self):
        return '/iswc/searchByAgencyWorkCode/batch'

    def get_parameters(self):
        None

    def get_body(self):
        return {
            'agency': self.fields.get_field('AgencyCode'),
            'workCode': self.fields.get_field('AgencyWorkCode')
        }
