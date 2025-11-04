import json
import requests
import time
from utils.utils import Settings


class DatabricksClient():
    def __init__(self):
        self.settings = Settings()
        self.headers = {
            'Authorization': f'Bearer {self.settings.job_api_token}'
        }
        self.process_file_job_id = self.settings.job_id
        self.edi_job_id = '2' if 'dev' in self.settings.iswc_api else '1'

    def start_ack_json_job(self, file_path):
        return self.__start_process_file_job(file_path, False)

    def start_ack_tsv_job(self, file_path):
        return self.__start_process_file_job(file_path, True)

    def get_run_details(self, run_id):
        return json.loads(requests.get(
            f'https://westeurope.azuredatabricks.net/api/2.0/jobs/runs/get?run_id={run_id}', headers=self.headers).content)

    def wait_for_run(self, run_id):
        end_time = time.time() + 60 * 12
        while time.time() < end_time:
            run_details = self.get_run_details(run_id)
            if run_details['state']['life_cycle_state'] == 'TERMINATED':
                break
            time.sleep(20)
        return run_details

    def start_edi_job(self):
        return json.loads(requests.post('https://westeurope.azuredatabricks.net/api/2.0/jobs/run-now', headers=self.headers, json={
            "job_id": self.edi_job_id
        }).content)['run_id']

    def __start_process_file_job(self, file_path, tsv):
        return json.loads(requests.post('https://westeurope.azuredatabricks.net/api/2.0/jobs/run-now', headers=self.headers, json={
            "job_id": self.process_file_job_id,
            "notebook_params": {
                "file_type": 'ACK_JSON' if not tsv else 'ACK_TSV',
                "file_name": file_path
            }
        }).content)['run_id']
