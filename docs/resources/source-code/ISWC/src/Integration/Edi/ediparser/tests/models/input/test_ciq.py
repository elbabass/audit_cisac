from ediparser.parser.models.input.ciq import CIQ
from ediparser.parser.models.edi_file import EdiFileVersion


def test_ciq_get_body_version_two():
    transaction = {
        'CIQ': 'CIQ0000000000000000DIARY OF A LATE EIGHTY BABY                                 021027679059           021           T9254164642'
    }
    header = {
        'SenderID': '000000161',
        'EdiFileVersion': EdiFileVersion.TWO
    }

    expected_result = {
        'agency': '021',
        'workCode': '027679059'
    }

    ciq = CIQ(transaction, header)

    assert expected_result == ciq.get_body()
    assert None == ciq.get_parameters()


def test_ciq_get_body_version_three():
    transaction = {
        'CIQ': 'CIQ0000000000000000DIARY OF A LATE EIGHTY BABY                                                                         021027679059           021           T9254164642                                          '
    }
    header = {
        'SenderID': '000000161',
        'EdiFileVersion': EdiFileVersion.THREE
    }

    expected_result = {
        'agency': '021',
        'workCode': '027679059'
    }

    ciq = CIQ(transaction, header)

    assert expected_result == ciq.get_body()
    assert None == ciq.get_parameters()
