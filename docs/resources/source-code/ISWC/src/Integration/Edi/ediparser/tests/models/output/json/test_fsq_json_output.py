from requests.models import Response

from ediparser.parser.models.output.json.fsq import FSQ


def test_fsq_get_record():
    api_response = Response()
    api_response._content = b'{"submissionId": 10869,"submission": {"verifiedSubmission": {"id": 11920435273,"iswc": "T9800273874", "iswcEligible": true, "deleted": false,"createdDate": "2020-06-12T13:35:13+00:00","lastModifiedDate": "2020-06-17T15:45:18.487041+00:00","agency": "021","sourcedb": 21,"workcode": "R00489167","category": "DOM","disambiguation": false,"disambiguateFrom": [], "derivedFromIswcs": [],"performers": [],"instrumentation": [],"cisnetCreatedDate": "2020-06-12T13:35:13+00:00","cisnetLastModifiedDate": "2020-06-12T13:35:13+00:00","originalTitle": "Every rakingVV","otherTitles": [],"interestedParties": [{"name": "JAMES MARK","lastName": "JAMES","nameNumber": 34493778,"baseNumber": "I-001644063-2","affiliation": "021","role": "C"}]},"linkedIswcs": []}}'
    api_response.status_code = 207
    fsq_record = FSQ(api_response.json())
    Expected_value = {'sourcedb': 21,
                      'originalTitle': 'Every rakingVV',
                      'preferredIswc': 'T9800273874',
                      'otherTitles': [],
                      'workcode': 'R00489167',
                      'originalTransactionType': 'FindSubmission',
                      'interestedParties': [{
                          'name': 'JAMES MARK',
                          'lastName': 'JAMES',
                          'role': 'C',
                          'baseNumber': 'I-001644063-2',
                          'affiliation': '021',
                          'nameNumber': 34493778}],
                      'agency': '021'}
    assert Expected_value == fsq_record.get_record()
