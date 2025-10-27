from datetime import datetime

from ediparser.parser.models.output.json.cmq import CMQ


class CSN_JSON():
    def __init__(self,
                 original_group_id: str,
                 header: dict,
                 original_transaction: dict,
                 api_response: dict,
                 api_response_ok: bool,
                 api_response_code: int,
                 current_datetime: datetime,
                 v2: bool = False):
        self.original_group_id = original_group_id
        self.original_transaction = original_transaction
        self.api_response = api_response
        self.api_response_ok = api_response_ok
        self.api_response_code = api_response_code
        self.header = header
        self.current_datetime = str(current_datetime)

        self.csn_record = {
            'originalFileCreationDateTime': self.header.get('fileCreationDateTime'),
            'originalSubmissionId': self.original_transaction.get('OriginalSubmissionId'),
            'processingDate': self.current_datetime,
            'transactionStatus': 'FullyAccepted' if self.api_response_ok else 'Rejected',
            'submissionId': int(self.original_transaction.get('OriginalSubmissionId')) + 1000,
            'workflowStatus': self.original_transaction.get('WorkflowStatus'),
            'workflowTaskId': self.original_transaction.get('WorkflowTaskID'),
            'workflowMessage': self.original_transaction.get('WorkflowMessage')
        }
        if self.csn_record['workflowTaskId'] != None and self.is_number(self.csn_record['workflowTaskId']):
            self.csn_record['workflowTaskId'] = int(
                self.csn_record.get('workflowTaskId'))
        if self.original_transaction.get('ParentIswc') is not None and self.original_transaction.get('OriginalTransactionType') != 'CDR':
            self.csn_record['parentIswc'] = self.original_transaction.get(
                'ParentIswc')
        if self.original_transaction.get('OriginalTransactionType') == 'CDR':
            self.csn_record['iswc'] = self.original_transaction.get('ParentIswc')
            self.csn_record['transactionStatus'] = 'FullyAccepted'

    def get_record(self, sequence_number: int):

        records = []
        if self.api_response_ok:
            transaction = globals()[self.original_transaction['TransactionType']](
                self.api_response)
            
            ack_record = {**self.csn_record, **transaction.get_record(True)} if (self.original_transaction.get('OriginalTransactionType') == 'CDR' and self.original_transaction.get("") is None) or self.original_transaction.get('OriginalTransactionType') != 'CDR'   else self.csn_record
            ack_record['originalTransactionType'] = self.get_original_transaction_type()
            ack_record['agency'] = self.original_transaction.get('AgencyCode')
            records.append(ack_record)
        return records

    def get_original_transaction_type(self):
        return {
            'CUR': 'UpdateSubmission',
            'MER': 'MergeSubmission',
            'DMR': 'DemergeSubmission',
            'CAR': 'AddSubmission',
            'CDR': 'DeleteSubmission'
        }.get(self.original_transaction.get('OriginalTransactionType'))

    def is_number(self, workflowTaskId: str):
        try:
            int(workflowTaskId)
            return True
        except ValueError:
            return False
