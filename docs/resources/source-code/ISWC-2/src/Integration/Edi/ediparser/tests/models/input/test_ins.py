from ediparser.parser.models.input.ins import INS


def test_ins_get_body():
    transaction = 'INS0000000000000004ALP'
    expected_result = {
        "code": "ALP"
    }

    ins = INS({'INS': transaction}, None)

    assert expected_result == ins.get_body()
