from ediparser.parser.models.input.wft import WFT
from ediparser.parser.models.edi_file import EdiFileVersion


def test_mer_get_body():
    transaction = {'WFT': 'WFT0000000000000000000000096420'}

    header = {
        'SenderID': '000000010',
        'EdiFileVersion': EdiFileVersion.THREE
    }

    expected_body_result = {
        "taskId": '0000000964',
        "workflowType": "MergeApproval",
        "status": "0"
    }

    wft = WFT(transaction, header)

    assert expected_body_result == wft.get_body()


def test_mer_get_parameters():
    transaction = {'WFT': 'WFT0000000000000000000000096420'}

    header = {
        'SenderID': '000000010',
        'EdiFileVersion': EdiFileVersion.THREE
    }

    expected_parameters_result = {
        'agency': '010'
    }

    wft = WFT(transaction, header)

    assert expected_parameters_result == wft.get_parameters()
