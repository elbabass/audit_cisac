from ediparser.parser.models.output.per import PER


def test_per_get_record():
    transaction = {
        'firstName': 'Paul',
        'lastName': 'Hewson'
    }

    expected_result = 'PER                Paul                                              Hewson                                            '

    per = PER(transaction)

    assert expected_result == per.get_record()
