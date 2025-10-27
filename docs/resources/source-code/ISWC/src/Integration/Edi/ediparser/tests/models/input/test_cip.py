from ediparser.parser.models.input.cip import CIP


def test_cip_get_body():
    transaction = 'CIP000000000000000100187739610             A '
    expected_result = {
        "baseNumber": "",
        "role": "A",
        "nameNumber": 187739610
    }

    cip = CIP({'CIP': transaction}, None)

    assert expected_result == cip.get_body()
