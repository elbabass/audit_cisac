from reporting.services.data_factory_service import DataFactoryService


class IswcFullExtract:
    data_factory_service = DataFactoryService
    pipeline_name = 'IswcFullExtract_RefreshCache'

    def __init__(self,  dependancies: dict):
        self.data_factory_service = DataFactoryService(
            dependancies['subscription_id'], dependancies['aad_client_id'], dependancies['aad_secret'],
            dependancies['aad_tenant'], dependancies['resource_group_name'], dependancies['data_factory_name'],
            self.pipeline_name)

    def execute_report(self, parameters: dict):
        most_recent_version = parameters['MostRecentVersion'] == 'True'

        self.data_factory_service.new_pipeline_run(
            parameters['SubmittingAgencyCode'], most_recent_version, parameters['Email'])

        return (None, None, None)
