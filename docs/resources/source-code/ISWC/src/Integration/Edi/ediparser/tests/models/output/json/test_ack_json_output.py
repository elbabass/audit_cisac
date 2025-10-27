from datetime import datetime

from requests.models import Response

from ediparser.parser.models.edi_file import EdiFileVersion
from ediparser.parser.models.input.json.cmq import CMQ
from ediparser.parser.models.output.json.ack import ACK_JSON


def test_ack_body_version_three():
    header = {
        "submittingAgency": "128",
        "submittingSourcedb": 128,
        "fileCreationDateTime": "2019-10-01T18:25:43.511Z",
        "receivingAgency": "315"
    }

    input_transaction = {
        'OriginalTransactionType': 'CMQ',
        'OriginalSubmissionId': '0',
    }

    api_response = Response()
    api_response._content = b'{"searchResults": [{"iswc":"T9031238434","iswcStatus":"Preferred","agency":"315","originalTitle":"THE BREATH AND THE BELL","otherTitles":[],"interestedParties":[], "works":[], "linkedISWC":[]}]}'
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
            'originalTransactionType': 'SearchByIswcSubmission',
            'otherTitles': [],
            'iswc': 'T9031238434',
            'iswcStatus': 'Preferred',
            'processingDate': '2017-05-01 01:01:01+02:00',
            'submissionId': 1000,
            'transactionStatus': 'FullyAccepted',
            'workInfo': [],
            'linkedISWC':[],
            'parentISWC': None,
            'overallParentISWC': None
        }]

    ack = ACK_JSON('1', header, input_transaction, api_response.json(), api_response.ok,
                   api_response.status_code, current_datetime).get_record(1)

    assert expected_result == ack


def test_ack_attribute_error():
    header = {
        "submittingAgency": "128",
        "submittingSourcedb": 128,
        "fileCreationDateTime": "2019-10-01T18:25:43.511Z",
        "receivingAgency": "315"
    }

    input_transaction = {
        'OriginalTransactionType': 'CAR',
        'OriginalSubmissionId': '0',
    }

    api_response = Response()
    api_response._content = b'{"submission":"a"}'

    api_response.status_code = 207

    creation_datetime = datetime.strptime(
        '2017-05-01T01:01:01+0200', "%Y-%m-%dT%H:%M:%S%z")
    current_datetime = datetime.strptime(
        '2017-05-01T01:01:01+0200', "%Y-%m-%dT%H:%M:%S%z")

    expected_result = [{'errorMessages': {'code': 500,
                                          'message': 'Internal Server Error'},
                        'originalFileCreationDateTime': '2019-10-01T18:25:43.511Z',
                        'originalSubmissionId': '0',
                        'originalTransactionType': 'AddSubmission',
                        'processingDate': '2017-05-01 01:01:01+02:00',
                        'submissionId': 1000,
                        'transactionStatus': 'Rejected'}]

    ack = ACK_JSON('1', header, input_transaction, api_response.json(), api_response.ok,
                   api_response.status_code, current_datetime).get_record(1)

    assert expected_result == ack

def test_ack_cdr_transaction_response_ok_false_return_status_rejected():
    header = {
        "submittingAgency": "010",
        "submittingSourcedb": 10,
        "fileCreationDateTime": "2025-05-01T13:38:57.867219",
        "receivingAgency": "010"
    }

    input_transaction = {
        'OriginalTransactionType': 'CDR',
        'OriginalSubmissionId': '1',
        'OriginalFileCreationDate': '2025-05-01T13:38:57.867219'
    }

    api_response = Response()
    api_response._content = b'{"code": "130", "message": "The Agency Work Number provided does not exist in the ISWC Database "}'
    api_response.status_code = 400  
    
    current_datetime = datetime.strptime(
        '2025-05-13T14:43:05.743659', "%Y-%m-%dT%H:%M:%S.%f")

    # Expected result for a rejected delete transaction
    expected_ack_output = [{
        'originalFileCreationDateTime': '2025-05-01T13:38:57.867219',
        'originalSubmissionId': '1',
        'processingDate': '2025-05-13 14:43:05.743659',
        'transactionStatus': 'Rejected',
        'submissionId': 1001,
        'errorMessages': {
            'code': 400,
            'message': {
                'code': '130', 
                'message': 'The Agency Work Number provided does not exist in the ISWC Database '
            }
        },
        'originalTransactionType': 'DeleteSubmission',
    }]

    # Call the ACK_JSON class with the error response
    ack_json = ACK_JSON('deleteSubmissions', header, input_transaction, 
                  api_response.json(), False, api_response.status_code, 
                  current_datetime).get_record(1)

    assert expected_ack_output[0]['transactionStatus'] == ack_json[0]['transactionStatus']

def test_ack_cdr_transaction_if_response_ok_true_for_400_transaction():
    header = {
        "submittingAgency": "010",
        "submittingSourcedb": 10,
        "fileCreationDateTime": "2025-05-01T13:38:57.867219",
        "receivingAgency": "010"
    }

    input_transaction = {
        'OriginalTransactionType': 'CDR',
        'OriginalSubmissionId': '1',
        'OriginalFileCreationDate': '2025-05-01T13:38:57.867219'
    }

    api_response = Response()
    api_response._content = b'{"code": "130", "message": "The Agency Work Number provided does not exist in the ISWC Database "}'
    api_response.status_code = 400  
    
    current_datetime = datetime.strptime(
        '2025-05-13T14:43:05.743659', "%Y-%m-%dT%H:%M:%S.%f")

    # Expected result for a rejected delete transaction
    expected_ack_output = [{
        'originalFileCreationDateTime': '2025-05-01T13:38:57.867219',
        'originalSubmissionId': '1',
        'processingDate': '2025-05-13 14:43:05.743659',
        'transactionStatus': 'Rejected',
        'submissionId': 1001,
        'errorMessages': {
            'code': 400,
            'message': {
                'code': '130', 
                'message': 'The Agency Work Number provided does not exist in the ISWC Database '
            }
        },
        'originalTransactionType': 'DeleteSubmission',
    }]

    # Call the ACK_JSON class with the error response
    ack_json = ACK_JSON('deleteSubmissions', header, input_transaction, 
                  api_response.json(), True, api_response.status_code, 
                  current_datetime).get_record(1)

    assert expected_ack_output[0]['transactionStatus'] != ack_json[0]['transactionStatus']

