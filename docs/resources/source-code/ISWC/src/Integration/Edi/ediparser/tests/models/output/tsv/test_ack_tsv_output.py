from datetime import datetime
from requests.models import Response
from ediparser.parser.models.output.tsv.ack import ACK_TSV


def test_ack_body():
    header = {}

    original_transaction = {'OriginalSubmissionId': '4', 'OriginalTransactionType': 'CAR', 'originalTitle': 'Sample Work Title 5', 'submittingAgency': '038', 'submittingPublisheremail': 'info@spanishpoint.ie',
                            'submittingPublishername': 'Sony/ATV Music Publishing LLC', 'submittingPublishernameNumber': '589330613', 'submittingPublisherrole': 'AM', 'submittingSourcedb': '038', 'workcode': 'SONY10001'}

    api_response = Response()
    api_response._content = b'{"submissionId": 1, "submission": {"verifiedSubmission": {"id": 11921751293,"iswc": "T9800834202","iswcEligible": true,"deleted": false,"createdDate": "2020-07-13T15:48:02.8961155+00:00","lastModifiedDate": "2020-07-13T15:48:02.8961155+00:00","agency": "128","sourcedb": 128,"workcode": "SONY10001","category": "DOM","disambiguation": false,"disambiguateFrom": [],"derivedFromIswcs": [],"performers": [],"instrumentation": [],"cisnetCreatedDate": "2020-07-13T15:48:02.8961155+00:00","cisnetLastModifiedDate": "2020-07-13T15:48:02.8961155+00:00","additionalIdentifiers": {"isrcs": ["AB1231212345", "IE1231212345"],"publisherIdentifiers": [{"submitterCode": "SA","workCode": ["SONY10001"]}]},"originalTitle": "Sample Work Title 1","otherTitles": [],"interestedParties": [{"name": "PRELVUKAJ GENC","lastName": "PRELVUKAJ","nameNumber": 375001586,"baseNumber": "I-005128908-6","affiliation": "128","role": "C"}]},"linkedIswcs": []}}'
    api_response.status_code = 207

    current_datetime = datetime.strptime(
        '2017-05-01T01:01:01+0200', "%Y-%m-%dT%H:%M:%S%z")

    expected_result = ['acknowledgements\t038\t038\tSony/ATV Music Publishing LLC\t589330613\tAM\tinfo@spanishpoint.ie\t4\tT9800834202\tSONY10001\tSample Work Title 5\t2017-05-01 01:01:01+02:00\tFullyAccepted\t\t\t\t']

    ack = ACK_TSV('1', header, original_transaction, api_response.json(), api_response.ok,
                  api_response.status_code, current_datetime).get_record(1)

    assert expected_result == ack


def test_ack_attribute_error():
    header = {}

    original_transaction = {'OriginalSubmissionId': '4', 'OriginalTransactionType': 'CAR', 'originalTitle': 'Sample Work Title 5', 'submittingAgency': '038', 'submittingPublisheremail': 'info@spanishpoint.ie',
                            'submittingPublishername': 'Sony/ATV Music Publishing LLC', 'submittingPublishernameNumber': '589330613', 'submittingPublisherrole': 'AM', 'submittingSourcedb': '038', 'workcode': 'SONY10001'}

    api_response = Response()
    api_response._content = b'{"submission":"a"}'

    api_response.status_code = 207

    current_datetime = datetime.strptime(
        '2017-05-01T01:01:01+0200', "%Y-%m-%dT%H:%M:%S%z")

    expected_result = ['acknowledgements\t038\t038\tSony/ATV Music Publishing LLC\t589330613\tAM\tinfo@spanishpoint.ie\t4\t\tSONY10001\tSample Work Title 5\t2017-05-01 01:01:01+02:00\tRejected\t500|Internal Server Error\t\t\t']

    ack = ACK_TSV('1', header, original_transaction, api_response.json(), api_response.ok,
                  api_response.status_code, current_datetime).get_record(1)

    assert expected_result == ack
