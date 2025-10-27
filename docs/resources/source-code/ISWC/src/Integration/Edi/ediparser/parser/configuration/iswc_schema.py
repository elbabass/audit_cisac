schema = {
    "definitions": {
        "SubmissionId": {
            "type": "number",
            "description": "A unique ID for each submission in the file.The same identifier will be populated for the submission in the response."
        },
        "Agency": {
            "maxLength": 3,
            "minLength": 3,
            "type": "string"
        },
        "Sourcedb": {
            "type": "integer",
            "format": "int32"
        },
        "SubmittingPublisher": {
            "type": "object",
            "required": ["name", "nameNumber", "email"],
            "properties": {
                "name": {
                    "type": "string"
                },
                "nameNumber": {
                    "type": "integer",
                    "format": "int64"
                },
                "email": {
                    "type": "string"
                },
                "role": {
                    "type": "string",
                    "enum": [
                        "AM",
                        "E"
                    ]
                }
            }
        },
        "Workcode": {
            "type": "string"
        },
        "Category": {
            "type": "string",
            "enum": ["DOM", "INT"]
        },
        "AdditionalIdentifier": {
            "type": "object",
            "properties": {
                "isrcs": {
                    "type": "array",
                    "items": {
                        "type": "string"
                    }
                },
                "PublisherIdentifier":{
                    "type":"array",
                    "items": {
                        "properties":{
                            "nameNumber":{
                                "type":"string"
                            },
                            "submitterCode":{
                                "type":"string"
                            },
                            "workCode":{
                                "type": "array",
                                    "items":{
                                        "type":"string"
                                }
                            }
                        }
                    }
                },
                "agencyWorkCodes": {
                    "type": "array",
                    "items": {
                        "properties": {
                                "agency": {
                                    "maxLength": 3,
                                    "minLength": 3,
                                    "type": "string"
                                },
                                "workCode": {
                                    "maxLength": 20,
                                    "type": "string"
                                }
                        }
                    }
                }
            }
        },
        "WorkNumber": {
            "type": "object",
            "properties": {
                "agencyCode": {
                    "type": "string",
                    "minLength": 3,
                    "maxLength": 3
                },
                "agencyWorkCode": {
                    "type": "string",
                    "maxLength": 20
                }
            }
        },
        "MultipleAgencyWorkCodes": {
            "type": "object",
            "properties": {
                    "agency": {
                        "maxLength": 3,
                        "minLength": 3,
                        "type": "string"
                    },
                    "workCode": {
                        "maxLength": 20,
                        "type": "string"
                    }
            }
        },
        "MultipleAgencyWorkCodes2": {
            "type": "object",
            "properties": {
                    "agency": {
                        "maxLength": 3,
                        "minLength": 3,
                        "type": "string"
                    },
                "workCode": {
                        "maxLength": 20,
                        "type": "string"
                    },
                "rejection": {
                    "type": "object",
                        "properties": {
                            "code": {
                                "type": "string"
                            },
                            "message": {
                                "type": "string"
                            }
                        }
                }
            }
        },
        "Disambiguation": {
            "type": "boolean"
        },
        "DisambiguationReason": {
            "type": "string",
            "description": "Disambiguation Reason Code",
            "enum": ["DIT", "DIA", "DIE", "DIC", "DIP", "DIV"]
        },
        "DisambiguateFrom": {
            "type": "array",
                    "items": {
                        "type": "object",
                        "properties": {
                            "iswc": {
                                "type": "string",
                                "pattern": "T[0-9]{10}"
                            }
                        }
                    }},
        "BVLTR": {
            "type": "string",
            "description": "Background, Logo, Theme, Visual or Rolled Up Cue",
            "enum": ["Background", "Logo", "Theme", "Visual", "RolledUpCue"]
        },
        "DerivedWorkType": {
            "type": "string",
            "description": "Derived Work Type- if not provided then this isn't a derived work",
            "enum": ["ModifiedVersion", "Excerpt", "Composite"]
        },
        "DerivedFrom": {
            "type": "object",
            "properties": {
                "iswc": {
                    "type": "string",
                    "pattern": "T[0-9]{10}"
                },
                "title": {
                    "type": "string"
                }
            }
        },
        "Performer": {
            "type": "object",
            "required": ["lastName"],
            "properties": {
                "firstName": {
                    "type": "string"
                },
                "lastName": {
                    "type": "string"
                }
            }
        },
        "Instrumentation": {
            "type": "object",
            "required": ["code"],
            "properties": {
                "code": {
                    "type": "string",
                    "minLength": 1,
                    "maxLength": 3
                }
            }
        },
        "ISWC": {
            "type": "string",
            "description": "International Standard Work Code",
            "pattern": "T[0-9]{10}"
        },
        "LocallyAllocatedIswc": {
            "type": "string",
            "pattern": "T[0-9]{10}"
        },
        "InterestedParty": {
            "type": "object",
            "required": ["nameNumber", "role"],
            "properties": {
                "name": {
                    "type": "string"
                },
                "nameNumber": {
                    "type": "integer",
                    "format": "int64"
                },
                "baseNumber": {
                    "type": "string"
                },
                "role": {
                    "type": "string",
                    "enum": [
                        "CA",
                        "MA",
                        "AR",
                        "SE",
                        "PA",
                        "ES",
                        "AM",
                        "SA",
                        "C",
                        "AD",
                        "A",
                        "E",
                        "AQ",
                        "SR",
                        "TR"
                    ]
                }
            }
        },
        "WorkInfo": {
            "type": "object",
            "required": ["agency", "sourcedb", "workcode"],
            "properties": {
                "agency": {
                    "$ref": "#/definitions/Agency"
                },
                "sourcedb": {
                    "$ref": "#/definitions/Sourcedb"
                },
                "workcode": {
                    "$ref": "#/definitions/Workcode"
                },
                "disambiguation": {
                    "$ref": "#/definitions/Disambiguation"
                },
                "disambiguationReason": {
                    "$ref": "#/definitions/DisambiguationReason"
                },
                "disambiguateFrom": {
                    "$ref": "#/definitions/DisambiguateFrom"
                },
                "bvltr": {
                    "$ref": "#/definitions/BVLTR"
                },
                "derivedWorkType": {
                    "$ref": "#/definitions/DerivedWorkType"
                },
                "derivedFromIswcs": {
                    "type": "array",
                    "items": {
                        "$ref": "#/definitions/DerivedFrom"
                    }
                },
                "performers": {
                    "type": "array",
                    "items": {
                        "$ref": "#/definitions/Performer"
                    }
                },
                "instrumentation": {
                    "type": "array",
                    "items": {
                        "$ref": "#/definitions/Instrumentation"
                    }
                },
                "iswc": {
                    "$ref": "#/definitions/ISWC"
                },
                "archivedIswc": {
                    "$ref": "#/definitions/ISWC"
                }
            }
        },
        "ErrorMsg": {
            "type": "object",
            "required": ["errorType", "errorNumber", "errorMessage"],
            "properties": {
                "errorType": {
                    "type": "string",
                    "enum": [
                        "Field Rejected",
                        "Transaction Rejected",
                        "Entire File Rejected"
                    ]
                },
                "errorNumber": {
                    "description": "Numeric identifier for error.  See associated specification for list of expected errorNumber values",
                    "type": "integer"
                },
                "errorMessage": {
                    "description": "Corresponding description for the error",
                    "type": "string"
                },
                "errorField": {
                    "description": "If a Field Rejected type error is generated then the invalid field value will be reported here",
                    "type": "string"
                }
            }
        },
        "Title": {
            "type": "object",
            "required": ["title", "type"],
            "properties": {
                "title": {
                    "description": "Musical work title",
                    "type": "string"
                },
                "type": {
                    "description": "Musical work title type",
                    "type": "string",
                    "enum": [
                        "ET",
                        "ST",
                        "FT",
                        "CT",
                        "IT",
                        "RT",
                        "AT",
                        "OL",
                        "TT",
                        "OA",
                        "TE",
                        "TO",
                        "PT",
                        "AL"
                    ]
                },
                "languageCode": {
                    "description": "ISO 639-1 two character language code.  E.G. EN",
                    "type": "string",
                    "minLength": 2,
                    "maxLength": 2
                }
            }
        },
        "OriginalTitle": {
            "description": "Original title of a musical composition",
            "type": "string"
        },
        "CisnetCreatedDate": {
            "type": "string",
            "description": "Date and time when this metadata was created on CISNET",
            "format": "date-time"
        },
        "CisnetLastModifiedDate": {
            "type": "string",
            "description": "Date and time when this metadata last modified on CISNET",
            "format": "date-time"
        },
        "FileCreationDateTime": {
            "type": "string",
            "description": "Date and time when this file or the related file (for ACK) was created. Used to tie together a file with its related ACKs",
            "format": "date-time"
        },
        "TaskId": {
            "type": "number",
            "format": "int32"
        },
        "WorkflowType": {
            "type": "string",
            "enum": ["UpdateApproval", "MergeApproval"]
        },
        "Status": {
            "type": "string",
            "enum": ["Outstanding", "Approved", "Rejected", "Cancelled"]
        },
        "ProcessingDate": {
            "type": "string",
            "description": "Date when file was processed",
            "format": "date-time"
        },
        "TransactionType": {
            "type": "string",
            "description": "Used in Acknowledgements to define the type of transaction being acknoweldged",
            "enum": [
                "AddSubmission",
                "UpdateSubmission",
                "DeleteSubmission",
                "SearchByIswcSubmission",
                "SearchByAgencyWorkCodeSubmission",
                "MergeSubmission",
                "UpdateWorkflowTask",
                "FindSubmission"
            ]
        },
        "TransactionStatus": {
            "type": "string",
            "description": "The current status of this transaction",
            "enum": ["FullyAccepted", "PartiallyAccepted", "Rejected"]
        },
        "Transactions": {
            "FileHeader": {
                "type": "object",
                "description": "File Header",
                "properties": {
                    "submittingAgency": {
                        "$ref": "#/definitions/Agency"
                    },
                    "submittingSourcedb": {
                        "$ref": "#/definitions/Sourcedb"
                    },
                    "submittingPublisher": {
                        "$ref": "#/definitions/SubmittingPublisher"
                    },
                    "fileCreationDateTime": {
                        "$ref": "#definitions/FileCreationDateTime",
                    },
                    "receivingAgency": {
                        "$ref": "#/definitions/Agency"
                    }
                },
                "required": [
                    "submittingAgency",
                    "submittingSourcedb",
                    "fileCreationDateTime",
                    "receivingAgency"
                ]
            },
            "AddSubmission": {
                "type": "object",
                "description": "Create a new submission.  This may be linked to an existing preferredISWC or a new preferredISWC may be generated for it",
                "properties": {
                    "submissionId": {
                        "$ref": "#/definitions/SubmissionId"
                    },
                    "agency": {
                        "$ref": "#/definitions/Agency"
                    },
                    "sourcedb": {
                        "$ref": "#/definitions/Sourcedb"
                    },
                    "workcode": {
                        "$ref": "#/definitions/Workcode"
                    },
                    "category": {
                        "$ref": "#/definitions/Category"
                    },
                    "disambiguation": {
                        "$ref": "#/definitions/Disambiguation"
                    },
                    "disambiguationReason": {
                        "$ref": "#/definitions/DisambiguationReason"
                    },
                    "disambiguateFrom": {
                        "$ref": "#/definitions/DisambiguateFrom"
                    },
                    "bvltr": {
                        "$ref": "#/definitions/BVLTR"
                    },
                    "derivedWorkType": {
                        "$ref": "#/definitions/DerivedWorkType"
                    },
                    "derivedFromIswcs": {
                        "type": "array",
                        "items": {
                            "$ref": "#/definitions/DerivedFrom"
                        }
                    },
                    "performers": {
                        "type": "array",
                        "items": {
                            "$ref": "#/definitions/Performer"
                        }
                    },
                    "instrumentation": {
                        "type": "array",
                        "items": {
                            "$ref": "#/definitions/Instrumentation"
                        }
                    },
                    "iswc": {
                        "$ref": "#/definitions/ISWC"
                    },
                    "originalTitle": {
                        "$ref": "#/definitions/OriginalTitle"
                    },
                    "otherTitles": {
                        "description": "Other titles associated with musical composition (aka alternative titles)",
                        "type": "array",
                        "items": {
                            "$ref": "#/definitions/Title"
                        }
                    },
                    "interestedParties": {
                        "description": "Interested party associated with musical composition, such as :composers, lyricist, translator and original publishers",
                        "type": "array",
                        "items": {
                            "$ref": "#/definitions/InterestedParty"
                        }
                    },
                    "additionalIdentifiers": {
                        "$ref": "#/definitions/AdditionalIdentifier"
                    },
                    "multipleAgencyWorkCodes": {
                        "type": "array",
                        "items": {
                            "$ref": "#/definitions/MultipleAgencyWorkCodes"
                        }
                    },
                    "locallyAllocatedIswc": {
                        "$ref": "#/definitions/LocallyAllocatedIswc"
                    }
                },
                "required": [
                    "agency",
                    "sourcedb",
                    "submissionId",
                    "workcode",
                    "originalTitle",
                    "interestedParties"
                ]
            },
            "UpdateSubmission": {
                "type": "object",
                "description": "Update a previously sent submission, identified by  agency, sourcedb, workcode and cross checked against the preferredISWC",
                "properties": {
                    "submissionId": {
                        "$ref": "#/definitions/SubmissionId"
                    },
                    "preferredIswc": {
                        "$ref": "#/definitions/ISWC"
                    },
                    "agency": {
                        "$ref": "#/definitions/Agency"
                    },
                    "sourcedb": {
                        "$ref": "#/definitions/Sourcedb"
                    },
                    "workcode": {
                        "$ref": "#/definitions/Workcode"
                    },
                    "category": {
                        "$ref": "#/definitions/Category"
                    },
                    "disambiguation": {
                        "$ref": "#/definitions/Disambiguation"
                    },
                    "disambiguationReason": {
                        "$ref": "#/definitions/DisambiguationReason"
                    },
                    "disambiguateFrom": {
                        "$ref": "#/definitions/DisambiguateFrom"
                    },
                    "bvltr": {
                        "$ref": "#/definitions/BVLTR"
                    },
                    "derivedWorkType": {
                        "$ref": "#/definitions/DerivedWorkType"
                    },
                    "derivedFromIswcs": {
                        "type": "array",
                        "items": {
                            "$ref": "#/definitions/DerivedFrom"
                        }
                    },
                    "performers": {
                        "type": "array",
                        "items": {
                            "$ref": "#/definitions/Performer"
                        }
                    },
                    "instrumentation": {
                        "type": "array",
                        "items": {
                            "$ref": "#/definitions/Instrumentation"
                        }
                    },
                    "originalTitle": {
                        "$ref": "#/definitions/OriginalTitle"
                    },
                    "otherTitles": {
                        "description": "Other titles associated with musical composition (aka alternative titles)",
                        "type": "array",
                        "items": {
                            "$ref": "#/definitions/Title"
                        }
                    },
                    "interestedParties": {
                        "description": "Interested party associated with musical composition, such as :composers, lyricist, translator, original and sub publishers",
                        "type": "array",
                        "items": {
                            "$ref": "#/definitions/InterestedParty"
                        }
                    },
                    "multipleAgencyWorkCodes": {
                        "type": "array",
                        "items": {
                            "$ref": "#/definitions/MultipleAgencyWorkCodes"
                        }
                    }
                },
                "required": [
                    "submissionId",
                    "preferredIswc",
                    "agency",
                    "sourcedb",
                    "workcode",
                    "category",
                    "originalTitle",
                    "interestedParties"
                ]
            },
            "DeleteSubmission": {
                "type": "object",
                "description": "Delete the agency submission identified by agency, sourcedb and workcode from the provided preferredISWC",
                "properties": {
                    "submissionId": {
                        "$ref": "#/definitions/SubmissionId"
                    },
                    "preferredIswc": {
                        "$ref": "#/definitions/ISWC"
                    },
                    "agency": {
                        "$ref": "#/definitions/Agency"
                    },
                    "sourcedb": {
                        "$ref": "#/definitions/Sourcedb"
                    },
                    "workcode": {
                        "$ref": "#/definitions/Workcode"
                    },
                    "reasonCode": {
                        "type": "string"
                    }
                },
                "required": [
                    "submissionId",
                    "preferredIswc",
                    "agency",
                    "sourcedb",
                    "workcode",
                    "reasonCode"
                ]
            },
            "FindSubmission": {
                "type": "object",
                "description": "Retrieve an existing preferredISWC for a set of metadata provided. The society does not have to be ISWC eligible for the metadata being submitted.  These transactions are similar to the existing resolution service requests. ",
                "properties": {
                    "submissionId": {
                        "$ref": "#/definitions/SubmissionId"
                    },
                    "workcode": {
                        "$ref": "#/definitions/Workcode"
                    },
                    "disambiguation": {
                        "$ref": "#/definitions/Disambiguation"
                    },
                    "disambiguationReason": {
                        "$ref": "#/definitions/DisambiguationReason"
                    },
                    "disambiguateFrom": {
                        "$ref": "#/definitions/DisambiguateFrom"
                    },
                    "derivedWorkType": {
                        "$ref": "#/definitions/DerivedWorkType"
                    },
                    "derivedFromIswcs": {
                        "type": "array",
                        "items": {
                            "$ref": "#/definitions/DerivedFrom"
                        }
                    },
                    "originalTitle": {
                        "$ref": "#/definitions/OriginalTitle"
                    },
                    "otherTitles": {
                        "description": "Other titles associated with musical composition (aka alternative titles)",
                        "type": "array",
                        "items": {
                            "$ref": "#/definitions/Title"
                        }
                    },
                    "interestedParties": {
                        "description": "Interested party associated with musical composition, such as :composers, lyricist, translator and original publishers",
                        "type": "array",
                        "items": {
                            "$ref": "#/definitions/InterestedParty"
                        }
                    },
                    "additionalIdentifiers": {
                        "$ref": "#/definitions/AdditionalIdentifier"
                    }
                },
                "required": [
                    "submissionId",
                    "workcode",
                    "originalTitle",
                    "interestedParties"
                ]
            },
            "SearchByIswcSubmission": {
                "type": "object",
                "description": "Request the Metadata for the provided iswc",
                "properties": {
                    "submissionId": {
                        "$ref": "#/definitions/SubmissionId"
                    },
                    "iswc": {
                        "$ref": "#/definitions/ISWC"
                    }
                },
                "required": ["submissionId", "iswc"]
            },
            "SearchByAgencyWorkCodeSubmission": {
                "type": "object",
                "description": "Request the Preferred ISWC for the provided Agency, Sourcedb and Workcode",
                "properties": {
                    "submissionId": {
                        "$ref": "#/definitions/SubmissionId"
                    },
                    "agency": {
                        "$ref": "#/definitions/Agency"
                    },
                    "sourcedb": {
                        "$ref": "#/definitions/Sourcedb"
                    },
                    "workcode": {
                        "$ref": "#/definitions/Workcode"
                    },
                    "originalTitle": {
                        "$ref": "#/definitions/OriginalTitle"
                    }
                },
                "required": ["submissionId", "agency", "sourcedb", "workcode"]
            },
            "MergeSubmission": {
                "type": "object",
                "description": "Merge into a designated preferredISWC (dentified by preferredIswc, agency, sourcedb and workcode), a list of one or more existign preferredISWCs",
                "properties": {
                    "submissionId": {
                        "$ref": "#/definitions/SubmissionId"
                    },
                    "preferredIswc": {
                        "$ref": "#/definitions/ISWC"
                    },
                    "agency": {
                        "$ref": "#/definitions/Agency"
                    },
                    "sourcedb": {
                        "$ref": "#/definitions/Sourcedb"
                    },
                    "workcode": {
                        "$ref": "#/definitions/Workcode"
                    },
                    "mergeIswcs": {
                        "type": "array",
                        "items": {
                            "$ref": "#/definitions/ISWC"
                        }
                    }
                },
                "required": [
                    "submissionId",
                    "preferredIswc",
                    "agency",
                    "sourcedb",
                    "workcode"
                ]
            },
            "UpdateWorkflowTask": {
                "type": "object",
                "description": "Update the status of an existing workflow task",
                "properties": {
                    "submissionId": {
                        "$ref": "#/definitions/SubmissionId"
                    },
                    "taskId": {
                        "$ref": "#/definitions/TaskId"
                    },
                    "workflowType": {
                        "$ref": "#/definitions/WorkflowType"
                    },
                    "status": {
                        "$ref": "#/definitions/Status"
                    }
                },
                "required": ["submissionId", "taskId", "status", "workflowType"]
            },
            "Acknowledgement": {
                "type": "object",
                "description": "Acknowledgement in response to transactions above",
                "properties": {
                    "submissionId": {
                        "$ref": "#/definitions/SubmissionId"
                    },
                    "originalFileCreationDateTime": {
                        "$ref": "#definitions/FileCreationDateTime"
                    },
                    "originalSubmissionId": {
                        "$ref": "#/definitions/SubmissionId"
                    },
                    "originalTransactionType": {
                        "$ref": "#/definitions/TransactionType"
                    },
                    "preferredIswc": {
                        "$ref": "#/definitions/ISWC"
                    },
                    "agency": {
                        "$ref": "#/definitions/Agency"
                    },
                    "sourcedb": {
                        "$ref": "#/definitions/Sourcedb"
                    },
                    "workcode": {
                        "$ref": "#/definitions/Workcode"
                    },
                    "originalTitle": {
                        "$ref": "#/definitions/OriginalTitle"
                    },
                    "processingDate": {
                        "$ref": "#/definitions/ProcessingDate"
                    },
                    "transactionStatus": {
                        "$ref": "#/definitions/TransactionStatus"
                    },
                    "otherTitles": {
                        "description": "Other titles associated with musical composition (aka alternative titles)",
                        "type": "array",
                        "items": {
                            "$ref": "#/definitions/Title"
                        }
                    },
                    "interestedParties": {
                        "description": "Interested party associated with musical composition, such as :composers, lyricist, translator, original and sub publishers",
                        "type": "array",
                        "items": {
                            "$ref": "#/definitions/InterestedParty"
                        }
                    },
                    "workInfo": {
                        "description": "Other society work information associated with the returned ISWC",
                        "type": "array",
                        "items": {
                            "$ref": "#/definitions/WorkInfo"
                        }
                    },
                    "multipleAgencyWorkCodes": {
                        "type": "array",
                        "items": {
                            "$ref": "#/definitions/MultipleAgencyWorkCodes2"
                        }
                    },
                    "errorMessages": {
                        "description": "Details of any errors that ocurred with the transaction",
                        "type": "array",
                        "items": {
                            "$ref": "#/definitions/ErrorMsg"
                        }
                    }
                },
                "required": [
                    "submissionId",
                    "originalFileCreationDateTime",
                    "originalSubmissionId",
                    "originalTransactionType",
                    "processingDate",
                    "transactionStatus"
                ]
            },
            "Notification": {
                "type": "object",
                "description": "Notification to agencies that are potentially impacted by a transaction processed by the ISWC database",
                "properties": {
                    "submissionId": {
                        "$ref": "#/definitions/SubmissionId"
                    },
                    "originalFileCreationDateTime": {
                        "ref": "#definitions/FileCreationDateTime"
                    },
                    "originalTransactionType": {
                        "ref": "#/definitions/TransactionType"
                    },
                    "preferredIswc": {
                        "$ref": "#/definitions/ISWC"
                    },
                    "agency": {
                        "$ref": "#/definitions/Agency"
                    },
                    "sourcedb": {
                        "$ref": "#/definitions/Sourcedb"
                    },
                    "workcode": {
                        "$ref": "#/definitions/Workcode"
                    },
                    "originalTitle": {
                        "$ref": "#/definitions/OriginalTitle"
                    },
                    "processingDate": {
                        "$ref": "#/definitions/ProcessingDate"
                    },
                    "workflowTaskId": {
                        "$ref": "#/definitions/TaskId"
                    },
                    "workflowStatus": {
                        "$ref": "#/definitions/Status"
                    },
                    "otherTitles": {
                        "description": "Other titles associated with musical composition (aka alternative titles)",
                        "type": "array",
                        "items": {
                            "$ref": "#/definitions/Title"
                        }
                    },
                    "interestedParties": {
                        "description": "Interested party associated with musical composition, such as :composers, lyricist, translator, original and sub publishers",
                        "type": "array",
                        "items": {
                            "$ref": "#/definitions/InterestedParty"
                        }
                    },
                    "workInfo": {
                        "description": "Other society work information associated with the returned ISWC",
                        "type": "array",
                        "items": {
                            "$ref": "#/definitions/WorkInfo"
                        }
                    }
                },
                "required": [
                    "submissionId",
                    "originalFileCreationDateTime",
                    "originalSubmissionId",
                    "originalTransactionType",
                    "preferredIswc",
                    "agency",
                    "sourcedb",
                    "workcode",
                    "originalTitle",
                    "processingDate",
                    "transactionStatus"
                ]
            },
            "IPContextSearchModel": {
                "type": "object",
                "required": ["lastName", "works"],
                "properties": {
                    "submissionId": {
                        "$ref": "#/definitions/SubmissionId"
                    },
                    "firstName": {
                        "type": "string",
                        "minLength": 1,
                        "maxLength": 50
                    },
                    "lastName": {
                        "type": "string",
                        "minLength": 1,
                        "maxLength": 50
                    },
                    "nameNumber": {
                        "type": "integer",
                        "format": "int64"
                    },
                    "dateOfBirth": {
                        "type": "string",
                        "format": "date"
                    },
                    "dateOfDeath": {
                        "type": "string",
                        "format": "date"
                    },
                    "affiliations": {
                        "type": "array",
                        "items": {
                            "type": "object",
                            "properties": {
                                "affiliation": {
                                    "type": "string"
                                }
                            }
                        }
                    },
                    "age": {
                        "type": "integer",
                        "format": "int64"
                    },
                    "ageTolerance": {
                        "type": "integer",
                        "format": "int64"
                    },
                    "works": {
                        "type": "array",
                        "items": {
                            "type": "object",
                            "properties": {
                                "iswc": {
                                    "type": "string",
                                    "pattern": "T[0-9]{10}"
                                },
                                "titles": {
                                    "type": "array",
                                    "items": {
                                        "type": "object",
                                        "required": ["title", "type"],
                                        "properties": {
                                            "title": {
                                                "description": "Musical work title",
                                                "type": "string"
                                            },
                                            "type": {
                                                "description": "Musical work title type",
                                                "type": "string",
                                                "enum": [
                                                    "ET", "ST", "FT", "CT", "IT", "RT", "AT",
                                                    "OL", "TT", "OA", "TE", "TO", "PT", "AL",
                                                    "OT"
                                                ]
                                            },
                                            "languageCode": {
                                                "description": "ISO 639-1 two character language code. E.G. EN",
                                                "type": "string",
                                                "minLength": 2,
                                                "maxLength": 2
                                            }
                                        }
                                    }
                                },
                                "workCodes": {
                                    "type": "array",
                                    "items": {
                                        "$ref": "#/definitions/WorkNumber"
                                    }
                                }
                            }
                        }
                    },
                    "additionalIdentifiers": {
                        "type": "object",
                        "properties": {
                            "isrcs": {
                                "type": "array",
                                "items": {
                                    "type": "string"
                                }
                            }
                        }
                    },
                    "societyWorkCodes": {
                        "type": "boolean"
                    },
                    "additionalIPNames": {
                        "type": "boolean"
                    }
                }
            }
        }
    },
    "$schema": "http://json-schema.org/draft-07/schema#",
    "title": "submissions",
    "type": "object",
    "properties": {
        "fileHeader": {
            "type": "object",
            "$ref": "#/definitions/Transactions/FileHeader"
        },
        "addSubmissions": {
            "type": "array",
            "items": {
                "$ref": "#/definitions/Transactions/AddSubmission"
            }
        },
        "updateSubmissions": {
            "type": "array",
            "items": {
                "$ref": "#/definitions/Transactions/UpdateSubmission"
            }
        },
        "deleteSubmissions": {
            "type": "array",
            "items": {
                "$ref": "#/definitions/Transactions/DeleteSubmission"
            }
        },
        "searchByIswcSubmissions": {
            "type": "array",
            "items": {
                "$ref": "#/definitions/Transactions/SearchByIswcSubmission"
            }
        },
        "searchByAgencyWorkCodeSubmissions": {
            "type": "array",
            "items": {
                "$ref": "#/definitions/Transactions/SearchByAgencyWorkCodeSubmission"
            }
        },
        "mergeSubmissions": {
            "type": "array",
            "items": {
                "$ref": "#/definitions/Transactions/MergeSubmission"
            }
        },
        "updateWorkflowTasks": {
            "type": "array",
            "items": {
                "$ref": "#/definitions/Transactions/UpdateWorkflowTask"
            }
        },
        "findSubmissions": {
            "type": "array",
            "items": {
                "$ref": "#/definitions/Transactions/FindSubmission"
            }
        },
        "acknowledgements": {
            "type": "array",
            "items": {
                "$ref": "#/definitions/Transactions/Acknowledgement"
            }
        },
        "notifications": {
            "type": "array",
            "items": {
                "$ref": "#/definitions/Transactions/Notification"
            }
        },
        "publisherContextSearch": {
            "type": "array",
            "items": {
                "$ref": "#/definitions/Transactions/IPContextSearchModel"
            }
        }
    },
    "required": ["fileHeader"]
}
