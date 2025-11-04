from ediparser.parser.models.input.json.cdr import CDR


def test_cdr_get_body_version_three():
    transaction = {
        "submissionId": 4,
        "preferredIswc": "T0302332075",
        "agency": "128",
        "sourcedb": 128,
        "workcode": "R28995186",
        "reasonCode": "Reason"
    }

    file_header = {
        "submittingAgency": "128",
        "submittingSourcedb": 128,
        "fileCreationDateTime": "2019-10-01T18:25:43.511Z",
        "receivingAgency": "315"
    },

    expected_result_parameters = {
        "preferredIswc": "T0302332075",
        "agency": "128",
        "workcode": "R28995186",
        "sourceDb": 128,
        "reasonCode": "Reason"
    }

    expected_result_submission_id = 4

    cdr = CDR(transaction, file_header)

    assert None == cdr.get_body()
    assert expected_result_submission_id == cdr.get_submission().get('submissionId')
    assert expected_result_parameters == cdr.get_submission().get('parameters')