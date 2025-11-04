import pandas as pd
import json

from ediparser.parser.models.input.json.car import CAR
from ediparser.parser.models.input.json.cur import CUR
from ediparser.parser.models.input.json.cmq import CMQ
from ediparser.parser.models.input.json.ciq import CIQ
from ediparser.parser.models.input.json.wft import WFT
from ediparser.parser.models.input.json.mer import MER
from ediparser.parser.models.input.json.cdr import CDR
from ediparser.parser.models.input.json.fsq import FSQ
from ediparser.parser.models.input.json.ciq_ext import CIQEXT

from ediparser.parser.models.group import Group
from ediparser.parser.models.edi_file import EdiFile


class DeserializerJsonService:
    def __init__(self, file_name: str, file_contents: list):
        self.file_name = file_name
        self.file_contents = json.loads(file_contents)

    def deserialize_file(self):
        data = self.file_contents
        header = data.get('fileHeader')
        groups = []
        workcodes=[]

        for item in data:
            model = self.get_submission_model(item)
            transaction_type = self.get_transaction_type(item)

            if model is not None:
                group = []
                for transaction in data[item]:
                    transaction_model = model(transaction, header)
                    if (transaction_type == 'CAR' or transaction_type == 'FSQ') and header.get('submittingPublisher') is not None:
                        submitted_workcode = transaction['workcode']
                
                    if item.__eq__('searchByTitleAndContributorsSubmissions'):
                        group_item = transaction_model.get_title_submission()
                    elif item.__eq__('publisherContextSearch'):
                        group_item = transaction_model.get_context_submission()
                    else:
                        group_item = transaction_model.get_submission()

                    if transaction_type == 'CMQ':
                        group_item = {**group_item, **
                                      transaction_model.get_csn_data()}
                    if transaction_type == 'CAR' and self.file_name.__contains__('FromLocalRange'):
                        group_item['body']['submission'] = {
                            **group_item['body']['submission'], **transaction_model.get_from_local_range_data()}

                    if transaction_type == 'CAR' and group_item['body']['submission'].get('additionalIdentifiers') is not None and header.get('submittingPublisher') is not None:
                        group_item['body']['submission']['publisherWorkcode'] = submitted_workcode
                        workcode = self.__check_and_replace_duplicate_workcode(group_item['body']['submission']['workcode'], submitted_workcode, group_item['body']['submission'], workcodes, transaction_model)
                        workcodes.append(workcode)
                        group_item['body']['submission']['workcode'] = workcode
                    

                    group.append(group_item)

                parsed_group = self.parse_group(
                    transaction_type, item, group)
                groups.append(parsed_group)

        for group in groups:
            for df in group.df_arr:
                for idx, transaction in df.iterrows():
                    df.iloc[idx]['api_request'] = globals()[group.transaction_type](
                        transaction, header)

        return EdiFile(self.file_name, header, groups)

    def parse_group(self, transaction_type: str, group_id: str, groups: list):
        arr_limit = 1 if transaction_type in ['CDR', 'MER', 'WFT'] else 10
        if '/Allocation/' in self.file_name or '/Resolution/' in self.file_name:
            arr_limit = 1

        # 1. divide group_dict into sections
        group_sections_arr = self.split_group_into_sections(
            list(groups), arr_limit)

        cols = {'submission'}

        # 2. for each section create df with values and add to df_arr
        df_arr = []
        for group_section in group_sections_arr:
            df = pd.DataFrame(columns=cols)
            df['api_request'] = None

            for gs in group_section:
                df = df.append({'submission': gs}, ignore_index=True)

            df_arr.append(df)

        return Group(transaction_type, group_id, df_arr)

    def split_group_into_sections(self, group, size):
        sections = []
        while len(group) > size:
            section = group[:size]
            sections.append(section)
            group = group[size:]
        sections.append(group)
        return sections

    def get_submission_model(self, group):
        return {
            'addSubmissions': CAR,
            'updateSubmissions': CUR,
            'searchByIswcSubmissions': CMQ,
            'searchByAgencyWorkCodeSubmissions': CIQ,
            'searchByTitleAndContributorsSubmissions': CIQ,
            'updateWorkflowTasks': WFT,
            'mergeSubmissions': MER,
            'deleteSubmissions': CDR,
            'findSubmissions': FSQ,
            'publisherContextSearch': CIQEXT,

        }.get(group)

    def get_transaction_type(self, group):
        return {
            'addSubmissions': 'CAR',
            'updateSubmissions': 'CUR',
            'searchByIswcSubmissions': 'CMQ',
            'searchByAgencyWorkCodeSubmissions': 'CIQ',
            'searchByTitleAndContributorsSubmissions': 'CIQ',
            'updateWorkflowTasks': 'WFT',
            'mergeSubmissions': 'MER',
            'deleteSubmissions': 'CDR',
            'findSubmissions': 'FSQ',
            'publisherContextSearch': 'CIQEXT',
        }.get(group)

    def __check_and_replace_duplicate_workcode(self, workcode, submitted_workcode, submission, workcodes, transaction_model):
        if workcode in workcodes:
            workcode = transaction_model.generate_workcode(submitted_workcode, submission)
            while workcode in workcodes:
                workcode = transaction_model.generate_workcode(submitted_workcode, submission)
        return workcode
