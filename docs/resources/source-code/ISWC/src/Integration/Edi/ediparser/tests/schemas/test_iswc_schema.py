from jsonschema import RefResolutionError, validate, ValidationError, Draft7Validator, SchemaError
from ediparser.parser.configuration.iswc_schema import schema
import jsonschema

# all types of submission in json to test schema validation
valid_submissions_json = {
    "fileHeader": {
        "submittingAgency": "128",
        "submittingSourcedb": 128,
        "fileCreationDateTime": "2019-10-01T18:25:43.511Z",
        "receivingAgency": "123"
    },
    "addSubmissions": [
        {
            "submissionId": 1,
            "agency": "128",
            "sourcedb": 128,
            "workcode": "R28995186",
            "category": "DOM",
            "originalTitle": "string",
            "interestedParties": [
                {
                    "nameNumber": 375001586,
                    "role": "CA"
                }
            ]
        },
        {
            "submissionId": 2,
            "agency": "128",
            "sourcedb": 128,
            "category": "DOM",
            "workcode": "R28995187",
            "originalTitle": "string",
            "interestedParties": [
                {
                    "nameNumber": 375001500,
                    "role": "CA"
                }
            ]
        }
    ],
    "updateSubmissions": [
        {
            "submissionId": 3,
            "preferredIswc": "T0302332075",
            "agency": "128",
            "sourcedb": 128,
            "workcode": "R28995186",
            "category": "DOM",
            "originalTitle": "string",
            "interestedParties": [
                {
                    "nameNumber": 375001586,
                    "role": "CA"
                }
            ]
        }
    ],
    "deleteSubmissions": [
        {
            "submissionId": 4,
            "preferredIswc": "T0302332075",
            "agency": "128",
            "sourcedb": 128,
            "workcode": "R28995186",
            "category": "DOM",
            "reasonCode": "Deletion Reason",
            "originalTitle": "string",
            "interestedParties": [
                {
                    "nameNumber": 375001586,
                    "role": "CA"
                }
            ]
        }
    ],
    "searchByIswcSubmissions": [
        {
            "submissionId": 5,
            "iswc": "T0302332086"
        }
    ],
    "searchByAgencyWorkCodeSubmissions": [
        {
            "submissionId": 6,
            "agency": "128",
            "sourcedb": 128,
            "workcode": "R28995187"
        }
    ],
    "mergeSubmissions": [
        {
            "submissionId": 7,
            "agency": "128",
            "sourcedb": 128,
            "workcode": "R28995187",
            "preferredIswc": "T0302332086",
            "mergeIswcs": [
                "T0302331403",
                "T0302331414"
            ]
        }
    ]
}


def test_schema_is_valid_formated():
    """Checks that schema itself is structurally valid according to the JSON Schema Draft-07 meta-schema."""
    try:
        Draft7Validator.check_schema(schema)
        is_schema_valid = True
    except SchemaError:
        is_schema_valid = False

    assert is_schema_valid




def test_validate_json_against_schema():
    """Validates JSON data against the schema with detailed error reporting."""
    try:
        validate(instance=valid_submissions_json, schema=schema)
        is_valid = True
    except ValidationError as e:
        print(f"Validation failed: {e.message} at path {list(e.path)} (schema path: {list(e.schema_path)})")
        is_valid = False
    except SchemaError as e:
        print(f"Schema error: {e.message}")
        is_valid = False
    
    assert is_valid, "JSON validation failed"


