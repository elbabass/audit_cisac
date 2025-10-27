from ediparser.parser.services.tsv_deserializer_service import DeserializerTsvService
from ediparser.parser.models.input.tsv.fsq import FSQ


def test_deserialize_file_FSQ():
    file_name = 'cust1/In/DeserializeTest.txt'
    transaction = [['findSubmissions', '4', '038', '038', 'Sony/ATV Music Publishing LLC', '', 'AM', 'info@spanishpoint.ie', 'SONY10001',
                    'FALSE', '', '', '', '', 'Sample Work Title 5', 'AB1231212345|IE1231212345', 'PRELVUKAJ GENC', '165842942', 'CA', '', '', '']]

    ds = DeserializerTsvService(file_name, transaction)

    dsut = ds.deserialize_file()
    expected_result = {
        'code': '_134',
        'message': 'IP Name Number is invalid, must be numeric'
    }

    for row in dsut.groups[0].df_arr[0].itertuples():
        transaction = row.api_request

    assert expected_result == transaction.get_rejection()
