import json
import datetime
from pyspark.sql import SparkSession
from azure.cosmos import cosmos_client
from pyspark.sql.types import *

class CosmosService:

    def __init__(self, spark: SparkSession, cosmos_connection_string: str, database_id: str, container_name: str, schema: StructType):

        self.database_id = database_id
        self.container_name = container_name
        self.database_container_link = self.__get_database_container_link()
        self.spark = spark

        self.cosmos_endpoint = self.__get_key_from_connection_string(
            cosmos_connection_string, 'AccountEndpoint')
        self.cosmos_key = self.__get_key_from_connection_string(
            cosmos_connection_string, 'AccountKey')

        self.client = cosmos_client.CosmosClient(
            self.cosmos_endpoint, {'masterKey': self.cosmos_key}, connection_policy=None)

        self.container = self.client.ReadContainer(
            self.database_container_link)
        
        self.schema = schema

    def get_audit_requests(self, hwm: datetime):
        for item in self.client.QueryItems(
                self.database_container_link,
                self.__get_audit_requests_query(hwm),
                {'enableCrossPartitionQuery': True}):
            yield json.loads(json.dumps(item, indent=True))

    def __get_database_container_link(self):
        return "dbs/{}/colls/{}".format(self.database_id, self.container_name)

    def __get_key_from_connection_string(self, connection_string: str, key: str):
        dict = {i.partition('=')[0]: i.partition('=')[2]
                for i in connection_string.split(';')}
        return dict.get(key)

    def __get_audit_requests_query(self, hwm: datetime):
        query = 'SELECT * FROM {} c WHERE c.CreatedDate > "{}" order by c.CreatedDate'.format(self.container_name, hwm.strftime('%Y-%m-%dT%H:%M:%S.%f'))
        return query
    
    def read_change_feed(self, table_history_timestamp: str):
        self.spark.conf.set("spark.databricks.delta.schema.autoMerge.enabled", "true")

        cosmos_change_feed_config = {
            "spark.cosmos.accountEndpoint": self.cosmos_endpoint,
            "spark.cosmos.accountKey": self.cosmos_key,
            "spark.cosmos.database": self.database_id,
            "spark.cosmos.container": self.container_name,
            "spark.cosmos.read.partitioning.strategy": "Default",
            "spark.cosmos.read.inferSchema.enabled" : "false",
            "spark.cosmos.changeFeed.startFrom" : table_history_timestamp,
            "spark.cosmos.changeFeed.mode" : "Incremental",
        }

        cosmos_changes_df = self.spark.read.schema(self.schema).format("cosmos.oltp.changeFeed").options(**cosmos_change_feed_config).load()

        return cosmos_changes_df
    
    def read_change_feed_stream(self, table_history_timestamp: str):
        self.spark.conf.set("spark.databricks.delta.schema.autoMerge.enabled", "true")

        cosmos_change_feed_config = {
            "spark.cosmos.accountEndpoint": self.cosmos_endpoint,
            "spark.cosmos.accountKey": self.cosmos_key,
            "spark.cosmos.database": self.database_id,
            "spark.cosmos.container": self.container_name,
            "spark.cosmos.read.partitioning.strategy": "Default",
            "spark.cosmos.read.inferSchema.enabled" : "false",
            "spark.cosmos.changeFeed.startFrom" : table_history_timestamp,
            "spark.cosmos.changeFeed.mode" : "Incremental",
        }

        cosmos_changes_df = self.spark.readStream.schema(self.schema).format("cosmos.oltp.changeFeed").options(**cosmos_change_feed_config).load()

        return cosmos_changes_df