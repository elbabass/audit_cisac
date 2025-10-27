import pytest
import json
from utils.api import IswcApi
from utils.cosmos_service import CosmosService
from utils.sftp_client import SftpClient
from utils.databricks_client import DatabricksClient
from submissions.submission import EligibleSubmission
from submissions.submission import trim_ip_name_to_lastname


@pytest.fixture(scope="session")
def third_party_identifier():
    yield "padpida3897722461g"

@pytest.fixture(scope="session")
def sftp_client():
    sftp_client = SftpClient()
    yield sftp_client
    sftp_client.close_client()


@pytest.fixture(scope="session")
def cosmos_service():
    cosmos_service = CosmosService()
    yield cosmos_service


@pytest.fixture(scope="session")
def databricks_client():
    databricks_client = DatabricksClient()
    yield databricks_client

@pytest.fixture(scope="session")
def shared_submissions():
    submissions = [EligibleSubmission(x) for x in range(1,5)]
    workcodes = [x.submission["workcode"] for x in submissions]
    agency = submissions[0].submission["agency"]
    titles = [x.submission['originalTitle'] for x in submissions]
    interestedParties = [x.submission['interestedParties'] for x in submissions]
    interestedPartiesModified = []
    ips = []
    for i in interestedParties:
        for j in i:
            ips.append({"lastName" : trim_ip_name_to_lastname(j.get('name')), "nameNumber" : j.get('nameNumber'), "role" : j.get('role')})
        interestedPartiesModified.append(ips)
        ips = []
    api = IswcApi()
    submissions_results = [api.add_submission(x) for x in submissions]
    iswcs = [json.loads(x.content)["verifiedSubmission"]["iswc"] for x in submissions_results]
    shared_submissions = [submissions, iswcs, workcodes, agency, titles, interestedPartiesModified]
    yield shared_submissions


@pytest.fixture(scope="session")
def non_existant_iswc():
    yield 9999999994




