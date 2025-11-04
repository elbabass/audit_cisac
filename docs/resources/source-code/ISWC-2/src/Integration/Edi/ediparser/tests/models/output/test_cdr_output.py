from requests.models import Response

from ediparser.parser.models.edi_file import EdiFileVersion
from ediparser.parser.models.input.cdr import CDR as CDRInput
from ediparser.parser.models.output.cdr import CDR


def test_cdr_get_record_version_two():
    header = {
        'SenderID': '000000161',
        'EdiFileVersion': EdiFileVersion.TWO
    }
    original_transaction_input = {
        'CDR': 'CDR0000000000000000SIS IR TAS GAIDU PILNAIS LAIKS                              122453536355zycb       122T0029584960T0029584960'
    }

    original_transaction = CDRInput(original_transaction_input, header)

    api_response = None
    expected_result = None

    cdr = CDR(api_response, original_transaction, header)

    assert expected_result == cdr.get_record()


def test_cdr_get_record_version_three():
    header = {
        'SenderID': '000000161',
        'EdiFileVersion': EdiFileVersion.THREE
    }
    original_transaction_input = {
        'CDR': 'CDR0000000000000000SIS IR TAS GAIDU PILNAIS LAIKS                                                                      122453536355zycb       122T0029584960T0029584960                                          Test'
    }

    original_transaction = CDRInput(original_transaction_input, header)

    api_response = None
    expected_result = None

    cdr = CDR(api_response, original_transaction, header)

    assert expected_result == cdr.get_record()
