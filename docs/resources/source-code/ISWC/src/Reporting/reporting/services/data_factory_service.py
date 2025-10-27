from azure.common.credentials import ServicePrincipalCredentials
from azure.mgmt.datafactory import DataFactoryManagementClient


class DataFactoryService:
    resource_group_name = ''
    data_factory_name = ''
    pipeline_name = ''

    def __init__(self, subscription_id: str, client_id: str, secret: str, tenant: str, resource_group_name: str, data_factory_name: str, pipeline_name: str):
        self.resource_group_name = resource_group_name
        self.data_factory_name = data_factory_name
        self.pipeline_name = pipeline_name

        credentials = ServicePrincipalCredentials(
            client_id=client_id, secret=secret, tenant=tenant)
        self.adf_client = DataFactoryManagementClient(
            credentials, subscription_id)

    def new_pipeline_run(self, agency_id: str, most_recent_version: bool, email: str, consider_original_titles_only: bool = None, from_date: str = None, to_date: str = None, submitting_agency: str = None):
        parameters = {
            'AgencyId': agency_id,
            'RefreshCache': str(not most_recent_version).lower(),
            'Email': email
        }

        if consider_original_titles_only is not None:
            parameters['ConsiderOriginalTitlesOnly'] = str(
                consider_original_titles_only)

        if from_date is not None:
            parameters['FromDate'] = from_date

        if to_date is not None:
            parameters['ToDate'] = to_date

        if submitting_agency is not None:
            parameters['SubmittingAgency'] = submitting_agency

        self.adf_client.pipelines.create_run(
            self.resource_group_name, self.data_factory_name, self.pipeline_name, parameters=parameters)
