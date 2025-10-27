from ediparser.parser.models.input.upw import UPW
from ediparser.parser.models.edi_file import EdiFileVersion

def test_upw_get_body_ips_and_performers():
    transaction = {
        'UPW': 'UPW00000002000000001000 MEMORIES                                                                                                                                                                                           ',
        'PIP': ['PIP0000000000000002ABBA                                                                                00000000000IN000000',
                'PIP0000000100000002QUEEN                                                                               00000000000IN000000',
                'PIP0000000000000004EDDY DUANE                                                                                     CA      ',
                'PIP0000000300000011FRIEND CLIFF                                                                                   CA      ']
    }
    header = {'SenderID': '000000161',
              'EdiFileVersion': EdiFileVersion.ONE}

    expected_result = {
        'titles': [
            {
                'title': '1000 MEMORIES',
                'type': 'OT'
            }
        ],
        'interestedParties': [{'lastName': 'EDDY DUANE', 'name': '', 'role': 'CA'}, {'lastName': 'FRIEND CLIFF', 'name': '', 'role': 'CA'}],
        'performers': [{'lastName': 'ABBA', 'firstName': ''}, {'lastName': 'QUEEN', 'firstName': ''}]
    }
    
    upw = UPW(transaction, header)
    body = upw.get_body()

    assert expected_result == body
    assert None == upw.get_parameters()

def test_upw_get_body_ips():
    transaction = {
        'UPW': 'UPW0000000200000000PART OF THE GAME                                               57748308                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      ',
        'PIP': ['PIP0000000200000006KOHAVI                                       TAL                           4366021  563602752  CA                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            ',
                'PIP0000000200000007GLUSMAN                                      YOGEV                         8387002  744956501  CA                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            ',
                'PIP0000000200000008HERSHKOVITS                                  NITAI                         9615437  814944524  CA                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            ',
                'PIP0000000200000009PENKIN                                       JENNY                         10780346 899876036  CA                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            ',
                'PIP0000000200000010NOMOK                                                                      12847564 0          CA                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            ']
    }
    header = {'SenderID': '000000161',
              'EdiFileVersion': EdiFileVersion.ONE}

    expected_result = {
        'titles': [
            {
                'title': 'PART OF THE GAME',
                'type': 'OT'
            }
        ],
        'interestedParties': [{'lastName': 'KOHAVI', 'name': 'TAL', 'role': 'CA'}, {'lastName': 'GLUSMAN', 'name': 'YOGEV', 'role': 'CA'}, {'lastName': 'HERSHKOVITS', 'name': 'NITAI', 'role': 'CA'}, {'lastName': 'PENKIN', 'name': 'JENNY', 'role': 'CA'}, {'lastName': 'NOMOK', 'name': '', 'role': 'CA'}],
        'performers': []
    }
    
    upw = UPW(transaction, header)
    body = upw.get_body()

    assert expected_result == body
    assert None == upw.get_parameters()

def test_upw_get_body_performers():
    transaction = {
        'UPW': 'UPW0000000500000000STAYIN ALIVE                                                                                                                                                                            ',
        'PIP': ['PIP0000000400000002THIN LIZZY                                                                          00000000000IN000000',
                'PIP0000001200000002SZA                                                                                 00000000000IN000000',
                'PIP0000001400000002CHARLI XCX                                                                          00000000000IN000000',
                'PIP0000000200000002RED HOT CHILI PEPPERS                                                               00000000000IN000000']
    }
    header = {'SenderID': '000000161',
              'EdiFileVersion': EdiFileVersion.ONE}

    expected_result = {
        'titles': [
            {
                'title': 'STAYIN ALIVE',
                'type': 'OT'
            }
        ],
        'interestedParties': [],
        'performers': [{'lastName': 'THIN LIZZY', 'firstName': ''}, {'lastName': 'SZA', 'firstName': ''}, {'lastName': 'CHARLI XCX', 'firstName': ''}, {'lastName': 'RED HOT CHILI PEPPERS', 'firstName': ''}]
    }
    
    upw = UPW(transaction, header)
    body = upw.get_body()

    assert expected_result == body
    assert None == upw.get_parameters()