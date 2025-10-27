from ediparser.parser.models.input.json.cur import CUR


def test_car_get_body_version_three():
    transaction = {
        "submissionId": 0,
        "preferredIswc": "T0302331390",
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
        ]
    }

    file_header = {
        "submittingAgency": "128",
        "submittingSourcedb": 128,
        "fileCreationDateTime": "2019-10-01T18:25:43.511Z",
        "receivingAgency": "315"
    },

    expected_result = {
        "submissionId": 0,
        "submission": {
            "submissionId": 0,
            "preferredIswc": "T0302331390",
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
            "originalTitle": "11111 test title 11111"
        }
    }

    cur = CUR(transaction, file_header)

    assert expected_result == cur.get_submission().get('body')
    assert None == cur.get_parameters()


def test_cur_IPvalidation_role_required():
    transaction = {'submission': {'body': {'submission': {
        "submissionId": 1005,
        "preferredIswc": "T0029584960",
        "agency": "128",
        "sourcedb": 128,
        "workcode": "122453536355zycb",
        "category": "DOM",
        "originalTitle": "A Bunch of Wild Thyme 123",
        "interestedParties": [
            {
                    "nameNumber": 589238793,
                    "role": ""
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

    cur = CUR(transaction, file_header)

    assert expected_result == cur.get_rejection()


def test_cur_IPvalidation_role_invalid():
    transaction = {'submission': {'body': {'submission': {
        "submissionId": 1005,
        "preferredIswc": "T0029584960",
        "agency": "128",
        "sourcedb": 128,
        "workcode": "122453536355zycb",
        "category": "DOM",
        "originalTitle": "A Bunch of Wild Thyme 123",
        "interestedParties": [
            {
                    "nameNumber": 589238793,
                    "role": 0
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

    cur = CUR(transaction, file_header)

    assert expected_result == cur.get_rejection()


def test_cur_IPvalidation_namenumber_invalid():
    transaction = {'submission': {'body': {'submission': {
        "submissionId": 1005,
        "preferredIswc": "T0029584960",
        "agency": "128",
        "sourcedb": 128,
        "workcode": "122453536355zycb",
        "category": "DOM",
        "originalTitle": "A Bunch of Wild Thyme 123",
        "interestedParties": [
            {
                    "nameNumber": 'a589238793',
                    "role": "CA"
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

    cur = CUR(transaction, file_header)

    assert expected_result == cur.get_rejection()


def test_cur_Titlevalidation_type_required():
    transaction = {'submission': {'body': {'submission': {
        "submissionId": 1005,
        "preferredIswc": "T0029584960",
        "agency": "128",
        "sourcedb": 128,
        "workcode": "122453536355zycb",
        "category": "DOM",
        "originalTitle": "A Bunch of Wild Thyme 123",
        "interestedParties": [
            {
                    "nameNumber": 589238793,
                    "role": "CA"
            }
        ],
        "otherTitles": [
            {
                "title": "some other title",
                "type": ""
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

    cur = CUR(transaction, file_header)

    assert expected_result == cur.get_rejection()


def test_cur_Titlevalidation_type_invalid():
    transaction = {'submission': {'body': {'submission': {
        "submissionId": 1005,
        "preferredIswc": "T0029584960",
        "agency": "128",
        "sourcedb": 128,
        "workcode": "122453536355zycb",
        "category": "DOM",
        "originalTitle": "A Bunch of Wild Thyme 123",
        "interestedParties": [
            {
                    "nameNumber": 589238793,
                    "role": "CA"
            }
        ],
        "otherTitles": [
            {
                "title": "some other title",
                "type": 0
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

    cur = CUR(transaction, file_header)

    assert expected_result == cur.get_rejection()
