from ediparser.parser.models.output.ins import INS


def test_ins_get_record():
    transaction = {
        "code": "ALP"
    }
    expected_result = 'INS                ALP'

    ins = INS(transaction)

    assert expected_result == ins.get_record()
