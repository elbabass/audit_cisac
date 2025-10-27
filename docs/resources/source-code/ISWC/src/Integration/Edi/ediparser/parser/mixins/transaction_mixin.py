from ediparser.parser.models.input.cip import CIP
from ediparser.parser.models.input.ctl import CTL
from ediparser.parser.models.input.nat import NAT
from ediparser.parser.models.input.per import PER
from ediparser.parser.models.input.ins import INS
from ediparser.parser.models.input.mli import MLI
from ediparser.parser.models.input.dis import DIS
from ediparser.parser.models.input.der import DER
from ediparser.parser.models.input.pip import PIP
from ediparser.parser.models.input.upi import UPI

from ediparser.parser.models.edi_file import EdiFileVersion
from ediparser.parser.models.input.input_row_definition import InputRowDefinition


class TransactionMixin:
    def __init__(self, transaction, header: dict):
        super().__init__()
        self.transaction = transaction
        self.header = header
        self.fields = InputRowDefinition(transaction, self.__class__.__name__, {
            'RecordType': (1, 3),
            'TransactionSequence#': (4, 8),
            'RecordSequence#': (12, 8),
            'WorkTitle': (20, 60),
            'AgencyCode': (80, 3),
            'AgencyWorkCode': (83, 20),
            'SourceDBCode': (103, 3),
            'PreferredISWC': (106, 11),
            'ArchivedISWC': (117, 11)
        } if header['EdiFileVersion'] == EdiFileVersion.TWO else {
            'RecordType': (1, 3),
            'TransactionSequence#': (4, 8),
            'RecordSequence#': (12, 8),
            'WorkTitle': (20, 100),
            'AgencyCode': (120, 3),
            'AgencyWorkCode': (123, 20),
            'SourceDBCode': (143, 3),
            'PreferredISWC': (146, 11),
            'ArchivedISWC': (157, 11),
            'DerivedWorkType': (168, 20),
            'Disambiguation': (188, 1),
            'DisambiguationReason': (189, 20),
            'bvltr': (209, 1),
            'DeletionReason': (210, 25)
        })

    def get_children(self, transaction_type: str):
        def isNaN(num):
            return num != num

        children = self.transaction.get(transaction_type)

        if children == None or isNaN(children):
            return []

        results = []

        for child in children:
            results.append(globals()[transaction_type](
                {transaction_type: child}, self.header).get_body())

        return results
