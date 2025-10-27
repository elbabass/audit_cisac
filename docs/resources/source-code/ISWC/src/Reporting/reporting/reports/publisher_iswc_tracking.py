from io import StringIO

from dateutil.relativedelta import relativedelta
from pyspark.sql import SparkSession
from pyspark.sql.functions import col, date_format
from reporting.reports.report_output import ReportOutput
from reporting.services.parquet_reporting_service import \
    ParquetReportingService


class PublisherIswcTracking:
    spark = SparkSession
    reporting_service = ParquetReportingService
    report_output = ReportOutput

    def __init__(self, dependancies: dict):
        self.spark = dependancies['spark']
        self.reporting_service = ParquetReportingService(
            self.spark, dependancies['dbutils'])
        self.report_output = ReportOutput()

    def execute_report(self, parameters: dict):
        self.reporting_service.get_data(
            parameters).createOrReplaceTempView("allocation")

        parameters['ToDate'] = parameters['ToDate'] + relativedelta(years=10)
        parameters['AgencyCode'] = None

        self.reporting_service.get_data(
            parameters).createOrReplaceTempView("submissions")

        data = self.spark.sql(
            """select * from allocation a
               inner join submissions s on a.PreferredIswc = s.PreferredIswc
               where a.PublisherNameNumber <> 0 and (s.TransactionType = 'CAR' or s.TransactionType = 'CUR')
            """)

        data = data.select(
            date_format('a.CreatedDate', ReportOutput.iso8601_format).alias(
                'AllocationDate'),
            col('a.AgencyCode').alias('AllocatingAgency'),
            col('a.PublisherNameNumber').alias(
                'SubmittingPublisherIPNameNumber'),
            col('a.PublisherWorkNumber').alias(
                'SubmittingPublisherWorkNumber'),
            col('a.PreferredIswc'),
            col('s.AgencyCode').alias('RelatedSubmissionAgency'),
            col('s.TransactionType').alias('RelatedSubmissionType'),
            col('s.AgencyWorkCode').alias('RelatedSubmissionAgencyWorkNo'),
            col('s.RelatedSubmissionIncludedISWC').alias('RelatedSubmissionIncludedISWC'))

        file_name = self.report_output.get_output_file_name(parameters)
        return (parameters['SubmittingAgencyCode'], file_name, StringIO(self.to_csv(data)))

    def to_csv(self, df):
        extract_data_pd = df.toPandas()
        extract_data_pd['SubmittingPublisherIPNameNumber'] = extract_data_pd['SubmittingPublisherIPNameNumber'].fillna(
            0.0).astype(int)
        extract_data_csv = extract_data_pd.to_csv(index=False)
        return extract_data_csv
