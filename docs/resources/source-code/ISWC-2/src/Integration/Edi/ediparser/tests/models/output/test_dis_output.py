from ediparser.parser.models.output.dis import DIS
from ediparser.parser.models.edi_file import EdiFileVersion


def test_dis_get_record():
    ds = {
        'iswc': 'T0302206116',
    }

    expected_result = 'DIS                T0302206116'

    dis = DIS(ds)

    assert expected_result == dis.get_record()
