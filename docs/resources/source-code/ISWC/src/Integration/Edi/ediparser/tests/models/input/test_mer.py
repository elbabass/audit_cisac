from ediparser.parser.models.input.mer import MER
from ediparser.parser.models.edi_file import EdiFileVersion


def test_mer_get_body():
    transaction = {'MER': 'MER0000000000000000DIARY OF A LATE EIGHTY BABY                                                                         021027679059           021T9254164642                                                     ',
                   'MLI': ['MLI0000000000000001T9254164642']}

    header = {
        'SenderID': '000000161',
        'EdiFileVersion': EdiFileVersion.THREE
    }

    expected_body_result = {
        'iswcs': ['T9254164642']
    }

    mer = MER(transaction, header)

    assert expected_body_result == mer.get_body()


def test_mer_get_parameters():
    transaction = {'MER': 'MER0000000000000000DIARY OF A LATE EIGHTY BABY                                                                         021027679059           021T9254164642                                                     ',
                   'MLI': ['MLI0000000000000001T9254164642']}

    header = {
        'SenderID': '000000161',
        'EdiFileVersion': EdiFileVersion.THREE
    }

    expected_parameters_result = {
        'preferredIswc': 'T9254164642', 'agency': '021'}

    mer = MER(transaction, header)

    assert expected_parameters_result == mer.get_parameters()
