import requests

from ediparser.parser.models.input.input_transaction import InputTransaction
from ediparser.parser.models.output.cip import CIP
from ediparser.parser.models.output.per import PER
from ediparser.parser.models.output.ins import INS
from ediparser.parser.models.output.der import DER
from ediparser.parser.models.output.ctl import CTL
from ediparser.parser.models.output.dis import DIS
from ediparser.parser.models.output.nat import NAT
from ediparser.parser.models.output.output_row_definition import \
    OutputRowDefinition
from ediparser.parser.models.output.output_transaction import OutputTransaction
from ediparser.parser.models.edi_file import EdiFileVersion


class CWI(OutputTransaction):
    def __init__(self, work: dict, header: dict):
        super().__init__(None, None)
        self.work = work
        self.header = header
        self.fields = OutputRowDefinition({
            'RecordType': (1, 3),
            'TransactionSequence#': (4, 8),
            'RecordSequence#': (12, 8),
            'AgencyCode': (20, 3),
            'AgencyWorkCode': (23, 20),
            'SourceDBCode': (43, 3),
            'PreferredISWC': (46, 11),
            'ArchivedISWC': (57, 11)
        })

    def get_record(self):
        record = {
            'RecordType': self.__class__.__name__,
            'AgencyCode': self.work['agency'].rjust(3, '0'),
            'AgencyWorkCode': self.work['workcode'],
            'SourceDBCode': str(self.work.get('sourcedb')).rjust(3, '0'),
            'PreferredISWC': self.work['iswc'],
            'ArchivedISWC': self.work.get('archivedIswc', '')
        }
        return self.serialize_record(record)

    def get_child_records(self):
        records = []
        keys = sorted(self.work.keys())
        for key in keys:
            child_transaction = self.get_child_transaction_type(key)
            if child_transaction != None:
                for child in self.work[key]:
                    if child_transaction == CTL:
                        try:
                            child['title'].encode('ascii')
                            records.append(
                                CTL(child, self.header).get_record())
                        except:
                            if self.header['EdiFileVersion'] == EdiFileVersion.TWO:
                                continue
                            records.append(
                                NAT(child, self.header).get_record())
                    else:
                        records.append(child_transaction(child).get_record())
        return records

    def get_child_transaction_type(self, key: str):
        return {
            'interestedParties': CIP,
            'performers': PER,
            'instrumentation': INS,
            'derivedFromIswcs': DER,
            'otherTitles': CTL,
            'disambiguateFrom': DIS
        }.get(key)
