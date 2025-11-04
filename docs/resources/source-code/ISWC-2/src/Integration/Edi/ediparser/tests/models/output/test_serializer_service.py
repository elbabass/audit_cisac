from requests.models import Response
from datetime import datetime

from ediparser.parser.services.serializer_service import SerializerService
from ediparser.parser.models.edi_file import EdiFileVersion
from ediparser.parser.models.edi_file import EdiFile
from ediparser.parser.models.group import Group
from ediparser.parser.models.output.ack import ACK
from ediparser.parser.models.input.hdr import HDR
from ediparser.parser.models.input.cmq import CMQ
from ediparser.parser.models.input.cur import CUR
from ediparser.parser.models.api_response import ApiResponse
from ediparser.parser.services.logger_service import LoggerService


def test_serialize_file_CMQ():

    file_name = 'cust1/In/CMQTest.030'
    header = {'CharacterSet': '', 'CreationDate': '20170502', 'CreationTime': '175023', 'EdiFileVersion': EdiFileVersion.THREE, 'OriginalFile': '', 'RecordType': 'HDR',
              'SenderID': '000000021', 'SenderIPNameNumber': '', 'SenderName': 'BMI', 'SenderType': 'SO', 'StandardVersionNumber': '03.00', 'TransmissionDate': '20170502'}
    groups = ['HDRSO000000021BMI                                          03.002017050217502320170502',
              'GRHCMQ0000101.00',
              'CMQ0000000200000000                                                                                                    000                    000T9116239771',
              'GRT000010007727000077272',
              'TRL000010007727000077274']
    file = EdiFile(file_name, header, groups)

    original_transaction = [{
        'TransactionSequence#': '00000001',
        'RecordType': 'CMQ',
        'RecordSequence': '00000000',
        'CreationDate': header.get('CreationDate'),
        'CreationTime': header.get('CreationTime'),
        'WorkTitle': '',
        'AgencyCode': '021',
        'AgencyWorkCode': '',
        'SourceDBCode': '021',
        'ParentIswc': ''
    }]

    api_responses = {}
    response_output = [
        {
            "searchId": 1,
            "searchResults": [{
                "iswc": "T9800104012",
                "agency": "128",
                "originalTitle": "TEST MULTIPLE IP BASE NUMBERS",
                "otherTitles": [],
                "interestedParties": [
                    {
                        "name": "HOZIER ",
                        "lastName": "HOZIER",
                        "nameNumber": 755792595,
                        "baseNumber": "I-002985150-9",
                        "affiliation": "21 ",
                        "role": "C"
                    },
                    {
                        "name": "HOZIER BYRNE ANDREW JOHN",
                        "lastName": "HOZIER BYRNE",
                        "nameNumber": 589238793,
                        "baseNumber": "I-002985150-9",
                        "affiliation": "21 ",
                        "role": "C"
                    }
                ],
                "works": [
                    {
                        "id": 11887966729,
                        "iswc": "T9800104012",
                        "iswcEligible": True,
                        "deleted": False,
                        "createdDate": "2020-05-13T15:14:41+01:00",
                        "lastModifiedDate": "2020-05-19T14:04:26+01:00",
                        "agency": "128",
                        "sourcedb": 128,
                        "workcode": "AFBVUIDS22",
                        "category": "DOM",
                        "disambiguation": False,
                        "disambiguateFrom": [],
                        "derivedFromIswcs": [],
                        "performers": [],
                        "instrumentation": [],
                        "cisnetCreatedDate": "2020-05-13T15:14:41+01:00",
                        "cisnetLastModifiedDate": "2020-05-13T15:14:41+01:00",
                        "originalTitle": "TEST MULTIPLE IP BASE NUMBERS",
                        "otherTitles": [],
                        "interestedParties": [
                            {
                                "name": "HOZIER ",
                                "lastName": "HOZIER",
                                "nameNumber": 755792595,
                                "baseNumber": "I-002985150-9",
                                "affiliation": "21 ",
                                "role": "C"
                            },
                            {
                                "name": "HOZIER BYRNE ANDREW JOHN",
                                "lastName": "HOZIER BYRNE",
                                "nameNumber": 589238793,
                                "baseNumber": "I-002985150-9",
                                "affiliation": "21 ",
                                "role": "C"
                            }
                        ]
                    }
                ],
                "linkedISWC": [],
                "createdDate": "2020-05-13T15:14:40+01:00",
                "lastModifiedDate": "2020-05-13T15:14:40+01:00"
            }]
        }
    ]
    api_responses.setdefault('00001', []).append(
        ApiResponse(response_output, original_transaction, 207))

    header_CMQ = {'SenderID': '000000161',
                  'EdiFileVersion': EdiFileVersion.THREE}

    logger = LoggerService('', '')
    sc = SerializerService(file, api_responses, 'ACK', logger)

    current_datetime = datetime.utcnow()
    ack_filename = "ISWC{}{}315.{}0".format(
        current_datetime.strftime(
            '%Y%m%d%H%M%S'), file.header['SenderID'][-3:], file.header['StandardVersionNumber'][0: 2]
    )
    cd = current_datetime.strftime('%Y%m%d')
    ct = current_datetime.strftime('%H%M%S')
    td = current_datetime.strftime('%Y%m%d')
    pd = current_datetime.strftime('%Y%m%d')

    expected_value_1 = (ack_filename, ['HDRSO000000315CSI CENTER                                   03.00'+cd+ct+td + '                                                     ',
                                       'GRHACK0000101.00',
                                       'ACK0000000000000000201705021750230000100000001CMQTEST MULTIPLE IP BASE NUMBERS                                                                       128                    128T9800104012'+pd+'FA',
                                       'CWI0000000000000001128AFBVUIDS22          128T9800104012           ',
                                       'CIP0000000000000002755792595  I-002985150-9C ',
                                       'CIP0000000000000003589238793  I-002985150-9C ',
                                       'GRT000010000000100000006',
                                       'TRL000010000000100000008']
                        )

    records = sc.serialize_file()[1]
    assert expected_value_1 == (ack_filename, records)  # TODO


def test_serialize_file_CUR():

    file_name = 'cust1/In/DISTest.030'
    header = {'CharacterSet': '', 'CreationDate': '20180705', 'CreationTime': '170507', 'EdiFileVersion': EdiFileVersion.THREE, 'OriginalFile': '', 'RecordType': 'HDR',
              'SenderID': '000000128', 'SenderIPNameNumber': '', 'SenderName': 'WID', 'SenderType': 'AA', 'StandardVersionNumber': '03.00', 'TransmissionDate': '20180705'}
    groups = ['HDRAA000000128WID CENTER                                   03.002018070517050720180705',
              'GRHCUR0000101.00',
              'CUR0000000100000000PARADISE84                                                                                          128R26919333oku        300           T0302206116ModifiedVersion     TDIT                 B',
              'CTL0000000100000002PARADISE84T1                                                                                        OA',
              'NAT0000000100000003OAENPARADISE84N1',
              'CTL0000000100000002PARADISE84T2                                                                                        OA',
              'CTL0000000100000002PARADISE84T3                                                                                        OA',
              'NAT0000000100000003OAENPARADISE84N2',
              'NAT0000000100000003OAENPARADISE84N3',
              'CIP000000010000000400555446827I-002770183-1CA',
              'DIS0000000000000005T0302206116',
              'GRT000010000007200000650',
              'TRL000010000007200000652']
    file = EdiFile(file_name, header, groups)

    original_transaction = [{
        'TransactionSequence#': '00000001',
        'RecordType': 'CUR',
        'RecordSequence': '00000000',
        'CreationDate': header.get('CreationDate'),
        'CreationTime': header.get('CreationTime'),
        'WorkTitle': 'PARADISE84',
        'AgencyCode': '128',
        'AgencyWorkCode': '1234',
        'SourceDBCode': '300',
        'ParentIswc': ''
    }]

    api_responses = {}
    response_output = [
        {
            "submissionId": 0,
            "submission": {
                "verifiedSubmission": {
                    "id": 11887966729,
                    "iswc": "T9800104012",
                    "iswcEligible": True,
                    "deleted": False,
                    "createdDate": "2020-05-13T15:14:41+01:00",
                    "lastModifiedDate": "2020-05-19T14:04:26.2107715+00:00",
                    "agency": "128",
                    "sourcedb": 128,
                    "workcode": "AFBVUIDS22",
                    "category": "DOM",
                    "disambiguation": False,
                    "disambiguateFrom": [],
                    "derivedFromIswcs": [],
                    "performers": [],
                    "instrumentation": [],
                    "cisnetCreatedDate": "2020-05-13T15:14:41+01:00",
                    "cisnetLastModifiedDate": "2020-05-13T15:14:41+01:00",
                    "originalTitle": "TEST MULTIPLE IP BASE NUMBERS",
                    "otherTitles": [],
                    "interestedParties": [
                        {
                            "name": "HOZIER ",
                            "lastName": "HOZIER",
                            "nameNumber": 755792595,
                            "baseNumber": "I-002985150-9",
                            "affiliation": "21 ",
                            "role": "C"
                        },
                        {
                            "name": "HOZIER BYRNE ANDREW JOHN",
                            "lastName": "HOZIER BYRNE",
                            "nameNumber": 589238793,
                            "baseNumber": "I-002985150-9",
                            "affiliation": "21 ",
                            "role": "C"
                        }
                    ]
                },
                "linkedIswcs": []
            }
        }
    ]
    api_responses.setdefault('00001', []).append(
        ApiResponse(response_output, original_transaction, 207))

    header_CMQ = {'SenderID': '000000161',
                  'EdiFileVersion': EdiFileVersion.THREE}

    logger = LoggerService('', '')
    sc = SerializerService(file, api_responses, 'ACK', logger)

    current_datetime = datetime.utcnow()

    cd = current_datetime.strftime('%Y%m%d')
    ct = current_datetime.strftime('%H%M%S')
    td = current_datetime.strftime('%Y%m%d')
    pd = current_datetime.strftime('%Y%m%d')

    expected_value_1 = ['HDRSO000000315CSI CENTER                                   03.00'+cd+ct+td + '                                                     ',
                        'GRHACK0000101.00',
                        'ACK0000000000000000201807051705070000100000001CURTEST MULTIPLE IP BASE NUMBERS                                                                       128AFBVUIDS22          128T9800104012'+pd+'FA',
                        'CWI0000000000000001128AFBVUIDS22          128T9800104012           ',
                        'CIP0000000000000002755792595  I-002985150-9C ',
                        'CIP0000000000000003589238793  I-002985150-9C ',
                        'GRT000010000000100000006',
                        'TRL000010000000100000008']

    expected_ack_filename = "ISWC{}{}315.{}0".format(
        current_datetime.strftime(
            '%Y%m%d%H%M%S'), file.header['SenderID'][-3:], file.header['StandardVersionNumber'][0: 2]
    )

    result = sc.serialize_file()
    records = result[1]
    ack_filename = result[0]
    assert expected_value_1 == records
    assert expected_ack_filename == ack_filename


def test_WID_title():

    file_name = 'cust1/In/DISTest.030'
    header = {'CharacterSet': '', 'CreationDate': '20180705', 'CreationTime': '170507', 'EdiFileVersion': EdiFileVersion.THREE, 'OriginalFile': '', 'RecordType': 'HDR',
              'SenderID': '000000300', 'SenderIPNameNumber': '', 'SenderName': 'WID', 'SenderType': 'AA', 'StandardVersionNumber': '03.00', 'TransmissionDate': '20180705'}
    groups = ['HDRAA000000300WID CENTER                                   03.002018070517050720180705',
              'GRHCUR0000101.00',
              'CUR0000000100000000PARADISE84                                                                                          128R26919333oku        300           T0302206116ModifiedVersion     TDIT                 B',
              'CTL0000000100000002PARADISE84T1                                                                                        OA',
              'NAT0000000100000003OAENPARADISE84N1',
              'CTL0000000100000002PARADISE84T2                                                                                        OA',
              'CTL0000000100000002PARADISE84T3                                                                                        OA',
              'NAT0000000100000003OAENPARADISE84N2',
              'NAT0000000100000003OAENPARADISE84N3',
              'CIP000000010000000400555446827I-002770183-1CA',
              'DIS0000000000000005T0302206116',
              'GRT000010000007200000650',
              'TRL000010000007200000652']
    file = EdiFile(file_name, header, groups)

    original_transaction = [{
        'TransactionSequence#': '00000001',
        'RecordType': 'CUR',
        'RecordSequence': '00000000',
        'CreationDate': header.get('CreationDate'),
        'CreationTime': header.get('CreationTime'),
        'WorkTitle': 'PARADISE84',
        'AgencyCode': '128',
        'AgencyWorkCode': '1234',
        'SourceDBCode': '300',
        'ParentIswc': ''
    }]

    api_responses = {}
    response_output = [
        {
            "submissionId": 0,
            "submission": {
                "verifiedSubmission": {
                    "id": 11887966729,
                    "iswc": "T9800104012",
                    "iswcEligible": True,
                    "deleted": False,
                    "createdDate": "2020-05-13T15:14:41+01:00",
                    "lastModifiedDate": "2020-05-19T14:04:26.2107715+00:00",
                    "agency": "128",
                    "sourcedb": 128,
                    "workcode": "AFBVUIDS22",
                    "category": "DOM",
                    "disambiguation": False,
                    "disambiguateFrom": [],
                    "derivedFromIswcs": [],
                    "performers": [],
                    "instrumentation": [],
                    "cisnetCreatedDate": "2020-05-13T15:14:41+01:00",
                    "cisnetLastModifiedDate": "2020-05-13T15:14:41+01:00",
                    "originalTitle": "TEST MULTIPLE IP BASE NUMBERS",
                    "otherTitles": [],
                    "interestedParties": [
                        {
                            "name": "HOZIER ",
                            "lastName": "HOZIER",
                            "nameNumber": 755792595,
                            "baseNumber": "I-002985150-9",
                            "affiliation": "21 ",
                            "role": "C"
                        },
                        {
                            "name": "HOZIER BYRNE ANDREW JOHN",
                            "lastName": "HOZIER BYRNE",
                            "nameNumber": 589238793,
                            "baseNumber": "I-002985150-9",
                            "affiliation": "21 ",
                            "role": "C"
                        }
                    ]
                },
                "linkedIswcs": []
            }
        }
    ]
    api_responses.setdefault('00001', []).append(
        ApiResponse(response_output, original_transaction, 207))

    header_CMQ = {'SenderID': '000000161',
                  'EdiFileVersion': EdiFileVersion.THREE}

    logger = LoggerService('', '')
    sc = SerializerService(file, api_responses, 'ACK', logger)

    current_datetime = datetime.utcnow()

    cd = current_datetime.strftime('%Y%m%d')
    ct = current_datetime.strftime('%H%M%S')
    td = current_datetime.strftime('%Y%m%d')
    pd = current_datetime.strftime('%Y%m%d')

    expected_ack_filename = "CSI20180705170507300315.030"

    ack_filename = sc.serialize_file()[0]
    assert expected_ack_filename == (ack_filename)


def test_serialize_file_CMQ_with_NAT():

    file_name = 'cust1/In/CMQTest.030'
    header = {'CharacterSet': '', 'CreationDate': '20170502', 'CreationTime': '175023', 'EdiFileVersion': EdiFileVersion.THREE, 'OriginalFile': '', 'RecordType': 'HDR',
              'SenderID': '000000021', 'SenderIPNameNumber': '', 'SenderName': 'BMI', 'SenderType': 'SO', 'StandardVersionNumber': '03.00', 'TransmissionDate': '20170502'}
    groups = ['HDRSO000000021BMI                                          03.002017050217502320170502',
              'GRHCMQ0000101.00',
              'CMQ0000000200000000                                                                                                    000                    000T9116239771',
              'GRT000010007727000077272',
              'TRL000010007727000077274']
    file = EdiFile(file_name, header, groups)

    original_transaction = [{
        'TransactionSequence#': '00000001',
        'RecordType': 'CMQ',
        'RecordSequence': '00000000',
        'CreationDate': header.get('CreationDate'),
        'CreationTime': header.get('CreationTime'),
        'WorkTitle': '',
        'AgencyCode': '021',
        'AgencyWorkCode': '',
        'SourceDBCode': '021',
        'ParentIswc': ''
    }]

    api_responses = {}
    response_output = [
        {
            "searchId": 1,
            "searchResults": [{
                "iswc": "T9800104012",
                "agency": "128",
                "originalTitle": "TEST MULTIPLE IP BASE NUMBERS",
                "otherTitles": [],
                "interestedParties": [
                    {
                        "name": "HOZIER ",
                        "lastName": "HOZIER",
                        "nameNumber": 755792595,
                        "baseNumber": "I-002985150-9",
                        "affiliation": "21 ",
                        "role": "C"
                    },
                    {
                        "name": "HOZIER BYRNE ANDREW JOHN",
                        "lastName": "HOZIER BYRNE",
                        "nameNumber": 589238793,
                        "baseNumber": "I-002985150-9",
                        "affiliation": "21 ",
                        "role": "C"
                    }
                ],
                "works": [
                    {
                        "id": 11887966729,
                        "iswc": "T9800104012",
                        "iswcEligible": True,
                        "deleted": False,
                        "createdDate": "2020-05-13T15:14:41+01:00",
                        "lastModifiedDate": "2020-05-19T14:04:26+01:00",
                        "agency": "128",
                        "sourcedb": 128,
                        "workcode": "AFBVUIDS22",
                        "category": "DOM",
                        "disambiguation": False,
                        "disambiguateFrom": [],
                        "derivedFromIswcs": [],
                        "performers": [],
                        "instrumentation": [],
                        "cisnetCreatedDate": "2020-05-13T15:14:41+01:00",
                        "cisnetLastModifiedDate": "2020-05-13T15:14:41+01:00",
                        "originalTitle": "TEST MULTIPLE IP BASE NUMBERS",
                        "otherTitles": [{'title': 'ＴＨＥ　ＬＡＳＴ　ＭＥＳＳＡＧＥ海猿よ〜', 'type': 'OA'}],
                        "interestedParties": [
                            {
                                "name": "HOZIER ",
                                "lastName": "HOZIER",
                                "nameNumber": 755792595,
                                "baseNumber": "I-002985150-9",
                                "affiliation": "21 ",
                                "role": "C"
                            },
                            {
                                "name": "HOZIER BYRNE ANDREW JOHN",
                                "lastName": "HOZIER BYRNE",
                                "nameNumber": 589238793,
                                "baseNumber": "I-002985150-9",
                                "affiliation": "21 ",
                                "role": "C"
                            }
                        ]
                    }
                ],
                "linkedISWC": [],
                "createdDate": "2020-05-13T15:14:40+01:00",
                "lastModifiedDate": "2020-05-13T15:14:40+01:00"
            }]
        }
    ]
    api_responses.setdefault('00001', []).append(
        ApiResponse(response_output, original_transaction, 207))

    header_CMQ = {'SenderID': '000000161',
                  'EdiFileVersion': EdiFileVersion.THREE}

    logger = LoggerService('', '')
    sc = SerializerService(file, api_responses, 'ACK', logger)

    current_datetime = datetime.utcnow()
    ack_filename = "ISWC{}{}315.{}0".format(
        current_datetime.strftime(
            '%Y%m%d%H%M%S'), file.header['SenderID'][-3:], file.header['StandardVersionNumber'][0: 2]
    )
    cd = current_datetime.strftime('%Y%m%d')
    ct = current_datetime.strftime('%H%M%S')
    td = current_datetime.strftime('%Y%m%d')
    pd = current_datetime.strftime('%Y%m%d')

    expected_value_1 = (ack_filename, ['HDRSO000000315CSI CENTER                                   03.00'+cd+ct+td + '                                                     ',
                                       'GRHACK0000101.00',
                                       'ACK0000000000000000201705021750230000100000001CMQTEST MULTIPLE IP BASE NUMBERS                                                                       128                    128T9800104012'+pd+'FA',
                                       'CWI0000000000000001128AFBVUIDS22          128T9800104012           ',
                                       'CIP0000000000000002755792595  I-002985150-9C ',
                                       'CIP0000000000000003589238793  I-002985150-9C ',
                                       'NAT0000000000000004OA  ＴＨＥ　ＬＡＳＴ　ＭＥＳＳＡＧＥ海猿よ〜                                                                                ',
                                       'GRT000010000000100000007',
                                       'TRL000010000000100000009']
                        )

    records = sc.serialize_file()[1]
    assert expected_value_1 == (ack_filename, records)
