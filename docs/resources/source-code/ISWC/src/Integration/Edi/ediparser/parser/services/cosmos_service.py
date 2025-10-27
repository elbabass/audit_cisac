import json

from azure.cosmos import cosmos_client


class CosmosService:

    def __init__(self, cosmos_connection_string: str, database_id: str, container_id):

        self.database_id = database_id
        self.container_id = container_id
        self.database_container_link = self.__get_database_container_link()

        cosmos_endpoint = self.__get_key_from_connection_string(
            cosmos_connection_string, 'AccountEndpoint')
        cosmos_key = self.__get_key_from_connection_string(
            cosmos_connection_string, 'AccountKey')

        self.client = cosmos_client.CosmosClient(
            cosmos_endpoint, {'masterKey': cosmos_key}, connection_policy=None)

        self.container = self.client.ReadContainer(
            self.database_container_link)

    def get_csn_records_unprocessed(self, agency_id: str):
        for item in self.client.QueryItems(
                self.database_container_link,
                self.__get_csn_records_agency_query(agency_id),
                {'enableCrossPartitionQuery': True}):
            yield json.loads(json.dumps(item, indent=True))

    def set_processed_items(self, document):
        self.client.UpsertItem(self.database_container_link, document, {
                               'partitionKey': document['PartitionKey']})

    def __get_database_container_link(self):
        return "dbs/{}/colls/{}".format(self.database_id, self.container_id)

    def __get_csn_records_agency_query(self, agency_id: str):
        return 'SELECT * FROM {} c WHERE c.ProcessedOnDate = null AND c.HttpResponse <> null AND c.ReceivingAgencyCode = "{}"'.format(self.container_id, agency_id)

    def __get_key_from_connection_string(self, connection_string: str, key: str):
        dict = {i.partition('=')[0]: i.partition('=')[2]
                for i in connection_string.split(';')}
        return dict.get(key)

    def get_records_with_null_HttpResponse(self):
        items = []
        for item in self.client.QueryItems(
                self.database_container_link,
                self.__get_records_without_HttpResponse_query(),
                {'enableCrossPartitionQuery': True}):
            items.append(json.loads(json.dumps(item, indent=True)))
        return items

    def __get_records_without_HttpResponse_query(self):
        return 'SELECT * FROM {} c WHERE c.HttpResponse = "null"'.format(self.container_id)

    def get_agent_runs_by_agency(self, agency_id, from_date, to_date):
        items = []
        for item in self.client.QueryItems(
                self.database_container_link,
                self.__get_agent_run_records_agency_query(
                    agency_id, from_date, to_date),
                {'enableCrossPartitionQuery': True}):
            items.append(json.loads(json.dumps(item, indent=True)))
        return items

    def get_agent_runs(self, from_date, to_date):
        items = []
        for item in self.client.QueryItems(
                self.database_container_link,
                self.__get_agent_run_records_query(from_date, to_date),
                {'enableCrossPartitionQuery': True}):
            items.append(json.loads(json.dumps(item, indent=True)))
        return items

    def __get_agent_run_records_agency_query(self, agency_id: str, from_date, to_date):
        return 'SELECT * FROM {} c WHERE c.AgencyCode = "{}" and c.RunStartDate >= "{}" and c.RunStartDate < "{}"  order by c.RunStartDate desc'.format(self.container_id, agency_id, from_date, to_date)

    def __get_agent_run_records_query(self, from_date, to_date):
        return 'SELECT * FROM {} c WHERE c.RunStartDate >= "{}" and c.RunStartDate < "{}"  order by c.RunStartDate desc'.format(self.container_id, from_date, to_date)
