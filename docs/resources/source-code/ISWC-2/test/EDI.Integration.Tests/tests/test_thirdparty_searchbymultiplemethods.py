import pytest
import json
from utils.output_files import *


def test_thirdparty_searchbymultiplemethods_allvalid(sftp_client, cosmos_service, databricks_client, shared_submissions, third_party_identifier):
    remote_filepath = sftp_client.upload(third_party_identifier, generate_json_edi_thirdparty_allsearchtypes_allvalid(shared_submissions[1], shared_submissions[2], shared_submissions[3], shared_submissions[4], shared_submissions[5],third_party_identifier))
    run_id = databricks_client.start_ack_json_job(remote_filepath)
    run_details = databricks_client.wait_for_run(run_id)

    assert run_details['state']['life_cycle_state'] == 'TERMINATED', 'Job did not finish within time limit'
    assert run_details['state']['result_state'] == 'SUCCESS', f'Job failed: {run_details["run_page_url"]}'

    ack_filename = cosmos_service.get_ack_filename(remote_filepath)
    ack_file = sftp_client.get_ack_file(ack_filename, third_party_identifier)

    with open(ack_file) as out_file:
        data = json.load(out_file)
        transaction_types_log = json.loads('{"SearchByIswcSubmission" : 0, "SearchByAgencyWorkCodeSubmission" : 0, "SearchByTitleAndContributorsSubmission" : 0}')
        for ack in data['acknowledgements']:
            assert ack['transactionStatus'] == 'FullyAccepted', f'Transaction not accepted: {ack}'
            assert ack['preferredIswc'] in shared_submissions[1], 'Wrong ISWC returned'
            transaction_types_log[ack["originalTransactionType"]] += 1
        for key, value in transaction_types_log.items():
            assert value == 4, 'Wrong transaction count'


def test_thirdparty_searchbymultiplemethods_mixedvalidity(sftp_client, cosmos_service, databricks_client, shared_submissions, third_party_identifier, non_existant_iswc):
    invalid_iswcs = [f'T{non_existant_iswc}', f'T{non_existant_iswc + 1}', shared_submissions[1][0]]
    remote_filepath = sftp_client.upload(third_party_identifier, generate_json_edi_thirdparty_allsearchtypes_allvalid(invalid_iswcs, shared_submissions[2], shared_submissions[3], shared_submissions[4], shared_submissions[5],third_party_identifier))
    run_id = databricks_client.start_ack_json_job(remote_filepath)
    run_details = databricks_client.wait_for_run(run_id)

    assert run_details['state']['life_cycle_state'] == 'TERMINATED', 'Job did not finish within time limit'
    assert run_details['state']['result_state'] == 'SUCCESS', f'Job failed: {run_details["run_page_url"]}'

    ack_filename = cosmos_service.get_ack_filename(remote_filepath)
    ack_file = sftp_client.get_ack_file(ack_filename, third_party_identifier)

    with open(ack_file) as out_file:
        data = json.load(out_file)
        ack = data['acknowledgements']
        assert len(list(filter(lambda x: x['transactionStatus'] == 'Rejected', ack))) == 2, f'Unexpected number of rejected transactions'
        assert len(list(filter(lambda x: x['transactionStatus'] == 'FullyAccepted', ack))) == 9, f'Unexpected number of accepted transactions'