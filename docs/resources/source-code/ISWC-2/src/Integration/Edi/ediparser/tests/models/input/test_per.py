from ediparser.parser.models.input.per import PER


def test_per_get_body():
    transaction = 'PER0000000000000003Paul                                               Hewson                                           '
    expected_result = {
        'firstName': 'Paul',
        'lastName': 'Hewson'
    }

    per = PER({'PER': transaction}, None)

    assert expected_result == per.get_body()
