import pytest
from utils.sftp_client import SftpClient
from utils.databricks_client import DatabricksClient
from utils.utils import *
from utils.api import IswcApi
from submissions.submission import *
from utils.output_files import *


@pytest.fixture
def sftp_client():
    sftp_client = SftpClient()
    yield sftp_client
    sftp_client.close_client()


@pytest.fixture
def databricks_client():
    databricks_client = DatabricksClient()
    yield databricks_client


"""
In file goes to the Publisher Allocation Archive folder after successful job run.
JSON Format.
"""


def test_allocation_in_file_goes_to_publisher_archive_json(sftp_client, databricks_client):
    submissions = generate_eligible_submissions(2)
    filename = generate_json_allocation_in_file(submissions)
    remote_filepath = sftp_client.upload_to_publisher_allocation_folder(
        '128', 'SA', filename)

    run_id = databricks_client.start_ack_json_job(remote_filepath)
    run_details = databricks_client.wait_for_run(run_id)

    assert run_details['state']['life_cycle_state'] == 'TERMINATED', 'Job did not finish within time limit'
    assert run_details['state'][
        'result_state'] == 'SUCCESS', f'Job failed: {run_details["run_page_url"]}'
    assert sftp_client.is_file_in_publisher_allocation_archive_directory(
        filename, '128', 'SA') is True


"""
In file goes to the Publisher Resolution Archive folder after successful job run.
JSON Format.
"""


def test_resolution_in_file_goes_to_publisher_archive_json(sftp_client, databricks_client):
    api = IswcApi()
    submission_one = EligibleSubmission(0)
    api.add_submission(submission_one)
    filename = generate_json_resolution_in_file(
        [submission_one])
    remote_filepath = sftp_client.upload_to_publisher_resolution_folder(
        '312', 'SA', filename)

    run_id = databricks_client.start_ack_json_job(remote_filepath)
    run_details = databricks_client.wait_for_run(run_id)

    assert run_details['state']['life_cycle_state'] == 'TERMINATED', 'Job did not finish within time limit'
    assert run_details['state'][
        'result_state'] == 'SUCCESS', f'Job failed: {run_details["run_page_url"]}'
    assert sftp_client.is_file_in_publisher_resolution_archive_directory(
        filename, '312', 'SA') is True


"""
In file goes to the Agency Archive folder after successful job run.
JSON Format.
"""


def test_allocation_in_file_goes_to_agency_archive_json(sftp_client, databricks_client):
    submissions = generate_eligible_submissions(2)
    filename = generate_json_edi_add_submissions_in_file(submissions)
    remote_filepath = sftp_client.upload('128', filename)

    run_id = databricks_client.start_ack_json_job(remote_filepath)
    run_details = databricks_client.wait_for_run(run_id)

    assert run_details['state']['life_cycle_state'] == 'TERMINATED', 'Job did not finish within time limit'
    assert run_details['state'][
        'result_state'] == 'SUCCESS', f'Job failed: {run_details["run_page_url"]}'
    assert sftp_client.is_file_in_agency_archive_directory(
        filename, '128') is True
