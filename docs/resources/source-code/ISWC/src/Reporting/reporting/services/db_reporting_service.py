from pyspark.sql import SparkSession
from reporting.services.jdbc_service import JdbcService


class DbReportingService:
    container = '/mnt/submission-audit/'

    def __init__(self, connection_string: str, spark: SparkSession):
        self.jdbc_service = JdbcService(connection_string, spark)

    def get_data(self, query: str):
        return self.jdbc_service.get_data(query)
