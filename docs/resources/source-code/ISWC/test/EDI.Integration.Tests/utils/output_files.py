import os
import json
from datetime import datetime
from faker import Faker

fake = Faker()


def generate_json_allocation_in_file(submissions):
    batch_file = {
        "$schema": "./iswc_publisher_schema.json",
        "fileHeader": {
            "submittingAgency": "128",
            "submittingSourcedb": 128,
            "submittingPublisher": {
                "name": "SONY/ATV MUSIC PUBLISHING (UK) LIMITED",
                "nameNumber": 269137346,
                "role": "AM",
                "email": "info@spanishpoint.ie",
            },
            "fileCreationDateTime": datetime.utcnow().isoformat(),
            "receivingAgency": "315",
        },
        "addSubmissions": [x.get_json_allocation_submission() for x in submissions],
    }

    filename = f"ISWCP_{get_new_filename()}.json"
    return create_json_file(filename, batch_file)


def generate_json_edi_add_submissions_in_file(submissions, locallyAllocated=False):
    batch_file = {
        "$schema": "./iswc_schema.json",
        "fileHeader": {
            "submittingAgency": "128",
            "submittingSourcedb": 128,
            "fileCreationDateTime": datetime.utcnow().isoformat(),
            "receivingAgency": "315",
        },
        "addSubmissions": [x.submission for x in submissions],
    }
    filename = f"{get_new_filename()}.json" if not locallyAllocated else f"{get_locally_allocated_filename('128')}.json"
    return create_json_file(filename, batch_file)


def generate_json_edi_update_in_file(submissions):
    batch_file = {
        "$schema": "./iswc_schema.json",
        "fileHeader": {
            "submittingAgency": "128",
            "submittingSourcedb": 128,
            "fileCreationDateTime": datetime.utcnow().isoformat(),
            "receivingAgency": "315",
        },
        "updateSubmissions": [x.submission for x in submissions],
    }

    filename = f"{get_new_filename()}.json"
    return create_json_file(filename, batch_file)


def generate_json_edi_delete_in_file(submissions):
    batch_file = {
        "$schema": "./iswc_schema.json",
        "fileHeader": {
            "submittingAgency": "128",
            "submittingSourcedb": 128,
            "fileCreationDateTime": datetime.utcnow().isoformat(),
            "receivingAgency": "315",
        },
        "deleteSubmissions": [x.submission for x in submissions],
    }

    filename = f"{get_new_filename()}.json"
    return create_json_file(filename, batch_file)


def generate_json_edi_merge_in_file(submissions):
    batch_file = {
        "$schema": "./iswc_schema.json",
        "fileHeader": {
            "submittingAgency": "128",
            "submittingSourcedb": 128,
            "fileCreationDateTime": datetime.utcnow().isoformat(),
            "receivingAgency": "315",
        },
        "mergeSubmissions": [x.get_json_merge_submission() for x in submissions],
    }

    filename = f"{get_new_filename()}.json"
    return create_json_file(filename, batch_file)


def generate_json_edi_iswc_search_in_file(iswcs):
    batch_file = {
        "$schema": "./iswc_schema.json",
        "fileHeader": {
            "submittingAgency": "128",
            "submittingSourcedb": 128,
            "fileCreationDateTime": datetime.utcnow().isoformat(),
            "receivingAgency": "315",
        },
        "searchByIswcSubmissions": [
            {"submissionId": i, "iswc": iswcs[i]} for i in range(len(iswcs))
        ],
    }

    filename = f"{get_new_filename()}.json"
    return create_json_file(filename, batch_file)


def generate_json_edi_workcode_search_in_file(submissions):
    batch_file = {
        "$schema": "./iswc_schema.json",
        "fileHeader": {
            "submittingAgency": "128",
            "submittingSourcedb": 128,
            "fileCreationDateTime": datetime.utcnow().isoformat(),
            "receivingAgency": "315",
        },
        "searchByAgencyWorkCodeSubmissions": [
            x.get_json_workcode_search_submission() for x in submissions
        ],
    }

    filename = f"{get_new_filename()}.json"
    return create_json_file(filename, batch_file)

def generate_json_edi_thirdparty_iswc_search_in_file(iswcs, thirdPartyIdentifier):
    batch_file ={
        "$schema": "./iswc_publisher_schema.json",
        "fileHeader": {
            "submittingPartyId": thirdPartyIdentifier,
            "fileCreationDateTime": datetime.utcnow().isoformat()
        },
        "searchByIswcSubmissions": [
            {"submissionId": i, "iswc": iswcs[i]} for i in range(len(iswcs))
        ],
    }
    jsonFilename = f"{get_new_filename_thirdparty_format(thirdPartyIdentifier)}"
    return create_json_file(jsonFilename, batch_file)


def generate_json_edi_thirdparty_iswc_search_in_file_no_transaction(thirdPartyIdentifier):
    batch_file ={
        "$schema": "./iswc_publisher_schema.json",
        "fileHeader": {
            "submittingPartyId": thirdPartyIdentifier,
            "fileCreationDateTime": datetime.utcnow().isoformat()
        },
        "searchByIswcSubmissions": [{}]
    }
    jsonFilename = f"{get_new_filename_thirdparty_format(thirdPartyIdentifier)}"
    return create_json_file(jsonFilename, batch_file)


def generate_json_edi_thirdparty_iswc_search_in_file_invalid_header(iswcs, thirdPartyIdentifier):
    batch_file ={
        "$schema": "./iswc_publisher_schema.json",
        "fileHeader": {
            "fileCreationDateTime": datetime.utcnow().isoformat()
        },
        "searchByIswcSubmissions": [
            {"submissionId": i, "iswc": iswcs[i]} for i in range(len(iswcs))
        ],
    }
    jsonFilename = f"{get_new_filename_thirdparty_format(thirdPartyIdentifier)}"
    return create_json_file(jsonFilename, batch_file)


def generate_json_edi_thirdparty_agencyworkcode_search_in_file(workcodes, agency, thirdPartyIdentifier):
    batch_file ={
        "$schema": "./iswc_publisher_schema.json",
        "fileHeader": {
            "submittingPartyId": thirdPartyIdentifier,
            "fileCreationDateTime": datetime.utcnow().isoformat()
        },
        "searchByAgencyWorkCodeSubmissions": [
            {"submissionId": i, "agency": agency, "workcode": f"{workcodes[i]}"} for i in range(len(workcodes))
        ],
    }
    jsonFilename = f"{get_new_filename_thirdparty_format(thirdPartyIdentifier)}"
    return create_json_file(jsonFilename, batch_file)

def generate_json_edi_thirdparty_titlecontributor_search_in_file(titles, interestedParties, thirdPartyIdentifier):
    batch_file ={
        "$schema": "./iswc_publisher_schema.json",
        "fileHeader": {
            "submittingPartyId": thirdPartyIdentifier,
            "fileCreationDateTime": datetime.utcnow().isoformat()
        },
        "searchByTitleAndContributorsSubmissions": [
            {"submissionId": i, "titles": [{"title": f"{titles[i]}", "type" : "OT"}], "interestedParties": interestedParties[i]} for i in range(len(titles))
        ],
    }
    jsonFilename = f"{get_new_filename_thirdparty_format(thirdPartyIdentifier)}"
    return create_json_file(jsonFilename, batch_file)

def generate_json_edi_thirdparty_titlecontributor_invalid_header(titles, interestedParties, thirdPartyIdentifier):
    batch_file ={
        "$schema": "./iswc_publisher_schema.json",
        "fileHeader": {
            "fileCreationDateTime": datetime.utcnow().isoformat()
        },
        "searchByTitleAndContributorsSubmissions": [
            {"submissionId": i, "titles": [{"title": f"{titles[i]}", "type" : "OT"}], "interestedParties": interestedParties[i]} for i in range(len(titles))
        ],
    }
    jsonFilename = f"{get_new_filename_thirdparty_format(thirdPartyIdentifier)}"
    return create_json_file(jsonFilename, batch_file)

def generate_json_edi_thirdparty_titlecontributor_search_no_contributors(titles, thirdPartyIdentifier):
    batch_file ={
        "$schema": "./iswc_publisher_schema.json",
        "fileHeader": {
            "submittingPartyId": thirdPartyIdentifier,
            "fileCreationDateTime": datetime.utcnow().isoformat()
        },
        "searchByTitleAndContributorsSubmissions": [
            {"submissionId": i, "titles": [{"title": f"{titles[i]}", "type" : "OT"}]} for i in range(len(titles))
        ],
    }
    jsonFilename = f"{get_new_filename_thirdparty_format(thirdPartyIdentifier)}"
    return create_json_file(jsonFilename, batch_file)

def generate_json_edi_thirdparty_allsearchtypes_allvalid(iswcs, workcodes, agency, titles, interestedParties, thirdPartyIdentifier):
    batch_file ={
        "$schema": "./iswc_publisher_schema.json",
        "fileHeader": {
            "submittingPartyId": thirdPartyIdentifier,
            "fileCreationDateTime": datetime.utcnow().isoformat()
        },
        "searchByIswcSubmissions": [
            {"submissionId": i, "iswc": iswcs[i]} for i in range(len(iswcs))
        ],
        "searchByAgencyWorkCodeSubmissions": [
            {"submissionId": 100 + i, "agency": agency, "workcode": f"{workcodes[i]}"} for i in range(len(workcodes))
        ],
        "searchByTitleAndContributorsSubmissions": [
            {"submissionId": 200 + i, "titles": [{"title": f"{titles[i]}", "type" : "OT"}], "interestedParties": interestedParties[i]} for i in range(len(titles))
        ],
    }
    jsonFilename = f"{get_new_filename_thirdparty_format(thirdPartyIdentifier)}"
    return create_json_file(jsonFilename, batch_file)


def generate_tsv_allocation_in_file(submissions):
    tsv_submissions = [x.get_tsv_allocation_submission() for x in submissions]
    filename = f"{get_new_filename()}.txt"
    return create_tsv_file(filename, tsv_submissions)


def generate_json_resolution_in_file(submissions, submittingAgency="312"):
    batch_file = {
        "$schema": "./iswc_publisher_schema.json",
        "fileHeader": {
            "submittingAgency": submittingAgency,
            "submittingSourcedb": 315,
            "submittingPublisher": {
                "name": "SONY/ATV MUSIC PUBLISHING (UK) LIMITED",
                "nameNumber": 269137346,
                "role": "AM",
                "email": "info@spanishpoint.ie",
            },
            "fileCreationDateTime": datetime.utcnow().isoformat(),
            "receivingAgency": "315",
        },
        "findSubmissions": [x.get_json_resolution_submission() for x in submissions],
    }

    filename = f"ISWCP_{get_new_filename()}.json"
    return create_json_file(filename, batch_file)


def create_json_file(filename, content):
    os.makedirs("files", exist_ok=True)
    with open("files/" + filename, "w") as outfile:
        json.dump(content, outfile)
    return filename


def create_tsv_file(filename, submissions):
    os.makedirs("files", exist_ok=True)
    with open("files/" + filename, "w") as outfile:
        for x in submissions:
            outfile.write(f"{x}\n")
    return filename


def generate_tsv_resolution_in_file(submissions, submittingAgency="312"):
    tsv_submissions = [
        x.get_tsv_resolution_submission(submittingAgency) for x in submissions
    ]
    filename = f"{get_new_filename()}.txt"
    return create_tsv_file(filename, tsv_submissions)


def generate_invalid_json_file():
    filename = f"{get_new_filename()}.json"
    os.makedirs("files", exist_ok=True)
    with open("files/" + filename, "w") as outfile:
        outfile.write('{"test": {}')
    return filename


def get_new_filename():
    return fake.pystr(min_chars=20, max_chars=30)

def get_new_filename_thirdparty_format(third_party_identifier):
    current_datetime = datetime.utcnow()
    return "ISWC3_{}_315_{}_{}.json".format(
        current_datetime.strftime("%Y%m%d%H%M%S"), third_party_identifier, fake.pystr(min_chars=5, max_chars=10)
    )

def get_locally_allocated_filename(submitting_agency):
    current_datetime = datetime.utcnow()
    return "ISWC{}{}315.json_FromLocalRange".format(
        current_datetime.strftime("%Y%m%d%H%M%S"), submitting_agency
    )
