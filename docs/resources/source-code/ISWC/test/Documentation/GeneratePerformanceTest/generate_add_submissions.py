import random
import string
import json
from datetime import datetime
import shutil
import os
import secrets


number_of_files = 20
subs_per_file = 12500
workcode_prefix = 'PF'  # Two characters

interested_parties = [
    {
        'nameNumber': 36303314,
        'role': 'CA'
    },
    {
        'nameNumber': 159837032,
        'role': 'CA'
    },
    {
        'nameNumber': 46932859,
        'role': 'CA'
    },
    {
        'nameNumber': 254891146,
        'role': 'CA'
    },
    {
        'nameNumber': 254891342,
        'role': 'CA'
    },
    {
        'nameNumber': 122395984,
        'role': 'CA'
    },
    {
        'nameNumber': 460926841,
        'role': 'CA'
    },
    {
        'nameNumber': 264188549,
        'role': 'CA'
    },
    {
        'nameNumber': 279348322,
        'role': 'CA'
    }
]


def get_string(chars):
    alphabet = string.ascii_letters + string.digits
    random_string = ''.join(secrets.choice(alphabet) for i in range(chars))
    return random_string.upper()


def get_sub(id):
    return {
        "agency": "128",
        "sourcedb": 128,
        "workcode": f'{workcode_prefix}-{get_string(16)}',
        "category": "DOM",
        "disambiguation": False,
        "interestedParties": [{'nameNumber': x['nameNumber'], 'role': x['role']} for x in random.sample(interested_parties, 2)],
        "originalTitle": f"{get_string(10)} {get_string(10)} {get_string(10)}",
        "submissionId": id
    }


shutil.rmtree('.\\add', ignore_errors=True)
os.makedirs('.\\add')
for i in range(number_of_files):
    batch_file = {
        "$schema": "./iswc_schema.json",
        "fileHeader": {
            "submittingAgency": "128",
            "submittingSourcedb": 128,
            "fileCreationDateTime": datetime.utcnow().isoformat(),
            "receivingAgency": "315"
        }
    }
    batch_file['addSubmissions'] = [get_sub(x) for x in range(subs_per_file)]
    file_name = 'perf_test_' + str(i) + '.json'

    with open('.\\add\\' + file_name, 'w') as outfile:
        json.dump(batch_file, outfile)
