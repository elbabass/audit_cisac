import json
from submissions.submission import EligibleSubmission, IneligibleSubmission


def generate_eligible_submissions(number_of_submissions):
    return [EligibleSubmission(i) for i in range(number_of_submissions)]


def generate_ineligible_submissions(number_of_submissions):
    return [IneligibleSubmission(i) for i in range(number_of_submissions)]


class Settings:
    def __init__(self):
        with open('settings.json', 'r') as config:
            data = json.load(config)
            self.sftp_host = data['AzureKeyVaultSecret-EDI-SFTP-Hostname']
            self.sftp_username = data['AzureKeyVaultSecret-EDI-SFTP-Username']
            self.sftp_password = data['AzureKeyVaultSecret-EDI-SFTP-Password']
            self.job_api_token = data['AzureKeyVaultSecret-EDI-Databricks-JobApiBearerToken']
            self.job_id = data['AzureKeyVaultSecret-EDI-Databricks-JobID']
            self.iswc_api = data['AzureKeyVaultSecret-ISWC-BaseAddress-IswcApi']
            self.iswc_api_secret = data['AzureKeyVaultSecret-ISWC-Secret-IswcApiManagement']
            self.cosmos_string = data['AzureKeyVaultSecret-ISWC-ConnectionString-ISWCCosmosDb']
