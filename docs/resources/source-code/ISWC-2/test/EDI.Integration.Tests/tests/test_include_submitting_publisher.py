from submissions.interested_parties import SonyPublisherIp
import pytest
import json
from utils.cosmos_service import CosmosService
from utils.sftp_client import SftpClient
from utils.databricks_client import DatabricksClient
from utils.output_files import generate_json_allocation_in_file, generate_tsv_allocation_in_file
from submissions.submission import IneligibleSubmission


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
Submitting Publisher is included in IP's.
IP's are not eligible without the Submitting Publisher.
In file is in JSON format.
"""


def test_submitting_publisher_is_eligible_ip_is_not_json(sftp_client, cosmos_service, databricks_client):
    sub = IneligibleSubmission(0)
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
            assert any(
                ip['lastName'] == SonyPublisherIp.name and ip['role'] == SonyPublisherIp.role and ip[
                    'nameNumber'] == SonyPublisherIp.name_number for ip in ack['interestedParties'])


"""
Submission is accepted by the Allocation Service.
Submitting Publisher is included in IP's.
IP's are not eligible without the Submitting Publisher.
In file is in TSV format.
"""


def test_submitting_publisher_is_eligible_ip_is_not_tsv(sftp_client, cosmos_service, databricks_client):
    sub = IneligibleSubmission(0)
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
