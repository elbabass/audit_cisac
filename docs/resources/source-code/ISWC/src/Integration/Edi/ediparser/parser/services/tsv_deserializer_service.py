import pandas as pd
from ediparser.parser.models.edi_file import EdiFile
from ediparser.parser.models.group import Group
from ediparser.parser.models.input.tsv.car import CAR
from ediparser.parser.models.input.tsv.fsq import FSQ


class DeserializerTsvService:
    def __init__(self, file_name: str,  file_contents: list):
        self.file_name = file_name
        self.file_contents = file_contents

    def deserialize_file(self):
        data = self.file_contents
        group = []
        groups = []
        workcodes = []

        item = data[0][0]
        model = self.get_submission_model(item)
        transaction_type = self.get_transaction_type(item)

        for transaction_line in data:
            transaction_model = model(transaction_line, {})
            submitted_workcode = transaction_line[8]
            submission = transaction_model.get_submission()

            if transaction_type == 'CAR':
                submission['body']['submission']['publisherWorkcode'] = submitted_workcode
                workcode = self.__check_and_replace_duplicate_workcode(submission['body']['submission']['workcode'], submitted_workcode, submission['body']['submission'], workcodes, transaction_model)
                workcodes.append(workcode)
                submission['body']['submission']['workcode'] = workcode


            group.append(submission)
        
        parsed_group = self.parse_group(
            transaction_type, item, group)
        groups.append(parsed_group)

        for group in groups:
            for df in group.df_arr:
                for idx, transaction in df.iterrows():
                    df.iloc[idx]['api_request'] = globals()[group.transaction_type](
                        transaction, {})

        return EdiFile(self.file_name, {}, groups)

    def parse_group(self, transaction_type: str, group_id: str, groups: list):
        arr_limit = 10

        # 1. divide group_dict into sections
        group_sections_arr = self.split_group_into_sections(
            list(groups), arr_limit)

        cols = {'submission'}

        # 2. for each section create df with values and add to df_arr
        df_arr = []
        for group_section in group_sections_arr:
            items = []
            for gs in group_section:
                items.append({'submission': gs})

            df = pd.DataFrame(items, columns=cols)
            df['api_request'] = None

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
            'findSubmissions': FSQ
        }.get(group)

    def get_transaction_type(self, group):
        return {
            'addSubmissions': 'CAR',
            'findSubmissions': 'FSQ'
        }.get(group)

    def __check_and_replace_duplicate_workcode(self, workcode, submitted_workcode, submission, workcodes, transaction_model):
        if workcode in workcodes:
            workcode = transaction_model.generate_workcode(submitted_workcode, submission)
            while workcode in workcodes:
                workcode = transaction_model.generate_workcode(submitted_workcode, submission)
        return workcode
