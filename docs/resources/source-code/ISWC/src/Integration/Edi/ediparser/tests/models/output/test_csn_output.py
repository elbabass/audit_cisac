from datetime import datetime

from requests.models import Response

from ediparser.parser.models.edi_file import EdiFileVersion
from ediparser.parser.models.input.json.cmq import CMQ
from ediparser.parser.models.output.csn import CSN


def test_csn_body():
    header = {
        "submittingAgency": "315",
        "submittingSourcedb": 315,
        "fileCreationDateTime": "2019-10-01T18:25:43.511Z",
        "receivingAgency": "52",
        'EdiFileVersion': EdiFileVersion.THREE
    }

    original_transaction = {
        'OriginalTransactionType': 'CUR',
        'RecordSequence': '',
        'TransactionSequence#': '',
        'RecordType': 'CMQ',
        'WorkflowTaskID': '1397',
        'WorkflowStatus': '0',
        'AgencyCode': '52'
    }

    api_response = Response()
    api_response._content = {"searchResults": [{'agency': '52',
                                              'createdDate': '2008-03-04T17:20:20+00:00',
                                              'interestedParties': [{'affiliation': '52 ', 'baseNumber': 'I-001654863-1', 'lastName': 'MCCARTNEY', 'name': 'MCCARTNEY PAUL JAMES', 'nameNumber': 18873266, 'role': 'C'}],
                                              'iswc': 'T0101475073',
                                              'lastModifiedDate': '2008-03-04T17:20:20+00:00',
                                              'linkedISWC': [],
                                              'originalTitle': 'BEATLES MEDLEY',
                                              'otherTitles': [{'title': 'TOMORROW', 'type': 'AT'}],
                                              'works': [{
                                                  'agency': '52',
                                                  'disambiguateFrom': [],
                                                  'disambiguation':False,
                                                  'id':5648148,
                                                  'instrumentation':[],
                                                  'interestedParties':[{'affiliation': '52 ', 'baseNumber': 'I-001654863-1', 'lastName': 'MCCARTNEY', 'name': 'MCCARTNEY PAUL JAMES', 'nameNumber': 18873266, 'role': 'C'}],
                                                  'iswc': 'T0101475073',
                                                  'iswcEligible': True,
                                                  'lastModifiedDate': '2020-02-21T12:37:51+00:00',
                                                  'originalTitle': 'BEATLES MEDLEY',
                                                  'otherTitles': [{'title': 'TOMORROW', 'type': 'AT'}],
                                                  'sourcedb': 52,
                                                  'workcode': '0010103N'}]}]}
    api_response.status_code = 207
    api_response_ok = True

    creation_datetime = datetime.strptime(
        '2017-05-01T01:01:01+0200', "%Y-%m-%dT%H:%M:%S%z")
    current_datetime = datetime.strptime(
        '2017-05-01T01:01:01+0200', "%Y-%m-%dT%H:%M:%S%z")

    expected_result = 'CSN0000000000000000201705010101011    00000000CURBEATLES MEDLEY                                                                                      0520010103N            052T010147507320170501FA      13970'
    csn = CSN('1', header, original_transaction, api_response._content, api_response_ok,
              api_response.status_code, creation_datetime, current_datetime).get_record(0)

    assert expected_result == next(csn)
