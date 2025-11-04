from datetime import datetime

from ediparser.parser.mixins.output_transaction_mixin import \
    OutputTransactionMixin
from ediparser.parser.models.output.output_row_definition import \
    OutputRowDefinition
from ediparser.parser.models.output.output_transaction import OutputTransaction
from ediparser.parser.models.edi_file import EdiFileVersion


class CSE(OutputTransactionMixin, OutputTransaction):
    def __init__(self,
                 original_group_id: str,
                 header: dict,
                 original_transaction: dict,
                 api_response: dict,
                 api_response_ok: bool,
                 api_response_code: int,
                 creation_datetime: datetime,
                 current_datetime: datetime):
        super().__init__(original_group_id, header, original_transaction, api_response,
                         api_response_ok, api_response_code, creation_datetime, current_datetime)

        self.fields = OutputRowDefinition({
            'RecordType': (1, 3),
            'TransactionSequence#': (4, 8),
            'RecordSequence#': (12, 8),
            'CreationDate': (20, 8),
            'CreationTime': (28, 6),
            'OriginalGroupNumber': (34, 5),
            'OriginalTransactionSequence#': (39, 8),
            'OriginalTransactionType': (47, 3),
            'WorkTitle': (50, 60),
            'ReceivingAgencyCode': (110, 3),
            'AgencyWorkCode': (113, 20),
            'ReceivingAgencySourceDb': (133, 3),
            'PreferredISWC': (136, 11),
            'ProcessingDate': (147, 8),
            'TransactionStatus': (155, 2),
            'WorkflowTaskID': (157, 10),
            'WorkflowStatus': (167, 1)
        })

    def get_record(self, sequence_number: int):

        search_result = self.api_response.get('searchResults')[0]

        cse_record = {
            'RecordType': 'CSN',
            'WorkflowTaskID':  self.original_transaction.get('WorkflowTaskID').rjust(10),
            'WorkflowStatus': self.original_transaction.get('WorkflowStatus'),
            'OriginalTransactionType': self.original_transaction.get('OriginalTransactionType'),
            'ReceivingAgencyCode': search_result.get('works')[0].get('agency').rjust(3, '0'),
            'AgencyWorkCode': search_result.get('works')[0].get('workcode'),
            'ReceivingAgencySourceDb': str(search_result.get('works')[0].get('sourcedb')).rjust(3, '0'),
            'WorkTitle': search_result.get('works')[0].get('originalTitle'),
            'PreferredISWC': search_result.get('works')[0].get('iswc')
        }

        return super().get_record(sequence_number, cse_record)
