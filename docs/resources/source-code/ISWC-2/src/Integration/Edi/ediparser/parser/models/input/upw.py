from ediparser.parser.models.input.pip import PIP
from ediparser.parser.models.input.upi import UPI
from ediparser.parser.models.input.input_row_definition import InputRowDefinition
from ediparser.parser.models.input.input_transaction import InputTransaction

class UPW(InputTransaction):
    def __init__(self, transaction: object, header: dict):
        super().__init__()
        self.transaction = transaction
        self.header = header
        self.fields = InputRowDefinition(transaction, self.__class__.__name__, {
            'RecordType': (1, 3),
            'TransactionSequence#': (4, 8),
            'RecordSequence#': (12, 8),
            'WorkTitle': (20, 60)
        })

    def get_http_verb(self):
        return 'POST'

    def get_url(self):
        return '/usage/search/batch'

    def get_parameters(self):
        None

    def get_body(self):  
        body = {}

        body['titles'] = [{
            'title': self.fields.get_field('WorkTitle'),
            'type': 'OT'
        }]

        body['interestedParties'] = self.get_children('PIP', 'InterestedParty')

        body['performers'] = self.get_children('PIP', 'Performer')

        return body

    def get_reference_number(self):
        referenceNumber = 0
        
        upi_records = self.get_children(transaction_type = 'UPI') 
        if (len(upi_records) > 0):
            referenceNumber = upi_records[0].get('referenceNumber')

        return referenceNumber

    def get_children(self, transaction_type: str, type: str = None):
        def isNaN(num):
            return num != num

        children = self.transaction.get(transaction_type)

        if children == None or isNaN(children):
            return []

        results = []

        for child in children:
            result = globals()[transaction_type]({transaction_type: child}, self.header).get_body(type)
            if result != None:
                results.append(result)

        return results