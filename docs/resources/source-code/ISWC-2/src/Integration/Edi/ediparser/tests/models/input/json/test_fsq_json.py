from time import sleep
from ediparser.parser.models.input.json.fsq import FSQ


def test_fsq_get_body():
    transaction = {
        "submissionId": 10869,
        "workcode": "R00489167",
        "disambiguation": False,
        "originalTitle": "Every rakingVV",
        "interestedParties": [
            {
                    "nameNumber": 34493778,
                    "role": "C"
            }
        ],
        "agencyWorkCodes": [
        {
          "agency": "312",
          "workCode": "123456789"
        },
        {
          "agency": "312",
          "workCode": "123456789"
        }
      ]
    }
    header = {
        "submittingAgency": "021",
        "submittingSourcedb": 21,
        "submittingPublisher": {
            "name": "Sony/ATV Music Publishing LLC",
            "nameNumber": 269021863,
            "email": "info@spanishpoint.ie",
            "role": "AM"
        },
        "fileCreationDateTime": "2020-03-31T09:10:43.511Z",
        "receivingAgency": "315"
    }
    fsq = FSQ(transaction, header)

    Expected_value = {'submissionId': 10869,
                      'submission': {
                          'disambiguation': False,
                          'category': 'DOM',
                          'workcode': '',
                          'sourcedb': 21,
                          'submissionId': 10869,
                          'agency': '021',
                          'additionalIdentifiers': {
                              'publisherIdentifiers': [{
                                  'nameNumber': 269021863,
                                    'workCode': ['R00489167']
                              }],
                                "agencyWorkCodes": [
                                {
                                    "agency": "312",
                                    "workCode": "123456789"
                                },
                                {
                                    "agency": "312",
                                    "workCode": "123456789"
                                }]
                                },
                          'interestedParties': [
                              {
                                  'role': 'C',
                                  'nameNumber': 34493778
                              }
                          ],
                          'originalTitle': 'Every rakingVV',
                          'previewDisambiguation': True,
                      }
                      }
    submission = fsq.get_submission().get('body')
    submission['submission']['workcode'] = ''
    assert Expected_value == submission
