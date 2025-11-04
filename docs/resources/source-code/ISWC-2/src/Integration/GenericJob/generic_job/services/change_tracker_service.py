from datetime import datetime
from pyspark.sql import SparkSession
from pyspark.sql.dataframe import DataFrame
from pyspark.sql.types import *
from pyspark.sql.functions import col, explode, to_json, from_json, when, lit, row_number

class ChangeTrackerService:
    
    def __init__(self, spark: SparkSession):
        self.spark = spark

    def _get_file_name(self):
        current_datetime = datetime.utcnow().strftime('%Y%m%d%H%M%S')
        return f"RecentIswcChanges{current_datetime}.csv"

    def create_extract(self, audit_data_df: DataFrame):
        if (audit_data_df.head() is None):
            return (self._get_file_name(), "No changes available.", 0)
        updated_iswcs_df = audit_data_df.select("PreferredIswc").distinct()
        return (self._get_file_name(), self.to_csv(updated_iswcs_df), updated_iswcs_df.count())

    def get_third_parties(self, submitting_party_id):
        if submitting_party_id == 'All':
            return self.spark.sql("select SubmittingPartyId from iswc.recentiswcchangessubscribers").rdd.collect()
        else:
            return self.spark.sql("select SubmittingPartyId from iswc.recentiswcchangessubscribers "
                                "where SubmittingPartyId = '{}'".format(submitting_party_id)).rdd.collect()

    def to_csv(self, df: DataFrame):
        pandas_df = df.toPandas()
        extract_data_csv = pandas_df.to_csv(index=False)
        return extract_data_csv 