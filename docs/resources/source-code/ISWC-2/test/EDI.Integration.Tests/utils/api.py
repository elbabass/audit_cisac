import json
import requests
from utils.utils import Settings


class IswcApi():
    def __init__(self):
        headers = {
            "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8"}
        self.settings = Settings()
        req = {
            "grant_type": "client_credentials",
            "client_id": "iswcapimanagement",
            "client_secret": self.settings.iswc_api_secret,
            "scope": "iswcapi",
            "AgentID": "000"
        }
        res = requests.post(f'{self.settings.iswc_api}/connect/token', headers=headers,
                            auth=(req["client_id"], self.settings.iswc_api_secret), data=req)
        token = json.loads(res.content)["access_token"]
        self.headers = {
            "authorization": f"Bearer {token}", 'Content-type': 'application/json'}

    def add_submission(self, submission):
        res = requests.post(
            f'{self.settings.iswc_api}/submission', data=json.dumps(submission.submission), headers=self.headers)
        res.raise_for_status()
        return res

    def delete_submission(self, submission):
        res = requests.delete(
            f'{self.settings.iswc_api}/submission',
            params={
                'preferredIswc': submission['preferredIswc'],
                'workcode': submission['workcode'],
                'agency': submission['agency'],
                'sourceDb': submission['sourcedb'],
                'reasonCode': 'Deleted'
            }, headers=self.headers)
        res.raise_for_status()
        return res

