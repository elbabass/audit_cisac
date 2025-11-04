from ediparser.parser.models.edi_file import EdiFileVersion
from ediparser.parser.models.input.input_row_definition import InputRowDefinition
from ediparser.parser.models.input.input_transaction import InputTransaction
from ediparser.parser.mixins.transaction_mixin import TransactionMixin


class CMQ(TransactionMixin, InputTransaction):
    def __init__(self, transaction: dict, header: dict):
        super().__init__(transaction, header)

        csn_fields = {
            'WorkflowStatus': (207, 1),
            'WorkflowTaskID': (235, 10),
            'OriginalTransactionType': (245, 3),
            'ReceivingAgencyCode': (248, 3),
            'ParentIswc': (251, 11)
        }

        self.fields.fields = {**csn_fields, **self.fields.fields}

    def get_http_verb(self):
        return 'POST'

    def get_url(self):
        return '/iswc/searchByIswc/batch'

    def get_parameters(self):
        None

    def get_body(self):
        return {
            'iswc': self.fields.get_field('PreferredISWC')
        }
