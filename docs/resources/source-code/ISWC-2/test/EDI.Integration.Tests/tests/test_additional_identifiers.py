from submissions.interested_parties import SonyPublisherIp
import pytest
import os
import json
from utils.cosmos_service import CosmosService
from utils.sftp_client import SftpClient
from utils.databricks_client import DatabricksClient
from submissions.submission import EligibleSubmission, get_new_workcode
from utils.output_files import *
from utils.api import IswcApi


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
Submission containes performers with a last name and first name.
In file is in JSON format.
"""


def test_isrcs_in_allocation_submission_json(sftp_client, cosmos_service, databricks_client):
    sub = EligibleSubmission(0)
    sub.add_isrc(get_new_workcode())
    sub.add_isrc(get_new_workcode())
    filename = generate_json_allocation_in_file([sub])

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


def test_isrcs_in_allocation_submission_tsv(sftp_client, cosmos_service, databricks_client):
    sub = EligibleSubmission(0)
    sub.add_isrc(get_new_workcode())
    sub.add_isrc(get_new_workcode())
    filename = generate_tsv_allocation_in_file([sub])

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


def test_isrcs_in_resolution_submission_json(sftp_client, cosmos_service, databricks_client):
    sub = EligibleSubmission(0)
    sub.add_isrc(get_new_workcode())
    sub.add_isrc(get_new_workcode())
    sub.add_publisher_identifier(
        SonyPublisherIp.name_number, get_new_workcode())
    api = IswcApi()
    res = api.add_submission(sub)
    iswc = json.loads(res.content)[
        "verifiedSubmission"]["iswc"]
    del sub.submission['additionalIdentifiers']['publisherIdentifiers']
    filename = generate_json_resolution_in_file(
        [sub])

    remote_filepath = sftp_client.upload_to_publisher_resolution_folder(
        '312', 'SA', filename)

    run_id = databricks_client.start_ack_json_job(remote_filepath)
    run_details = databricks_client.wait_for_run(run_id)

    assert run_details['state']['life_cycle_state'] == 'TERMINATED', 'Job did not finish within time limit'
    assert run_details['state'][
        'result_state'] == 'SUCCESS', f'Job failed: {run_details["run_page_url"]}'

    ack_filename = cosmos_service.get_ack_filename(remote_filepath)
    ack_file = sftp_client.get_ack_file_from_publisher_resolution_directory(
        ack_filename, '312', 'SA')
    with open(ack_file) as out_file:
        data = json.load(out_file)
        assert data['acknowledgements'][0][
            'transactionStatus'] == 'FullyAccepted', f'Transaction not accepted: {data["acknowledgements"][0]}'
        assert data['acknowledgements'][0]['preferredIswc'] == iswc


def test_isrcs_in_resolution_submission_tsv(sftp_client, cosmos_service, databricks_client):
    sub = EligibleSubmission(0)
    sub.add_isrc(get_new_workcode())
    sub.add_isrc(get_new_workcode())
    sub.add_publisher_identifier(
        SonyPublisherIp.name_number, get_new_workcode())
    api = IswcApi()
    res = api.add_submission(sub)
    iswc = json.loads(res.content)[
        "verifiedSubmission"]["iswc"]
    del sub.submission['additionalIdentifiers']['publisherIdentifiers']
    filename = generate_tsv_resolution_in_file(
        [sub])

    remote_filepath = sftp_client.upload_to_publisher_resolution_folder(
        '312', 'SA', filename)

    run_id = databricks_client.start_ack_tsv_job(remote_filepath)
    run_details = databricks_client.wait_for_run(run_id)

    assert run_details['state']['life_cycle_state'] == 'TERMINATED', 'Job did not finish within time limit'
    assert run_details['state'][
        'result_state'] == 'SUCCESS', f'Job failed: {run_details["run_page_url"]}'

    ack_filename = cosmos_service.get_ack_filename(remote_filepath)
    ack_file = sftp_client.get_ack_file_from_publisher_resolution_directory(
        ack_filename, '312', 'SA')
    with open(ack_file) as out_file:
        ack = out_file.readline()
        assert 'FullyAccepted' in ack
        assert iswc in ack
