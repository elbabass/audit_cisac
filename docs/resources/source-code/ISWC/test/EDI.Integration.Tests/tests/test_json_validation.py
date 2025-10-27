import pytest
from utils.sftp_client import SftpClient
from utils.databricks_client import DatabricksClient
from utils.utils import *
from utils.api import IswcApi
from submissions.submission import *
from utils.output_files import *
from utils.cosmos_service import CosmosService


@pytest.fixture
def sftp_client():
    sftp_client = SftpClient()
    yield sftp_client
    sftp_client.close_client()


@pytest.fixture
def databricks_client():
    databricks_client = DatabricksClient()
    yield databricks_client


@pytest.fixture
def cosmos_service():
    cosmos_service = CosmosService()
    yield cosmos_service


"""
Schema validation fails.
Output file is still created with the error and the job succeeds.
JSON Format.
"""


def test_validation_error_job_still_succeeds(sftp_client, databricks_client, cosmos_service):
    sub = EligibleSubmission(0)
    sub.submission['sourcedb'] = '128'
    filename = generate_json_edi_add_submissions_in_file([sub])

    remote_filepath = sftp_client.upload('128', filename)

    run_id = databricks_client.start_ack_json_job(remote_filepath)
    run_details = databricks_client.wait_for_run(run_id)

    assert run_details['state']['life_cycle_state'] == 'TERMINATED', 'Job did not finish within time limit'
    assert run_details['state'][
        'result_state'] == 'SUCCESS', f'Job failed: {run_details["run_page_url"]}'

    ack_filename = cosmos_service.get_ack_filename(remote_filepath)
    ack_file = sftp_client.get_ack_file(ack_filename, '128')

    with open(ack_file) as out_file:
        data = json.load(out_file)
        error = data['fileLevelError']
        assert error['errorType'] == 'Validation Error'


"""
JSON is invalid.
Output file is still created with the error and the job succeeds.
"""


def test_invalid_json_job_still_succeeds(sftp_client, databricks_client, cosmos_service):
    filename = generate_invalid_json_file()
    remote_filepath = sftp_client.upload(
        '128', filename)

    run_id = databricks_client.start_ack_json_job(remote_filepath)
    run_details = databricks_client.wait_for_run(run_id)

    assert run_details['state']['life_cycle_state'] == 'TERMINATED', 'Job did not finish within time limit'
    assert run_details['state'][
        'result_state'] == 'SUCCESS', f'Job failed: {run_details["run_page_url"]}'
    ack_filename = cosmos_service.get_ack_filename(remote_filepath)
    ack_file = sftp_client.get_ack_file(ack_filename, '128')

    with open(ack_file) as out_file:
        data = json.load(out_file)
        error = data['fileLevelError']
        assert error['errorType'] == 'JSON Format Error'
