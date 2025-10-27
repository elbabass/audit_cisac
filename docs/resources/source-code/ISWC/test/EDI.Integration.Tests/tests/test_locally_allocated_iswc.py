import json
import pytest
from submissions.interested_parties import SonyPublisherIp
from submissions.submission import *
from utils.api import IswcApi
from utils.cosmos_service import CosmosService
from utils.databricks_client import DatabricksClient
from utils.output_files import *
from utils.sftp_client import SftpClient
from utils.utils import *
import time


@pytest.fixture
def sftp_client():
    sftp_client = SftpClient()
    yield sftp_client
    sftp_client.close_client()


@pytest.fixture
def cosmos_service():
    cosmos_service = CosmosService()
    yield cosmos_service


@pytest.fixture
def databricks_client():
    databricks_client = DatabricksClient()
    yield databricks_client


"""
Filename follows FromLocalRange naming convention.
locallyAllocatedIswc already exists.
"""


def test_locally_allocated_iswc_exists_json(
    sftp_client, cosmos_service, databricks_client
):
    api = IswcApi()
    submission_one = EligibleSubmission(0)
    submission_one_res = api.add_submission(submission_one)
    submission_one.submission["preferredIswc"] = json.loads(submission_one_res.content)[
        "verifiedSubmission"]["iswc"]
    alloc_sub = EligibleSubmission(0)
    alloc_sub.submission["locallyAllocatedIswc"] = submission_one.submission[
        "preferredIswc"
    ]
    filename = generate_json_edi_add_submissions_in_file([alloc_sub], True)
    remote_filepath = sftp_client.upload(
        "128", filename
    )

    run_id = databricks_client.start_ack_json_job(remote_filepath)
    run_details = databricks_client.wait_for_run(run_id)

    assert (
        run_details["state"]["life_cycle_state"] == "TERMINATED"
    ), "Job did not finish within time limit"
    assert (
        run_details["state"]["result_state"] == "SUCCESS"
    ), f'Job failed: {run_details["run_page_url"]}'

    ack_filename = cosmos_service.get_ack_filename(remote_filepath)
    ack_file = sftp_client.get_ack_file(
        ack_filename, "128"
    )
    with open(ack_file) as out_file:
        data = json.load(out_file)
        assert data["acknowledgements"][0]["transactionStatus"] == "Rejected"
        assert data["acknowledgements"][0]["errorMessages"]["code"] == "169"


"""
Filename does not follow FromLocalRange naming convention.
locallyAllocatedIswc already exists.
"""


def test_locally_allocated_iswc_exists_filename_incorrect_json(
    sftp_client, cosmos_service, databricks_client
):
    api = IswcApi()
    submission_one = EligibleSubmission(0)
    submission_one_res = api.add_submission(submission_one)
    submission_one.submission["preferredIswc"] = json.loads(submission_one_res.content)[
        "verifiedSubmission"]["iswc"]
    alloc_sub = EligibleSubmission(0)
    alloc_sub.submission["locallyAllocatedIswc"] = submission_one.submission["preferredIswc"]
    filename = generate_json_edi_add_submissions_in_file([alloc_sub])
    remote_filepath = sftp_client.upload(
        "128", filename
    )

    run_id = databricks_client.start_ack_json_job(remote_filepath)
    run_details = databricks_client.wait_for_run(run_id)

    assert (
        run_details["state"]["life_cycle_state"] == "TERMINATED"
    ), "Job did not finish within time limit"
    assert (
        run_details["state"]["result_state"] == "SUCCESS"
    ), f'Job failed: {run_details["run_page_url"]}'

    ack_filename = cosmos_service.get_ack_filename(remote_filepath)
    ack_file = sftp_client.get_ack_file(
        ack_filename, "128"
    )
    with open(ack_file) as out_file:
        data = json.load(out_file)
        assert data["acknowledgements"][0]["transactionStatus"] == "FullyAccepted"
        assert (
            data["acknowledgements"][0]["preferredIswc"] != submission_one.submission["preferredIswc"]
        )


"""
locallyAllocatedIswc has been deleted and is valid for use.
Submission matches an existing ISWC.
Submission is added to existing ISWC and the locallyAllocatedIswc is added as the archivedIswc.
"""


def test_locally_allocated_iswc_add_as_archived(
    sftp_client, cosmos_service, databricks_client
):
    api = IswcApi()
    submission_one = EligibleSubmission(0)
    submission_one_res = api.add_submission(submission_one)
    submission_one.submission["preferredIswc"] = json.loads(submission_one_res.content)[
        "verifiedSubmission"]["iswc"]
    time.sleep(5)
    api.delete_submission(submission_one.submission)
    submission_two = EligibleSubmission(0)
    submission_two_res = api.add_submission(submission_two)
    matching_iswc = json.loads(submission_two_res.content)["verifiedSubmission"]["iswc"]
    submission_two.submission["workcode"] = get_new_workcode()
    submission_two.submission["locallyAllocatedIswc"] = submission_one.submission["preferredIswc"]
    filename = generate_json_edi_add_submissions_in_file([submission_two], True)
    remote_filepath = sftp_client.upload(
        "128", filename
    )

    run_id = databricks_client.start_ack_json_job(remote_filepath)
    run_details = databricks_client.wait_for_run(run_id)

    assert (
        run_details["state"]["life_cycle_state"] == "TERMINATED"
    ), "Job did not finish within time limit"
    assert (
        run_details["state"]["result_state"] == "SUCCESS"
    ), f'Job failed: {run_details["run_page_url"]}'

    ack_filename = cosmos_service.get_ack_filename(remote_filepath)
    ack_file = sftp_client.get_ack_file(
        ack_filename, "128"
    )
    with open(ack_file) as out_file:
        data = json.load(out_file)
        assert data["acknowledgements"][0]["transactionStatus"] == "FullyAccepted"
        assert data["acknowledgements"][0]["preferredIswc"] == matching_iswc
        assert data["acknowledgements"][0]["archivedIswc"] == submission_two.submission["locallyAllocatedIswc"]


"""
Same locallyAllocatedIswc is used in multiple submissions.
"""


def test_locally_allocated_iswc_duplicate_json(
    sftp_client, cosmos_service, databricks_client
):
    alloc_sub = EligibleSubmission(0)
    alloc_sub.submission["locallyAllocatedIswc"] = 'T9802848299'
    duplicate_submission = EligibleSubmission(1)
    duplicate_submission.submission["locallyAllocatedIswc"] = 'T9802848299'
    filename = generate_json_edi_add_submissions_in_file([alloc_sub, duplicate_submission], True)
    remote_filepath = sftp_client.upload(
        "128", filename
    )

    run_id = databricks_client.start_ack_json_job(remote_filepath)
    run_details = databricks_client.wait_for_run(run_id)

    assert (
        run_details["state"]["life_cycle_state"] == "TERMINATED"
    ), "Job did not finish within time limit"
    assert (
        run_details["state"]["result_state"] == "SUCCESS"
    ), f'Job failed: {run_details["run_page_url"]}'

    ack_filename = cosmos_service.get_ack_filename(remote_filepath)
    ack_file = sftp_client.get_ack_file(
        ack_filename, "128"
    )
    with open(ack_file) as out_file:
        data = json.load(out_file)
        assert data["fileLevelError"]['errorMessage'] == 'There are duplicate locallyAllocatedIswc values in the file.'
