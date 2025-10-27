from ediparser.parser.models.input.cur import CUR
from ediparser.parser.models.edi_file import EdiFileVersion


def test_cur_get_body_version_two():
    transaction = {'CUR': 'CUR0000000000000000CHENG SHI LI DE GUANG YOU LIANG YOU MI MANG QUE CAN LAN DE R1611202741395          161',
                   'CTL': ['CTL0000000000000001CHENG SHI LI DE GUANG YOU LIANG YOU MI MANG QUE CAN LAN DE  AT'],
                   'CIP': ['CIP000000000000000100187739610             A ',
                           'CIP000000000000000200792350624             C ']}
    header = {
        'SenderID': '000000161',
        'EdiFileVersion': EdiFileVersion.TWO
    }

    expected_result = {
        "otherTitles": [
            {'title': 'CHENG SHI LI DE GUANG YOU LIANG YOU MI MANG QUE CAN LAN DE',
             'type': 'AT'
             }
        ],
        "interestedParties": [
            {
                "role": "A",
                "baseNumber": "",
                "nameNumber": 187739610
            },
            {
                "role": "C",
                "baseNumber": "",
                "nameNumber": 792350624
            }
        ],
        "workcode": "1202741395",
        "agency": "161",
        "disambiguation": False,
        "originalTitle": "CHENG SHI LI DE GUANG YOU LIANG YOU MI MANG QUE CAN LAN DE R",
        "sourcedb": "161",
        "category": "DOM",
        "preferredIswc": ""

    }

    expected_parameters = {'preferredIswc': ''}

    cur = CUR(transaction, header)

    assert expected_result == cur.get_body()
    assert expected_parameters == cur.get_parameters()
    assert None == cur.get_rejection()


def test_cur_get_body_version_three():
    transaction = {'CUR': 'CUR0000000000000000CHENG SHI LI DE GUANG YOU LIANG YOU MI MANG QUE CAN LAN DE R version 3                              1611202741395          161T0302331390           ModifiedVersion     TDIT                 B',
                   'CTL': ['CTL0000000000000001CHENG SHI LI DE GUANG YOU LIANG YOU MI MANG QUE CAN LAN DE                                          AT'],
                   'NAT': ['NAT0000000000000002ATENCHENG SHI LI DE GUANG YOU LIANG YOU MI MANG QUE CAN LAN DE N'],
                   'CIP': ['CIP000000000000000300187739610             A ',
                           'CIP000000000000000400792350624             C '],
                   'PER': ['PER0000000000000005                                              Paul                                            Hewson'],
                   'INS': ['INS0000000000000006ALP'],
                   'DIS': ['DIS0000000000000007T0302331390']}

    header = {
        'SenderID': '000000161',
        'EdiFileVersion': EdiFileVersion.THREE
    }

    expected_result = {
        "bvltr": 'Background',
        "disambiguationReason": "DIT",
        "sourcedb": "128",
        "disambiguateFrom": [
            {
                "iswc": "T0302331390"
            }
        ],
        "otherTitles": [
            {'title': 'CHENG SHI LI DE GUANG YOU LIANG YOU MI MANG QUE CAN LAN DE',
             'type': 'AT'
             },
            {'title': 'CHENG SHI LI DE GUANG YOU LIANG YOU MI MANG QUE CAN LAN DE N',
             'type': 'AT'
             }
        ],
        "interestedParties": [
            {
                "role": "A",
                "baseNumber": "",
                "nameNumber": 187739610
            },
            {
                "role": "C",
                "baseNumber": "",
                "nameNumber": 792350624
            }
        ],
        "agency": "161",
        "sourcedb": "161",
        "derivedWorkType": 'ModifiedVersion',
        "workcode": "1202741395",
        "category": "DOM",
        "originalTitle": "CHENG SHI LI DE GUANG YOU LIANG YOU MI MANG QUE CAN LAN DE R version 3",
        "preferredIswc": "T0302331390",
        "instrumentation": [
            {
                "code": "ALP"
            }
        ],
        "disambiguation": True,
        "performers": [
            {
                "firstName": "Paul",
                "lastName": "Hewson"
            }
        ],
        "derivedFromIswcs": []
    }

    expected_parameters = {'preferredIswc': 'T0302331390'}

    cur = CUR(transaction, header)

    assert expected_result == cur.get_body()
    assert expected_parameters == cur.get_parameters()
    assert None == cur.get_rejection()


def test_cur_get_rejection():
    transaction = {'CUR': 'CUR0000000000000000CHENG SHI LI DE GUANG YOU LIANG YOU MI MANG QUE CAN LAN DE R version 3                              1611202741395          161T0302331390',
                   'CTL': ['CTL0000000200000001CRACK WAS NINETY IN THE IS                                                                          ZQ']}

    header = {
        'SenderID': '000000161',
        'EdiFileVersion': EdiFileVersion.THREE
    }

    expected_result = {
        'code': '_135',
        'message': 'Work Title Type is invalid'
    }

    cur = CUR(transaction, header)

    assert expected_result == cur.get_rejection()
