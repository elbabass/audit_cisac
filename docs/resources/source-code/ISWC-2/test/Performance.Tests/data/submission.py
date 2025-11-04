from utils.utils import get_new_workcode, get_new_title


def new_iswc_submission():
    return {
        "agency": "003",
        "sourcedb": 300,
        "workcode": get_new_workcode(),
        "category": "DOM",
        "disambiguation": False,
        "interestedParties": [
            {
                "BaseNumber": "I-000240590-3",
                "LastName": "PAPAVRAMIDIS",
                "NameNumber": 23429210,
                "Role": 7
            },
            {
                "BaseNumber": "I-000122272-4",
                "LastName": "KANAKIS",
                "NameNumber": 255096361,
                "Role": 7
            },
            {
                "BaseNumber": "I-000272973-7",
                "LastName": "MALIARAS",
                "NameNumber": 19265386,
                "Role": 7
            },
            {
                "BaseNumber": "I-000118019-2",
                "LastName": "VOSTANJOGLOU",
                "NameNumber": 255128672,
                "Role": 7
            },
            {
                "BaseNumber": "I-000203756-9",
                "LastName": "KONTAKIS",
                "NameNumber": 272889810,
                "Role": 7
            },
            {
                "BaseNumber": "I-000203750-3",
                "LastName": "STATHOULIS",
                "NameNumber": 226045004,
                "Role": 7
            }
        ],
        "originalTitle": get_new_title()
    }
