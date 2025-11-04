from datetime import datetime

from ediparser.parser.mixins.output_transaction_mixin import \
    OutputTransactionMixin
from ediparser.parser.models.output.output_row_definition import \
    OutputRowDefinition
from ediparser.parser.models.output.output_transaction import OutputTransaction
from ediparser.parser.models.edi_file import EdiFileVersion


class CSN(OutputTransactionMixin, OutputTransaction):
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
            'ReceivingAgencyCode': (110, 3),
            'AgencyWorkCode': (113, 20),
            'ReceivingAgencySourceDb': (133, 3),
            'PreferredISWC': (136, 11),
            'ProcessingDate': (147, 8),
            'TransactionStatus': (155, 2),
            'WorkflowTaskID': (157, 10),
            'WorkflowStatus': (167, 1)
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
            'ReceivingAgencyCode': (150, 3),
            'AgencyWorkCode': (153, 20),
            'ReceivingAgencySourceDb': (173, 3),
            'PreferredISWC': (176, 11),
            'ProcessingDate': (187, 8),
            'TransactionStatus': (195, 2),
            'WorkflowTaskID': (197, 10),
            'WorkflowStatus': (207, 1)
        })

    def get_record(self, sequence_number: int):
        search_results = self.api_response.get('searchResults')
        search_result = search_results[0] if search_results else self.api_response.get('searchResult')
        agency_work = self.get_work_for_submitting_agency(search_result)
        if self.header['EdiFileVersion'] == EdiFileVersion.TWO:
            search_result = self.filter_out_performers(search_result)

        csn_record = {
            'RecordType': self.__class__.__name__,
            'WorkflowTaskID':  self.original_transaction.get('WorkflowTaskID').rjust(10),
            'WorkflowStatus': self.original_transaction.get('WorkflowStatus'),
            'OriginalTransactionType': self.original_transaction.get('OriginalTransactionType'),
            'ReceivingAgencyCode': agency_work.get('agency').rjust(3, '0'),
            'AgencyWorkCode': agency_work.get('workcode'),
            'ReceivingAgencySourceDb': str(agency_work.get('sourcedb')).rjust(3, '0'),
            'WorkTitle': agency_work.get('originalTitle'),
            'PreferredISWC': agency_work.get('iswc'),
            'ParentIswc': self.original_transaction.get('ParentIswc')
        } if search_result is not None and search_result['works'].__len__() > 0 else {
            'RecordType': self.__class__.__name__,
            'WorkflowTaskID':  self.original_transaction.get('WorkflowTaskID').rjust(10),
            'WorkflowStatus': self.original_transaction.get('WorkflowStatus'),
            'OriginalTransactionType': self.original_transaction.get('OriginalTransactionType'),
            'ParentIswc': self.original_transaction.get('ParentIswc'),
            'ReceivingAgencyCode': self.original_transaction.get('ReceivingAgencyCode').rjust(3, '0'),
            'AgencyWorkCode': self.original_transaction.get('ReceivingAgencyWorkCode'),
            'ReceivingAgencySourceDb': self.original_transaction.get('ReceivingAgencyCode').rjust(3, '0')
        }

        return super().get_record(sequence_number, csn_record)

    def get_work_for_submitting_agency(self, search_result):
        if search_result is not None and search_result.get('works') is not None and search_result.get('works').__len__() > 0:
            for work in search_result.get('works'):
                if work.get('agency') == self.original_transaction.get('AgencyCode'):
                    return work
                else:
                    continue
            return search_result.get('works')[0]

    def filter_out_performers(self, search_result):
        if search_result is not None and search_result.get('works') != None and search_result.get('works').__len__() > 0:
            for work in search_result.get('works'):
                work['performers'] = []
        return search_result
