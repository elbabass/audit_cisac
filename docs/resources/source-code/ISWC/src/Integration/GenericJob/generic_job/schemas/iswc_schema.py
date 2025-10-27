from pyspark.sql.types import *

class IswcSchema:
    def iswc_schema() -> StructType:
        return StructType([
            StructField("WorkId", LongType(), True),
            StructField("PreferredIswc", StringType(), True),
            StructField("IswcStatus", StringType(), True),
            StructField("AgencyCode", StringType(), True),
            StructField("SourceDb", IntegerType(), True),
            StructField("AgencyWorkCode", StringType(), True),
            StructField("CreatedDate", TimestampType(), True),
            StructField("LastModifiedDate", TimestampType(), True),
            StructField("CisnetCreatedDate", TimestampType(), True),
            StructField("CisnetLastModifiedDate", TimestampType(), True),
            StructField("Status", BooleanType(), True),
            StructField("IsEligible", BooleanType(), True),
            StructField("IsReplaced", BooleanType(), True),
            StructField("Disambiguation", BooleanType(), True),
            StructField("DisambiguationReason", IntegerType(), True),
            StructField("DerivedWorkType", IntegerType(), True),
            StructField("Category", IntegerType(), True),
            StructField("BVLTR", IntegerType(), True),
            StructField("Titles", ArrayType(StructType([
                StructField("TitleID", LongType(), True),
                StructField("Name", StringType(), True),
                StructField("StandardizedTitle", StringType(), True),
                StructField("Type", StringType(), True)
            ])), True),
            StructField("DisambiguateFrom", ArrayType(StructType([
                StructField("Iswc", StringType(), True)
            ])), True),
            StructField("DerivedFrom", ArrayType(StructType([
                StructField("Iswc", StringType(), True),
                StructField("Title", StringType(), True)
            ])), True),
            StructField("Instrumentation", ArrayType(StructType([
                StructField("Code", StringType(), True),
                StructField("Name", StringType(), True)
            ])), True),
            StructField("AdditionalIdentifiers", ArrayType(StructType([
                StructField("WorkCode", StringType(), True),
                StructField("SubmitterCode", StringType(), True),
                StructField("NameNumber", LongType(), True),
                StructField("NumberTypeId", IntegerType(), True),
                StructField("SubmitterDPID", StringType(), True),
                StructField("RecordingTitle", StringType(), True),
                StructField("SubTitle", StringType(), True),
                StructField("LabelName", StringType(), True),
                StructField("ReleaseEmbargoDate", TimestampType(), True),
                StructField("Performers", ArrayType(StructType([
                    StructField("PerformerID", LongType(), True),
                    StructField("FirstName", StringType(), True),
                    StructField("LastName", StringType(), True),
                    StructField("PerformerDesignationID", IntegerType(), True)
                ])), True)
            ])), True),
            StructField("InterestedParties", ArrayType(StructType([
                StructField("ContributorID", LongType(), True),
                StructField("IpBaseNumber", StringType(), True),
                StructField("Type", IntegerType(), True),
                StructField("CisacType", IntegerType(), True),                  
                StructField("Names", ArrayType(StructType([
                    StructField("IpNameNumber", LongType(), True),
                    StructField("AmendedDateTime", TimestampType(), True),
                    StructField("FirstName", StringType(), True),
                    StructField("LastName", StringType(), True),
                    StructField("CreatedDate", TimestampType(), True),
                    StructField("TypeCode", IntegerType(), True),
                    StructField("ForwardingNameNumber", LongType(), True),
                    StructField("Agency", StringType(), True)
                ])), True),
                StructField("IsAuthoritative", BooleanType(), True),
                StructField("Status", BooleanType(), True),
                StructField("Agency", StringType(), True),
                StructField("DeathDate", TimestampType(), True),
                StructField("IPNameNumber", LongType(), True),
                StructField("Name", StringType(), True),
                StructField("LastName", StringType(), True),
                StructField("DisplayName", StringType(), True),
                StructField("CreatedDate", TimestampType(), True),
                StructField("Affiliation", StringType(), True),
                StructField("ContributorType", StringType(), True),
                StructField("IsWriter", BooleanType(), True),
                StructField("IsExcludedFromIswc", BooleanType(), True),
                StructField("LegalEntityType", StringType(), True),
                StructField("IsPseudonymGroupMember", BooleanType(), True)
            ])), True),
            StructField("Performers", ArrayType(StructType([
                StructField("PerformerID", IntegerType(), True),
                StructField("Isni", StringType(), True),
                StructField("Ipn", IntegerType(), True),
                StructField("FirstName", StringType(), True),
                StructField("LastName", StringType(), True),
                StructField("Designation", StringType(), True)
            ])), True)
        ])
