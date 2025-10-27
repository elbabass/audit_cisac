from io import StringIO
from pyspark.sql.dataframe import DataFrame
from pyspark.sql.functions import col, date_format
from reporting.reports.report_output import ReportOutput
from reporting.services.parquet_reporting_service import ParquetReportingService
from pyspark.sql import SparkSession


class SubmissionAudit:
    spark = SparkSession
    reporting_service = ParquetReportingService
    report_output = ReportOutput

    def __init__(self, dependancies: dict):
        self.spark = dependancies['spark']
        self.reporting_service = ParquetReportingService(
            self.spark, dependancies['dbutils'])
        self.report_output = ReportOutput()

    def execute_report(self, parameters: dict):
        data = self.reporting_service.get_data(parameters)
        extract_data = data.select(date_format('CreatedDate', ReportOutput.iso8601_format).alias('Date'),
                                   col('TransactionType').alias('Type'), col('AgencyCode').alias(
                                       'SubmittingAgency'), col('AgencyWorkCode').alias('SubmittingAgencyWorkNo'),
                                   col('PublisherNameNumber').alias('SubmittingPublisherIPNameNumber'), col(
                                       'PublisherWorkNumber').alias('SubmittingPublisherWorkNumber'),
                                   col('PreferredIswc'),
                                   col('CreatorNames'), col('CreatorNameNumbers'), col(
                                       'IsProcessingError').alias('Status'),
                                   col('Code').alias('ErrorCode'),
                                   col('Message').alias('ErrorDescription'))

        file_name = self.report_output.get_output_file_name(parameters)
        return (parameters['SubmittingAgencyCode'], file_name, StringIO(self.to_csv(extract_data)))

    def to_csv(self, df: DataFrame):
        extract_data_pd = df.toPandas()
        extract_data_pd['SubmittingPublisherIPNameNumber'] = extract_data_pd['SubmittingPublisherIPNameNumber'].fillna(
            0.0).astype(int)
        extract_data_csv = extract_data_pd.to_csv(index=False)
        return extract_data_csv
