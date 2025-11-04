import pytest
import json
from utils.cosmos_service import CosmosService
from utils.sftp_client import SftpClient
from utils.databricks_client import DatabricksClient
from utils.utils import *
from utils.api import IswcApi
from submissions.submission import *
from utils.output_files import *
from submissions.interested_parties import AkkaIps


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
Submission is found by Resolution service.
In file is in JSON format.
Full IP data is returned regardless of fields included in the submission.
"""


def test_full_ip_data_returned_in_resolution_ack_json(sftp_client, cosmos_service, databricks_client):
    submission = EligibleSubmission(0)
    api = IswcApi()
    submission_res = api.add_submission(submission)
    iswc = json.loads(submission_res.content)[
        "verifiedSubmission"]["iswc"]
    del submission.submission['interestedParties'][0]['name']
    submission.submission['interestedParties'][1]['affiliation'] = '128'
    submission.submission['interestedParties'][2]['baseNumber'] = 'I-001221086-3'
    submission.submission['interestedParties'][2]['lastName'] = 'SCHWEPPE'
    filename = generate_json_resolution_in_file([submission])
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
        assert len(data['acknowledgements']) == 1
        assert data['acknowledgements'][0]['preferredIswc'] == iswc
        assert data['acknowledgements'][0][
            'transactionStatus'] == 'FullyAccepted', f'Transaction not accepted: {data["acknowledgements"][0]}'
        assert data['acknowledgements'][0]['interestedParties'][0]['affiliation'] == ImroIps.ips[0].affiliation
        assert data['acknowledgements'][0]['interestedParties'][0]['nameNumber'] == ImroIps.ips[0].name_number
        assert data['acknowledgements'][0]['interestedParties'][0]['name'] == ImroIps.ips[0].name
        assert data['acknowledgements'][0]['interestedParties'][0]['lastName'] == ImroIps.ips[0].last_name
        assert data['acknowledgements'][0]['interestedParties'][1]['affiliation'] == ImroIps.ips[1].affiliation
        assert data['acknowledgements'][0]['interestedParties'][1]['nameNumber'] == ImroIps.ips[1].name_number
        assert data['acknowledgements'][0]['interestedParties'][1]['name'] == ImroIps.ips[1].name
        assert data['acknowledgements'][0]['interestedParties'][1]['lastName'] == ImroIps.ips[1].last_name
        assert data['acknowledgements'][0]['interestedParties'][2]['affiliation'] == ImroIps.ips[2].affiliation
        assert data['acknowledgements'][0]['interestedParties'][2]['nameNumber'] == ImroIps.ips[2].name_number
        assert data['acknowledgements'][0]['interestedParties'][2]['name'] == ImroIps.ips[2].name
        assert data['acknowledgements'][0]['interestedParties'][2]['lastName'] == ImroIps.ips[2].last_name


"""
Submissions are accepted by the Allocation Service.
In file is in JSON format.
Full IP data is returned regardless of fields included in the submission.
Incorrect affiliation is corrected in Ack.
"""


def test_full_ip_data_returned_in_allocation_ack_json(sftp_client, cosmos_service, databricks_client):
    submission = EligibleSubmission(0)
    del submission.submission['interestedParties'][0]['name']
    submission.submission['interestedParties'][1]['affiliation'] = '128'
    submission.submission['interestedParties'][2]['affiliation'] = '052'
    submission.submission['interestedParties'][2]['baseNumber'] = ImroIps.ips[2].base_number
    submission.submission['interestedParties'][2]['lastName'] = ImroIps.ips[2].last_name
    filename = generate_json_allocation_in_file([submission])
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
        assert len(data['acknowledgements']) == 1
        assert data['acknowledgements'][0][
            'transactionStatus'] == 'FullyAccepted', f'Transaction not accepted: {data["acknowledgements"][0]}'
        assert data['acknowledgements'][0]['interestedParties'][0]['affiliation'] == ImroIps.ips[0].affiliation
        assert data['acknowledgements'][0]['interestedParties'][0]['nameNumber'] == ImroIps.ips[0].name_number
        assert data['acknowledgements'][0]['interestedParties'][0]['name'] == ImroIps.ips[0].name
        assert data['acknowledgements'][0]['interestedParties'][0]['lastName'] == ImroIps.ips[0].last_name
        assert data['acknowledgements'][0]['interestedParties'][1]['affiliation'] == ImroIps.ips[1].affiliation
        assert data['acknowledgements'][0]['interestedParties'][1]['nameNumber'] == ImroIps.ips[1].name_number
        assert data['acknowledgements'][0]['interestedParties'][1]['name'] == ImroIps.ips[1].name
        assert data['acknowledgements'][0]['interestedParties'][1]['lastName'] == ImroIps.ips[1].last_name
        assert data['acknowledgements'][0]['interestedParties'][2]['affiliation'] == ImroIps.ips[2].affiliation
        assert data['acknowledgements'][0]['interestedParties'][2]['nameNumber'] == ImroIps.ips[2].name_number
        assert data['acknowledgements'][0]['interestedParties'][2]['name'] == ImroIps.ips[2].name
        assert data['acknowledgements'][0]['interestedParties'][2]['lastName'] == ImroIps.ips[2].last_name
        assert data['acknowledgements'][0]['interestedParties'][3]['affiliation'] == SonyPublisherIp.affiliation
        assert data['acknowledgements'][0]['interestedParties'][3]['nameNumber'] == SonyPublisherIp.name_number
        assert data['acknowledgements'][0]['interestedParties'][3]['name'].strip(
        ) == SonyPublisherIp.name
        assert data['acknowledgements'][0]['interestedParties'][3][
            'lastName'] == SonyPublisherIp.name


"""
Submissions are rejected by the Allocation Service.
In file is in JSON format.
Publisher workcode is returned
"""


def test_publisher_workcode_returned_after_allocation_rejection_json(sftp_client, cosmos_service, databricks_client):
    submission = EligibleSubmission(0)
    submission.submission['interestedParties'] = [
        {'name': x.name, 'nameNumber': x.name_number, 'role': x.role} for x in AkkaIps.ips]
    submission.submission['interestedParties'][0]['role'] = 'E'
    filename = generate_json_allocation_in_file([submission])
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
        assert len(data['acknowledgements']) == 1
        assert data['acknowledgements'][0][
            'transactionStatus'] == 'Rejected', f'Transaction not rejected: {data["acknowledgements"][0]}'
        assert data['acknowledgements'][0]['workcode'] == submission.submission['workcode']


"""
Submission is rejected by Resolution service.
In file is in JSON format.
Publisher workcode is included in ACK.
"""


def test_publisher_workcode_returned_after_resolution_rejection_json(sftp_client, cosmos_service, databricks_client):
    submission = EligibleSubmission(0)
    filename = generate_json_resolution_in_file([submission], '128')
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
        assert len(data['acknowledgements']) == 1
        assert data['acknowledgements'][0][
            'transactionStatus'] == 'Rejected', f'Transaction not rejected: {data["acknowledgements"][0]}'
        assert data['acknowledgements'][0]['workcode'] == submission.submission['workcode']


"""
Submissions are rejected by the Allocation Service.
In file is in TSV format.
Publisher workcode is returned
"""


def test_publisher_workcode_returned_after_allocation_rejection_tsv(sftp_client, cosmos_service, databricks_client):
    submission = EligibleSubmission(0)
    submission.submission['interestedParties'] = [
        {'name': x.name, 'nameNumber': x.name_number, 'role': x.role} for x in AkkaIps.ips]
    submission.submission['interestedParties'][0]['role'] = 'E'
    filename = generate_tsv_allocation_in_file([submission])
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
        for x in out_file.readlines():
            assert 'Rejected' in x
            assert submission.submission['workcode'] in x


"""
Submission is rejected by Resolution service.
In file is in TSV format.
Publisher workcode is included in ACK.
"""


def test_publisher_workcode_returned_after_resolution_rejection_tsv(sftp_client, cosmos_service, databricks_client):
    submission = EligibleSubmission(0)
    filename = generate_tsv_resolution_in_file([submission], '128')
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
        for x in out_file.readlines():
            assert 'Rejected' in x
            assert submission.submission['workcode'] in x
