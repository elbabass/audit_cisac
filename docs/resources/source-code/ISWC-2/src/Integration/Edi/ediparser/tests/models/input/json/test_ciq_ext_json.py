from ediparser.parser.models.input.json.ciq_ext import CIQEXT


def test_ciq_ext_get_context_submission():
    transaction = {
        "submissionId": 1,
        "nameNumber": 589238793,
        "firstName": "Andrew",
        "lastName": "Hozier byrne",
        "dateOfBirth": "1990-03-17",
        "age": 30,
        "ageTolerance": 5,
        "works": [{
                "titles": [{
                        "title": "Take me to church",
                        "type": "OT"
                    }
                ],
                "iswc": "T12345"
            }
        ],
        "affiliations": [{
                "affiliation": "021"
            }, {
                "affiliation": "052"
            }
        ]
    }

    file_header = {
        "submittingAgency": "128",
        "submittingSourcedb": 128,
        "fileCreationDateTime": "2019-10-01T18:25:43.511Z",
        "receivingAgency": "315"
    },

    expected_result_body = {
        "nameNumber": 589238793,
        "firstName": "Andrew",
        "lastName": "Hozier byrne",
        "dateOfBirth": "1990-03-17",
        "age": 30,
        "ageTolerance": 5,
        "works": [{
                "titles": [{
                        "title": "Take me to church",
                        "type": "OT"
                    }
                ],
                "iswc": "T12345"
            }
        ],
        "affiliations": [{
                "affiliation": "021"
            }, {
                "affiliation": "052"
            }
        ]
    }

    expected_result_submisison_id = 1

    ciqext = CIQEXT(transaction, file_header)

    assert expected_result_body == ciqext.get_context_submission().get('body')
    assert expected_result_submisison_id == ciqext.get_context_submission().get('submissionId')