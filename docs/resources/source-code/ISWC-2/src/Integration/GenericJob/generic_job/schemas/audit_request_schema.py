from pyspark.sql.types import *

class AuditRequestSchema:
    def audit_request_schema() -> StructType:
        return StructType([
            StructField("id", StringType(), True),
            StructField("AuditId", StringType(), True),
            StructField("RecordId", IntegerType(), True),
            StructField("AgencyCode", StringType(), True),
            StructField("CreatedDate", TimestampType(), True),
            StructField("IsProcessingFinished", BooleanType(), True),
            StructField("IsProcessingError", BooleanType(), True),
            StructField("ProcessingCompletionDate", TimestampType(), True),        
            StructField("RulesApplied", ArrayType(StructType([
                StructField("RuleName", StringType(), True),
                StructField("RuleVersion", StringType(), True),
                StructField("RuleConfiguration", StringType(), True),
                StructField("TimeTaken", StringType(), True)
            ])), True),          
            StructField("PartitionKey", StringType(), True),
            StructField("WorkIdBefore", LongType(), True),
            StructField("WorkIdAfter", LongType(), True),           
            StructField("TransactionError", StructType([
                    StructField("Code", StringType(), True),
                    StructField("Message", StringType(), True)
                ]), True),
            StructField("IswcStatus", StringType(), True),          
            StructField("Work", StructType([
                StructField("PreferredIswc", StringType(), True),
                StructField("SourceDb", IntegerType(), True),
                StructField("WorkNumber", StructType([
                    StructField("Type", StringType(), True),
                    StructField("Number", StringType(), True)
                ]), True),
                StructField("Disambiguation", BooleanType(), True),
                StructField("DisambiguationReason", IntegerType(), True),
                StructField("DisambiguateFrom", ArrayType(StructType([
                    StructField("Iswc", StringType(), True)
                ])), True),
                StructField("BVLTR", IntegerType(), True),
                StructField("DerivedWorkType", IntegerType(), True),
                StructField("DerivedFrom", ArrayType(StructType([
                    StructField("Iswc", StringType(), True),
                    StructField("Title", StringType(), True)
                ])), True),
                StructField("Performers", ArrayType(StructType([
                    StructField("PerformerID", IntegerType(), True),
                    StructField("Isni", StringType(), True),
                    StructField("Ipn", IntegerType(), True),
                    StructField("FirstName", StringType(), True),
                    StructField("LastName", StringType(), True),
                    StructField("Designation", StringType(), True)
                ])), True),
                StructField("Instrumentation", ArrayType(StructType([
                    StructField("Code", StringType(), True),
                    StructField("Name", StringType(), True)
                ])), True),
                StructField("CisnetCreatedDate", TimestampType(), True),
                StructField("CisnetLastModifiedDate", TimestampType(), True),
                StructField("Iswc", StringType(), True),
                StructField("Agency", StringType(), True),
                StructField("Titles", ArrayType(StructType([
                    StructField("TitleID", LongType(), True),
                    StructField("Name", StringType(), True),
                    StructField("StandardizedTitle", StringType(), True),
                    StructField("Type", StringType(), True)
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
                StructField("IsReplaced", BooleanType(), True),
                StructField("ReasonCode", StringType(), True),
                StructField("ApproveWorkflowTasks", BooleanType(), True),
                StructField("Category", IntegerType(), True),
                StructField("IswcsToMerge", ArrayType(StringType()), True),
                StructField("WorkNumbersToMerge", ArrayType(StringType()), True),
                StructField("StartIndex", IntegerType(), True),
                StructField("PageLength", IntegerType(), True),
                StructField("WorkflowTasks", ArrayType(StringType()), True),
                StructField("PreviewDisambiguation", BooleanType(), True),
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
                StructField("RelatedSubmissionIncludedIswc", BooleanType(), True),
                StructField("IsPublicRequest", BooleanType(), True),
                StructField("DisableAddUpdateSwitching", BooleanType(), True),
                StructField("AllowProvidedIswc", BooleanType(), True),
                StructField("AdditionalAgencyWorkNumbers", ArrayType(StringType()), True),
                StructField("HashCode", StringType(), True)
            ]), True),
            StructField("TransactionType", StringType(), True),
            StructField("WorkflowInstanceId", StringType(), True),
            StructField("TransactionSource", StringType(), True),
            StructField("IsEligible", BooleanType(), True),
            StructField("RelatedSubmissionIncludedIswc", BooleanType(), True),
            StructField("RequestSource", IntegerType(), True),
            StructField("AgentVersion", StringType(), True),
            StructField("UpdateAllocatedIswc", BooleanType(), True),
            StructField("WorkNumberToReplaceIasWorkNumber", StringType(), True),
            StructField("MultipleAgencyWorkCodes", ArrayType(StringType()), True),
            StructField("MultipleAgencyWorkCodesChild", BooleanType(), True),           
            StructField("_rid", StringType(), True),
            StructField("_self", StringType(), True),
            StructField("_etag", StringType(), True),
            StructField("_attachments", StringType(), True),
            StructField("_ts", LongType(), True),
            StructField("_lsn", LongType(), True)
        ])
