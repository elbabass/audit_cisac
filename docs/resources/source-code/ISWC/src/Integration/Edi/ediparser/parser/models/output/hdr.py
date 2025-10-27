import requests
from ediparser.parser.models.input.input_transaction import InputTransaction
from ediparser.parser.models.output.output_transaction import OutputTransaction
from ediparser.parser.models.output.output_row_definition import OutputRowDefinition
from ediparser.parser.models.edi_file import EdiFileVersion


class HDR(OutputTransaction):
    def __init__(self, header: dict, current_datetime):
        super().__init__(None, None)
        self.header = header
        self.current_datetime = current_datetime
        self.fields = OutputRowDefinition({
            'RecordType': (1, 3),
            'SenderType': (4, 2),
            'SenderID': (6, 9),
            'SenderName': (15, 45),
            'StandardVersionNumber': (60, 5),
            'CreationDate': (65, 8),
            'CreationTime': (73, 6),
            'TransmissionDate': (79, 8),
            'CharacterSet': (87, 15),
            'OriginalFile': (102, 27),
            'SenderIPNameNumber': (129, 11)
        })

    def get_record(self):
        record = {
            'RecordType': self.__class__.__name__,
            'SenderType': 'SO',
            'SenderID': '315'.rjust(9, '0'),
            'SenderName': 'CSI CENTER'.ljust(45),
            'StandardVersionNumber': '02.00' if self.header['EdiFileVersion'] == EdiFileVersion.TWO else '03.00',
            'CreationDate': self.current_datetime.strftime('%Y%m%d'),
            'CreationTime': self.current_datetime.strftime('%H%M%S'),
            'TransmissionDate': self.current_datetime.strftime('%Y%m%d'),
            'CharacterSet': '',
            'OriginalFile': '',
            'SenderIPNameNumber': ''
        }
        return self.serialize_record(record)

    def get_child_records(self):
        pass
