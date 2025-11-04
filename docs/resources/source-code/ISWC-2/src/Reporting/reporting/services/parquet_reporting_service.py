from pyspark.sql import SparkSession

from pyspark.sql.functions import col
from datetime import datetime, timedelta
from pyspark.sql.dataframe import DataFrame
from pyspark.sql import functions as functions
from reporting.services.audit_reader_service import AuditReaderService


class ParquetReportingService:
    spark = SparkSession
    dbutils = None
    container = '/mnt/submission-audit/'

    def __init__(self, spark: SparkSession, dbutils):
        self.spark = spark
        self.dbutils = dbutils

    def get_data(self, parameters: dict):
        agency_code = parameters['AgencyCode']
        agency_work_code = parameters['AgencyWorkCode']
        status = parameters['Status']
        from_date = parameters['FromDate']
        to_date = parameters['ToDate']
        transaction_source = parameters['TransactionSource']
        report_type = parameters['ReportType']

        if report_type == 'PublisherIswcTracking' or report_type == 'AgencyInterestExtract':
            status = 'Succeeded'
        
        if not to_date:
            to_date = from_date

        audit_reader = AuditReaderService(self.spark)
        data = audit_reader.get(from_date, to_date)
        
        if agency_code:
            data = data.filter(col('AgencyCode') == agency_code)

        if agency_work_code:
            data = data.filter(col('AgencyWorkCode') == agency_work_code)

        data = self.change_status_column_value(data)        

        if status and status != 'All':
            data = data.filter(col('IsProcessingError') == status)

        if transaction_source and transaction_source != 'All':
            data = data.filter(col('TransactionSource') == transaction_source)

        if report_type  == 'PublisherIswcTracking':
            data = self.change_related_submission_included_Iswc_column_value(data)

        return data

    def change_status_column_value(self, df: DataFrame):
        df = df.withColumn("IsProcessingError", functions.when(functions.col("IsProcessingError") == False, "Succeeded")
                           .when(functions.col("IsProcessingError") == True, "Error")
                           .otherwise(functions.col("IsProcessingError").cast('string')))
        return df

    def change_related_submission_included_Iswc_column_value(self, df: DataFrame):
        df = df.withColumn("RelatedSubmissionIncludedISWC", functions.when(functions.col("RelatedSubmissionIncludedISWC") == False, "No")
                           .when(functions.col("RelatedSubmissionIncludedISWC") == True, "Yes")
                           .otherwise(functions.col("RelatedSubmissionIncludedISWC").cast('string')))
        return df
