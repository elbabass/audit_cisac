from submissions.interested_parties import SonyPublisherIp
import pytest
import json
from utils.cosmos_service import CosmosService
from utils.sftp_client import SftpClient
from utils.databricks_client import DatabricksClient
from utils.output_files import generate_json_allocation_in_file, generate_tsv_allocation_in_file
from submissions.submission import EligibleSubmission, IneligibleSubmission


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
Suibmitting Publisher is not included in IP's as publisher role is in submission IP's already.
IP's are still eligible without Submitting Publisher.
In file is in JSON format.
"""


def test_submitting_publisher_excluded_if_pub_role_in_ip_eligible_json(sftp_client, cosmos_service, databricks_client):
    sub = EligibleSubmission(0)
    sub.submission['interestedParties'][0]['role'] = 'E'
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
            assert all(
                ip['nameNumber'] != SonyPublisherIp.name_number for ip in ack['interestedParties']
            )


"""
Submission is rejected by the Allocation Service.
Suibmitting Publisher is not included in IP's as publisher role is in submission IP's already.
IP's are not eligible without the Submitting Publisher.
In file is in JSON format.
"""


def test_submitting_publisher_excluded_if_pub_role_in_ip_ineligible_json(sftp_client, cosmos_service, databricks_client):
    sub = IneligibleSubmission(0)
    sub.submission['interestedParties'].append({
        "name": sub.submission['interestedParties'][0]['name'],
        "nameNumber": sub.submission['interestedParties'][0]['nameNumber'],
        "role": "E"
    })
    filename = generate_json_allocation_in_file([sub])

    remote_filepath = sftp_client.upload('035', filename)

    run_id = databricks_client.start_ack_json_job(remote_filepath)
    run_details = databricks_client.wait_for_run(run_id)

    assert run_details['state']['life_cycle_state'] == 'TERMINATED', 'Job did not finish within time limit'
    assert run_details['state'][
        'result_state'] == 'SUCCESS', f'Job failed: {run_details["run_page_url"]}'

    ack_filename = cosmos_service.get_ack_filename(remote_filepath)
    ack_file = sftp_client.get_ack_file(ack_filename, '035')
    with open(ack_file) as out_file:
        data = json.load(out_file)
        for ack in data['acknowledgements']:
            assert ack[
                'transactionStatus'] == 'Rejected', f'Transaction not rejected: {ack}'


"""
Submission is rejected by the Allocation Service.
Suibmitting Publisher is not included in IP's as publisher role is in submission IP's already.
IP's are not eligible without the Submitting Publisher.
In file is in TSV format.
"""


def test_submitting_publisher_excluded_if_pub_role_in_ip_ineligible_tsv(sftp_client, cosmos_service, databricks_client):
    sub = IneligibleSubmission(0)
    sub.submission['interestedParties'].append({
        "name": sub.submission['interestedParties'][0]['name'],
        "nameNumber": sub.submission['interestedParties'][0]['nameNumber'],
        "role": "E"
    })
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
        assert 'Rejected' in out_file.readline()
