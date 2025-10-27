from ediparser.parser.models.output.der import DER


def test_der_get_body():
    transaction = {
        'iswc': 'T0300161027',
        'title': 'NATIONAL LOTTERY NIFTY FIFTY SCRATCH CARD'
    }

    expected_result = 'DER                T0300161027NATIONAL LOTTERY NIFTY FIFTY SCRATCH CARD                                                           '

    der = DER(transaction)

    assert expected_result == der.get_record()
