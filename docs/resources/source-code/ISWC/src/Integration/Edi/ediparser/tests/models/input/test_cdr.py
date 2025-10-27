from ediparser.parser.models.input.cdr import CDR
from ediparser.parser.models.edi_file import EdiFileVersion


def test_cdr_get_parameters_version_three():
    transaction = {'CDR': 'CDR0000000000000000SIS IR TAS GAIDU PILNAIS LAIKS                                                                      122453536355zycb       122T0029584960T0029584960                                          Test'}

    header = {
        'SenderID': '000000161',
        'EdiFileVersion': EdiFileVersion.THREE
    }

    expected_result = {
        'preferredIswc': 'T0029584960',
        'agency': '122',
        'workcode': '453536355zycb',
        'sourceDb': '122',
        'reasonCode': 'Test'
    }

    cdr = CDR(transaction, header)

    assert expected_result == cdr.get_parameters()
