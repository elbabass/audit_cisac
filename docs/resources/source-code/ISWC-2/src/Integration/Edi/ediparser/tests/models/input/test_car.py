from ediparser.parser.models.input.car import CAR
from ediparser.parser.models.edi_file import EdiFileVersion


def test_car_get_body_version_two():
    transaction = {'CAR': 'CAR0000000000000000CHENG SHI LI DE GUANG YOU LIANG YOU MI MANG QUE CAN LAN DE R1611202741395          161',
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

    car = CAR(transaction, header)

    assert expected_result == car.get_body()
    assert None == car.get_parameters()
    assert None == car.get_rejection()


def test_car_get_body_version_three():
    transaction = {'CAR': 'CUR0000000000000000CHENG SHI LI DE GUANG YOU LIANG YOU MI MANG QUE CAN LAN DE R version 3                              1611202741395          161T0302331390           ModifiedVersion     TDIT                 B',
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
        "disambiguateFrom": [
            {
                'iswc': 'T0302331390'
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

    car = CAR(transaction, header)

    assert expected_result == car.get_body()
    assert None == car.get_parameters()
    assert None == car.get_rejection()


def test_car_get_rejection():
    transaction = {'CAR': 'CUR0000000000000000CHENG SHI LI DE GUANG YOU LIANG YOU MI MANG QUE CAN LAN DE R version 3                              1611202741395          161T0302331390           ModifiedVersion     TDIT                 B',
                   'CIP': ['CIP00000000000000030018xx39610             A ',
                           'CIP000000000000000400792350624             C ']}

    header = {
        'SenderID': '000000161',
        'EdiFileVersion': EdiFileVersion.THREE
    }

    expected_result = {
        'code': '_134',
        'message': 'IP Name Number is invalid, must be numeric'
    }

    car = CAR(transaction, header)
    assert expected_result == car.get_rejection()
