from ediparser.parser.models.input.json.wft import WFT

def test_wft_get_body_version_three():
    transaction = {
        "submissionId": 1,
		"taskId": 1349,
		"status": "Approved",
		"workflowType": 1
    }

    file_header = {
        "submittingAgency": "128",
        "submittingSourcedb": 128,
        "fileCreationDateTime": "2019-10-01T18:25:43.511Z",
        "receivingAgency": "315"
    }

    expected_result = {
        "taskId": 1349,
        "workflowType": "UpdateApproval",
        "status": "Approved"
    }

    wft = WFT(transaction, file_header)

    assert expected_result == wft.get_submission().get('body')

def test_wft_get_parameters_version_three():
    transaction = {
        "submissionId": 1,
		"taskId": 1349,
		"status": "Approved",
		"workflowType": 1
    }

    file_header = {
        "submittingAgency": "128",
        "submittingSourcedb": 128,
        "fileCreationDateTime": "2019-10-01T18:25:43.511Z",
        "receivingAgency": "315"
    }

    expected_result = {
        "agency": "128"
    }

    wft = WFT(transaction, file_header)

    assert expected_result == wft.get_submission().get('parameters')
