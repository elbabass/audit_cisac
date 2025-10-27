import pytest
import json
from utils.output_files import *

"""
Search JSON submitted with valid structure and multiple agency workcodes corresponding to existing ISWCs from the test fixture
Output file contains data from the ISWCs searched for
"""

def test_thirdparty_search_multi_agencyworkcode(sftp_client, cosmos_service, databricks_client, shared_submissions, third_party_identifier):
    remote_filepath = sftp_client.upload(third_party_identifier, generate_json_edi_thirdparty_agencyworkcode_search_in_file(shared_submissions[2], shared_submissions[3], third_party_identifier))
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
            assert ack['originalTransactionType'] == 'SearchByAgencyWorkCodeSubmission', f'Incorrect transaction type: {ack}'
            assert ack['preferredIswc'] in shared_submissions[1], 'Wrong ISWC returned'


"""
Search JSON submitted with valid structure and multiple agency workcodes, where some correspond to existing ISWCs, and the others correspond to no existing ISWC
Output file contains data for the agency workcodes which have corresponding ISWCs, no results found for the others
"""
def test_thirdparty_search_multi_agencyworkcode_non_existant(sftp_client, cosmos_service, databricks_client, shared_submissions, third_party_identifier):
    extra_workcodes = shared_submissions[2] + [(x + ' extra') for x in shared_submissions[2]]
    remote_filepath = sftp_client.upload(third_party_identifier, generate_json_edi_thirdparty_agencyworkcode_search_in_file(extra_workcodes, shared_submissions[3], third_party_identifier))
    run_id = databricks_client.start_ack_json_job(remote_filepath)
    run_details = databricks_client.wait_for_run(run_id)

    assert run_details['state']['life_cycle_state'] == 'TERMINATED', 'Job did not finish within time limit'
    assert run_details['state']['result_state'] == 'SUCCESS', f'Job failed: {run_details["run_page_url"]}'

    ack_filename = cosmos_service.get_ack_filename(remote_filepath)
    ack_file = sftp_client.get_ack_file(ack_filename, third_party_identifier)
    counts = [0, 0] #keeping count of successful and unsuccessful transactions in the file as we assert on them
    with open(ack_file) as out_file:
        data = json.load(out_file)
        for ack in data['acknowledgements']:
            if ack['transactionStatus'] == 'FullyAccepted':
                counts[0] += 1
            else:
                counts[1] += 1
                assert ack['transactionStatus'] == 'Rejected', f'Transaction not rejected: {ack}'
                assert ack['originalTransactionType'] == 'SearchByAgencyWorkCodeSubmission', f'Incorrect transaction type: {ack}'
                assert ack['errorMessages']['code'] == '180'
                assert ack['errorMessages']['message'] == 'ISWC not found'

    assert counts[0] == counts[1], 'Count of successful transactions should be equal to the unsuccessful transaction count'
                
            