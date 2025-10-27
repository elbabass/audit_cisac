from datetime import datetime

from requests.models import Response

from ediparser.parser.models.edi_file import EdiFileVersion
from ediparser.parser.models.input.cmq import CMQ
from ediparser.parser.models.output.ack import ACK


def test_ack_body_version_three():
    header = {'SenderID': '000000161',
              'EdiFileVersion': EdiFileVersion.THREE}
    input_transaction = {
        'TransactionSequence#': '00000001',
        'RecordType': 'CMQ',
        'RecordSequence': '00000000',
        'WorkTitle': 'THE BREATH AND THE BELL',
        'AgencyCode': '315',
        'AgencyWorkCode': '1234',
        'SourceDBCode': '315'
    }

    api_response = Response()
    api_response._content = b'{"searchResults": [{"iswc":"T9031238434","agency":"315","originalTitle":"THE BREATH AND THE BELL","otherTitles":[],"interestedParties":[], "works":[]}]}'
    api_response.status_code = 200

    creation_datetime = datetime.strptime(
        '2017-05-01T01:01:01+0200', "%Y-%m-%dT%H:%M:%S%z")
    current_datetime = datetime.strptime(
        '2017-05-01T01:01:01+0200', "%Y-%m-%dT%H:%M:%S%z")

    expected_result = 'ACK0000000000000000201705010101011    00000001CMQTHE BREATH AND THE BELL                                                                             3151234                315T903123843420170501FA'

    ack = ACK('1', header, input_transaction, api_response.json(), api_response.ok,
              api_response.status_code, creation_datetime, current_datetime).get_record(0)

    assert expected_result == next(ack)


def test_ack_body_version_two():
    header = {'SenderID': '000000161',
              'EdiFileVersion': EdiFileVersion.TWO}
    input_transaction = {
        'TransactionSequence#': '00000001',
        'RecordType': 'CMQ',
        'RecordSequence': '00000000',
        'WorkTitle': 'THE BREATH AND THE BELL',
        'AgencyCode': '315',
        'AgencyWorkCode': '1234',
        'SourceDBCode': '315'
    }

    # mock response object
    api_response = Response()
    api_response._content = b'{"searchResults": [{"iswc":"T9031238434","agency":"315","originalTitle":"THE BREATH AND THE BELL","otherTitles":[],"interestedParties":[], "works":[]}]}'
    api_response.status_code = 200

    creation_datetime = datetime.strptime(
        '2017-05-01T01:01:01+0200', "%Y-%m-%dT%H:%M:%S%z")
    current_datetime = datetime.strptime(
        '2017-05-01T01:01:01+0200', "%Y-%m-%dT%H:%M:%S%z")

    expected_result = 'ACK0000000000000000201705010101011    00000001CMQTHE BREATH AND THE BELL                                     3151234                315T903123843420170501FA'

    ack = ACK('1', header, input_transaction, api_response.json(), api_response.ok,
              api_response.status_code, creation_datetime, current_datetime).get_record(0)

    assert expected_result == next(ack)
