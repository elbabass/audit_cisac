from ediparser.parser.models.input.json.car import CAR


def test_car_get_body_version_three():
    transaction = {
        "submissionId": 0,
        "agency": "21",
        "sourcedb": 21,
        "workcode": "jhgsvdch33yy",
        "category": "DOM",
        "originalTitle": "11111 test title 11111",
        "interestedParties": [
            {
                "nameNumber": 266469723,
                "role": "C"
            }
        ],
        "disambiguation": True,
        "bvltr": 'Background',
        "disambiguationReason": "DIT",
        "disambiguateFrom": [
            {
                "iswc": "T0302331390"
            }
        ]
    }

    file_header = {
        "submittingAgency": "128",
        "submittingSourcedb": 128,
        "fileCreationDateTime": "2019-10-01T18:25:43.511Z",
        "receivingAgency": "315"
    }

    expected_result = {
        "submissionId": 0,
        "submission": {
            "submissionId": 0,
            "bvltr": 'Background',
            "disambiguationReason": "DIT",
            "disambiguateFrom": [
                {
                    'iswc': 'T0302331390'
                }
            ],
            "interestedParties": [
                {
                    "nameNumber": 266469723,
                    "role": "C"
                }
            ],
            "agency": "21",
            "sourcedb": 21,
            "workcode": "jhgsvdch33yy",
            "category": "DOM",
            "originalTitle": "11111 test title 11111",
            "disambiguation": True,
        }}
    expected_result2 = '/submission/batch'

    car = CAR(transaction, file_header)

    assert expected_result == car.get_submission().get('body')
    assert None == car.get_parameters()
    assert expected_result2 == car.get_url()


def test_car_IPvalidation_role_required():
    transaction = {'submission': {'body': {'submission': {
        "submissionId": 0,
        "agency": "21",
        "sourcedb": 21,
        "workcode": "jhgsvdch33yy",
        "category": "DOM",
        "originalTitle": "11111 test title 11111",
        "interestedParties": [
            {
                "nameNumber": 266469723,
                "role": ""
            }
        ],
        "disambiguation": True,
        "bvltr": 'Background',
        "disambiguationReason": "DIT",
        "disambiguateFrom": [
            {
                "iswc": "T0302331390"
            }
        ]
    }}}}

    file_header = {
        "submittingAgency": "128",
        "submittingSourcedb": 128,
        "fileCreationDateTime": "2019-10-01T18:25:43.511Z",
        "receivingAgency": "315"
    },

    expected_result = {'code': '_106', 'message': 'IP Role Code is required'}

    car = CAR(transaction, file_header)

    assert expected_result == car.get_rejection()


def test_car_IPvalidation_role_invalid():
    transaction = {'submission': {'body': {'submission': {
        "submissionId": 0,
        "agency": "21",
        "sourcedb": 21,
        "workcode": "jhgsvdch33yy",
        "category": "DOM",
        "originalTitle": "11111 test title 11111",
        "interestedParties": [
            {
                "nameNumber": 266469723,
                "role": 0
            }
        ],
        "disambiguation": True,
        "bvltr": 'Background',
        "disambiguationReason": "DIT",
        "disambiguateFrom": [
            {
                "iswc": "T0302331390"
            }
        ]
    }}}}

    file_header = {
        "submittingAgency": "128",
        "submittingSourcedb": 128,
        "fileCreationDateTime": "2019-10-01T18:25:43.511Z",
        "receivingAgency": "315"
    },

    expected_result = {'code': '_139', 'message': 'IP Role Code is invalid'}

    car = CAR(transaction, file_header)

    assert expected_result == car.get_rejection()


def test_car_IPvalidation_namenumber_invalid():
    transaction = {'submission': {'body': {'submission': {
        "submissionId": 0,
        "agency": "21",
        "sourcedb": 21,
        "workcode": "jhgsvdch33yy",
        "category": "DOM",
        "originalTitle": "11111 test title 11111",
        "interestedParties": [
            {
                "nameNumber": 'a66469723',
                "role": 0
            }
        ],
        "disambiguation": True,
        "bvltr": 'Background',
        "disambiguationReason": "DIT",
        "disambiguateFrom": [
            {
                "iswc": "T0302331390"
            }
        ]
    }}}}

    file_header = {
        "submittingAgency": "128",
        "submittingSourcedb": 128,
        "fileCreationDateTime": "2019-10-01T18:25:43.511Z",
        "receivingAgency": "315"
    },

    expected_result = {'code': '_134',
                       'message': 'IP Name Number is invalid, must be numeric'}

    car = CAR(transaction, file_header)

    assert expected_result == car.get_rejection()


def test_car_Titlevalidation_type_required():
    transaction = {'submission': {'body': {'submission': {
        "submissionId": 0,
        "agency": "21",
        "sourcedb": 21,
        "workcode": "jhgsvdch33yy",
        "category": "DOM",
        "originalTitle": "11111 test title 11111",
        "interestedParties": [
            {
                "nameNumber": 266469723,
                "role": "C"
            }
        ],
        "otherTitles": [
            {
                "title": "some other title",
                "type": ""
            }
        ],
        "disambiguation": True,
        "bvltr": 'Background',
        "disambiguationReason": "DIT",
        "disambiguateFrom": [
            {
                "iswc": "T0302331390"
            }
        ]
    }}}}

    file_header = {
        "submittingAgency": "128",
        "submittingSourcedb": 128,
        "fileCreationDateTime": "2019-10-01T18:25:43.511Z",
        "receivingAgency": "315"
    },

    expected_result = {'code': '_112',
                       'message': 'Work Title Type is required'}

    car = CAR(transaction, file_header)

    assert expected_result == car.get_rejection()


def test_car_Titlevalidation_type_invalid():
    transaction = {'submission': {'body': {'submission': {
        "submissionId": 0,
        "agency": "21",
        "sourcedb": 21,
        "workcode": "jhgsvdch33yy",
        "category": "DOM",
        "originalTitle": "11111 test title 11111",
        "interestedParties": [
            {
                "nameNumber": 266469723,
                "role": "C"
            }
        ],
        "otherTitles": [
            {
                "title": "some other title",
                "type": 0
            }
        ],
        "disambiguation": True,
        "bvltr": 'Background',
        "disambiguationReason": "DIT",
        "disambiguateFrom": [
            {
                "iswc": "T0302331390"
            }
        ]
    }}}}

    file_header = {
        "submittingAgency": "128",
        "submittingSourcedb": 128,
        "fileCreationDateTime": "2019-10-01T18:25:43.511Z",
        "receivingAgency": "315"
    },

    expected_result = {'code': '_135',
                       'message': 'Work Title Type is invalid'}

    car = CAR(transaction, file_header)

    assert expected_result == car.get_rejection()


def test_car_publisher_header_role():
    transaction = {'submission': {'body': {'submission': {
        "submissionId": 0,
        "agency": "21",
        "sourcedb": 21,
        "workcode": "jhgsvdch33yy",
        "category": "DOM",
        "originalTitle": "11111 test title 11111",
        "interestedParties": [
            {
                "nameNumber": 266469723,
                "role": "C"
            }
        ],
        "disambiguation": True,
        "bvltr": 'Background',
        "disambiguationReason": "DIT",
        "disambiguateFrom": [
            {
                "iswc": "T0302331390"
            }
        ]
    }}}}

    file_header = {
        "submittingAgency": "128",
        "submittingSourcedb": 128,
        "submittingPublisher": {
            "name": "Sony/ATV Music Publishing LLC",
            "nameNumber": 269021863,
            "email": "info@spanishpoint.ie",
            "role": "AM"
        },
        "fileCreationDateTime": "2020-03-31T09:10:43.511Z",
        "receivingAgency": "315"
    }
    file_header2 = {
        "submittingAgency": "128",
        "submittingSourcedb": 128,
        "submittingPublisher": {
            "name": "Sony/ATV Music Publishing LLC",
            "nameNumber": 269021863,
            "email": "info@spanishpoint.ie",
            "role": ""
        },
        "fileCreationDateTime": "2020-03-31T09:10:43.511Z",
        "receivingAgency": "315"
    }
    file_header3 = {
        "submittingAgency": "128",
        "submittingSourcedb": 128,
        "submittingPublisher": {
            "name": "Sony/ATV Music Publishing LLC",
            "nameNumber": 269021863,
            "email": "info@spanishpoint.ie"
        },
        "fileCreationDateTime": "2020-03-31T09:10:43.511Z",
        "receivingAgency": "315"
    }
    expected_result = {
        'submission': {
            'disambiguateFrom': [
                {
                    'iswc': 'T0302331390'
                }
            ],
            'bvltr': 'Background',
            'disambiguationReason': 'DIT',
            'agency': '21',
            'category': 'DOM',
            'disambiguation': True,
            'interestedParties': [
                {
                    'nameNumber': 266469723,
                    'role': 'C'
                },
                {
                    'nameNumber': 269021863,
                    'name': 'Sony/ATV Music Publishing LLC',
                    'role': 'AM'
                }
            ],
            'submissionId': 0,
            'workcode': '',
            'originalTitle': '11111 test title 11111',
            'sourcedb': 21
        }
    }
    expected_result2 = '/allocation/batch'
    car = CAR(transaction, file_header)
    car_body = car.get_body()
    car_body['submission']['workcode'] = ''

    car2 = CAR(transaction, file_header2)
    car2_body = car2.get_body()
    car2_body['submission']['workcode'] = ''

    car3 = CAR(transaction, file_header3)
    car3_body = car3.get_body()
    car3_body['submission']['workcode'] = ''

    assert expected_result == car_body
    assert expected_result == car2_body
    assert expected_result == car3_body
    assert expected_result2 == car.get_url()


def test_car_publisheridentifiers():
    transaction = {
        "submissionId": 0,
        "workcode": "jhgsvdch33yy",
        "category": "DOM",
        "originalTitle": "11111 test title 11111",
        "interestedParties": [
            {
                "nameNumber": 266469723,
                "role": "C"
            }
        ],
        "disambiguation": True,
        "bvltr": 'Background',
        "disambiguationReason": "DIT",
        "disambiguateFrom": [
            {
                "iswc": "T0302331390"
            }
        ]
    }

    file_header = {
        "submittingAgency": "128",
        "submittingSourcedb": 128,
        "submittingPublisher": {
            "name": "Sony/ATV Music Publishing LLC",
            "nameNumber": 269021863,
            "email": "info@spanishpoint.ie",
            "role": "AM"
        },
        "fileCreationDateTime": "2020-03-31T09:10:43.511Z",
        "receivingAgency": "315"
    }

    expected_result = {'submissionId': 0, 'submission': {'originalTitle': '11111 test title 11111', 'interestedParties': [{'role': 'C', 'nameNumber': 266469723}], 'disambiguationReason': 'DIT', 'workcode': '', 'submissionId': 0,
                                                         'category': 'DOM', 'sourcedb': 128, 'agency': '128', 'bvltr': 'Background', 'disambiguation': True, 'additionalIdentifiers': {'publisherIdentifiers': [{'workCode': ['jhgsvdch33yy'], 'nameNumber': 269021863}]}, 'disambiguateFrom': [{'iswc': 'T0302331390'}]}}
    car = CAR(transaction, file_header)
    car_sub = car.get_submission().get('body')
    car_sub['submission']['workcode'] = ''
    assert expected_result == car_sub


def test_car_ias_generated_workcode():
    transaction = {
        "submissionId": 0,
        "workcode": "jhgsvdch33yy",
        "category": "DOM",
        "originalTitle": "11111 test title 11111",
        "interestedParties": [
            {
                "nameNumber": 266469723,
                "role": "C"
            }
        ],
        "disambiguation": True,
        "bvltr": 'Background',
        "disambiguationReason": "DIT",
        "disambiguateFrom": [
            {
                "iswc": "T0302331390"
            }
        ]
    }
    file_header = {
        "submittingAgency": "128",
        "submittingSourcedb": 128,
        "submittingPublisher": {
            "name": "Sony/ATV Music Publishing LLC",
            "nameNumber": 269021863,
            "email": "info@spanishpoint.ie",
            "role": "AM"
        },
        "fileCreationDateTime": "2020-03-31T09:10:43.511Z",
        "receivingAgency": "315"
    }
    car = CAR(transaction, file_header)
    car_sub_workcode = car.get_submission().get('body')['submission']['workcode']
    workcode_prefix = car_sub_workcode[:2]
    assert 'AS' == workcode_prefix
    assert 20 == car_sub_workcode.__len__()


def test_car_locally_allocated_iswc():
    transaction = {
        "submissionId": 0,
        "agency": "21",
        "sourcedb": 21,
        "workcode": "jhgsvdch33yy",
        "category": "DOM",
        "originalTitle": "11111 test title 11111",
        "interestedParties": [
            {
                "nameNumber": 266469723,
                "role": "C"
            }
        ],
        "locallyAllocatedIswc": "T0302331390"
    }

    file_header = {
        "submittingAgency": "128",
        "submittingSourcedb": 128,
        "fileCreationDateTime": "2019-10-01T18:25:43.511Z",
        "receivingAgency": "315"
    }

    expected_result = {
        "submissionId": 0,
        "submission": {
            "submissionId": 0,
            "interestedParties": [
                {
                    "nameNumber": 266469723,
                    "role": "C"
                }
            ],
            "agency": "21",
            "sourcedb": 21,
            "workcode": "jhgsvdch33yy",
            "category": "DOM",
            "originalTitle": "11111 test title 11111",
            "iswc": "T0302331390",
            "disableAddUpdateSwitching": True,
            "allowProvidedIswc": True,
            "locallyAllocatedIswc": "T0302331390"
        }}
    expected_result2 = '/submission/batch'

    car = CAR(transaction, file_header)

    car.transaction = {**car.transaction, **car.get_from_local_range_data()}

    assert expected_result == car.get_submission().get('body')
    assert None == car.get_parameters()
    assert expected_result2 == car.get_url()
