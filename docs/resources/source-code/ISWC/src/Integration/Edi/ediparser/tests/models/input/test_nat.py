from ediparser.parser.models.input.nat import NAT
from ediparser.parser.models.edi_file import EdiFileVersion


def test_nat_get_body():
    transaction = {
        'NAT': 'NAT0000000000000002ATENJ W T - HERE COMES THE HOLIDAYS NAT'}
    header = {
        'SenderID': '000000161',
        'EdiFileVersion': EdiFileVersion.THREE
    }
    nat = NAT(transaction, header)

    expected_result = {
        'title': 'J W T - HERE COMES THE HOLIDAYS NAT',
        'type': 'AT'
    }

    assert expected_result == nat.get_body()
