from ediparser.parser.models.input.der import DER


def test_der_get_body():
    transaction = 'DER0000000000000002T0300161027NATIONAL LOTTERY NIFTY FIFTY SCRATCH CARD                                                           '
    expected_result = {
        'ISWC': 'T0300161027',
        'Title': 'NATIONAL LOTTERY NIFTY FIFTY SCRATCH CARD'
    }

    der = DER({'DER': transaction}, None)

    assert expected_result == der.get_body()
