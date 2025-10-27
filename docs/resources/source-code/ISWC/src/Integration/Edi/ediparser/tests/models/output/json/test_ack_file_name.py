import datetime
from ediparser.parser.models.edi_file import EdiFile
from ediparser.parser.models.api_response import ApiResponse


def test_ack_file_name_pub_pubidentifier():
    current_datetime = datetime.datetime.utcnow()
    file_name = '052/Allocation/SA/In/ISWCP20200609090032128SA315.json'
    file_header = {
        "submittingAgency": "128",
        "submittingSourcedb": 128,
        "submittingPublisher": {
            "name": "Sony/ATV Music Publishing LLC",
            "nameNumber": 269021863,
            "email": "info@spanishpoint.ie",
            "role": "AM"
        },
        "fileCreationDateTime": "2019-10-01T18:25:43.511Z",
        "receivingAgency": "315"
    }
    Groups = []
    output = [{
        "submissionId": 0,
        "submission": {
            "verifiedSubmission": {
                "id": 11921332275,
                "iswc": "T9800266982",
                "iswcEligible": True,
                "deleted": False,
                "createdDate": "2020-07-02T16:04:24.1803466+00:00",
                "lastModifiedDate": "2020-07-02T16:04:24.1803466+00:00",
                "agency": "128",
                "sourcedb": 128,
                "workcode": "t5",
                "category": "DOM",
                "disambiguation": False,
                "disambiguateFrom": [],
                "derivedFromIswcs": [],
                "performers": [],
                "instrumentation": [],
                "cisnetCreatedDate": "2020-07-02T16:04:24.1803466+00:00",
                "cisnetLastModifiedDate": "2020-07-02T16:04:24.1803466+00:00",
                "additionalIdentifiers": {
                    "isrcs": [],
                    "publisherIdentifiers": [{
                        "submitterCode": "SA",
                        "workCode": ["test1", "t5"]
                    }]
                },
                "originalTitle": "Come all ye maidens young and fair",
                "otherTitles": [],
                "interestedParties": [{
                    "name": "HOZIER BYRNE ANDREW JOHN",
                    "lastName": "HOZIER BYRNE",
                    "nameNumber": 589238793,
                    "baseNumber": "I-002985150-9",
                    "affiliation": "021",
                    "role": "C"
                }, {
                    "name": "SONY/ATV MUSIC PUBLISHING LLC ",
                    "lastName": "SONY/ATV MUSIC PUBLISHING LLC",
                    "nameNumber": 269021863,
                    "baseNumber": "I-000512437-6",
                    "role": "E"
                }]
            },
            "linkedIswcs": []
        }
    }]
    api_response = [ApiResponse(output, {}, 207)]
    cd = current_datetime.replace(microsecond=0).isoformat().replace(':', '-')
    expected_result = 'ISWCP_'+cd+'_315_SA_128.json'

    file_name = EdiFile(
        file_name, file_header, Groups).get_ack_file_name(file_header['submittingAgency'], '.json', 'ACK_JSON')

    assert expected_result == file_name


def test_ack_file_name_pub_pubidentifier_blank():
    current_datetime = datetime.datetime.utcnow()
    file_name = '052/Allocation/SA/In/ISWCP20200609090032128SA315.json'
    file_header = {
        "submittingAgency": "128",
        "submittingSourcedb": 128,
        "submittingPublisher": {
            "name": "Sony/ATV Music Publishing LLC",
            "nameNumber": 269021863,
            "email": "info@spanishpoint.ie",
            "role": "AM"
        },
        "fileCreationDateTime": "2019-10-01T18:25:43.511Z",
        "receivingAgency": "315"
    }
    Groups = []
    output = [{
        "submissionId": 0,
        "submission": {
            "verifiedSubmission": {
                "id": 11921332275,
                "iswc": "T9800266982",
                "iswcEligible": True,
                "deleted": False,
                "createdDate": "2020-07-02T16:04:24.1803466+00:00",
                "lastModifiedDate": "2020-07-02T16:04:24.1803466+00:00",
                "agency": "128",
                "sourcedb": 128,
                "workcode": "t5",
                "category": "DOM",
                "disambiguation": False,
                "disambiguateFrom": [],
                "derivedFromIswcs": [],
                "performers": [],
                "instrumentation": [],
                "cisnetCreatedDate": "2020-07-02T16:04:24.1803466+00:00",
                "cisnetLastModifiedDate": "2020-07-02T16:04:24.1803466+00:00",
                "additionalIdentifiers": {
                    "isrcs": [],
                    "publisherIdentifiers": [{}]
                },
                "originalTitle": "Come all ye maidens young and fair",
                "otherTitles": [],
                "interestedParties": [{
                    "name": "HOZIER BYRNE ANDREW JOHN",
                    "lastName": "HOZIER BYRNE",
                    "nameNumber": 589238793,
                    "baseNumber": "I-002985150-9",
                    "affiliation": "021",
                    "role": "C"
                }, {
                    "name": "SONY/ATV MUSIC PUBLISHING LLC ",
                    "lastName": "SONY/ATV MUSIC PUBLISHING LLC",
                    "nameNumber": 269021863,
                    "baseNumber": "I-000512437-6",
                    "role": "E"
                }]
            },
            "linkedIswcs": []
        }
    }]
    api_response = [ApiResponse(output, {}, 207)]

    cd = current_datetime.replace(microsecond=0).isoformat().replace(':', '-')
    expected_result = 'ISWCP_'+cd+'_315_SA_128.json'

    file_name = EdiFile(
        file_name, file_header, Groups).get_ack_file_name(file_header['submittingAgency'], '.json', 'ACK_JSON')

    assert expected_result == file_name


def test_ack_file_name():
    current_datetime = datetime.datetime.utcnow()
    file_name = 'ISWC20200609090032128315.json'
    file_header = {
        "submittingAgency": "128",
        "submittingSourcedb": 128,
        "fileCreationDateTime": "2019-10-01T18:25:43.511Z",
        "receivingAgency": "315"
    }
    Groups = []
    output = [{
        "submissionId": 0,
        "submission": {
            "verifiedSubmission": {
                "id": 11921332275,
                "iswc": "T9800266982",
                "iswcEligible": True,
                "deleted": False,
                "createdDate": "2020-07-02T16:04:24.1803466+00:00",
                "lastModifiedDate": "2020-07-02T16:04:24.1803466+00:00",
                "agency": "128",
                "sourcedb": 128,
                "workcode": "t5",
                "category": "DOM",
                "disambiguation": False,
                "disambiguateFrom": [],
                "derivedFromIswcs": [],
                "performers": [],
                "instrumentation": [],
                "cisnetCreatedDate": "2020-07-02T16:04:24.1803466+00:00",
                "cisnetLastModifiedDate": "2020-07-02T16:04:24.1803466+00:00",
                "additionalIdentifiers": {
                    "isrcs": [],
                    "publisherIdentifiers": [{}]
                },
                "originalTitle": "Come all ye maidens young and fair",
                "otherTitles": [],
                "interestedParties": [{
                    "name": "HOZIER BYRNE ANDREW JOHN",
                    "lastName": "HOZIER BYRNE",
                    "nameNumber": 589238793,
                    "baseNumber": "I-002985150-9",
                    "affiliation": "021",
                    "role": "C"
                }, {
                    "name": "SONY/ATV MUSIC PUBLISHING LLC ",
                    "lastName": "SONY/ATV MUSIC PUBLISHING LLC",
                    "nameNumber": 269021863,
                    "baseNumber": "I-000512437-6",
                    "role": "E"
                }]
            },
            "linkedIswcs": []
        }
    }]
    api_response = [ApiResponse(output, {}, 207)]

    cd = current_datetime.strftime('%Y%m%d%H%M%S')
    expected_result = 'ISWC'+cd+'128315.json'

    file_name = EdiFile(
        file_name, file_header, Groups).get_ack_file_name(file_header['submittingAgency'], '.json', 'ACK_JSON')

    assert expected_result == file_name


def test_ack_file_name_with_invalid_submittingPublisher():
    current_datetime = datetime.datetime.utcnow()
    file_name = '052/Allocation/PR/In/iswcp_2021-04-15T13-27-44_052_PR_315_SampleSubmissions.json'
    file_header = {
        "submittingAgency": "052",
        "submittingSourcedb": 52,
        "submittingPublisher": {},
        "fileCreationDateTime": "2021-04-13T15:51:44.003Z",
        "receivingAgency": "315"
    }
    Groups = []

    cd = current_datetime.replace(microsecond=0).isoformat().replace(':', '-')
    expected_result = 'ISWCP_'+cd+'_315_PR_052.json'

    file_name = EdiFile(
        file_name, file_header, Groups).get_ack_file_name(file_header['submittingAgency'], '.json', 'ACK_JSON')

    assert expected_result == file_name


def test_ack_file_name_with_valid_submittingPublisher():
    current_datetime = datetime.datetime.utcnow()
    file_name = '052/Allocation/PR/In/iswcp_2021-04-15T13-27-44_052_PR_315_SampleSubmissions.json'
    file_header = {
        "submittingAgency": "052",
        "submittingSourcedb": 52,
        "submittingPublisher": {
            "name": "(Peer Music (UK) Ltd.)",
            "nameNumber": 162536085,
            "email": "iswc.notifications@peermusic.com",
            "role": "AM"
        },
        "fileCreationDateTime": "2021-04-13T15:51:44.003Z",
        "receivingAgency": "315"
    }
    Groups = []

    cd = current_datetime.replace(microsecond=0).isoformat().replace(':', '-')
    expected_result = 'ISWCP_'+cd+'_315_PR_052.json'

    file_name = EdiFile(
        file_name, file_header, Groups).get_ack_file_name(file_header['submittingAgency'], '.json', 'ACK_JSON')

    assert expected_result == file_name
