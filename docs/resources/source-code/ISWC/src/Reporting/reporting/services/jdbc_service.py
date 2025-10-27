from pyspark.sql import SparkSession


class JdbcService():
    def __init__(self, connection_string: str, spark: SparkSession):
        self.connection_string = connection_string
        self.spark = spark

    def get_data(self, db_query):
        (jdbcUrl, connectionProperties) = self.__get_jdbc_url(
            self.connection_string)

        df = self.spark.read.jdbc(url=jdbcUrl, table=db_query,
                                  properties=connectionProperties)
        return df

    def __get_jdbc_url(self, connection_string):
        user = self.__get_key_from_connection_string(
            connection_string, 'User ID')
        password = self.__get_key_from_connection_string(
            connection_string, 'Password')
        server = self.__get_key_from_connection_string(
            connection_string, 'Server')
        database = self.__get_key_from_connection_string(
            connection_string, 'Initial Catalog')

        connectionProperties = {
            "user": user,
            "password": password
        }
        jdbcUrl = "jdbc:sqlserver://{0}:{1};database={2}".format(
            server, 1433, database)

        return (jdbcUrl, connectionProperties)

    def __get_key_from_connection_string(self, connection_string, key: str):
        dict = {i.partition('=')[0]: i.partition('=')[2]
                for i in connection_string.split(';')}
        return(dict.get(key))
