from ediparser.parser.models.input.pip import PIP

def test_pip_interested_party_get_body():
    transaction = 'PIP0000000300000002GUSTAFSON MONA AGNETA                                                               00000000000CA000000'
    expected_result = {
        'lastName': 'GUSTAFSON MONA AGNETA',
        'name': '',
        'role': 'CA'
    }

    pip = PIP({'PIP': transaction}, None)

    assert expected_result == pip.get_body('InterestedParty')

def test_pip_performer_get_body():
    transaction = 'PIP0000001100000002TUPAC SHAKUR                                                                        00000000000IN000000'
    expected_result = {
        'lastName': 'TUPAC SHAKUR',
        'firstName': ''
    }

    pip = PIP({'PIP': transaction}, None)

    assert expected_result == pip.get_body('Performer')