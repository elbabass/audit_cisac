from datetime import datetime, time
from typing import List
from pyspark.sql import SparkSession
from pyspark.sql.dataframe import DataFrame
from pyspark.sql.types import *
from pyspark.sql.functions import col, explode, to_json, when, lit
from pyspark.sql.streaming import StreamingQuery

class DeltaService:
    def __init__(self, spark: SparkSession) -> None:
        self.__spark = spark
        self.__spark.conf.set("spark.databricks.delta.schema.autoMerge.enabled", "true")
    
    def write_delta_table(self, df: DataFrame, database_name: str, table_name: str, write_mode: str, merge=True, schema: StructType = None) -> None:

        merge_option = "true" if merge else "false"

        if schema:
            df = self.__spark.createDataFrame(df.rdd, schema=schema)

        (df.write.format("delta")
         .mode(write_mode)
         .option("mergeSchema", merge_option)
         .option("overwriteSchema", merge_option)
         .saveAsTable(f'{database_name}.{table_name}'))
        
    def merge_delta(self, incremental: DataFrame, database_name: str, table_name: str, id: str = 'id'):
        if (self.__delta_table_exists(database_name, table_name)):
            incremental.createOrReplaceTempView("incremental")
            self.get_df_by_sql_query(f"""
                MERGE INTO {database_name}.{table_name} t
                USING incremental i
                ON i.{id} = t.{id}
                WHEN MATCHED THEN UPDATE SET *
                WHEN NOT MATCHED THEN INSERT *
            """)
        else:
            self.write_delta_table(incremental, database_name, table_name, 'append', merge=True)

    def get_df_by_sql_query(self, sql_query: str) -> DataFrame:
        return self.__spark.sql(sql_query)

    def get_table_as_df(self, table_name: str) -> DataFrame:
        return self.__spark.sql(f"""
            SELECT * FROM {table_name}
        """)

    def __delta_table_exists(self, database_name: str, table_name: str) -> bool:
        try:
            table_exists = False
            all_tables = self.__spark.catalog.listTables(dbName=database_name)

            for table in all_tables:
                if table[0] == table_name:
                    table_exists = True

            return table_exists
        except Exception as e:
            if 'java.io.FileNotFoundException' in str(e):
                return False
            else:
                raise

    def write_delta_stream(self, df_change_feed: DataFrame, checkpoint_location: str, table_name: str, class_name) -> StreamingQuery:
        return df_change_feed \
            .writeStream \
            .trigger(once=True) \
            .format('delta') \
            .outputMode("update") \
            .option("checkpointLocation", f"{checkpoint_location}") \
            .option("mergeSchema", "true") \
            .foreachBatch(lambda i, b: self._merge_delta(i, table_name, class_name)) \
            .start()

    def wait_for_streams_to_complete(self) -> None:
        for s in self.__spark.streams.active:
            while s.isActive:
                print('waiting for trigger once to finish')
                time.sleep(1)

    def _merge_delta(self, incremental: DataFrame, target: str, class_name: str) -> None: 
        mount_point_error_csv = f'/mnt/generic_job_error_{class_name}_{datetime.now().strftime("%Y%m%d%H%M%S")}'
        incremental.createOrReplaceTempView("incremental")
        try:
            incremental._jdf.sparkSession().sql(f"""
                MERGE INTO {target} t
                USING incremental i
                ON i.id=t.id 
                WHEN MATCHED THEN UPDATE SET *
                WHEN NOT MATCHED THEN INSERT *
            """)
        except:
            print(f'[ERROR] Calling merge_delta. Df will be saved as csv in {mount_point_error_csv}')
            (incremental._jdf.sparkSession()
                .sql('select * from incremental')
                .write
                .option('header', True)
                .csv(mount_point_error_csv)
            )

            raise