from submissions.interested_parties import SonyPublisherIp
import pytest
import json
from utils.cosmos_service import CosmosService
from utils.sftp_client import SftpClient
from utils.databricks_client import DatabricksClient
from utils.utils import *
from utils.api import IswcApi
from submissions.submission import *
from utils.output_files import *
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
Submissions are successfully added through EDI.
In file is in JSON format.
"""


def test_edi_add_success_json(sftp_client, cosmos_service, databricks_client):
    submissions = generate_eligible_submissions(2)
    filename = generate_json_edi_add_submissions_in_file(submissions)
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
        for ack in data['acknowledgements']:
            assert ack[
                'transactionStatus'] == 'FullyAccepted', f'Transaction not accepted: {ack}'
        assert len(data['acknowledgements']) == 2


"""
Update is applied through EDI.
In file is in JSON format.
"""


def test_edi_update_success_json(sftp_client, cosmos_service, databricks_client):
    api = IswcApi()
    submission_one = EligibleSubmission(0)
    submission_two = EligibleSubmission(1)
    submission_one_res = api.add_submission(submission_one)
    submission_two_res = api.add_submission(submission_two)
    submission_one.submission['preferredIswc'] = json.loads(submission_one_res.content)[
        "verifiedSubmission"]["iswc"]
    submission_two.submission['preferredIswc'] = json.loads(submission_two_res.content)[
        "verifiedSubmission"]["iswc"]
    submission_one.submission['originalTitle'] = get_new_title()
    submission_two.submission['originalTitle'] = get_new_title()
    filename = generate_json_edi_update_in_file(
        [submission_one, submission_two])
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
        assert len(data['acknowledgements']) == 2
        for ack in data['acknowledgements']:
            assert ack[
                'transactionStatus'] == 'FullyAccepted', f'Transaction not accepted: {ack}'
        assert data['acknowledgements'][0]['originalTitle'] == submission_one.submission['originalTitle']
        assert data['acknowledgements'][1]['originalTitle'] == submission_two.submission['originalTitle']


"""
Work is deleted through EDI.
In file is in JSON format.
"""


def test_edi_delete_success_json(sftp_client, cosmos_service, databricks_client):
    api = IswcApi()
    submission_one = EligibleSubmission(0)
    submission_two = EligibleSubmission(1)
    submission_one_res = api.add_submission(submission_one)
    submission_two_res = api.add_submission(submission_two)
    submission_one.submission['preferredIswc'] = json.loads(submission_one_res.content)[
        "verifiedSubmission"]["iswc"]
    submission_two.submission['preferredIswc'] = json.loads(submission_two_res.content)[
        "verifiedSubmission"]["iswc"]
    submission_one.submission['originalTitle'] = get_new_title()
    submission_two.submission['originalTitle'] = get_new_title()
    submission_one.add_deletion_reason()
    submission_two.add_deletion_reason()
    filename = generate_json_edi_delete_in_file(
        [submission_one, submission_two])
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
        assert len(data['acknowledgements']) == 2
        for ack in data['acknowledgements']:
            assert ack[
                'transactionStatus'] == 'FullyAccepted', f'Transaction not accepted: {ack}'


"""
Merge is applied through EDI.
In file is in JSON format.
"""


def test_edi_merge_success_json(sftp_client, cosmos_service, databricks_client):
    api = IswcApi()
    submission_one = EligibleSubmission(0)
    submission_two = EligibleSubmission(1)
    submission_one_res = api.add_submission(submission_one)
    submission_two_res = api.add_submission(submission_two)
    submission_one.submission['preferredIswc'] = json.loads(submission_one_res.content)[
        "verifiedSubmission"]["iswc"]
    submission_two.submission['preferredIswc'] = json.loads(submission_two_res.content)[
        "verifiedSubmission"]["iswc"]
    submission_one.add_merge_iswcs(
        [submission_two.submission['preferredIswc']])
    filename = generate_json_edi_merge_in_file(
        [submission_one])
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
        assert len(data['acknowledgements']) == 1
        for ack in data['acknowledgements']:
            assert ack[
                'transactionStatus'] == 'FullyAccepted', f'Transaction not accepted: {ack}'


"""
Search by ISWC through EDI.
In file is in JSON format.
"""


def test_edi_iswc_search_success_json(sftp_client, cosmos_service, databricks_client):
    api = IswcApi()
    submission_one = EligibleSubmission(0)
    submission_two = EligibleSubmission(1)
    submission_one_res = api.add_submission(submission_one)
    submission_two_res = api.add_submission(submission_two)
    submission_one.submission['preferredIswc'] = json.loads(submission_one_res.content)[
        "verifiedSubmission"]["iswc"]
    submission_two.submission['preferredIswc'] = json.loads(submission_two_res.content)[
        "verifiedSubmission"]["iswc"]
    filename = generate_json_edi_iswc_search_in_file(
        [submission_one.submission['preferredIswc'], submission_two.submission['preferredIswc']])
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
        assert len(data['acknowledgements']) == 2
        for ack in data['acknowledgements']:
            assert ack[
                'transactionStatus'] == 'FullyAccepted', f'Transaction not accepted: {ack}'
        assert data['acknowledgements'][0]['originalTitle'] == submission_one.submission['originalTitle']
        assert data['acknowledgements'][1]['originalTitle'] == submission_two.submission['originalTitle']


"""
Search by agency workcode through EDI.
In file is in JSON format.
"""


def test_edi_workcode_search_success_json(sftp_client, cosmos_service, databricks_client):
    api = IswcApi()
    submission_one = EligibleSubmission(0)
    submission_two = EligibleSubmission(1)
    submission_one_res = api.add_submission(submission_one)
    submission_two_res = api.add_submission(submission_two)
    submission_one.submission['preferredIswc'] = json.loads(submission_one_res.content)[
        "verifiedSubmission"]["iswc"]
    submission_two.submission['preferredIswc'] = json.loads(submission_two_res.content)[
        "verifiedSubmission"]["iswc"]
    filename = generate_json_edi_workcode_search_in_file(
        [submission_one, submission_two])
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
        assert len(data['acknowledgements']) == 2
        for ack in data['acknowledgements']:
            assert ack[
                'transactionStatus'] == 'FullyAccepted', f'Transaction not accepted: {ack}'
        assert data['acknowledgements'][0]['preferredIswc'] == submission_one.submission['preferredIswc']
        assert data['acknowledgements'][1]['preferredIswc'] == submission_two.submission['preferredIswc']


"""
Submissions are accepted by the Allocation Service.
In file is in JSON format.
"""


def test_allocation_success_json(sftp_client, cosmos_service, databricks_client):
    submissions = generate_eligible_submissions(2)
    del submissions[0].submission['interestedParties'][0]['name']
    del submissions[0].submission['interestedParties'][1]['name']
    filename = generate_json_allocation_in_file(submissions)
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
        assert len(data['acknowledgements']) == 2


"""
Submissions are accepted by the Allocation Service.
In file is in TSV format.
"""


def test_allocation_success_tsv(sftp_client, cosmos_service, databricks_client):
    submissions = generate_eligible_submissions(2)
    filename = generate_tsv_allocation_in_file(submissions)
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
            assert 'FullyAccepted' in x


"""
Submission is found by the Resolution Service.
In file is in JSON format.
"""


def test_resolution_success_json(sftp_client, cosmos_service, databricks_client):
    api = IswcApi()
    submission_one = EligibleSubmission(0)
    submission_two = EligibleSubmission(1)
    submission_one_res = api.add_submission(submission_one)
    submission_two_res = api.add_submission(submission_two)
    filename = generate_json_resolution_in_file(
        [submission_one, submission_two])
    iswc_one = json.loads(submission_one_res.content)[
        "verifiedSubmission"]["iswc"]
    iswc_two = json.loads(submission_two_res.content)[
        "verifiedSubmission"]["iswc"]
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
        for ack in data['acknowledgements']:
            assert ack[
                'transactionStatus'] == 'FullyAccepted', f'Transaction not accepted: {ack}'
            assert ack['preferredIswc'] == iswc_one or ack['preferredIswc'] == iswc_two


"""
Submission is found by the Resolution Service.
In file is in TSV format.
"""


def test_resolution_success_tsv(sftp_client, cosmos_service, databricks_client):
    api = IswcApi()
    submission_one = EligibleSubmission(0)
    submission_two = EligibleSubmission(1)
    submission_one_res = api.add_submission(submission_one)
    submission_two_res = api.add_submission(submission_two)
    filename = generate_tsv_resolution_in_file(
        [submission_one, submission_two])
    iswc_one = json.loads(submission_one_res.content)[
        "verifiedSubmission"]["iswc"]
    iswc_two = json.loads(submission_two_res.content)[
        "verifiedSubmission"]["iswc"]
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
            assert 'FullyAccepted' in x
            assert iswc_one in x or iswc_two in x


"""
locallyAllocatedIswc has been deleted and is valid for use.
Submission is created with the locallyAllocatedIswc
"""


def test_locally_allocated_iswc_created_json(
    sftp_client, cosmos_service, databricks_client
):
    api = IswcApi()
    submission_one = EligibleSubmission(0)
    submission_one_res = api.add_submission(submission_one)
    submission_one.submission["preferredIswc"] = json.loads(submission_one_res.content)[
        "verifiedSubmission"]["iswc"]
    time.sleep(5)
    api.delete_submission(submission_one.submission)
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
        assert data["acknowledgements"][0]["transactionStatus"] == "FullyAccepted"
        assert data["acknowledgements"][0]["preferredIswc"] == alloc_sub.submission["locallyAllocatedIswc"]
        assert data["acknowledgements"][0]["archivedIswc"] is None
