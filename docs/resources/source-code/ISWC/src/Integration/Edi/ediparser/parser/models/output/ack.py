from datetime import datetime

from ediparser.parser.mixins.output_transaction_mixin import \
    OutputTransactionMixin
from ediparser.parser.models.edi_file import EdiFileVersion
from ediparser.parser.models.output.output_row_definition import \
    OutputRowDefinition
from ediparser.parser.models.output.output_transaction import OutputTransaction


class ACK(OutputTransactionMixin, OutputTransaction):
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

        file_version = header['EdiFileVersion']

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
            'AgencyCode': (110, 3),
            'AgencyWorkCode': (113, 20),
            'SourceDBCode': (133, 3),
            'PreferredISWC': (136, 11),
            'ProcessingDate': (147, 8),
            'TransactionStatus': (155, 2)
        } if file_version == EdiFileVersion.TWO else {
            'RecordType': (1, 3),
            'TransactionSequence#': (4, 8),
            'RecordSequence#': (12, 8),
            'CreationDate': (20, 8),
            'CreationTime': (28, 6),
            'OriginalGroupNumber': (34, 5),
            'OriginalTransactionSequence#': (39, 8),
            'OriginalTransactionType': (47, 3),
            'WorkTitle': (50, 100),
            'AgencyCode': (150, 3),
            'AgencyWorkCode': (153, 20),
            'SourceDBCode': (173, 3),
            'PreferredISWC': (176, 11),
            'ProcessingDate': (187, 8),
            'TransactionStatus': (195, 2)
        })

    def get_record(self, sequence_number: int):

        ack_record = {
            'RecordType': self.__class__.__name__,
            'OriginalTransactionType': self.original_transaction.get('RecordType'),
            'WorkTitle': self.original_transaction.get('WorkTitle'),
            'AgencyCode': self.original_transaction.get('AgencyCode'),
            'AgencyWorkCode': self.original_transaction.get('AgencyWorkCode'),
            'SourceDBCode': self.original_transaction.get('SourceDBCode')
        }

        return super().get_record(sequence_number, ack_record)
