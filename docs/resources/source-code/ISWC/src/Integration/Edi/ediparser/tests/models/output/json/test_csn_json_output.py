from datetime import datetime

from requests.models import Response

from ediparser.parser.models.edi_file import EdiFileVersion
from ediparser.parser.models.input.json.cmq import CMQ
from ediparser.parser.models.output.json.csn import CSN_JSON


def test_csn_json_body():
    header = {
        "submittingAgency": "128",
        "submittingSourcedb": 128,
        "fileCreationDateTime": "2019-10-01T18:25:43.511Z",
        "receivingAgency": "315"
    }

    input_transaction = {
        'OriginalTransactionType': 'CUR',
        'OriginalSubmissionId': '0',
        'TransactionType': 'CMQ',
        'WorkflowTaskID': 0,
        'WorkflowStatus': '0',
        'AgencyCode': '315'
    }

    api_response = Response()
    api_response._content = b'{"searchResults": [{"iswc":"T9031238434","iswcStatus":"Preferred","agency":"315","originalTitle":"THE BREATH AND THE BELL","otherTitles":[],"linkedISWC":[],"interestedParties":[], "works":[]}]}'
    api_response.status_code = 207

    creation_datetime = datetime.strptime(
        '2017-05-01T01:01:01+0200', "%Y-%m-%dT%H:%M:%S%z")
    current_datetime = datetime.strptime(
        '2017-05-01T01:01:01+0200', "%Y-%m-%dT%H:%M:%S%z")

    expected_result = [
        {
            'agency': '315',
            'interestedParties': [],
            'originalFileCreationDateTime': '2019-10-01T18:25:43.511Z',
            'originalSubmissionId': '0',
            'originalTitle': 'THE BREATH AND THE BELL',
            'originalTransactionType': 'UpdateSubmission',
            'otherTitles': [],
            'iswc': 'T9031238434',
            'iswcStatus': 'Preferred',
            'processingDate': '2017-05-01 01:01:01+02:00',
            'submissionId': 1000,
            'transactionStatus': 'FullyAccepted',
            'workInfo': [],
            'workflowTaskId': 0,
            'workflowStatus': '0',
            'workflowMessage': None,
            'linkedISWC':[],
            'parentISWC': None,
            'overallParentISWC': None
        }]

    csn = CSN_JSON('1', header, input_transaction, api_response.json(), api_response.ok,
                   api_response.status_code, current_datetime).get_record(1)

    assert expected_result == csn
