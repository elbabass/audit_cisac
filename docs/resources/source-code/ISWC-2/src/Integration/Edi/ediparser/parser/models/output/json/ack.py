from datetime import datetime

from ediparser.parser.models.output.json.car import CAR
from ediparser.parser.models.output.json.cdr import CDR
from ediparser.parser.models.output.json.ciq import CIQ
from ediparser.parser.models.output.json.cmq import CMQ
from ediparser.parser.models.output.json.cur import CUR
from ediparser.parser.models.output.json.mer import MER
from ediparser.parser.models.output.json.fsq import FSQ
from ediparser.parser.models.output.json.rejection import Rejection
from ediparser.parser.models.output.json.wft import WFT
from ediparser.parser.models.output.json.ciq_ext import CIQEXT


class ACK_JSON():
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
        self.v2 = v2

        self.is_allocation_or_resolution = 'submittingPublisher' in header and original_transaction['OriginalTransactionType'] in [
            'CAR', 'FSQ']

        self.ack_record = {
            'originalFileCreationDateTime': self.header.get('fileCreationDateTime'),
            'originalSubmissionId': self.original_transaction.get('OriginalSubmissionId'),
            'processingDate': self.current_datetime,
            'transactionStatus': 'FullyAccepted' if self.api_response_ok else 'Rejected',
            'submissionId': int(self.original_transaction.get('OriginalSubmissionId')) + 1000
        }

    def get_record(self, sequence_number: int):
        records = []
        no_attribute_err = True
        if self.original_transaction.get('OriginalTransactionType') in ['CAR', 'CUR', 'FSQ']:
            try:
                self.api_response.get('submission').get('verifiedSubmission')
            except AttributeError:
                no_attribute_err = False

        if self.api_response_ok and no_attribute_err:
            transaction = globals()[self.original_transaction['OriginalTransactionType']](
                self.api_response)

            transaction_record = transaction.get_record(self.v2)

            if self.original_group_id in ['searchByTitleAndContributorsSubmissions']:
                transaction_record['originalTransactionType'] = 'SearchByTitleAndContributorsSubmission'

            if self.is_allocation_or_resolution:
                for i in transaction_record['interestedParties']:
                    del i['baseNumber']

            if self.original_transaction['OriginalTransactionType'] in ['CAR', 'FSQ']:
                transaction_record['workcode'] = self.original_transaction['AgencyWorkCode']

            if self.original_transaction['OriginalTransactionType'] in ['CAR', 'CUR']:
                response = self.api_response.get('submission')
                if len(response["multipleAgencyWorkCodes"]) > 0:
                    self.ack_record['multipleAgencyWorkCodes'] = response["multipleAgencyWorkCodes"]

            self.ack_record = {**self.ack_record, **transaction_record}

            records.append(self.ack_record)
        elif not no_attribute_err:
            self.api_response_code = 500
            rejection = Rejection(
                self.api_response, self.original_transaction, self.api_response_code)

            ack_record = self.ack_record
            ack_record['errorMessages'] = rejection.get_record()
            ack_record['originalTransactionType'] = rejection.get_original_transaction_type()
            ack_record['transactionStatus'] = 'Rejected'
            records.append(ack_record)

        else:
            rejection = Rejection(
                self.api_response, self.original_transaction, self.api_response_code)

            ack_record = self.ack_record
            ack_record['errorMessages'] = rejection.get_record()
            ack_record['originalTransactionType'] = rejection.get_original_transaction_type()
            if self.original_group_id in ['searchByTitleAndContributorsSubmissions']:
                ack_record['originalTransactionType'] = 'SearchByTitleAndContributorsSubmission'
            if self.original_transaction['OriginalTransactionType'] in ['CAR', 'FSQ']:
                ack_record['workcode'] = self.original_transaction['AgencyWorkCode']

            records.append(ack_record)

        return records
