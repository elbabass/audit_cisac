from ediparser.parser.models.input.cmq import CMQ
from ediparser.parser.models.edi_file import EdiFileVersion


def test_cmq_get_body_version_two():
    transaction = {
        'CMQ': 'CMQ0000000000000000                                                            000                    000T9031153170           '
    }
    header = {'SenderID': '000000161',
              'EdiFileVersion': EdiFileVersion.TWO}

    expected_result = {
        "iswc": "T9031153170"
    }

    cmq = CMQ(transaction, header)

    assert expected_result == cmq.get_body()
    assert None == cmq.get_parameters()


def test_cmq_get_body_version_three():
    transaction = {
        'CMQ': 'CMQ0000000000000000                                                                                                    000                    000T9031153170                                                     '
    }
    header = {'SenderID': '000000161',
              'EdiFileVersion': EdiFileVersion.THREE}

    expected_result = {
        "iswc": "T9031153170"
    }

    cmq = CMQ(transaction, header)

    assert expected_result == cmq.get_body()
    assert None == cmq.get_parameters()
