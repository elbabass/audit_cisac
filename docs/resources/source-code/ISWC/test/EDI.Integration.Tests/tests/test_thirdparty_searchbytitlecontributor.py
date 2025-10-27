import pytest
import json
from utils.output_files import *


def test_thirdparty_search_title_contributor(sftp_client, cosmos_service, databricks_client, shared_submissions, third_party_identifier):
    remote_filepath = sftp_client.upload(third_party_identifier, generate_json_edi_thirdparty_titlecontributor_search_in_file(shared_submissions[4], shared_submissions[5], third_party_identifier))
    run_id = databricks_client.start_ack_json_job(remote_filepath)
    run_details = databricks_client.wait_for_run(run_id)

    assert run_details['state']['life_cycle_state'] == 'TERMINATED', 'Job did not finish within time limit'
    assert run_details['state']['result_state'] == 'SUCCESS', f'Job failed: {run_details["run_page_url"]}'

    ack_filename = cosmos_service.get_ack_filename(remote_filepath)
    ack_file = sftp_client.get_ack_file(ack_filename, third_party_identifier)

    with open(ack_file) as out_file:
        data = json.load(out_file)
        for ack in data['acknowledgements']:
            assert ack['transactionStatus'] == 'FullyAccepted', f'Transaction not accepted: {ack}'
            assert ack['originalTransactionType'] == 'SearchByTitleAndContributorsSubmission', f'Incorrect transaction type: {ack}'
            assert ack['preferredIswc'] in shared_submissions[1], 'Wrong ISWC returned'

def test_thirdparty_search_title_contributor_invalidheader(sftp_client, cosmos_service, databricks_client, shared_submissions, third_party_identifier):
    remote_filepath = sftp_client.upload(third_party_identifier, generate_json_edi_thirdparty_titlecontributor_invalid_header(shared_submissions[4], shared_submissions[5], third_party_identifier))
    run_id = databricks_client.start_ack_json_job(remote_filepath)
    run_details = databricks_client.wait_for_run(run_id)

    assert run_details['state']['life_cycle_state'] == 'TERMINATED', 'Job did not finish within time limit'
    assert run_details['state']['result_state'] == 'SUCCESS', f'Job failed: {run_details["run_page_url"]}'

    ack_filename = cosmos_service.get_ack_filename(remote_filepath)
    ack_file = sftp_client.get_ack_file(ack_filename, third_party_identifier)

    with open(ack_file) as out_file:
        data = json.load(out_file)
        assert data['fileLevelError']['errorType'] == 'Validation Error', 'Unexpected Error Type'
        assert data['fileLevelError']['errorMessage'] == "'submittingPartyId' is a required property", 'Unexpected Error Message'

def test_thirdparty_search_title_contributor_missing_interestedparties_property(sftp_client, cosmos_service, databricks_client, shared_submissions, third_party_identifier):
    remote_filepath = sftp_client.upload(third_party_identifier, generate_json_edi_thirdparty_titlecontributor_search_no_contributors(shared_submissions[4], third_party_identifier))
    run_id = databricks_client.start_ack_json_job(remote_filepath)
    run_details = databricks_client.wait_for_run(run_id)

    assert run_details['state']['life_cycle_state'] == 'TERMINATED', 'Job did not finish within time limit'
    assert run_details['state']['result_state'] == 'SUCCESS', f'Job failed: {run_details["run_page_url"]}'

    ack_filename = cosmos_service.get_ack_filename(remote_filepath)
    ack_file = sftp_client.get_ack_file(ack_filename, third_party_identifier)

    with open(ack_file) as out_file:
        data = json.load(out_file)
        res = data['fileLevelError']
        assert res['errorMessage'] == "'interestedParties' is a required property", f'Unexpected message returned {res["errorMessage"]}'
        assert res['errorType'] == 'Validation Error', f'Unexpected error type returned {res["errorType"]}'

def test_thirdparty_search_title_contributor_title_empty_contributors(sftp_client, cosmos_service, databricks_client, shared_submissions, third_party_identifier):
    empty_contributors = []
    for i in shared_submissions[5]:
        empty_contributors.append([{"lastName" : '', "nameNumber" : 0, "role" : 'CA'}])
    
    remote_filepath = sftp_client.upload(third_party_identifier, generate_json_edi_thirdparty_titlecontributor_search_in_file(shared_submissions[4], empty_contributors, third_party_identifier))
    run_id = databricks_client.start_ack_json_job(remote_filepath)
    run_details = databricks_client.wait_for_run(run_id)

    assert run_details['state']['life_cycle_state'] == 'TERMINATED', 'Job did not finish within time limit'
    assert run_details['state']['result_state'] == 'SUCCESS', f'Job failed: {run_details["run_page_url"]}'

    ack_filename = cosmos_service.get_ack_filename(remote_filepath)
    ack_file = sftp_client.get_ack_file(ack_filename, third_party_identifier)

    with open(ack_file) as out_file:
        data = json.load(out_file)
        for ack in data['acknowledgements']:
            assert ack['transactionStatus'] == 'Rejected', f'Transaction accepted: {ack}'
            assert ack['originalTransactionType'] == 'SearchByTitleAndContributorsSubmission', f'Incorrect transaction type: {ack}'
            assert ack['errorMessages']['code'] == '102', f'Wrong error code returned: {ack}'
            assert ack['errorMessages']['message'] == 'I.P. Name Number is required', f'Unexpected error message returned: {ack}'