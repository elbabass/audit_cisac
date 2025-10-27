from ediparser.parser.models.edi_file import EdiFileVersion
from ediparser.parser.models.output.cwi import CWI


def test_cwi_body_version_three():
    work = {
        "workcode": "880637991",
        "agency": "10",
        "disambiguation": False,
        "originalTitle": "SAFARI",
        "sourcedb": "10",
        "category": "DOM",
        "iswc": 'T9031153170'
    }

    expected_result = 'CWI                010880637991           010T9031153170           '

    cwi = CWI(work,None)

    assert expected_result == cwi.get_record()
