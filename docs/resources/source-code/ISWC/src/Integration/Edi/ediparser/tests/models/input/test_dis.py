from ediparser.parser.models.input.dis import DIS
from ediparser.parser.models.edi_file import EdiFileVersion


def test_dis_get_body():
    transaction = {'DIS': 'DIS0000000000000002T0302206116'}
    header = {
        'SenderID': '000000161',
        'EdiFileVersion': EdiFileVersion.THREE
    }
    dis = DIS(transaction, header)

    expected_result = {
        'iswc': 'T0302206116',
    }

    assert expected_result == dis.get_body()
