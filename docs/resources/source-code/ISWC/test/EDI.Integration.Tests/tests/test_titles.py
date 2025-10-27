import pytest
import json
from utils.sftp_client import SftpClient
from utils.databricks_client import DatabricksClient
from utils.api import IswcApi
from utils.output_files import generate_json_allocation_in_file, generate_json_resolution_in_file, generate_tsv_allocation_in_file, generate_tsv_resolution_in_file
from submissions.submission import EligibleSubmission, get_new_title
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
Submission contains other titles along with the OT.
JSON Format.
"""


def test_add_submission_other_title_given_json(sftp_client, cosmos_service, databricks_client):
    submission = EligibleSubmission(0)
    other_title = get_new_title()
    submission.add_other_title(other_title, 'AT')
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
        ack = data['acknowledgements'][0]
        assert ack[
            'transactionStatus'] == 'FullyAccepted', f'Transaction not accepted: {ack}'
        assert other_title in [x['title'] for x in ack['otherTitles']]


"""
Submission is accepted by the Allocation Service.
Submission contains other titles along with the OT.
TSV Format.
"""


def test_add_submission_other_title_given_tsv(sftp_client, cosmos_service, databricks_client):
    submission = EligibleSubmission(0)
    submission.add_other_title(get_new_title(), 'AT')
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
        assert 'FullyAccepted' in out_file.readline()


"""
Submission is found by the Resolution Service.
Submission contains other titles along with the OT.
ACK contains the other titles as well as the OT.
JSON Format.
"""


def test_find_submission_other_title_given_json(sftp_client, cosmos_service, databricks_client):
    api = IswcApi()
    submission = EligibleSubmission(0)
    other_title_one = get_new_title()
    submission.add_other_title(other_title_one, 'AT')
    other_title_two = get_new_title()
    submission.add_other_title(other_title_two, 'AT')
    submission_res = api.add_submission(submission)
    iswc = json.loads(submission_res.content)[
        "verifiedSubmission"]["iswc"]
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
        ack = data['acknowledgements'][0]
        assert ack[
            'transactionStatus'] == 'FullyAccepted', f'Transaction not accepted: {ack}'
        assert ack['preferredIswc'] == iswc
        assert all(x in [y['title'] for y in ack['otherTitles']]
                   for x in [other_title_one, other_title_two])
        assert all(x['type'] == 'AT' for x in ack['otherTitles'])
        assert submission.submission['originalTitle'] in ack['originalTitle']


"""
Submission is found by the Resolution Service.
Submission contains other titles along with the OT.
ACK only contains OT as per spec.
TSV Format.
"""


def test_find_submission_other_title_given_tsv(sftp_client, cosmos_service, databricks_client):
    api = IswcApi()
    submission = EligibleSubmission(0)
    other_title = get_new_title()
    submission.add_other_title(other_title, 'AT')
    submission_res = api.add_submission(submission)
    iswc = json.loads(submission_res.content)[
        "verifiedSubmission"]["iswc"]
    filename = generate_tsv_resolution_in_file([submission])
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
