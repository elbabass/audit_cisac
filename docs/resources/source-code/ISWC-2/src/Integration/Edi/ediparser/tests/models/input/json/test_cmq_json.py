from ediparser.parser.models.input.json.cmq import CMQ


def test_cmq_get_body_version_three():
    transaction = {
        "submissionId": 0,
        "iswc": "T0302331390"
    }

    file_header = {
        "submittingAgency": "128",
        "submittingSourcedb": 128,
        "fileCreationDateTime": "2019-10-01T18:25:43.511Z",
        "receivingAgency": "315"
    },

    expected_result_body = {
        "iswc": "T0302331390"
    }

    expected_result_submission_id = 0

    cmq = CMQ(transaction, file_header)

    assert expected_result_body == cmq.get_submission().get('body')
    assert expected_result_submission_id == cmq.get_submission().get('submissionId')
    assert None == cmq.get_parameters()
