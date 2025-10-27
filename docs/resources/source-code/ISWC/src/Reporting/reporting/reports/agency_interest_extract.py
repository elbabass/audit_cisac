from io import StringIO

from pyspark.sql import SparkSession
from pyspark.sql.dataframe import DataFrame
from pyspark.sql.functions import (col, concat, date_format, expr, lit,
                                   posexplode, split)
from pyspark.sql.types import IntegerType
from reporting.reports.report_output import ReportOutput
from reporting.services.db_reporting_service import DbReportingService
from reporting.services.parquet_reporting_service import \
    ParquetReportingService


class AgencyInterestExtract:
    spark = SparkSession
    reporting_service = ParquetReportingService
    report_output = ReportOutput
    db_reporting_service = DbReportingService

    def __init__(self, dependancies: dict):
        self.spark = dependancies['spark']
        self.reporting_service = ParquetReportingService(
            self.spark, dependancies['dbutils'])
        self.report_output = ReportOutput()
        self.db_reporting_service = DbReportingService(
            dependancies['connection_string'], self.spark)

    def execute_report(self, parameters: dict):
        sql_date_format = '%Y-%m-%d'

        agreement_from_date = parameters['AgreementFromDate'].strftime(
            sql_date_format)
        agreement_to_date = parameters['AgreementToDate'].strftime(
            sql_date_format)

        query = self.query_current_ips.format(
            parameters['AgencyCode'], agreement_from_date, agreement_to_date, agreement_from_date)

        self.db_reporting_service.get_data(
            query).createOrReplaceTempView("IpNameNumbers")

        parameters['AgencyCode'] = None
        data = self.reporting_service.get_data(parameters)

        (data.select(
            col("*"),
            split("CreatorNameNumbers", ";").alias("NameNumbers"),
            posexplode(split("CreatorNameNumbers", ";")).alias("pos", "val"))
         .drop("val")
         .select(
            col("*"),
            concat(lit("Creator"), col("pos").cast("string")).alias("Creator"),
            expr("NameNumbers[pos]").cast("int").alias("IpNameNumber")
        )).createOrReplaceTempView("DenormalizedCreators")

        data = self.spark.sql(
            "select c.* from DenormalizedCreators c inner join IpNameNumbers ip on c.IpNameNumber = ip.IpNameNumber")

        data = (data.select(
            date_format('CreatedDate',
                        ReportOutput.iso8601_format).alias('Date'),
            col('TransactionType').alias('Type'),
            col('AgencyCode').alias('SubmittingAgency'),
            col('AgencyWorkCode').alias('SubmittingAgencyWorkNo'),
            col('PublisherNameNumber').alias(
                'SubmittingPublisherIPNameNumber').cast(IntegerType()),
            col('PublisherWorkNumber').alias('SubmittingPublisherWorkNumber'),
            col('PreferredISWC').alias('PreferredISWC'),
            col('OriginalTitle'),
            col('CreatorNameNumbers')))

        file_name = self.report_output.get_output_file_name(parameters)
        return (parameters['SubmittingAgencyCode'], file_name, StringIO(self.to_csv(data)))

    def to_csv(self, df: DataFrame):
        pandas_df = df.toPandas()
        pandas_df['SubmittingPublisherIPNameNumber'] = pandas_df['SubmittingPublisherIPNameNumber'].fillna(
            0.0).astype(int)
        extract_data_csv = pandas_df.to_csv(index=False)
        return extract_data_csv

    query_current_ips = '''(
        select distinct ref.IPNameNumber from IPI.Agreement a
        join IPI.NameReference ref on a.IPBaseNumber = ref.IPBaseNumber
        where CreationClass = 'MW' and 
        EconomicRights in ('MP', 'OB', 'OD', 'PC', 'PR', 'PT', 'RB', 'RT', 'TB', 'TO', 'TP', 'TV', 'MA', 'MB', 'MD', 'MR', 'MT', 'MV', 'SY', 'DB', 'RL', 'BT', 'RP', 'ER', 'RG', 'RR') 
        and AgencyID = '{}' and '{}' > FromDate and '{}' between '{}' and ToDate
    ) alias'''
