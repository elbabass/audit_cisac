import json
import traceback

import pandas as pd
import requests
from ediparser.parser.models.api_response import ApiResponse
from ediparser.parser.models.edi_file import EdiFile
from ediparser.parser.services.logger_service import LoggerService
from pyspark.sql import SparkSession
from pyspark.sql.functions import udf
from pyspark.sql.types import StringType
from requests.models import Response


class RequestService:
    def __init__(self,
                 file: EdiFile,
                 spark: SparkSession,
                 api_base_address: str,
                 api_subscription_key: str,
                 partition_count: int,
                 file_type: str,
                 logger: LoggerService):
        self.file = file
        self.spark = spark
        self.api_base_address = api_base_address
        self.api_subscription_key = api_subscription_key
        self.partition_count = partition_count
        self.file_type = file_type
        self.logger = logger

    def run(self):
        api_responses = {}
        for group in self.file.groups:
            api_requests = []
            for batch in group.df_arr:
                api_requests.append(self.__get_batch_request_body(group.transaction_type, batch)
                                    + [group.get_http_verb(), 
                                       self.__get_url(group),
                                       self.api_subscription_key])

            df1 = pd.DataFrame(api_requests,
                               columns=['body', 'original_transaction', 'parameters', 'http_verb', 'url', 'api_subscription_key'])

            df2 = self.spark.createDataFrame(df1)

            def makeApiRequests(url, http_verb, api_subscription_key,  parameters, body):
                response = Response
                try:
                    response = requests.request(
                        http_verb,
                        url=url,
                        headers={
                            'Ocp-Apim-Subscription-Key': api_subscription_key
                        },
                        params=parameters,
                        json=json.loads(body),
                        timeout=600
                    )
                except:
                    response.text = traceback.format_exc()
                    response.status_code = 500

                return json.dumps({"text": response.text, "status_code": response.status_code})

            udf_makeApiRequests = udf(makeApiRequests, StringType())

            apidf = (df2
                     .repartition(self.partition_count)
                     .withColumn('output', udf_makeApiRequests(df2.url, df2.http_verb, df2.api_subscription_key, df2.parameters, df2.body))
                     .drop('body')
                     .drop('parameters')
                     .drop('http_verb')
                     .drop('url')
                     .drop('api_subscription_key'))

            for row in apidf.collect():
                original_transaction = json.loads(row[0])
                response = json.loads(row[1])
                status_code = response['status_code']
                response_text = response['text']

                # handle non-JSON responses
                if status_code == 204:
                    response_output = None
                elif response_text == 'Iswc not found':
                    response_output = {'code': '404',
                                       'message': 'ISWC Not Found'}
                elif status_code == 500 or not self.is_json(response_text):
                    response_output = {'code': '500',
                                       'message': 'Internal Server Error'}
                    status_code = 500
                    output = 'HttpStatus:' + \
                        str(status_code) + ', HttpResponse:' + response_text
                    self.logger.log_exception(output)
                else:
                    response_output = json.loads(response_text)

                api_responses.setdefault(group.group_id, []).append(
                    ApiResponse(response_output, original_transaction, status_code))

        return api_responses

    def is_json(self, myjson):
        try:
            json.loads(myjson)
        except ValueError as e:
            return False
        return True

    def __get_batch_request_body(self, transaction_type: str, df):
        request_body = []
        request_parameters = []
        original_transactions = []

        for row in df.itertuples():
            transaction = row.api_request
            original_transactions = self.populate_original_transactions(
                original_transactions, transaction, transaction_type)
            if transaction_type in ['CAR', 'CUR']:
                if self.file.file_name.__contains__('.json'):
                    submission = transaction.get_body()
                elif self.file.file_name.__contains__('.txt'):
                    submission = transaction.get_body()
                else:
                    submission = {
                        "submissionId": int(row.Index),
                        "submission": transaction.get_body()
                    }
                transaction_rejection = transaction.get_rejection()
                if transaction_rejection != None:
                    submission['rejection'] = transaction_rejection             
                request_body.append(submission)
            elif transaction_type in ['CMQ', 'CIQ', 'WFT', 'MER', 'CDR', 'FSQ', 'UPW', 'CIQEXT']:
                body = transaction.get_body()
                if body is not None:
                    request_body.append(transaction.get_body())

                parameters = transaction.get_parameters()
                if parameters is not None:
                    request_parameters.append(parameters)
            else:
                request_body = None

            body = request_body[0] if transaction_type in [
                'MER'] else request_body
            parameters = request_parameters[0] if len(
                request_parameters) > 0 else "[]"

        return [json.dumps(body), json.dumps(original_transactions), parameters]

    def populate_original_transactions(self, original_transactions, transaction, transaction_type):
        if self.file_type.__contains__('JSON'):
            if transaction_type in ['CAR', 'CUR', 'FSQ']:
                transaction_body = transaction.transaction.get(
                    'submission').get('body')
            else:
                transaction_body = transaction.transaction.get('submission')

            submission_id = transaction_body.get('submissionId')

            if transaction_body.get('submission') is not None:
                submitted_workcode = transaction_body.get('submission').get('publisherWorkcode') if transaction_body.get('submission').get('publisherWorkcode') is not None else transaction_body.get('submission').get('workcode')
            else:
                submitted_workcode = ''
            original_transactions.append({
                'TransactionType': transaction_type,
                'OriginalTransactionType': transaction.transaction.get('submission').get('originalTransactionType') if self.file_type == 'CSN_JSON' else transaction_type,
                'OriginalSubmissionId': submission_id,
                'OriginalFileCreationDate': transaction.header.get('fileCreationDateTime'),
                'AgencyCode': transaction.transaction.get('submission').get('agency'),
                'WorkflowTaskID': transaction.transaction.get('submission').get('workflowTaskId'),
                'WorkflowStatus': transaction.transaction.get('submission').get('workflowStatus'),
                'ReceivingAgencyCode': transaction.transaction.get('submission').get('receivingAgency'),
                'ParentIswc': transaction.transaction.get('submission').get('parentIswc')
            } if transaction_type == 'CMQ' else {
                'OriginalTransactionType': transaction_type,
                'OriginalSubmissionId': submission_id,
                'OriginalFileCreationDate': transaction.header.get('fileCreationDateTime'),
                'AgencyWorkCode': submitted_workcode if submitted_workcode is not None else ''
            })
        elif self.file_type.__contains__('TSV'):
            transaction_body = transaction.transaction.get(
                'submission').get('body')
            submission_id = transaction_body.get('submissionId')
            submitted_workcode = transaction_body.get('submission').get('publisherWorkcode') if transaction_body.get('submission').get('publisherWorkcode') is not None else transaction_body.get('submission').get('workcode')
            original_transactions.append({
                'OriginalTransactionType': transaction_type,
                'OriginalSubmissionId': submission_id,
                'submittingAgency': transaction_body.get('submission').get('agency'),
                'submittingSourcedb': transaction_body.get('submission').get('sourcedb'),
                'workcode': submitted_workcode,
                'originalTitle': transaction_body.get('submission').get('originalTitle'),
                'submittingPublishername': transaction_body.get('submittingPublisher').get('name'),
                'submittingPublishernameNumber': transaction_body.get('submittingPublisher').get('nameNumber') if transaction_body.get('submittingPublisher').get('nameNumber') is not None else '',
                'submittingPublisheremail': transaction_body.get('submittingPublisher').get('email'),
                'submittingPublisherrole': transaction_body.get('submittingPublisher').get('role')
            })
        elif self.file_type.__contains__('CSV'):
            original_transactions.append({
                'TransactionSequence#': transaction.get_field('TransactionSequence#'),
                'RecordType': transaction.get_field('RecordType'),
                'RecordSequence': transaction.get_field('RecordSequence#'),
                'OriginalFileCreationDate': self.file.header.get('CreationDate'),
                'OriginalFileCreationTime': self.file.header.get('CreationTime'),
                'WorkTitle': transaction.get_field('WorkTitle'),
                'ReferenceNumber': transaction.get_reference_number()
            })
        else:
            original_transactions.append({
                'TransactionSequence#': transaction.get_field('TransactionSequence#'),
                'RecordType': transaction.get_field('RecordType'),
                'RecordSequence': transaction.get_field('RecordSequence#'),
                'OriginalFileCreationDate': self.file.header.get('CreationDate'),
                'OriginalFileCreationTime': self.file.header.get('CreationTime'),
                'AgencyCode': transaction.get_field('AgencyCode'),
                'WorkflowTaskID': transaction.get_field('WorkflowTaskID'),
                'WorkflowStatus': transaction.get_field('WorkflowStatus'),
                'OriginalTransactionType': transaction.get_field('OriginalTransactionType'),
                'ReceivingAgencyCode': transaction.get_field('ReceivingAgencyCode'),
                'ParentIswc': transaction.get_field('ParentIswc')
            } if transaction_type == 'CMQ' else {
                'TransactionSequence#': transaction.get_field('TransactionSequence#'),
                'RecordType': transaction.get_field('RecordType'),
                'RecordSequence': transaction.get_field('RecordSequence#'),
                'OriginalFileCreationDate': self.file.header.get('CreationDate'),
                'OriginalFileCreationTime': self.file.header.get('CreationTime'),
                'WorkTitle': transaction.get_field('WorkTitle'),
                'AgencyCode': transaction.get_field('AgencyCode'),
                'AgencyWorkCode': transaction.get_field('AgencyWorkCode'),
                'SourceDBCode': transaction.get_field('SourceDBCode'),
            })
        return original_transactions

    def __get_url(self, group):
        url = self.api_base_address + group.get_url()
        if self.file.file_name.__contains__('ISWC3'):
            url = self.api_base_address + group.get_thirdParty_url()
        elif group.group_id.__eq__('publisherContextSearch'):
            url = self.api_base_address + group.get_publisherContextSearch_url()
        return url