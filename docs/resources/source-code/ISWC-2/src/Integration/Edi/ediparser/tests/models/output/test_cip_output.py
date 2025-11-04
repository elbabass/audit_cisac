from ediparser.parser.models.edi_file import EdiFileVersion
from ediparser.parser.models.output.cip import CIP


def test_cip_body_version_three():
    ip = {
        "baseNumber": 'I-002234809-8',
        "nameNumber": 463130289,
        "role": 'C'
    }

    expected_result = 'CIP                463130289  I-002234809-8C '

    cip = CIP(ip)

    assert expected_result == cip.get_record()
