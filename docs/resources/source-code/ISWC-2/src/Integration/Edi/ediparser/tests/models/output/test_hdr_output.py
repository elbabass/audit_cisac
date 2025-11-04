from datetime import datetime

from ediparser.parser.models.edi_file import EdiFileVersion
from ediparser.parser.models.output.hdr import HDR


def test_hdr_output_version_three():
    header = {'SenderID': '000000161',
              'EdiFileVersion': EdiFileVersion.THREE}
    current_datetime = datetime.strptime(
        '2017-05-01T01:01:01+0200', "%Y-%m-%dT%H:%M:%S%z")

    expected_result = 'HDRSO000000315CSI CENTER                                   03.002017050101010120170501                                                     '

    hdr = HDR(header, current_datetime)

    assert expected_result == hdr.get_record()


def test_hdr_output_version_two():
    header = {'SenderID': '000000161',
              'EdiFileVersion': EdiFileVersion.TWO}
    current_datetime = datetime.strptime(
        '2017-05-01T01:01:01+0200', "%Y-%m-%dT%H:%M:%S%z")

    expected_result = 'HDRSO000000315CSI CENTER                                   02.002017050101010120170501                                                     '

    hdr = HDR(header, current_datetime)

    assert expected_result == hdr.get_record()
