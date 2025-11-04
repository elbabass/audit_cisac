from ediparser.parser.models.output.mer import MER
from ediparser.parser.models.input.mer import MER as inputMER
from ediparser.parser.models.edi_file import EdiFileVersion


def test_MER_get_record():
    transaction = {'MER': 'MER0000000000000000DIARY OF A LATE EIGHTY BABY                                                                         021027679059           021T9254164642                                                     ',
                   'MLI': ['MLI0000000000000001T9254164642']}

    header = {
        'SenderID': '000000161',
        'EdiFileVersion': EdiFileVersion.THREE
    }

    original_transaction = inputMER(transaction, header)

    mer_records = MER(None, original_transaction, header).get_record()

    Expected_value = None

    assert Expected_value == mer_records
