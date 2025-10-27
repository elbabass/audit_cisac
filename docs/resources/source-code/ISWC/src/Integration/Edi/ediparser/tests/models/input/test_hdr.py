import pytest

from ediparser.parser.models.edi_file import EdiFileVersion
from ediparser.parser.models.input.hdr import HDR


def test_hdr_get_body():
    transaction = 'HDRSO000000161MUST                                         02.002019041612030820190416'
    expected_result = {
        'SenderIPNameNumber': '',
        'SenderType': 'SO',
        'OriginalFile': '',
        'CharacterSet': '',
        'SenderID': '000000161',
        'CreationTime': '120308',
        'StandardVersionNumber': '02.00',
        'RecordType': 'HDR',
        'SenderName': 'MUST',
        'CreationDate': '20190416',
        'TransmissionDate': '20190416'
    }

    hdr = HDR({'HDR': transaction})
    body = hdr.get_body()
    del body['EdiFileVersion']

    assert expected_result == body

def test_edi_file_version_two():
    transaction = 'HDRSO000000161MUST                                         02.002019041612030820190416'
    expected_result = EdiFileVersion.TWO

    hdr = HDR({'HDR': transaction})

    assert expected_result == hdr.get_body()['EdiFileVersion']

def test_edi_file_version_three():
    transaction = 'HDRSO000000161MUST                                         03.002019041612030820190416'
    expected_result = EdiFileVersion.THREE

    hdr = HDR({'HDR': transaction})

    assert expected_result == hdr.get_body()['EdiFileVersion']

def test_edi_file_version_incorrect_raises_exception():
    with pytest.raises(NotImplementedError):
        transaction = 'HDRSO000000161MUST                                         04.002019041612030820190416'
        
        HDR({'HDR': transaction}).get_body()