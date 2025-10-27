from ediparser.parser.models.input.input_row_definition import InputRowDefinition
from ediparser.parser.models.input.input_transaction import InputTransaction
from enum import Enum

class performer_roles_enum(Enum):
    IN = 1,
    PR = 2,
    PER = 3
    
class PIP(InputTransaction):
    def __init__(self, transaction: object, header: dict):
        super().__init__()
        self.header = header
        self.fields = InputRowDefinition(transaction, self.__class__.__name__, {
            'RecordType': (1, 3),
            'TransactionSequence#': (4, 8),
            'RecordSequence#': (12, 8),
            'IPName': (20, 45),
            'IPFirstName': (65, 30),
            'IPNumber': (95, 9),
            'IPNameNumber': (104, 11),
            'IPRole': (115, 2)
        })

    def get_http_verb(self):
        return None

    def get_url(self):
        return None

    def get_parameters(self):
        return None

    def get_body(self, type: str):
        
        if type == 'Performer' and self.fields.get_field('IPRole') in performer_roles_enum.__members__:
            body = {
                'lastName': self.fields.get_field('IPName'),
                'firstName': self.fields.get_field('IPFirstName')
            }
        elif type == 'InterestedParty' and self.fields.get_field('IPRole') not in performer_roles_enum.__members__:
            body = {
                'lastName': self.fields.get_field('IPName'),
                'name': self.fields.get_field('IPFirstName'),
                'role': self.fields.get_field('IPRole')
            } 
        else:
            body = None

        error_codes = ['106', '139']
        if body is not None and self.fields.get_field('IPRole') in error_codes:
            del body['role']

        return body


