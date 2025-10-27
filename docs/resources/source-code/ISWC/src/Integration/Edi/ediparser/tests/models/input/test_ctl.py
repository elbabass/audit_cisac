from ediparser.parser.models.input.ctl import CTL
from ediparser.parser.models.edi_file import EdiFileVersion

def test_ctl_get_body_version_two():
    transaction={'CTL':'CTL0000006500000001PHATMATRIX LOUNGE 3                                         OA'}
    header = {
        'SenderID': '000000161',
        'EdiFileVersion': EdiFileVersion.TWO
    }
    ctl=CTL(transaction,header)

    expected_result={
            'title': 'PHATMATRIX LOUNGE 3',
            'type': 'OA'
            }

    assert expected_result==ctl.get_body()

def test_ctl_get_body_version_three():
    transaction={'CTL':'CTL0000006500000001PHATMATRIX LOUNGE 3                                                                                 OA'}
    header = {
        'SenderID': '000000161',
        'EdiFileVersion': EdiFileVersion.THREE
    }
    ctl=CTL(transaction,header)

    expected_result={
            'title': 'PHATMATRIX LOUNGE 3',
            'type': 'OA'
            }

    assert expected_result==ctl.get_body()

