from ediparser.parser.models.edi_file import EdiFileVersion
from ediparser.parser.models.output.ctl import CTL

def test_ctl_get_record_version_two():
    tl={
            'title': 'PHATMATRIX LOUNGE 3',
            'type': 'OA'
            }
    
    header = {
        'SenderID': '000000161',
        'EdiFileVersion': EdiFileVersion.TWO
    }

    expected_result='CTL                PHATMATRIX LOUNGE 3                                         OA'
    
    ctl=CTL(tl,header)
        
    assert expected_result==ctl.get_record()

def test_ctl_get_record_version_three():
    tl={
            'title': 'PHATMATRIX LOUNGE 3',
            'type': 'OA'
            }
    
    header = {
        'SenderID': '000000161',
        'EdiFileVersion': EdiFileVersion.THREE
    }

    expected_result='CTL                PHATMATRIX LOUNGE 3                                                                                 OA'
    
    ctl=CTL(tl,header)
        
    assert expected_result==ctl.get_record()