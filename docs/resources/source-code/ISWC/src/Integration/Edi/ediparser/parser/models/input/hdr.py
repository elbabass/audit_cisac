from ediparser.parser.models.edi_file import EdiFileVersion
from ediparser.parser.models.input.input_row_definition import InputRowDefinition
from ediparser.parser.models.input.input_transaction import InputTransaction


class HDR(InputTransaction):
    versions_supported = {
        '01.10': EdiFileVersion.ONE,
        '02.00': EdiFileVersion.TWO,
        '02.10': EdiFileVersion.TWO,
        '03.00': EdiFileVersion.THREE
    }

    def __init__(self, transaction: object):
        super().__init__()
        self.fields = InputRowDefinition(transaction, self.__class__.__name__, {
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

    def get_http_verb(self):
        return None

    def get_url(self):
        return None

    def get_parameters(self):
        return None

    def get_body(self):
        body = {
            key: self.fields.get_field(key)
            for key, value in
            self.fields.fields.items()
        }

        body['EdiFileVersion'] = self._get_file_version(
            self.fields.get_field('StandardVersionNumber'))

        return body

    def _get_file_version(self, version_field):
        version = self.versions_supported.get(version_field, None)
        if version is None:
            raise NotImplementedError(
                'EDI version is not supported: ' + version_field)
        return version
