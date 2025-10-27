from ediparser.parser.models.edi_file import EdiFileVersion
from ediparser.parser.models.input.input_row_definition import InputRowDefinition
from ediparser.parser.models.input.input_transaction import InputTransaction
from ediparser.parser.mixins.transaction_mixin import TransactionMixin


class MER(TransactionMixin, InputTransaction):
    def __init__(self, transaction: dict, header: dict):
        super().__init__(transaction, header)

    def get_http_verb(self):
        return 'POST'

    def get_url(self):
        return '/iswc/merge'

    def get_parameters(self):
        return {
            'preferredIswc': self.fields.get_field('PreferredISWC'),
            'agency': self.fields.get_field('AgencyCode')
        }

    def get_body(self):
        body = {
            'iswcs': self.get_children('MLI')
        }
        return body
