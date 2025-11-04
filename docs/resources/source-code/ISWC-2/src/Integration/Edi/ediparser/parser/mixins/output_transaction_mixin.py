from datetime import datetime
from typing import Sequence

from ediparser.parser.models.edi_file import EdiFileVersion
from ediparser.parser.models.input.input_transaction import InputTransaction
from ediparser.parser.models.output.car import CAR
from ediparser.parser.models.output.cdr import CDR
from ediparser.parser.models.output.ciq import CIQ
from ediparser.parser.models.output.cmq import CMQ
from ediparser.parser.models.output.cur import CUR
from ediparser.parser.models.output.mer import MER
from ediparser.parser.models.output.msg import MSG
from ediparser.parser.models.output.output_transaction import OutputTransaction
from ediparser.parser.models.output.wft import WFT


class OutputTransactionMixin():
    def __init__(self,
                 original_group_id: str,
                 header: dict,
                 original_transaction: dict,
                 api_response: dict,
                 api_response_ok: bool,
                 api_response_code: int,
                 creation_datetime: datetime,
                 current_datetime: datetime):

        self.original_group_id = original_group_id
        self.original_transaction = original_transaction
        self.api_response = api_response
        self.api_response_ok = api_response_ok
        self.api_response_code = api_response_code
        self.header = header
        self.creation_datetime = creation_datetime
        self.current_datetime = current_datetime

        file_version = header['EdiFileVersion']

    def get_record(self, sequence_number: int, record: dict):
        records = []

        ack_record = {
            'CreationDate': self.creation_datetime.strftime('%Y%m%d'),
            'CreationTime': self.creation_datetime.strftime('%H%M%S'),
            'OriginalGroupNumber': self.original_group_id,
            'OriginalTransactionSequence#': self.original_transaction['TransactionSequence#'].rjust(8, '0'),
            'ProcessingDate': self.current_datetime.strftime('%Y%m%d'),
            'TransactionStatus': 'FA' if self.api_response_ok else 'RJ'
        }

        ack_record = {**record, **ack_record}

        if self.api_response_ok:
            transaction = globals()[self.original_transaction['RecordType']](
                self.api_response, self.original_transaction, self.header)

            ack_record = {**ack_record, **transaction.get_record()
                          } if transaction.get_record() is not None else ack_record

            records.append(self.serialize_record(ack_record))

            if transaction.get_child_records() is not None:
                for child in transaction.get_child_records():
                    records.append(child)
        else:
            record = self.serialize_record(ack_record)
            records.append(record)
            records.append(
                MSG(self.api_response, self.original_transaction, self.api_response_code).get_record())

        records = self.__set_record_sequence(records, sequence_number)

        return records

    def get_child_records():
        pass

    def __set_record_sequence(self, records: Sequence[str], sequence_number: int):
        record_type_sequence_length = self.fields.fields['RecordType'][1]
        transaction_sequence_length = self.fields.fields['TransactionSequence#'][1]
        record_sequence_length = self.fields.fields['RecordSequence#'][1]
        rest_of_record_start = (record_type_sequence_length +
                                transaction_sequence_length +
                                record_sequence_length)
        for idx, record in enumerate(records):
            record = (record[:record_type_sequence_length] +
                      str(sequence_number).rjust(transaction_sequence_length, '0') +
                      str(idx).rjust(record_sequence_length, '0') +
                      record[rest_of_record_start:])
            yield record
