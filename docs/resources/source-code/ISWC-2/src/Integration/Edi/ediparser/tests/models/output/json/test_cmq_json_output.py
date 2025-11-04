from requests.models import Response

from ediparser.parser.models.output.json.cmq import CMQ


def test_CMQ_get_record():
    api_response = Response()
    api_response._content = b'{"searchResults": [{"iswc":"T9031153170","iswcStatus": "Preferred","agency":"315","originalTitle":"SAFARI","otherTitles":[],"linkedISWC":[],"interestedParties":[{"name":"VISGER JONATHAN ROBERT","nameNumber":463130289,"baseNumber":"I-002234809-8","role":"C"},{"name":"PRECIOUS PRECIOUS","nameNumber":463527938,"baseNumber":"I-002236982-8","role":"E"}],"works":[{"id":2524287308,"iswc":"T9031153170","iswcEligible":true,"deleted":false,"createdDate":"2010-01-05T19:35:21+00:00","lastModifiedDate":"2017-07-16T04:31:41+00:00","lastModifiedBy":"AGENT","agency":"10","sourcedb":10,"workcode":"00000880637991","category":"DOM","disambiguation":false,"disambiguateFrom":[],"derivedFromIswcs":[],"performers":[{"lastName":"ABSOFACTO"}],"instrumentation":[],"cisnetCreatedDate":"2016-11-04T01:00:00+00:00","cisnetLastModifiedDate":"2017-07-01T02:00:00+00:00","originalTitle":"SAFARI","otherTitles":[],"interestedParties":[{"name":"VISGER JONATHAN ROBERT","nameNumber":463130289,"baseNumber":"I-002234809-8","role":"C"}]}]}]}'
    api_response.status_code = 200

    cmq_records = CMQ(api_response.json()).get_record()

    expected_value = {
        'originalTransactionType': 'SearchByIswcSubmission',
        'originalTitle': 'SAFARI',
        'agency': '315',
        'iswc': 'T9031153170',
        'iswcStatus': 'Preferred',
        "interestedParties": [{
            "name": "VISGER JONATHAN ROBERT",
            "nameNumber": 463130289,
            "baseNumber": "I-002234809-8",
            "role": "C"
        },
            {
                "name": "PRECIOUS PRECIOUS",
                "nameNumber": 463527938,
                "baseNumber": "I-002236982-8",
                "role": "E"
        }],
        'otherTitles': [],
        'workInfo': [
            {
                'agency': '10',
                'sourcedb': 10,
                'workcode': '00000880637991'
            }
        ],
        'linkedISWC':[],
        'parentISWC': None,
        'overallParentISWC': None

    }

    assert expected_value == cmq_records