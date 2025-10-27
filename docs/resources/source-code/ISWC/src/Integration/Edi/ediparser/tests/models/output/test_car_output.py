from requests.models import Response

from ediparser.parser.models.output.car import CAR


def test_car_get_record():
    api_response = Response()
    api_response._content = b'{"submission":{"verifiedSubmission": {"id": 0,"iswc": "T9031238434","agency": "315","sourcedb": "315","workcode": "string","category": "DOM","originalTitle": "THE BREATH AND THE BELL","otherTitles": [],"interestedParties": [{"name": "string","nameNumber": 0,"baseNumber": "string","role": "CA"}],"createdDate": "string","lastModifiedDate": "string","lastModifiedBy": "string"}}}'
    api_response.status_code = 200

    car_record = CAR(api_response.json(), None, None).get_record()

    Expected_value = {
        'WorkTitle': 'THE BREATH AND THE BELL',
        'AgencyCode': '315',
        'AgencyWorkCode': 'string',
        'SourceDBCode': '315',
        'PreferredISWC': 'T9031238434',
    }

    assert Expected_value == car_record


def test_car_get_child_record():
    api_response = Response()
    api_response._content = b'{"submission": {"verifiedSubmission": {"id": 0,"iswc": "T9031238434","agency": "315","sourcedb": "315","workcode": "string","category": "DOM","originalTitle": "THE BREATH AND THE BELL","otherTitles": [],"interestedParties": [{"nameNumber": 463130289,"baseNumber": "I-002234809-8","role": "C"}]}}}'
    api_response.status_code = 200

    car_child_record = CAR(api_response.json(), None, None).get_child_records()

    expected_result = ['CWI                315string              315T9031238434           ',
                       'CIP                463130289  I-002234809-8C ']

    assert expected_result == car_child_record
