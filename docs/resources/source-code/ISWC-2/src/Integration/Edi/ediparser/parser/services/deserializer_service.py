import pandas as pd

from ediparser.parser.models.edi_file import EdiFile
from ediparser.parser.models.group import Group
from ediparser.parser.models.input.car import CAR
from ediparser.parser.models.input.ciq import CIQ
from ediparser.parser.models.input.cmq import CMQ
from ediparser.parser.models.input.cur import CUR
from ediparser.parser.models.input.hdr import HDR
from ediparser.parser.models.input.mer import MER
from ediparser.parser.models.input.wft import WFT
from ediparser.parser.models.input.cdr import CDR
from ediparser.parser.models.input.upw import UPW


class DeserializerService:
    def __init__(self, file_name: str,  file_contents: list):
        self.file_name = file_name
        self.file_contents = file_contents

    def deserialize_file(self) -> EdiFile:
        groups = []
        current_group = []
        for line in self.file_contents:
            transaction = line[0:3]
            if('HDR' == transaction):
                hdr = HDR({'HDR': line}).get_body()
            elif('GRH' == transaction):
                group_type = line[3:6]
                group_id = line[6:11]
            elif('GRT' == transaction):
                parsed_group = self.__parse_group(
                    group_type, group_id, current_group)
                groups.append(parsed_group)
                current_group = []
            else:
                current_group.append(line)

        for group in groups:
            for df in group.df_arr:
                for idx, transaction in df.iterrows():
                    df.loc[idx, 'api_request'] = globals()[group.transaction_type](
                        transaction, hdr)

        return EdiFile(self.file_name, hdr, groups)

    def __parse_group(self, transaction_type: str, group_id: str, rows: list):
        group_dict = {}
        for idx, line in enumerate(rows, 0):
            line = line.strip()
            transaction = line[0:3]
            if transaction == transaction_type:
                lookup_idx = idx
                group_dict[lookup_idx] = {
                    transaction_type: line
                }
            else:
                group_dict[lookup_idx].setdefault(
                    transaction, []).append(line)

        cols = list(set([item for sublist in group_dict.values()
                         for item in sublist]))

        section_size = 1 if transaction_type in ['CDR', 'MER', 'WFT'] else 10
        # 1. divide group_dict into sections
        group_sections_arr = self.split_group_into_sections(
            list(group_dict.items()), section_size)

        # 2. for each section create df with values and add to df_arr
        df_arr = []
        for group_section in group_sections_arr:
            group_section_dict = dict(group_section)
            df = pd.DataFrame(list(group_section_dict.values()), columns=cols)
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
