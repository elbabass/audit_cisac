from typing import List

from pandas import DataFrame


class Group:
    def __init__(self, transaction_type: str, group_id: str, df_arr: List[DataFrame]):
        self.transaction_type = transaction_type
        self.group_id = group_id
        self.df_arr = df_arr

    def get_url(self):
        return self.df_arr[0].iloc[0].api_request.get_url()
    
    def get_thirdParty_url(self):
        return self.df_arr[0].iloc[0].api_request.get_thirdParty_url(self.group_id)

    def get_publisherContextSearch_url(self):
        return self.df_arr[0].iloc[0].api_request.get_publisherContextSearch_url()    

    def get_http_verb(self):
        return self.df_arr[0].iloc[0].api_request.get_http_verb()
