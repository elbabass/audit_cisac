from requests.models import Response

from ediparser.parser.models.output.ciq import CIQ
from ediparser.parser.models.output.cwi import CWI


def test_CIQ_get_record():
    api_response = Response()
    api_response._content = b'{"searchResults": [{"iswc":"T9031153170","agency":"315","originalTitle":"SAFARI","otherTitles":[],"interestedParties":[{"name":"VISGER JONATHAN ROBERT","nameNumber":463130289,"baseNumber":"I-002234809-8","role":"C"},{"name":"PRECIOUS PRECIOUS","nameNumber":463527938,"baseNumber":"I-002236982-8","role":"E"}],"works":[{"id":2524287308,"iswc":"T9031153170","iswcEligible":true,"deleted":false,"createdDate":"2010-01-05T19:35:21+00:00","lastModifiedDate":"2017-07-16T04:31:41+00:00","lastModifiedBy":"AGENT","agency":"10","sourcedb":10,"workcode":"00000880637991","category":"DOM","disambiguation":false,"disambiguateFrom":[],"derivedFromIswcs":[],"performers":[{"lastName":"ABSOFACTO"}],"instrumentation":[],"cisnetCreatedDate":"2016-11-04T01:00:00+00:00","cisnetLastModifiedDate":"2017-07-01T02:00:00+00:00","originalTitle":"SAFARI","otherTitles":[],"interestedParties":[{"name":"VISGER JONATHAN ROBERT","nameNumber":463130289,"baseNumber":"I-002234809-8","role":"C"}]}]}]}'
    api_response.status_code = 200

    ciq_records = CIQ(api_response.json(), None, None).get_record()

    Expected_value = {
        'WorkTitle': 'SAFARI',
        'AgencyCode': '315',
        # 'AgencyWorkCode': (153, 20),
        'SourceDBCode': '315',
        'PreferredISWC': 'T9031153170',
    }

    assert Expected_value == ciq_records


def test_CIQ_get_child_records():
    api_response = Response()
    api_response._content = b'{"searchResults": [{"iswc":"T9031153170","agency":"315","originalTitle":"SAFARI","otherTitles":[],"interestedParties":[{"name":"VISGER JONATHAN ROBERT","nameNumber":463130289,"baseNumber":"I-002234809-8","role":"C"},{"name":"PRECIOUS PRECIOUS","nameNumber":463527938,"baseNumber":"I-002236982-8","role":"E"}],"works":[{"id":2524287308,"iswc":"T9031153170","iswcEligible":true,"deleted":false,"createdDate":"2010-01-05T19:35:21+00:00","lastModifiedDate":"2017-07-16T04:31:41+00:00","lastModifiedBy":"AGENT","agency":"10","sourcedb":10,"workcode":"00000880637991","category":"DOM","disambiguation":false,"disambiguateFrom":[],"derivedFromIswcs":[],"performers":[],"instrumentation":[],"cisnetCreatedDate":"2016-11-04T01:00:00+00:00","cisnetLastModifiedDate":"2017-07-01T02:00:00+00:00","originalTitle":"SAFARI","otherTitles":[],"interestedParties":[{"name":"VISGER JONATHAN ROBERT","nameNumber":463130289,"baseNumber":"I-002234809-8","role":"C"}]}]}]}'
    api_response.status_code = 200

    ciq_childrecords = CIQ(api_response.json(), None, None).get_child_records()

    Expected_value = ['CWI                01000000880637991      010T9031153170           ',
                      'CIP                463130289  I-002234809-8C ']

    assert Expected_value == ciq_childrecords
