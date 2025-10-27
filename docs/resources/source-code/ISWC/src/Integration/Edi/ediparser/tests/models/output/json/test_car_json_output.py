from requests.models import Response

from ediparser.parser.models.output.json.car import CAR


def test_car_get_record():
    api_response = Response()
    api_response._content = b'{"submission":{"verifiedSubmission": {"id": 0,"iswc": "T9031238434","iswcStatus": "Preferred","agency": "315","sourcedb": "315","workcode": "00112200","category": "DOM","originalTitle": "THE BREATH AND THE BELL","otherTitles": [],"interestedParties": [{"name": "Name","nameNumber": 12345,"baseNumber": "I-002234809-8C","role": "CA"}],"performers": [],"additionalIdentifiers": [],"createdDate": "string","lastModifiedDate": "string","lastModifiedBy": "string"},"alternateIswcMatches": []}}'
    api_response.status_code = 207

    car_record = CAR(api_response.json()).get_record(True)

    Expected_value = {
        'originalTransactionType': 'AddSubmission',
        'originalTitle': 'THE BREATH AND THE BELL',
        'agency': '315',
        'workcode': "00112200",
        'sourcedb': '315',
        'iswc': 'T9031238434',
        'iswcStatus': 'Preferred',
        'interestedParties': [
            {
                'name': 'Name',
                'nameNumber': 12345,
                'baseNumber': 'I-002234809-8C',
                'role': 'CA'
            }],
        'performers': [],
        'additionalIdentifiers': [],
        'otherTitles': [],
        'archivedIswc': None,
        'alternateIswcMatches': []
    }

    assert Expected_value == car_record
