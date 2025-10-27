from requests.models import Response

from ediparser.parser.models.output.json.ciq_ext import CIQEXT

    
def test_CIQ_EXT_get_record():
    api_response = Response()
    api_response._content = b'{"searchResult":{"firstName":"ANDREW JOHN","lastName":"HOZIER BYRNE","nameNumber":589238793,"matchingIswcs":[{"iswc":"T0301600225","agency":"128","originalTitle":"WORK SONG","workNumbers":[{"agencyCode":"128","agencyWorkCode":"R16760727"},{"agencyCode":"008","agencyWorkCode":"GW45975612"},{"agencyCode":"021","agencyWorkCode":"017688353"},{"agencyCode":"052","agencyWorkCode":"135711HQ"},{"agencyCode":"080","agencyWorkCode":"534583247"},{"agencyCode":"058","agencyWorkCode":"2789821911"}]}]}}'
    api_response.status_code = 200

    ciq_records = CIQEXT(api_response.json()).get_record()

    expected_value = {
        'originalTransactionType': 'PublisherContextSearch',
        'searchResult': {
            'firstName': 'ANDREW JOHN',
            'lastName': 'HOZIER BYRNE',
            'nameNumber': 589238793,
            'matchingIswcs': [{
                'iswc': 'T0301600225',
                'agency': '128',
                'originalTitle': 'WORK SONG',
                'workNumbers': [{
                    'agencyCode': '128',
                    'agencyWorkCode': 'R16760727'
                }, {
                    'agencyCode': '008',
                    'agencyWorkCode': 'GW45975612'
                }, {
                    'agencyCode': '021',
                    'agencyWorkCode': '017688353'
                }, {
                    'agencyCode': '052',
                    'agencyWorkCode': '135711HQ'
                }, {
                    'agencyCode': '080',
                    'agencyWorkCode': '534583247'
                }, {
                    'agencyCode': '058',
                    'agencyWorkCode': '2789821911'
                }]
            }]
        }
    }

    assert expected_value == ciq_records