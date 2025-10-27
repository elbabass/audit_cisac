thirdPartySchema = {
    "definitions": {
        "SubmissionId": {
            "type": "number",
            "description": "A unique ID for each submission in the file.The same identifier will be populated for the submission in the response."
        },
        "SubmittingPartyId": {
            "type": "string",
            "description": "A unique ID represents the submittingPartyId of the sender of the file."
        },
        "Sourcedb": {
            "type": "integer",
            "format": "int32"
        },
        "Agency": {
            "maxLength": 3,
            "minLength": 3,
            "type": "string"
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
        "OriginalTitle": {
            "type": "string"
        },
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
            "required": ["role"],
            "properties": {
                "lastName": {
                    "type": "string"
                },
                "nameNumber": {
                    "type": "integer",
                    "format": "int64"
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
                        "AL",
                        "OT"
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
                    "submittingPartyId": {
                        "$ref": "#/definitions/SubmittingPartyId"
                    },
                    "fileCreationDateTime": {
                        "$ref": "#definitions/FileCreationDateTime",
                        "receivingAgency": {
                            "$ref": "#/definitions/Agency"
                        }
                    }
                },
                "required": [
                    "submittingPartyId",
                    "fileCreationDateTime"
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
                "description": "Request the Preferred ISWC for the provided Agency and Workcode",
                "properties": {
                    "submissionId": {
                        "$ref": "#/definitions/SubmissionId"
                    },
                    "agency": {
                        "$ref": "#/definitions/Agency"
                    },
                    "workcode": {
                        "$ref": "#/definitions/Workcode"
                    },
                    "originalTitle": {
                        "$ref": "#/definitions/OriginalTitle"
                    }
                },
                "required": ["submissionId", "agency", "workcode"]
            },
            "SearchByTitleAndContributorsSubmission": {
                "type": "object",
                "description": "Request the Preferred ISWC for the provided Title and Contributors",
                "properties": {
                    "submissionId": {
                        "$ref": "#/definitions/SubmissionId"
                    },
                    "titles": {
                        "description": "Titles associated with musical composition.",
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
                    }
                },
                "required": ["submissionId", "titles", "interestedParties"]
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
                    "errorMessages": {
                        "description": "Details of any errors that ocurred with the transaction",
                        "type": "array",
                        "items": {
                            "$ref": "#/definitions/ErrorMsg"
                        }
                    },
                    "linkedISWC": {
                        "description": "All linked preferred ISWCs. Presence of this field indicates that those ISWCs merged to this ISWC.",
                        "type": "array",
                        "items": {
                            "$ref": "#/definitions/ISWC"
                        }
                    },
                    "parentISWC": {
                        "$ref": "#/definitions/ISWC"
                    },
                    "overallParentISWC": {
                        "$ref": "#/definitions/ISWC"
                    }
                },
                "required": [
                    "submissionId",
                    "originalFileCreationDateTime",
                    "originalSubmissionId",
                    "originalTransactionType",
                    "processingDate",
                    "transactionStatus",
                    "linkedISWC"
                ]
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
        "searchByTitleAndContributorsSubmissions": {
            "type": "array",
            "items": {
                "$ref": "#/definitions/Transactions/SearchByTitleAndContributorsSubmission"
            }
        },
        "acknowledgements": {
            "type": "array",
            "items": {
                "$ref": "#/definitions/Transactions/Acknowledgement"
            }
        }
    },
    "required": ["fileHeader"]
}