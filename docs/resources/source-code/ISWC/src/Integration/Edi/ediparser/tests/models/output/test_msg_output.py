import json
from requests.models import Response

from ediparser.parser.models.edi_file import EdiFileVersion
from ediparser.parser.models.input.cmq import CMQ
from ediparser.parser.models.output.msg import MSG


def test_msg_get_record_with_code():
    api_response = Response()
    api_response._content = b'{"code":"107","message":"Preferred ISWC is required"}'
    api_response.status_code = 400

    header = {'SenderID': '000000161',
              'EdiFileVersion': EdiFileVersion.THREE}
    original_transaction = {
        'TransactionSequence#': '00000001',
        'RecordType': 'CMQ',
        'RecordSequence': '00000000'
    }

    msg = MSG(api_response.json(), original_transaction,
              api_response.status_code)

    expected_value = 'MSG                T00000000CMQT107Preferred ISWC is required                                                                                                                            '

    assert expected_value == msg.get_record()


def test_MSG_get_record_without_code():
    api_response = Response()
    api_response._content = json.dumps(
        {"statusCode": "404", "message": "ISWC not found."}).encode('utf-8')
    api_response.status_code = 404

    header = {'SenderID': '000000161',
              'EdiFileVersion': EdiFileVersion.THREE}
    original_transaction = {
        'TransactionSequence#': '00000001',
        'RecordType': 'CMQ',
        'RecordSequence': '00000000'
    }

    msg = MSG(api_response.json(), original_transaction,
              api_response.status_code)

    expected_value = 'MSG                T00000000CMQT404ISWC not found.                                                                                                                                       '

    print(msg.get_record()+'.')
    print(expected_value+'.')

    assert expected_value == msg.get_record()
