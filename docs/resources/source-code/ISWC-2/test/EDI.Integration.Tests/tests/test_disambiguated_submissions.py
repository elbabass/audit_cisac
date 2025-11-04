import pytest
import os
import json
from utils.sftp_client import SftpClient
from utils.databricks_client import DatabricksClient
from utils.api import IswcApi
from utils.output_files import generate_json_allocation_in_file, generate_tsv_allocation_in_file
from submissions.submission import EligibleSubmission
from utils.cosmos_service import CosmosService


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
Submission is accepted by the Allocation Service.
Submission is disambiguated from two works.
In file is in JSON format.
"""


def test_add_disambiguated_submission_json(sftp_client, cosmos_service, databricks_client):
    api = IswcApi()
    submission_one = EligibleSubmission(0)
    submission_two = EligibleSubmission(0)
    res_one = api.add_submission(submission_one)
    res_two = api.add_submission(submission_two)
    iswc_one = json.loads(res_one.content)["verifiedSubmission"]["iswc"]
    iswc_two = json.loads(res_two.content)["verifiedSubmission"]["iswc"]
    disambiguated_submission = EligibleSubmission(0)
    disambiguated_submission.add_disambiguated_works([iswc_one, iswc_two])
    filename = generate_json_allocation_in_file([disambiguated_submission])
    remote_filepath = sftp_client.upload_to_publisher_allocation_folder(
        '128', 'SA', filename)

    run_id = databricks_client.start_ack_json_job(remote_filepath)
    run_details = databricks_client.wait_for_run(run_id)

    assert run_details['state']['life_cycle_state'] == 'TERMINATED', 'Job did not finish within time limit'
    assert run_details['state'][
        'result_state'] == 'SUCCESS', f'Job failed: {run_details["run_page_url"]}'

    ack_filename = cosmos_service.get_ack_filename(remote_filepath)
    ack_file = sftp_client.get_ack_file_from_publisher_allocation_directory(
        ack_filename, '128', 'SA')
    with open(ack_file) as out_file:
        data = json.load(out_file)
        for ack in data['acknowledgements']:
            assert ack[
                'transactionStatus'] == 'FullyAccepted', f'Transaction not accepted: {ack}'


"""
Submission is accepted by the Allocation Service.
Submission is disambiguated from two works.
In file is in TSV format.
"""


def test_add_disambiguated_submission_tsv(sftp_client, cosmos_service, databricks_client):
    api = IswcApi()
    submission_one = EligibleSubmission(0)
    submission_two = EligibleSubmission(0)
    submission_one_res = api.add_submission(submission_one)
    submission_two_res = api.add_submission(submission_two)
    iswc_one = json.loads(submission_one_res.content)[
        "verifiedSubmission"]["iswc"]
    iswc_two = json.loads(submission_two_res.content)[
        "verifiedSubmission"]["iswc"]
    disambiguated_submission = EligibleSubmission(0)
    disambiguated_submission.add_disambiguated_works([iswc_one, iswc_two])
    filename = generate_tsv_allocation_in_file([disambiguated_submission])

    remote_filepath = sftp_client.upload_to_publisher_allocation_folder(
        '128', 'SA', filename)

    run_id = databricks_client.start_ack_tsv_job(remote_filepath)
    run_details = databricks_client.wait_for_run(run_id)

    assert run_details['state']['life_cycle_state'] == 'TERMINATED', 'Job did not finish within time limit'
    assert run_details['state'][
        'result_state'] == 'SUCCESS', f'Job failed: {run_details["run_page_url"]}'

    ack_filename = cosmos_service.get_ack_filename(remote_filepath)
    ack_file = sftp_client.get_ack_file_from_publisher_allocation_directory(
        ack_filename, '128', 'SA')
    with open(ack_file) as out_file:
        assert 'FullyAccepted' in out_file.readline()


"""
Submission is accepted by the Allocation Service.
Submission is disambiguated and has a value for Bvltr.
In file is in JSON format.
"""


def test_add_submission_with_bvltr_json(sftp_client, cosmos_service, databricks_client):
    api = IswcApi()
    submission_one = EligibleSubmission(0)
    res_one = api.add_submission(submission_one)
    iswc_one = json.loads(res_one.content)["verifiedSubmission"]["iswc"]
    disambiguated_submission = EligibleSubmission(0)
    disambiguated_submission.add_disambiguated_works([iswc_one])
    disambiguated_submission.add_bvltr("Background")
    filename = generate_json_allocation_in_file([disambiguated_submission])
    remote_filepath = sftp_client.upload_to_publisher_allocation_folder(
        '128', 'SA', filename)

    run_id = databricks_client.start_ack_json_job(remote_filepath)
    run_details = databricks_client.wait_for_run(run_id)

    assert run_details['state']['life_cycle_state'] == 'TERMINATED', 'Job did not finish within time limit'
    assert run_details['state'][
        'result_state'] == 'SUCCESS', f'Job failed: {run_details["run_page_url"]}'

    ack_filename = cosmos_service.get_ack_filename(remote_filepath)
    ack_file = sftp_client.get_ack_file_from_publisher_allocation_directory(
        ack_filename, '128', 'SA')
    with open(ack_file) as out_file:
        data = json.load(out_file)
        for ack in data['acknowledgements']:
            assert ack[
                'transactionStatus'] == 'FullyAccepted', f'Transaction not accepted: {ack}'


"""
Submission is accepted by the Allocation Service.
Submission is disambiguated and has a value for Bvltr.
In file is in TSV format.
"""


def test_add_submission_with_bvltr_tsv(sftp_client, cosmos_service, databricks_client):
    api = IswcApi()
    submission_one = EligibleSubmission(0)
    res_one = api.add_submission(submission_one)
    iswc_one = json.loads(res_one.content)["verifiedSubmission"]["iswc"]
    disambiguated_submission = EligibleSubmission(0)
    disambiguated_submission.add_disambiguated_works([iswc_one])
    disambiguated_submission.add_bvltr("Background")
    filename = generate_tsv_allocation_in_file([disambiguated_submission])
    remote_filepath = sftp_client.upload_to_publisher_allocation_folder(
        '128', 'SA', filename)

    run_id = databricks_client.start_ack_tsv_job(remote_filepath)
    run_details = databricks_client.wait_for_run(run_id)

    assert run_details['state']['life_cycle_state'] == 'TERMINATED', 'Job did not finish within time limit'
    assert run_details['state'][
        'result_state'] == 'SUCCESS', f'Job failed: {run_details["run_page_url"]}'

    ack_filename = cosmos_service.get_ack_filename(remote_filepath)
    ack_file = sftp_client.get_ack_file_from_publisher_allocation_directory(
        ack_filename, '128', 'SA')
    with open(ack_file) as out_file:
        assert 'FullyAccepted' in out_file.readline()
