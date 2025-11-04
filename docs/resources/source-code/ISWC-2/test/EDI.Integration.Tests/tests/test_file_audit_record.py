import pytest
from datetime import datetime
from utils.cosmos_service import CosmosService
from utils.sftp_client import SftpClient
from utils.databricks_client import DatabricksClient
from utils.utils import *
from submissions.submission import *
from utils.output_files import *


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
Validate File Audit entry in Cosmos for processed file.
File is in JSON format.
"""


def test_file_audit_record_json(sftp_client, cosmos_service, databricks_client):
    submissions = generate_eligible_submissions(2)
    filename = generate_json_allocation_in_file(submissions)
    remote_filepath = sftp_client.upload_to_publisher_allocation_folder(
        '128', 'SA', filename)

    run_id = databricks_client.start_ack_json_job(remote_filepath)
    run_details = databricks_client.wait_for_run(run_id)

    assert run_details['state']['life_cycle_state'] == 'TERMINATED', 'Job did not finish within time limit'
    assert run_details['state'][
        'result_state'] == 'SUCCESS', f'Job failed: {run_details["run_page_url"]}'

    file_audit = cosmos_service.get_file_audit_record(remote_filepath)
    date = str(datetime(datetime.today().year, datetime.today().month,
                        datetime.today().day)).split(' ')[0]
    assert file_audit['AgencyCode'] == "128"
    assert file_audit['SubmittingPublisherIPNameNumber'] == SonyPublisherIp.name_number
    assert file_audit['Status'] == 'Finished OK'
    assert file_audit['FileName'] == remote_filepath
    assert date in file_audit['DatePickedUp']
    assert date in file_audit['DateAckGenerated']
    assert f'ISWCP_{date}' in file_audit['AckFileName']
    assert '315_SA_128.json' in file_audit['AckFileName']


"""
Validate File Audit entry in Cosmos for processed file.
File is in TSV format.
"""


def test_file_audit_record_tsv(sftp_client, cosmos_service, databricks_client):
    submissions = generate_eligible_submissions(2)
    filename = generate_tsv_allocation_in_file(submissions)
    remote_filepath = sftp_client.upload_to_publisher_allocation_folder(
        '128', 'SA', filename)

    run_id = databricks_client.start_ack_tsv_job(remote_filepath)
    run_details = databricks_client.wait_for_run(run_id)

    assert run_details['state']['life_cycle_state'] == 'TERMINATED', 'Job did not finish within time limit'
    assert run_details['state'][
        'result_state'] == 'SUCCESS', f'Job failed: {run_details["run_page_url"]}'

    file_audit = cosmos_service.get_file_audit_record(remote_filepath)
    date = str(datetime(datetime.today().year, datetime.today().month,
                        datetime.today().day)).split(' ')[0]
    assert file_audit['AgencyCode'] == "128"
    assert file_audit['SubmittingPublisherIPNameNumber'] == SonyPublisherIp.name_number
    assert file_audit['Status'] == 'Finished OK'
    assert file_audit['FileName'] == remote_filepath
    assert date in file_audit['DatePickedUp']
    assert date in file_audit['DateAckGenerated']
    assert f'ISWCP_{date}' in file_audit['AckFileName']
    assert '315_SA_128.txt' in file_audit['AckFileName']
