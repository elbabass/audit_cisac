from datetime import timezone, datetime
from pyspark.sql import SparkSession
from pyspark.sql.types import *
from generic_job.services.delta_service import DeltaService

class HighWatermarkService:

    def __init__(self, spark: SparkSession):
        self.delta_service = DeltaService(spark)
        self.spark = spark
   
    def get_high_watermark(self, watermark_name: str):
        hwm = (self.delta_service.get_df_by_sql_query(
            f"select value from iswc.highwatermark where name = '{watermark_name}'").collect()[0][0])
        
        if hwm is None:
            raise Exception(f'{watermark_name} missing from configuration table.')
        
        return hwm
    
    def set_high_watermark(self, watermark_name: str):
        new_hwm = datetime.now(timezone.utc)
        self.spark.sql(f"update iswc.highwatermark set value = '{new_hwm}' where name = '{watermark_name}'")
        
        return new_hwm
