from datetime import datetime

from ediparser.parser.models.output.tsv.car import CAR
from ediparser.parser.models.output.tsv.fsq import FSQ
from ediparser.parser.models.output.json.rejection import Rejection


class ACK_TSV():
    def __init__(self,
                 original_group_id: str,
                 header: dict,
                 original_transaction: dict,
                 api_response: dict,
                 api_response_ok: bool,
                 api_response_code: int,
                 current_datetime: datetime):

        self.original_group_id = original_group_id
        self.original_transaction = original_transaction
        self.api_response = api_response
        self.api_response_ok = api_response_ok
        self.api_response_code = api_response_code
        self.header = header
        self.current_datetime = str(current_datetime)

        self.ack_record = {
            1: self.original_transaction.get('submittingAgency'),
            2: self.original_transaction.get('submittingSourcedb'),
            3: self.original_transaction.get('submittingPublishername'),
            4: self.original_transaction.get('submittingPublishernameNumber'),
            5: self.original_transaction.get('submittingPublisherrole'),
            6: self.original_transaction.get('submittingPublisheremail'),
            7: self.original_transaction.get('OriginalSubmissionId'),
            9: self.original_transaction.get('workcode'),
            10: self.original_transaction.get('originalTitle'),
            11: self.current_datetime,
            12: 'FullyAccepted' if self.api_response_ok else 'Rejected',
        }

    def get_record(self, sequence_number: int):
        records = []
        no_attribute_err = True
        if self.original_transaction.get('OriginalTransactionType') in ['CAR', 'FSQ']:
            try:
                self.api_response.get('submission').get(
                    'verifiedSubmission')
            except AttributeError:
                no_attribute_err = False

        if self.api_response_ok and no_attribute_err:
            transaction = globals()[self.original_transaction['OriginalTransactionType']](
                self.api_response)

            self.ack_record.update(transaction.get_record())
            records.append(self.get_tab_list(self.ack_record))

        elif not no_attribute_err:
            if self.api_response_code != 400:
                self.api_response_code = 500
            rejection = Rejection(
                self.api_response, self.original_transaction, self.api_response_code)
            rej_record = rejection.get_record()
            ack_record = self.ack_record
            err_message = rej_record['message']['title'] if self.api_response_code == 400 else rej_record['message']
            ack_record[13] = str(rej_record['code'])+'|'+err_message
            ack_record[0] = rejection.get_original_transaction_type()
            ack_record[12] = 'Rejected'

            records.append(self.get_tab_list(ack_record))

        else:
            rejection = Rejection(
                self.api_response, self.original_transaction, self.api_response_code)
            rej_record = rejection.get_record()
            ack_record = self.ack_record
            ack_record[13] = str(rej_record['code'])+'|'+rej_record['message']
            ack_record[0] = rejection.get_original_transaction_type()

            records.append(self.get_tab_list(ack_record))

        return records

    def get_tab_list(self, ackrecord):
        tab_delimited_list = []
        ackrecord[0] = 'acknowledgements'

        for i in range(17):
            if i not in ackrecord.keys():
                ackrecord[i] = ''

        [tab_delimited_list.append(value)
         for (key, value) in sorted(ackrecord.items())]
        return '\t'.join(tab_delimited_list)
