import pytest
import json
from utils.output_files import *


"""
Search JSON submitted with valid structure and multiple valid ISWCs
Output file contains data from the ISWCs searched for
"""

def test_thirdparty_search_multi_iswc_json(sftp_client, cosmos_service, databricks_client, shared_submissions, third_party_identifier):
    remote_filepath = sftp_client.upload(third_party_identifier, generate_json_edi_thirdparty_iswc_search_in_file(shared_submissions[1], third_party_identifier))
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
            assert ack['originalTransactionType'] == 'SearchByIswcSubmission', f'Incorrect transaction type: {ack}'
            assert ack['preferredIswc'] in shared_submissions[1], 'Wrong ISWC returned'

        


"""
File is sent with a header missing required fields
Process completes successfully, output file contains appropriate file level error
"""

def test_thirdparty_search_invalid_header_json(sftp_client, cosmos_service, databricks_client, shared_submissions, third_party_identifier):
    remote_filepath = sftp_client.upload(third_party_identifier, generate_json_edi_thirdparty_iswc_search_in_file_invalid_header(shared_submissions[1], third_party_identifier))
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



"""
File is sent with ISWCs with invalid formats or that don't exist already in the Database 
File generated with individual error messages for each case
"""

def test_thirdparty_search_nonexistant_iswc_json(sftp_client, cosmos_service, databricks_client, third_party_identifier, non_existant_iswc):
    invalid_iswcs = [f'T{non_existant_iswc}', f'T{non_existant_iswc+1}']
    remote_filepath = sftp_client.upload(third_party_identifier, generate_json_edi_thirdparty_iswc_search_in_file(invalid_iswcs, third_party_identifier))
    run_id = databricks_client.start_ack_json_job(remote_filepath)
    run_details = databricks_client.wait_for_run(run_id)

    assert run_details['state']['life_cycle_state'] == 'TERMINATED', 'Job did not finish within time limit'
    assert run_details['state']['result_state'] == 'SUCCESS', f'Job failed: {run_details["run_page_url"]}'

    ack_filename = cosmos_service.get_ack_filename(remote_filepath)
    ack_file = sftp_client.get_ack_file(ack_filename, third_party_identifier)

    with open(ack_file) as out_file:
        data = json.load(out_file)
        ack = data['acknowledgements']
        assert len(list(filter(lambda x: x['errorMessages']['code'] == '180', ack))) == 1, f'Unexpected number of results with code 180 returned'
        assert len(list(filter(lambda x: x['errorMessages']['message'] == 'ISWC not found', ack))) == 1, f'Unexpected number of results with message "ISWC not found" returned'
        assert len(list(filter(lambda x: x['errorMessages']['code'] == '141', ack))) == 1, f'Unexpected number of results with code 141 returned'
        assert len(list(filter(lambda x: 'Invalid Check Digit' in x['errorMessages']['message'], ack))) == 1, f'Unexpected number of results with message "ISWC format error – Invalid Check Digit" returned'



"""
File is sent with ISWC with an S instead of a T
Expect error saying S character does not conform to expected ISWC format
"""     

def test_thirdparty_search_invalid_iswc_invalidformat_json(sftp_client, cosmos_service, databricks_client, third_party_identifier, non_existant_iswc):
    invalid_iswc = [f'S{non_existant_iswc}']
    remote_filepath = sftp_client.upload(third_party_identifier, generate_json_edi_thirdparty_iswc_search_in_file(invalid_iswc, third_party_identifier))
    run_id = databricks_client.start_ack_json_job(remote_filepath)
    run_details = databricks_client.wait_for_run(run_id)

    assert run_details['state']['life_cycle_state'] == 'TERMINATED', 'Job did not finish within time limit'
    assert run_details['state']['result_state'] == 'SUCCESS', f'Job failed: {run_details["run_page_url"]}'

    ack_filename = cosmos_service.get_ack_filename(remote_filepath)
    ack_file = sftp_client.get_ack_file(ack_filename, third_party_identifier)

    with open(ack_file) as out_file:
        data = json.load(out_file)
        res = data['fileLevelError']
        assert res['errorMessage'] == "'S9999999994' does not match 'T[0-9]{10}'", f'Unexpected message returned {res["errorMessage"]}'
        assert res['errorType'] == 'Validation Error', f'Unexpected error type returned {res["errorType"]}'

