from ediparser.parser.models.input.json.ciq import CIQ


def test_cmq_get_body_version_three():
    transaction = {
        "submissionId": 0,
        "agency": "003",
        "workcode": "00112200"
    }

    file_header = {
        "submittingAgency": "128",
        "submittingSourcedb": 128,
        "fileCreationDateTime": "2019-10-01T18:25:43.511Z",
        "receivingAgency": "315"
    },

    expected_result_body = {
        "agency": "003",
        "workCode": "00112200"
    }

    expected_result_submisison_id = 0

    ciq = CIQ(transaction, file_header)

    assert expected_result_body == ciq.get_submission().get('body')
    assert expected_result_submisison_id == ciq.get_submission().get('submissionId')
    assert None == ciq.get_parameters()
