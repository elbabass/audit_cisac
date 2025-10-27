from ediparser.parser.models.input.tsv.fsq import FSQ


def test_fsq_get_body():
    from ediparser.parser.models.input.tsv.fsq import FSQ
    transaction = ['findSubmissions', '4', '038', '038', 'Sony/ATV Music Publishing LLC', '589330613', 'AM', 'info@spanishpoint.ie', 'SONY10001',
                   'FALSE', '', '', '', '', 'Sample Work Title 5', 'AB1231212345|IE1231212345', '(312,1234)|(021,567897)', 'PRELVUKAJ GENC', '165842942', 'CA']
    header = {}
    expected_value = {
        'submissionId': '4',
        'submission': {
            'agency': '038',
            'disambiguation': 'FALSE',
            'workcode': 'SONY10001',
            'interestedParties': [{
                'role': 'CA',
                'name': 'PRELVUKAJ GENC',
                'nameNumber': '165842942'
            },
                {
                'name': 'Sony/ATV Music Publishing LLC',
                'nameNumber': '589330613',
                'role': 'AM'
            }
            ],
            'additionalIdentifiers': {
                'publisherIdentifiers': [{
                    'workCode': ['SONY10001'],
                    'nameNumber': '589330613'
                }],
                'isrcs': ['AB1231212345', 'IE1231212345'],
                'agencyWorkCodes': [
                    {
                        'agency': '312',
                        'workCode': '1234'
                    },
                    {
                        'agency': '021',
                        'workCode': '567897'
                    }]
            },
            'category': 'DOM',
            'sourcedb': '038',
            'originalTitle': 'Sample Work Title 5'
        },
        'submittingPublisher': {
            'email': 'info@spanishpoint.ie',
            'name': 'Sony/ATV Music Publishing LLC',
            'nameNumber': '589330613',
            'role': 'AM'
        }
    }
    fsq = FSQ(transaction, header)
    fsq_submission = fsq.get_submission().get('body')
    assert expected_value == fsq_submission


def test_fsq_get_body_multiple_ips():
    from ediparser.parser.models.input.tsv.fsq import FSQ
    transaction = ['findSubmissions', '4', '038', '038', 'Sony/ATV Music Publishing LLC', '589330613', 'AM', 'info@spanishpoint.ie', 'SONY10001',
                   'FALSE', '', '', '', '', 'Sample Work Title 5', 'AB1231212345|IE1231212345', '(312,1234)|(021,567897)', 'PRELVUKAJ GENC', '165842942', 'CA', 'BOURKE CIARAN FRANCIS', '36303314', 'C', 'O BRIEN LIAM PATRICK', '159837032', 'C', 'EDWARD JOHN SCHWEPPE', '46932859', 'C']
    header = {}

    expected_value = {
        'submissionId': '4',
        'submission': {
            'agency': '038',
            'disambiguation': 'FALSE',
            'workcode': 'SONY10001',
            'interestedParties': [
                {
                    'role': 'CA',
                    'name': 'PRELVUKAJ GENC',
                    'nameNumber': '165842942'
                },
                {
                    'role': 'C',
                    'name': 'BOURKE CIARAN FRANCIS',
                    'nameNumber': '36303314'
                },
                {
                    'role': 'C',
                    'name': 'O BRIEN LIAM PATRICK',
                    'nameNumber': '159837032'
                },
                {
                    'role': 'C',
                    'name': 'EDWARD JOHN SCHWEPPE',
                    'nameNumber': '46932859'
                },
                {
                    'name': 'Sony/ATV Music Publishing LLC',
                    'nameNumber': '589330613',
                    'role': 'AM'
                }
            ],
            'additionalIdentifiers': {
                'publisherIdentifiers': [{
                    'workCode': ['SONY10001'],
                    'nameNumber': '589330613'
                }],
                'isrcs': ['AB1231212345', 'IE1231212345'],
                'agencyWorkCodes': [
                    {
                        'agency': '312',
                        'workCode': '1234'
                    },
                    {
                        'agency': '021',
                        'workCode': '567897'
                    }]
            },
            'category': 'DOM',
            'sourcedb': '038',
            'originalTitle': 'Sample Work Title 5'
        },
        'submittingPublisher': {
            'email': 'info@spanishpoint.ie',
            'name': 'Sony/ATV Music Publishing LLC',
            'nameNumber': '589330613',
            'role': 'AM'
        }
    }
    fsq = FSQ(transaction, header)
    fsq_submission = fsq.get_submission().get('body')
    assert expected_value == fsq_submission
