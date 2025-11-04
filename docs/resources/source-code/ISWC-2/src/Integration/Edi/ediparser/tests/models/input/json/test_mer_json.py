from ediparser.parser.models.input.json.mer import MER


def test_mer_get_body_version_three():
    transaction = {
        "submissionId": 7,
        "agency": "128",
        "sourcedb": 128,
        "workcode": "R28995187",
        "preferredIswc": "T0302332086",
        "mergeIswcs": [
            "T0302331403",
            "T0302331414"
        ]
    }

    file_header = {
        "submittingAgency": "128",
        "submittingSourcedb": 128,
        "fileCreationDateTime": "2019-10-01T18:25:43.511Z",
        "receivingAgency": "315"
    },

    expected_result_body = {
        "iswcs": [
            "T0302331403",
            "T0302331414"
        ],
    }

    mer = MER(transaction, file_header)
    
    assert expected_result_body == mer.get_submission().get('body')

def test_mer_get_parameters_version_three():
    transaction = {
        "submissionId": 7,
        "agency": "128",
        "sourcedb": 128,
        "workcode": "R28995187",
        "preferredIswc": "T0302332086",
        "mergeIswcs": [
            "T0302331403",
            "T0302331414"
        ]
    }

    file_header = {
        "submittingAgency": "128",
        "submittingSourcedb": 128,
        "fileCreationDateTime": "2019-10-01T18:25:43.511Z",
        "receivingAgency": "315"
    },

    expected_result_parameters = {
        "preferredIswc": "T0302332086",
        "agency": "128"
    }

    mer = MER(transaction, file_header)
    
    assert expected_result_parameters == mer.get_submission().get('parameters')
