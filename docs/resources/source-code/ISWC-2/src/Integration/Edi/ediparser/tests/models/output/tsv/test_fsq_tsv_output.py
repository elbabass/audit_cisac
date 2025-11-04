from requests.models import Response

from ediparser.parser.models.output.tsv.fsq import FSQ


def test_fsq_get_record():
    api_response = Response()
    api_response._content = b'{"submissionId": 1, "submission": {"verifiedSubmission": {"id": 11921751293,"iswc": "T9800834202","iswcEligible": true,"deleted": false,"createdDate": "2020-07-13T15:48:02.8961155+00:00","lastModifiedDate": "2020-07-13T15:48:02.8961155+00:00","agency": "128","sourcedb": 128,"workcode": "SONY10001","category": "DOM","disambiguation": false,"disambiguateFrom": [],"derivedFromIswcs": [],"performers": [],"instrumentation": [],"cisnetCreatedDate": "2020-07-13T15:48:02.8961155+00:00","cisnetLastModifiedDate": "2020-07-13T15:48:02.8961155+00:00","additionalIdentifiers": {"isrcs": ["AB1231212345", "IE1231212345"],"publisherIdentifiers": [{"submitterCode": "SA","workCode": ["SONY10001"]}]},"originalTitle": "Sample Work Title 1","otherTitles": [],"interestedParties": [{"name": "PRELVUKAJ GENC","lastName": "PRELVUKAJ","nameNumber": 375001586,"baseNumber": "I-005128908-6","affiliation": "128","role": "C"}]},"linkedIswcs": []}}'

    api_response.status_code = 207

    fsq = FSQ(api_response.json())

    Expected_value = {8: 'T9800834202'}

    assert Expected_value == fsq.get_record()
