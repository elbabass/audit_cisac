import json

import requests

from ediparser.parser.services.logger_service import LoggerService


class JobService:

    OVERRIDING_PARAMS = 'overriding_parameters'
    NOTEBOOK_PARAMS = 'notebook_params'
    FILE_NAME = 'file_name'
    
    current_runs = []

    def __init__(self, job_id: int, bearer_token: str, logger: LoggerService):
        self.job_id = job_id
        self.bearer_token = bearer_token
        self.logger = logger        
        
        self.headers = {
            'Authorization': 'Bearer ' + self.bearer_token
        }

        job_runs = json.loads(requests.get(
            'https://westeurope.azuredatabricks.net/api/2.0/jobs/runs/list?active_only=true&job_id={}'.format(self.job_id), headers=self.headers).text)
        
        if 'runs' in job_runs:
            for job_run in job_runs['runs']:
                self.current_runs.append(job_run)
            while job_runs['has_more'] and 'next_page_token' in job_runs:
                job_runs = json.loads(requests.get(
                    'https://westeurope.azuredatabricks.net/api/2.0/jobs/runs/list?active_only=true&job_id={}&page_token={}'.format(self.job_id, job_runs['next_page_token']), headers=self.headers).text)
                for job_run in job_runs['runs']:
                    self.current_runs.append(job_run)

    def create_job(self, file_name: str, file_type: str):
        for job_run in self.current_runs:
            if self.OVERRIDING_PARAMS in job_run:
                if self.NOTEBOOK_PARAMS not in job_run[self.OVERRIDING_PARAMS] or self.FILE_NAME not in job_run[self.OVERRIDING_PARAMS][self.NOTEBOOK_PARAMS]:
                    self.logger.log_warning(
                        'Missing parameters for job run {}'.format(job_run['run_id']))
                    return None

                if job_run[self.OVERRIDING_PARAMS][self.NOTEBOOK_PARAMS][self.FILE_NAME] == file_name:
                    self.logger.log_warning(
                        "File is already being processed: " + file_name)
                    return None

        response = requests.post('https://westeurope.azuredatabricks.net/api/2.0/jobs/run-now', headers=self.headers, json={
            "job_id": self.job_id,
            "notebook_params": {
                "file_type": file_type,
                "file_name": file_name
            }
        })
        return response