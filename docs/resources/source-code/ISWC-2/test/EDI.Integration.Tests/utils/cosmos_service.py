from azure.cosmos import CosmosClient
from utils.utils import Settings


class CosmosService:

    def __init__(self):
        self.settings = Settings()
        cosmos_connection_string = self.settings.cosmos_string
        cosmos_endpoint = self.__get_key_from_connection_string(
            cosmos_connection_string, 'AccountEndpoint')
        cosmos_key = self.__get_key_from_connection_string(
            cosmos_connection_string, 'AccountKey')

        self.client: CosmosClient = CosmosClient(
            cosmos_endpoint, {'masterKey': cosmos_key}, connection_policy=None)

        self.database = self.client.get_database_client('ISWC')
        self.container = self.database.get_container_client('FileAudit')

    def get_ack_filename(self, filename: str):
        item = self.get_file_audit_record(filename)
        return item['AckFileName']

    def get_file_audit_record(self, filename: str):
        for item in self.container.query_items(
                query=f'SELECT * FROM FileAudit c WHERE c.FileName="{filename}" order by c.DatePickedUp desc',
                enable_cross_partition_query=True):
            return item

    def __get_key_from_connection_string(self, connection_string: str, key: str):
        dict = {i.partition('=')[0]: i.partition('=')[2]
                for i in connection_string.split(';')}
        return dict.get(key)
