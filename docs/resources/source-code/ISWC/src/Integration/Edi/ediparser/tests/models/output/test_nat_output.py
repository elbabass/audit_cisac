from ediparser.parser.models.edi_file import EdiFileVersion
from ediparser.parser.models.output.nat import NAT


def test_nat_get_record_version_three():
    tl = {
        'title': 'ＴＨＥ　ＬＡＳＴ　ＭＥＳＳＡＧＥ海猿よ〜',
        'type': 'OA'
    }

    header = {
        'SenderID': '000000161',
        'EdiFileVersion': EdiFileVersion.THREE
    }

    expected_result = 'NAT                OA  ＴＨＥ　ＬＡＳＴ　ＭＥＳＳＡＧＥ海猿よ〜                                                                                '

    nat = NAT(tl, header)

    assert expected_result == nat.get_record()
