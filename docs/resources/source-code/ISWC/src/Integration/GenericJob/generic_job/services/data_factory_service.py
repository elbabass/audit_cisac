import time
from azure.common.credentials import ServicePrincipalCredentials
from azure.mgmt.datafactory import DataFactoryManagementClient


class DataFactoryService:
    resource_group_name = ''
    data_factory_name = ''

    def __init__(self, subscription_id: str, client_id: str, secret: str, tenant: str, resource_group_name: str, data_factory_name: str):
        self.resource_group_name = resource_group_name
        self.data_factory_name = data_factory_name

        credentials = ServicePrincipalCredentials(
            client_id=client_id, secret=secret, tenant=tenant)
        self.adf_client = DataFactoryManagementClient(
            credentials, subscription_id)

    def new_pipeline_run(self, pipeline_name: str, parameters: dict):
        run_response = self.adf_client.pipelines.create_run(
            self.resource_group_name, self.data_factory_name, pipeline_name, parameters=parameters)
        return run_response.run_id

    def wait_for_pipeline_run(self, run_id: str, poll_interval: int = 15, timeout: int = 3600):
        elapsed = 0
        while elapsed < timeout:
            pipeline_run = self.adf_client.pipeline_runs.get(
                self.resource_group_name, self.data_factory_name, run_id)
            status = pipeline_run.status
            if status in ['Succeeded', 'Failed', 'Cancelled']:
                return status
            time.sleep(poll_interval)
            elapsed += poll_interval
        raise TimeoutError(f"Pipeline run {run_id} did not finish within {timeout} seconds.")
