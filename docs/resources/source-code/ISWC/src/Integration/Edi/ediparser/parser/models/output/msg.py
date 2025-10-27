from ediparser.parser.models.input.input_transaction import InputTransaction
from ediparser.parser.models.output.output_row_definition import \
    OutputRowDefinition
from ediparser.parser.models.output.output_transaction import OutputTransaction


class MSG(OutputTransaction):
    def __init__(self, api_response: dict, original_transaction: InputTransaction, api_response_code: int):
        super().__init__(None, None)
        self.api_response = api_response
        self.api_response_code = api_response_code
        self.original_transaction = original_transaction
        self.fields = OutputRowDefinition({
            'RecordType': (1, 3),
            'TransactionSequence#': (4, 8),
            'RecordSequence#': (12, 8),
            'MessageType': (20, 1),
            'OriginalRecordSequence': (21, 8),
            'OriginalRecordType': (29, 3),
            'MessageLevel': (32, 1),
            'ConditionNumber': (33, 3),
            'MessageText': (36, 150)
        })

    def get_record(self):
        if self.api_response_code == 207:
            error = self.api_response.get('rejection')
            code = error.get('code', '')
            message = error.get('message', '')
        else:
            error = self.api_response
            if error.get('code') is not None:
                code = error.get('code', '')
                message = error.get('message', '')
            elif error.get('statusCode') is not None:
                code = error.get('statusCode', '')
                message = error.get('message', '')
            else:
                code = self.api_response_code
                message = error

        record = {
            'RecordType': self.__class__.__name__,
            'MessageType': 'T',
            'OriginalRecordSequence': self.original_transaction['RecordSequence'],
            'OriginalRecordType': self.original_transaction['RecordType'],
            'MessageLevel': 'T',
            'ConditionNumber': code,
            'MessageText': message
        }
        return self.serialize_record(record)

    def get_child_records(self):
        pass
