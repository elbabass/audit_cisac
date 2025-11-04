from ediparser.parser.models.input.tsv.car import CAR


def test_car_get_body():
    transaction = ['addSubmissions', '4', '038', '038', 'Sony/ATV Music Publishing LLC', '589330613', 'AM', 'info@spanishpoint.ie', 'SONY10001',
                   'FALSE', '', '', '', '', '', '', '', '', 'Sample Work Title 5', 'AB1231212345|IE1231212345', 'PRELVUKAJ GENC', '165842942', 'CA', '', '', '']
    header = {}
    expected_value = {
        'submittingPublisher': {
            'nameNumber': '589330613',
            'email': 'info@spanishpoint.ie',
            'name': 'Sony/ATV Music Publishing LLC',
            'role': 'AM'
        },
        'submissionId': '4',
        'submission': {
            'disambiguation': 'FALSE',
            'sourcedb': '038',
            'interestedParties': [{
                'nameNumber': '165842942',
                'name': 'PRELVUKAJ GENC',
                'role': 'CA'
            },
            {
                'name': 'Sony/ATV Music Publishing LLC',
                'nameNumber': '589330613',
                'role': 'AM'
            }
            ],
            'agency': '038',
            'workcode': '',
            'additionalIdentifiers': {
                      'publisherIdentifiers': [{
                          'nameNumber': '589330613',
                          'workCode': ['SONY10001']
                      }],
                'isrcs': ['AB1231212345', 'IE1231212345']
            },
            'category': 'DOM',
            'originalTitle': 'Sample Work Title 5'
        }}
    car = CAR(transaction, header)
    car_submission = car.get_submission().get('body')
    car_submission['submission']['workcode'] = ''
    assert expected_value == car_submission
