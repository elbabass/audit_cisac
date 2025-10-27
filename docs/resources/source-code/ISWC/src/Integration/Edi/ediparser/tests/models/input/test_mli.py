from ediparser.parser.models.input.mli import MLI
from ediparser.parser.models.edi_file import EdiFileVersion


def test_mer_get_body():
    transaction = 'MLI0000000000000001T9254164642'

    expected_body_result = 'T9254164642'

    mli = MLI({'MLI': transaction}, None)

    assert expected_body_result == mli.get_body()
