from abc import ABC, abstractmethod
from ediparser.parser.models.input.input_transaction import InputTransaction

from requests import Response


class OutputTransaction(ABC):

    def __init__(self, api_response: dict, original_transaction: InputTransaction):
        self.api_response = api_response
        self.original_transaction = original_transaction

    @abstractmethod
    def get_record():
        pass

    @abstractmethod
    def get_child_records():
        pass

    def get_field(self, field_name):
        return self.api_response.get(field_name, '')

    def serialize_record(self, body):
        record = ''
        for field, (start_position, length) in sorted(self.fields.fields.items(), key=lambda x: x[1]):
            value = body.get(field, '')
            value = str(value).ljust(length)
            record += value

        return record
