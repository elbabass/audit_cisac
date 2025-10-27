from ediparser.parser.services.deserializer_service import DeserializerService
from ediparser.parser.models.edi_file import EdiFileVersion
from ediparser.parser.models.group import Group
from ediparser.parser.models.input.cmq import CMQ
from ediparser.parser.models.input.cur import CUR


def test_deserialize_file_header_version_two():
    file_name = 'cust1/In/DISTest.020'
    file_contents = [
        'HDRAA000000128WID CENTER                                   02.002018070517050720180705',
        'GRHCUR0000101.00',
        'GRT000010000007200000650',
        'TRL000010000007200000652'
    ]
    ds = DeserializerService(file_name, file_contents)
    expected_result = {'CharacterSet': '', 'CreationDate': '20180705', 'CreationTime': '170507', 'EdiFileVersion': EdiFileVersion.TWO, 'OriginalFile': '', 'RecordType': 'HDR',
                       'SenderID': '000000128', 'SenderIPNameNumber': '', 'SenderName': 'WID CENTER', 'SenderType': 'AA', 'StandardVersionNumber': '02.00', 'TransmissionDate': '20180705'}
    dsut = ds.deserialize_file()
    assert expected_result == dsut.header


def test_deserialize_file_header_version_three():
    file_name = 'cust1/In/DISTest.030'
    file_contents = [
        'HDRAA000000128WID CENTER                                   03.002018070517050720180705',
        'GRHCUR0000101.00',
        'GRT000010000007200000650',
        'TRL000010000007200000652'
    ]
    ds = DeserializerService(file_name, file_contents)
    expected_result = {'CharacterSet': '', 'CreationDate': '20180705', 'CreationTime': '170507', 'EdiFileVersion': EdiFileVersion.THREE, 'OriginalFile': '', 'RecordType': 'HDR',
                       'SenderID': '000000128', 'SenderIPNameNumber': '', 'SenderName': 'WID CENTER', 'SenderType': 'AA', 'StandardVersionNumber': '03.00', 'TransmissionDate': '20180705'}
    dsut = ds.deserialize_file()
    assert expected_result == dsut.header


def test_deserialize_file_groups_CUR():
    file_name = 'cust1/In/DISTest.030'
    file_contents = [
        'HDRAA000000128WID CENTER                                   03.002018070517050720180705',
        'GRHCUR0000101.00',
        'CUR0000000100000000PARADISE84                                                                                          128R26919333oku        300           T0302206116ModifiedVersion     TDIT                 B',
        'CTL0000000100000002PARADISE84T1                                                                                        OA',
        'CTL0000000100000002PARADISE84T2                                                                                        OA',
        'CTL0000000100000002PARADISE84T3                                                                                        OA',
        'GRT000010000007200000650',
        'TRL000010000007200000652'
    ]
    ds = DeserializerService(file_name, file_contents)
    expected_result_CUR = 'CUR0000000100000000PARADISE84                                                                                          128R26919333oku        300           T0302206116ModifiedVersion     TDIT                 B'
    expected_result_CTL_0 = 'CTL0000000100000002PARADISE84T1                                                                                        OA'
    expected_result_CTL_1 = 'CTL0000000100000002PARADISE84T2                                                                                        OA'
    expected_result_CTL_2 = 'CTL0000000100000002PARADISE84T3                                                                                        OA'

    dsut = ds.deserialize_file()
    assert expected_result_CUR == dsut.groups[0].df_arr[0].CUR._values[0]
    assert expected_result_CTL_0 == dsut.groups[0].df_arr[0].CTL._values[0][0]
    assert expected_result_CTL_1 == dsut.groups[0].df_arr[0].CTL._values[0][1]
    assert expected_result_CTL_2 == dsut.groups[0].df_arr[0].CTL._values[0][2]


def test_deserialize_file_groups_CMQ():
    file_name = 'cust1/In/CMQTest.030'
    file_contents = [
        'HDRSO000000021BMI                                          03.002017050217502320170502',
        'GRHCMQ0000101.00',
        'CMQ0000000200000000                                                                                                    000                    000T9116239771',
        'CMQ0000000200000000                                                                                                    000                    000T0300222314',
        'GRT000010007727000077272',
        'TRL000010007727000077274'
    ]

    ds = DeserializerService(file_name, file_contents)
    expected_result_CMQ_0 = 'CMQ0000000200000000                                                                                                    000                    000T9116239771'
    expected_result_CMQ_1 = 'CMQ0000000200000000                                                                                                    000                    000T0300222314'

    dsut = ds.deserialize_file()
    assert expected_result_CMQ_0 == dsut.groups[0].df_arr[0].CMQ._values[0]
    assert expected_result_CMQ_1 == dsut.groups[0].df_arr[0].CMQ._values[1]

