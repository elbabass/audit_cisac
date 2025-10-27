# Databricks notebook source
# RECORD SLICING
from pyspark.sql.types import *
import datetime
import decimal
from decimal import Decimal
from zipfile import ZipFile
import logging
import sys
import os
import time
import math
import multiprocessing
from multiprocessing.pool import ThreadPool as Pool
from itertools import starmap


class RowDefinition():
    def __init__(self, record_type: str, transaction: list, fields: dict):
        self.fields = fields
        self.transaction = transaction
        self.record_type = record_type

    def get_field(self, transaction_field: str, record_line: str):
        (start, size) = self.fields[transaction_field]
        start = start-1
        field_value = record_line[start:start+size]
        return(field_value)

    def get_record_lines(self):
        record_lines = [
            line for line in self.transaction if line.startswith(self.record_type)]
        return(record_lines)

###########################################


class DateTime():
    def get_datetime(self, date_value: str, time_value: str):
        a = date_value+time_value
        date_time_str = a[0:4]+'-'+a[4:6]+'-'+a[6:8] + \
            ' '+a[8:10]+':'+a[10:12]+':'+a[12:14]
        date_time = datetime.datetime.strptime(
            date_time_str, '%Y-%m-%d %H:%M:%S')
        return(date_time)

    def get_date(self, year_value: str, month_value: str, day_value: str):
        a = year_value+month_value+day_value
        if(not (a and not a.isspace())):
            pass
        elif year_value != '    ' and year_value != '0000' and month_value == '  ' and day_value == '  ':
            date_str = a[0:4]
            datetime_obj = datetime.datetime.strptime(date_str, '%Y')
            date = datetime.datetime.date(datetime_obj)
            return(date)
        elif year_value != '    ' and year_value != '0000' and month_value != '  ' and day_value == '  ':
            date_str = a[0:4]+'-'+a[4:6]
            datetime_obj = datetime.datetime.strptime(date_str, '%Y-%m')
            date = datetime.datetime.date(datetime_obj)
            return(date)
        elif year_value != '    ' and year_value != '0000' and month_value != '  ' and day_value != '  ':
            date_str = a[0:4]+'-'+a[4:6]+'-'+a[6:8]
            datetime_obj = datetime.datetime.strptime(date_str, '%Y-%m-%d')
            date = datetime.datetime.date(datetime_obj)
            return(date)

    def to_date(self, dates_value: str):
        date_str = dates_value[0:4]+'-'+dates_value[4:6]+'-'+dates_value[6:8]
        datetime_obj = datetime.datetime.strptime(date_str, '%Y-%m-%d')
        date = datetime.datetime.date(datetime_obj)
        return(date)

###########################################
# RECORD TYPES


class IPA():
    def __init__(self, transaction: list, key: int):
        self.transaction = transaction
        self.key = key
        self.fields = RowDefinition(self.__class__.__name__, self.transaction, {
            'RecordType': (1, 3),
            'TransactionSequenceNo': (4, 8),
            'DetailRecordSequenceNo': (12, 8),
            'TransactionDate': (20, 8),
            'TransactionTime': (28, 6),
            'CodeOfRemittingSociety': (34, 3),
            'NameOfRemittingSociety': (37, 20),
            'IpBaseNumber': (57, 13),
            'CurrentStatusCode': (70, 1),
            'IpNameNumber': (71, 11),
            'NameType': (82, 2),
            'IpBaseNumberRef': (84, 13),
            'CurrentStatusCodeRef': (97, 1),
            'IpNameNumberRef': (98, 11),
            'NameTypeRef': (109, 2)
        })

    def get_body(self):
        values = []
        for record_line in self.fields.get_record_lines():
            values.append({
                'Index': self.key,
                'IPBaseNumber': self.fields.get_field('IpBaseNumber', record_line),
                'AgencyID': str(int(self.fields.get_field('CodeOfRemittingSociety', record_line)))
            })
        return(values)

###########################################


class BDN():
    def __init__(self, transaction: list, key: int):
        self.key = key
        self.transaction = transaction
        self.fields = RowDefinition(self.__class__.__name__, self.transaction, {
            'RecordType': (1, 3),
            'TransactionSequenceNo': (4, 8),
            'DetailRecordSequenceNo': (12, 8),
            'TypeOfInterestedParty': (20, 1),
            'YearOfBirth/Foundation': (21, 4),
            'MonthOfBirth/Foundation': (25, 2),
            'DayOfBirth/Foundation': (27, 2),
            'YearOfDeath/dissolution': (29, 4),
            'MonthOfDeath/dissolution': (33, 2),
            'DayOfDeath/dissolution': (35, 2),
            'PlaceOfBirth/Foundation': (37, 30),
            'StateOfBirth/Foundation': (67, 30),
            'TISNBirth/Foundation': (97, 4),
            'TISNValidFrom': (101, 8),
            'TISANBirth/Foundation': (109, 20),
            'TISANValidFrom': (129, 8),
            'Sex': (137, 1),
            'AmendmentDate': (138, 8),
            'AmendmentTime': (146, 6),
        })

    def get_body(self):
        values = []
        for record_line in self.fields.get_record_lines():
            values.append({
                'Index': self.key,
                'Type': self.fields.get_field('TypeOfInterestedParty', record_line),
                'Gender': self.fields.get_field('Sex', record_line),
                'BirthPlace': self.fields.get_field('PlaceOfBirth/Foundation', record_line),
                'BirthState': self.fields.get_field('StateOfBirth/Foundation', record_line),
                'BirthDate': DateTime().get_date(self.fields.get_field('YearOfBirth/Foundation', record_line), self.fields.get_field('MonthOfBirth/Foundation', record_line), self.fields.get_field('DayOfBirth/Foundation', record_line)),
                'DeathDate': DateTime().get_date(self.fields.get_field('YearOfDeath/dissolution', record_line), self.fields.get_field('MonthOfDeath/dissolution', record_line), self.fields.get_field('DayOfDeath/dissolution', record_line)),
                'AmendedDateTime': DateTime().get_datetime(self.fields.get_field('AmendmentDate', record_line), self.fields.get_field('AmendmentTime', record_line))
            })
        return(values)

###########################################


class STN():
    def __init__(self, transaction: list, key: int):
        self.key = key
        self.transaction = transaction
        self.fields = RowDefinition(self.__class__.__name__, self.transaction, {
            'RecordType': (1, 3),
            'TransactionSequenceNo': (4, 8),
            'DetailRecordSequenceNo': (12, 8),
            'IpBaseNumber': (20, 13),
            'StatusCode': (33, 1),
            'ValidFromDate': (34, 8),
            'ValidFromTime': (42, 6),
            'ValidToDate': (48, 8),
            'ValidToTime': (56, 6),
            'AmendmentDate': (62, 8),
            'AmendmentTime': (70, 6)
        })

    def get_body(self):
        values = []
        for record_line in self.fields.get_record_lines():
            values.append({
                'Index': self.key,
                'FromDate': DateTime().get_datetime(self.fields.get_field('ValidFromDate', record_line), self.fields.get_field('ValidFromTime', record_line)),
                'ToDate': DateTime().get_datetime(self.fields.get_field('ValidToDate', record_line), self.fields.get_field('ValidToTime', record_line)),
                'StatusCode': int(self.fields.get_field('StatusCode', record_line)),
                'ForwardingBaseNumber': self.fields.get_field('IpBaseNumber', record_line),
                'AmendedDateTime': DateTime().get_datetime(self.fields.get_field('AmendmentDate', record_line), self.fields.get_field('AmendmentTime', record_line)),
                'IPBaseNumber': IPA(self.transaction, self.key).get_body()[0].get('IPBaseNumber')
            })
        return(values)
###############################################


class NUN():
    def __init__(self, transaction: list, key: int):
        self.key = key
        self.transaction = transaction
        self.fields = RowDefinition(self.__class__.__name__, self.transaction, {
            'RecordType': (1, 3),
            'TransactionSequenceNo': (4, 8),
            'DetailRecordSequenceNo': (12, 8),
            'IpNameNumber': (20, 11),
            'CreationClassCode': (31, 2),
            'RoleCode': (33, 2),
        })

    def get_body(self):
        values = []
        for record_line in self.fields.get_record_lines():
            values.append({
                'Index': self.key,
                'IPNameNumber': int(self.fields.get_field('IpNameNumber', record_line)),
                'CreationClass': self.fields.get_field('CreationClassCode', record_line),
                'Role': self.fields.get_field('RoleCode', record_line),
                'IPBaseNumber': IPA(self.transaction, self.key).get_body()[0].get('IPBaseNumber')
            })
        return(values)

##################################################


class INN():
    def __init__(self, transaction: list, key: int):
        self.key = key
        self.transaction = transaction
        self.fields = RowDefinition(self.__class__.__name__, self.transaction, {
            'RecordType': (1, 3),
            'TransactionSequenceNo': (4, 8),
            'DetailRecordSequenceNo': (12, 8),
            'IpNameNumber': (20, 11),
            'CreationClassCode': (31, 2),
            'RoleCode': (33, 2),
        })

    def get_body(self):
        values = []
        for record_line in self.fields.get_record_lines():
            values.append({
                'Index': self.key,
                'IPNameNumber': int(self.fields.get_field('IpNameNumber', record_line)),
                'CreationClass': self.fields.get_field('CreationClassCode', record_line),
                'Role': self.fields.get_field('RoleCode', record_line),
                'IPBaseNumber': IPA(self.transaction, self.key).get_body()[0].get('IPBaseNumber')
            })
        return(values)

##################################################


class MUN():
    def __init__(self, transaction: list, key: int):
        self.key = key
        self.transaction = transaction
        self.fields = RowDefinition(self.__class__.__name__, self.transaction, {
            'RecordType': (1, 3),
            'TransactionSequenceNo': (4, 8),
            'DetailRecordSequenceNo': (12, 8),
            'IpNameNumber': (20, 11),
            'CreationClassCode': (31, 2),
            'RoleCode': (33, 2),
            'IpBaseNumber': (35, 13)
        })

    def get_body(self):
        values = []
        for record_line in self.fields.get_record_lines():
            values.append({
                'Index': self.key,
                'IPNameNumber': int(self.fields.get_field('IpNameNumber', record_line)),
                'CreationClass': self.fields.get_field('CreationClassCode', record_line),
                'Role': self.fields.get_field('RoleCode', record_line),
                'IPBaseNumber': self.fields.get_field('IpBaseNumber', record_line),
            })
        return(values)

##################################################


class IMN():
    def __init__(self, transaction: list, key: int):
        self.key = key
        self.transaction = transaction
        self.fields = RowDefinition(self.__class__.__name__, self.transaction, {
            'RecordType': (1, 3),
            'TransactionSequenceNo': (4, 8),
            'DetailRecordSequenceNo': (12, 8),
            'IpNameNumber': (20, 11),
            'CreationClassCode': (31, 2),
            'RoleCode': (33, 2),
            'IpBaseNumber': (35, 13)
        })

    def get_body(self):
        values = []
        for record_line in self.fields.get_record_lines():
            values.append({
                'Index': self.key,
                'IPNameNumber': int(self.fields.get_field('IpNameNumber', record_line)),
                'CreationClass': self.fields.get_field('CreationClassCode', record_line),
                'Role': self.fields.get_field('RoleCode', record_line),
                'IPBaseNumber': self.fields.get_field('IpBaseNumber', record_line),
            })
        return(values)

##################################################


class MAN():
    def __init__(self, transaction: list, key: int):
        self.key = key
        self.transaction = transaction
        self.fields = RowDefinition(self.__class__.__name__, self.transaction, {
            'RecordType': (1, 3),
            'TransactionSequenceNo': (4, 8),
            'DetailRecordSequenceNo': (12, 8),
            'SocietyCode': (20, 3),
            'CreationClassCode': (23, 2),
            'RoleCode': (25, 2),
            'RightCode': (27, 2),
            'ValidFromDate': (29, 8),
            'ValidFromTime': (37, 6),
            'ValidToDate': (43, 8),
            'ValidToTime': (51, 6),
            'DateOfSignature': (57, 8),
            'MembershipShare': (65, 5),
            'AmendmentDate': (70, 8),
            'AmendmentTime': (78, 6),
        })

    def get_body(self):
        values = []
        for record_line in self.fields.get_record_lines():
            values.append({
                'Index': self.key,
                'AgencyID': str(int(self.fields.get_field('SocietyCode', record_line))),
                'CreationClass': self.fields.get_field('CreationClassCode', record_line),
                'Role': self.fields.get_field('RoleCode', record_line),
                'EconomicRights': self.fields.get_field('RightCode', record_line),
                'FromDate': DateTime().get_datetime(self.fields.get_field('ValidFromDate', record_line), self.fields.get_field('ValidFromTime', record_line)),
                'ToDate': DateTime().get_datetime(self.fields.get_field('ValidToDate', record_line), self.fields.get_field('ValidToTime', record_line)),
                'SignedDate': DateTime().to_date(self.fields.get_field('DateOfSignature', record_line)),
                'SharePercentage': round(Decimal(int(self.fields.get_field('MembershipShare', record_line))/100), 2),
                'AmendedDateTime': DateTime().get_datetime(self.fields.get_field('AmendmentDate', record_line), self.fields.get_field('AmendmentTime', record_line)),
                'IPBaseNumber': IPA(self.transaction, self.key).get_body()[0].get('IPBaseNumber')
            })
        return(values)

###############################################


class NCN():
    def __init__(self, transaction: list, key: int):
        self.key = key
        self.transaction = transaction
        self.fields = RowDefinition(self.__class__.__name__, self.transaction, {
            'RecordType': (1, 3),
            'TransactionSequenceNo': (4, 8),
            'DetailRecordSequenceNo': (12, 8),
            'IpNameNumber': (20, 11),
            'Name': (31, 90),
            'FirstName': (121, 45),
            'NameType': (166, 2),
            'DateOfCreation': (168, 8),
            'TimeOfCreation': (176, 6),
            'DateOfLastAmendment': (182, 8),
            'TimeOfLastAmendment': (190, 6),
        })

    def get_body(self):
        values = []
        for record_line in self.fields.get_record_lines():
            values.append({
                'Index': self.key,
                'IPNameNumber': int(self.fields.get_field('IpNameNumber', record_line)),
                'AmendedDateTime': DateTime().get_datetime(self.fields.get_field('DateOfLastAmendment', record_line), self.fields.get_field('TimeOfLastAmendment', record_line)),
                'LastName': self.fields.get_field('Name', record_line),
                'FirstName': self.fields.get_field('FirstName', record_line),
                'CreatedDate': DateTime().get_datetime(self.fields.get_field('DateOfCreation', record_line), self.fields.get_field('TimeOfCreation', record_line)),
                'TypeCode': self.fields.get_field('NameType', record_line),
                'IPBaseNumber': IPA(self.transaction, self.key).get_body()[0].get('IPBaseNumber'),
                'AgencyID': str(int(IPA(self.transaction, self.key).get_body()[0].get('AgencyID')))
            })
        return(values)

##################################################


class ONN():
    def __init__(self, transaction: list, key: int):
        self.key = key
        self.transaction = transaction
        self.fields = RowDefinition(self.__class__.__name__, self.transaction, {
            'RecordType': (1, 3),
            'TransactionSequenceNo': (4, 8),
            'DetailRecordSequenceNo': (12, 8),
            'IpNameNumber': (20, 11),
            'Name': (31, 90),
            'FirstName': (121, 45),
            'NameType': (166, 2),
            'DateOfCreation': (168, 8),
            'TimeOfCreation': (176, 6),
            'DateOfLastAmendment': (182, 8),
            'TimeOfLastAmendment': (190, 6),
            'IpNameNumberRef': (196, 11)
        })

    def get_body(self):
        values = []
        for record_line in self.fields.get_record_lines():
            values.append({
                'Index': self.key,
                'IPNameNumber': int(self.fields.get_field('IpNameNumber', record_line)),
                'AmendedDateTime': DateTime().get_datetime(self.fields.get_field('DateOfLastAmendment', record_line), self.fields.get_field('TimeOfLastAmendment', record_line)),
                'LastName': self.fields.get_field('Name', record_line),
                'FirstName': self.fields.get_field('FirstName', record_line),
                'CreatedDate': DateTime().get_datetime(self.fields.get_field('DateOfCreation', record_line), self.fields.get_field('TimeOfCreation', record_line)),
                'TypeCode': self.fields.get_field('NameType', record_line),
                'ForwardingNameNumber': int(self.fields.get_field('IpNameNumberRef', record_line)),
                'IPBaseNumber': IPA(self.transaction, self.key).get_body()[0].get('IPBaseNumber'),
                'AgencyID': str(int(IPA(self.transaction, self.key).get_body()[0].get('AgencyID')))
            })
        return(values)

##################################################


class MCN():
    def __init__(self, transaction: list, key: int):
        self.key = key
        self.transaction = transaction
        self.fields = RowDefinition(self.__class__.__name__, self.transaction, {
            'RecordType': (1, 3),
            'TransactionSequenceNo': (4, 8),
            'DetailRecordSequenceNo': (12, 8),
            'IpNameNumber': (20, 11),
            'Name': (31, 90),
            'NameType': (121, 2),
            'DateOfCreation': (123, 8),
            'TimeOfCreation': (131, 6),
            'DateOfLastAmendment': (137, 8),
            'TimeOfLastAmendment': (145, 6),
            'IpBaseNumber': (151, 13)
        })

    def get_body(self):
        values = []
        for record_line in self.fields.get_record_lines():
            values.append({
                'Index': self.key,
                'IPNameNumber': int(self.fields.get_field('IpNameNumber', record_line)),
                'AmendedDateTime': DateTime().get_datetime(self.fields.get_field('DateOfLastAmendment', record_line), self.fields.get_field('TimeOfLastAmendment', record_line)),
                'LastName': self.fields.get_field('Name', record_line),
                'CreatedDate': DateTime().get_datetime(self.fields.get_field('DateOfCreation', record_line), self.fields.get_field('TimeOfCreation', record_line)),
                'TypeCode': self.fields.get_field('NameType', record_line),
                'IPBaseNumber': IPA(self.transaction, self.key).get_body()[0].get('IPBaseNumber'),
                'AgencyID': str(int(IPA(self.transaction, self.key).get_body()[0].get('AgencyID')))
            })
        return(values)

##################################################
# TABLE SCHEMAS


class TblSchema():
    def __init__(self, tbl_name: str):
        self.tbl_name = tbl_name

    def get_tbl_schema(self):
        if self.tbl_name == 'InterestedParty':
            return(StructType([
                StructField("IPBaseNumber", StringType(), False),
                StructField("AmendedDateTime", TimestampType(), False),
                StructField("BirthDate", DateType(), True),
                StructField("BirthPlace", StringType(), True),
                StructField("BirthState", StringType(), True),
                StructField("DeathDate", DateType(), True),
                StructField("Gender", StringType(), True),
                StructField("Type", StringType(), False),
                StructField("AgencyID", StringType(), False)
            ]))
        elif self.tbl_name == 'Status':
            return(StructType([
                StructField("AmendedDateTime", TimestampType(), False),
                StructField("FromDate", TimestampType(), False),
                StructField("ToDate", TimestampType(), False),
                StructField("ForwardingBaseNumber", StringType(), False),
                StructField("StatusCode", IntegerType(), False),
                StructField("IPBaseNumber", StringType(), False),
            ]))
        elif self.tbl_name == 'IPNameUsage':
            return(StructType([
                StructField("IPNameNumber", LongType(), False),
                StructField("Role", StringType(), False),
                StructField("CreationClass", StringType(), False),
                StructField("IPBaseNumber", StringType(), False),
            ]))
        elif self.tbl_name == 'Agreement':
            return(StructType([
                StructField("AmendedDateTime", TimestampType(), False),
                StructField("CreationClass", StringType(), False),
                StructField("FromDate", TimestampType(), False),
                StructField("ToDate", TimestampType(), False),
                StructField("EconomicRights", StringType(), False),
                StructField("Role", StringType(), False),
                StructField("SharePercentage", DecimalType(), False),
                StructField("SignedDate", DateType(), True),
                StructField("AgencyID", StringType(), False),
                StructField("IPBaseNumber", StringType(), False),
            ]))
        elif self.tbl_name == 'Name':
            return(StructType([
                StructField("IPNameNumber", LongType(), False),
                StructField("AmendedDateTime", TimestampType(), False),
                StructField("FirstName", StringType(), True),
                StructField("LastName", StringType(), False),
                StructField("CreatedDate", TimestampType(), False),
                StructField("TypeCode", StringType(), False),
                StructField("ForwardingNameNumber", LongType(), True),
                StructField("AgencyID", StringType(), True),
            ]))
        elif self.tbl_name == 'NameReference':
            return(StructType([
                StructField("IPNameNumber", LongType(), True),
                StructField("AmendedDateTime", TimestampType(), True),
                StructField("IPBaseNumber", StringType(), True),
            ]))

###########################################

# PARSING


class Parsing():
    LOOKUP = 'IPA'

    def __init__(self):
        self.ls = LogService()

    def get_data(self, file_location):
        file_open = open(file_location, 'r')
        data = []
        h1 = file_open.readline()
        h2 = file_open.readline()
        for each_line in file_open:
            each_line = each_line.strip()
            data.append(each_line)
        return(data)

    def get_parsed_data(self, file_location):
        self.ls.get_log_parse(file_location)
        data = self.get_data(file_location)
        rows = []
        for num, line in enumerate(data, 0):
            if line.startswith(self.LOOKUP):
                rows.append(num)
        row_length = len(rows)

        def recordtype_extractor(x):
            t = []
            for i in range(0, len(x)):
                t.append(x[i][:3])
            return(list(set(t)))
        transaction_data = []
        for i in range(0, row_length):
            if i < (row_length-1):
                each_transaction = data[rows[i]:rows[i+1]]
                transaction_data.append(each_transaction)
            else:
                each_transaction = data[rows[i]:len(data)]
                transaction_data.append(each_transaction)
        recordtypes_list = map(recordtype_extractor, transaction_data)
        recordtypes_all = list(recordtypes_list)

        return(row_length, transaction_data, recordtypes_all)

    def get_concatenated_parsed_data(self, data_ls):
        if len(data_ls) > 1:
            for i in range(1, len(data_ls)):
                data_ls[0] = list(data_ls[0])
                data_ls[0][0] = data_ls[0][0]+data_ls[i][0]
                data_ls[0] = tuple(data_ls[0])
                data_ls[0][1].extend(data_ls[i][1])
                data_ls[0][2].extend(data_ls[i][2])
            return(data_ls[0])
        else:
            return(data_ls[0])

####################################################
# COLLECTING ALL ZIPPED FILES


class FileCollection():
    unziplocation = '/mnt/ipi-integration/IPI Unzip Files/'

    def collect_zip_files(self):
        files = []
        fileread = dbutils.fs.ls(self.unziplocation)
        for k in range(0, len(fileread)):
            file_location = '/dbfs'+self.unziplocation + \
                fileread[k].name+fileread[k].name[0:-5]
            files.append(file_location)
        return(files)

##########################################

# GENERATING TABLE GROUPS


class GenerateTableGroups():
    def __init__(self):
        self.parsed_file = Parsing()
        self.unzipped_file_location_list = FileCollection().collect_zip_files()
        self.ls = LogService()

    def get_all_records(self, record_body):
        return(record_body.get_body()[0:len(record_body.get_body())])

    def get_group_body(self, transaction: list, key: int, record1: str, record2=None):
        if record2 == None:
            d = self.get_all_records(globals()[record1](transaction, key))
            return(d)
        else:
            d1 = self.get_all_records(globals()[record1](transaction, key))
            d2 = self.get_all_records(globals()[record2](transaction, key))
            d = {**d1[0], **d2[0]}
            return(d)

    def get_group(self, Transaction_list: list, record_types_list: list, index_key: int):
        table_record_types = {'InterestedParty': ['IPA', 'BDN'],
                              'IPNameUsage': ['NUN', 'INN', 'MUN', 'IMN'],
                              'Name': ['ONN', 'MCN', 'NCN'],
                              'NameReference': ['ONN', 'MCN', 'NCN'],
                              'Agreement': ['MAN'],
                              'Status': ['STN']
                              }
        grp_object = {'InterestedParty': [], 'IPNameUsage': [], 'Name': [
        ], 'NameReference': [], 'Agreement': [], 'Status': []}

        for key in table_record_types.keys():
            if key is 'InterestedParty':
                grp = all(
                    item in record_types_list for item in table_record_types.get(key))
                if grp:
                    grp_object[key].append(self.get_group_body(
                        Transaction_list, index_key, table_record_types.get(key)[0], table_record_types.get(key)[1]))
                else:
                    match = set(record_types_list) & set(
                        table_record_types.get(key))
                    grp_object[key].append(self.get_group_body(
                        Transaction_list, index_key, [*match][0]))
            else:
                for value in table_record_types.get(key):
                    if value in record_types_list:
                        grp_object[key].append(self.get_group_body(
                            Transaction_list, index_key, value))
                    else:
                        pass
        return(grp_object)

    def get_table_grp_dict(self):
        table_dict = {'InterestedParty': [],
                      'IPNameUsage': [],
                      'Name': [],
                      'NameReference': [],
                      'Agreement': [],
                      'Status': []
                      }
        self.ls.get_log_start()
        if __name__ == '__main__':
            pool = Pool(25)
            list_total_data = pool.map(
                self.parsed_file.get_parsed_data, self.unzipped_file_location_list)
            pool.close()
            pool.join()
        self.ls.get_log_end()
        data_tuple = self.parsed_file.get_concatenated_parsed_data(
            list_total_data)
        for record_number in range(0, data_tuple[0]):
            Transaction_list = data_tuple[1][record_number]
            record_types_list = data_tuple[2][record_number]
            grp = self.get_group(
                Transaction_list, record_types_list, record_number)
            for key in grp.keys():
                if key is 'InterestedParty':
                    if grp.get(key) != []:
                        for j in range(0, len(grp.get(key))):
                            table_dict[key].append(grp.get(key)[j])
                else:
                    if grp.get(key) != []:
                        for j in range(0, len(grp.get(key))):
                            table_dict[key].extend(grp.get(key)[j])
        return(table_dict)

##########################################

# GENERATING TABLES


class GenerateTable():
    def __init__(self):
        self.grp = GenerateTableGroups()

    def get_tbl(self):
        record_type_object = self.grp.get_table_grp_dict()
        for key in record_type_object.keys():
            if record_type_object[key] != []:
                rdd = spark.sparkContext.parallelize(record_type_object[key])
                spark_dframe = spark.createDataFrame(
                    rdd, TblSchema(key).get_tbl_schema())
                if key == 'Name':
                    spark_dframe_name = spark_dframe.dropDuplicates(
                        ['IPNameNumber'])
                    spark_dframe_name.write.mode("overwrite").format(
                        "parquet").save("/mnt/ipi-integration/"+key)
                else:
                    spark_dframe.write.mode("overwrite").format(
                        "parquet").save("/mnt/ipi-integration/"+key)

            else:
                pass


##########################################
# LOGGER SERVICE


class LogService():
    def __init__(self):
        self.logger = logging.getLogger(__name__)
        self.logger.setLevel(logging.INFO)
        self.formatter = logging.Formatter('%(message)s'+'%(asctime)s')
        self.stream_handler = logging.StreamHandler(sys.stdout)
        self.stream_handler.flush = sys.stdout.flush
        self.stream_handler.setFormatter(self.formatter)
        self.logger.addHandler(self.stream_handler)

    def get_log_start(self):
        self.logger.info('Start of parallel processing on')

    def get_log_end(self):
        self.logger.info('End of parallel processing on')

    def get_log_parse(self, file_name: str):
        self.logger.info('Started processing file '+file_name+' on ')

##########################################

# FILE PROCESSING TO PARQUET


class IPIFileProcessing():
    def __init__(self):
        self.ls = LogService()

    def to_parquet(self):
        GenerateTable().get_tbl()


############################################
# RUN
IPIFileProcessing().to_parquet()

##########################################
# DISPLAY PARQUET DATA
# new_data=sqlContext.read.format("parquet").load("/mnt/ipi-integration/InterestedParty")
# display(new_data)
