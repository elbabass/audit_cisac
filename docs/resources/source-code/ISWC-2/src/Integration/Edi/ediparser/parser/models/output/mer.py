import requests

from ediparser.parser.models.input.input_transaction import InputTransaction
from ediparser.parser.models.output.cwi import CWI
from ediparser.parser.models.output.output_row_definition import \
    OutputRowDefinition
from ediparser.parser.models.output.output_transaction import OutputTransaction


class MER(OutputTransaction):
    def __init__(self, api_response: requests.Response, original_transaction: InputTransaction, header: dict):
        super().__init__(api_response, original_transaction)

    def get_record(self):
        return None

    def get_child_records(self):
        return {}
