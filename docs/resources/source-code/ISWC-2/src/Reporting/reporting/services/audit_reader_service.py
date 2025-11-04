from pyspark.sql import SparkSession
from pyspark.sql.functions import col, expr
import datetime

class AuditReaderService:
    def __init__(self, spark: SparkSession):
        self.spark = spark
        self.audit_request_df = self.spark.table("iswc.auditrequest")

    def _parse_date(self, date):
        if isinstance(date, str):
            return datetime.datetime.strptime(date, "%d/%m/%Y")
        elif isinstance(date, datetime.datetime):
            return date
        else:
            raise ValueError(f"Invalid date format: {type(date)}. Expected str or datetime.")

    def _filter_by_date_range(self, start_date=None, end_date=None):
        now = datetime.datetime.utcnow()
        start_datetime = self._parse_date(start_date) if start_date else now - datetime.timedelta(days=7)
        end_datetime = self._parse_date(end_date) if end_date else now

        start_of_day = start_datetime.replace(hour=0, minute=0, second=0, microsecond=0)
        end_of_day = end_datetime.replace(hour=23, minute=59, second=59, microsecond=999999)

        return self.audit_request_df.filter(
            (col("CreatedDate") >= start_of_day) & (col("CreatedDate") <= end_of_day)
        )

    def _apply_transformations(self, filtered_df):
        return filtered_df.withColumn(
            "OriginalTitle",
            expr("""
                CASE
                    WHEN size(filter(Work_Titles, x -> x.Type = 'OT')) > 0 
                    THEN element_at(filter(Work_Titles, x -> x.Type = 'OT'), 1).Name
                    ELSE null
                END
            """)
        ).withColumn(
            "CreatorNames",
            expr("""
                CASE
                    WHEN size(filter(Work_InterestedParties, x -> x.CisacType IN (3, 5, 6))) > 0 
                    THEN concat_ws(';', 
                        transform(
                            filter(Work_InterestedParties, x -> x.CisacType IN (3, 5, 6)),
                            (x -> 
                                concat(
                                    COALESCE(x.LastName, 
                                        element_at(filter(x.Names, y -> y.IpNameNumber = x.IPNameNumber), 1).LastName),
                                    ' ',
                                    COALESCE(x.Name, 
                                        element_at(filter(x.Names, y -> y.IpNameNumber = x.IPNameNumber), 1).FirstName)
                                )
                            )
                        )
                    )
                    ELSE ''
                END
            """)
        ).withColumn(
            "CreatorNameNumbers",
            expr("""
                CASE
                    WHEN size(filter(Work_InterestedParties, x -> x.CisacType IN (3, 5, 6))) > 0 
                    THEN concat_ws(';', 
                        transform(
                            filter(Work_InterestedParties, x -> x.CisacType IN (3, 5, 6)),
                            (x -> x.IPNameNumber)
                        )
                    )
                    ELSE ''
                END
            """)
        ).withColumn(
            "PublisherNameNumber",
            expr("""
                CASE
                    WHEN size(filter(Work_AdditionalIdentifiers, x -> x.NameNumber is not null)) > 0 
                    THEN element_at(filter(Work_AdditionalIdentifiers, x -> x.NameNumber is not null), 1).NameNumber
                    ELSE null
                END
            """)
        ).withColumn(
            "PublisherWorkNumber",
            expr("""
                CASE
                    WHEN size(filter(Work_AdditionalIdentifiers, x -> x.NameNumber is not null)) > 0 
                    THEN element_at(filter(Work_AdditionalIdentifiers, x -> x.NameNumber is not null), 1).WorkCode
                    ELSE null
                END
            """)
        ).withColumn(
            "TransactionSource",
            expr("""
                CASE
                    WHEN TransactionType = 'FSQ' THEN 'Publisher'
                    ELSE TransactionSource
                END
            """)
        )

    def get(self, start_date=None, end_date=None):
        filtered_df = self._filter_by_date_range(start_date, end_date)
        transformed_df = self._apply_transformations(filtered_df)

        return transformed_df.select(
            col("id").alias("AuditRequestId"),
            col("AuditId"),
            col("RecordId"),
            col("AgencyCode"),
            col("CreatedDate"),
            col("IsProcessingError"),
            col("IsProcessingFinished"),
            col("Code"),
            col("Message"),
            col("TransactionType"),
            col("PreferredIswc"),
            col("AgencyWorkCode"),
            col("Work_SourceDb").alias("SourceDb"),
            col("OriginalTitle"),
            col("TransactionSource"),
            col("CreatorNames"),
            col("CreatorNameNumbers"),
            col("PublisherNameNumber"),
            col("PublisherWorkNumber"),
            col("RelatedSubmissionIncludedIswc"),
            col("AgentVersion")
        )

