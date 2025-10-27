import math
import datetime
import json
from json.decoder import JSONDecodeError
from jsonschema import validate
from timeit import default_timer as timer
from ediparser.parser.services.logger_service import LoggerService
from pyspark.sql import SparkSession
from ediparser.parser.configuration.iswc_schema import schema
from ediparser.parser.configuration.iswc_schema_v2 import schemaV2
from ediparser.parser.configuration.third_party_schema import thirdPartySchema
from ediparser.parser.models.edi_file import EdiFile
from ediparser.parser.parser import Parser
from ediparser.parser.services.json_serializer_service import JsonSerializerService


class BatchParser(Parser):
    def __init__(self,
                 api_base_address: str,
                 api_subscription_key: str,
                 logger: LoggerService,
                 spark: SparkSession,
                 partition_count: int,
                 ):
        super().__init__(api_base_address, api_subscription_key, logger, spark, partition_count)
        self.parser = Parser(api_base_address, api_subscription_key,
                             logger, spark, partition_count)

    def run_batch(self, file_name: str, file_type: str, file_contents: object):
        start = timer()
        # 1.Validating json file with defined schema
        if file_type == 'ACK_JSON':
            error_ack_file = self.validate_file(
                file_name, file_type, file_contents)
            if error_ack_file:
                return error_ack_file

        # 2.Run batches of file in loop and compile the output
        file_contents_list = self.get_file_splits(file_contents, file_type)
        ack_file_contents_list = []
        ack_file_name_list = []
        edi_file_groups = []
        publisher_name_number = None
        agency_code = None
        for file_contents in file_contents_list:
            (ack_file_name, ack_file_contents, file) = self.parser.run(
                file_name, file_type, file_contents)
            ack_file_contents_list.append(ack_file_contents)
            ack_file_name_list.append(ack_file_name)
            edi_file_groups.append(file.groups)
            if publisher_name_number is None and file.publisher_name_number is not None:
                publisher_name_number = file.publisher_name_number
            if agency_code is None and file.agency_code is not None:
                agency_code = file.agency_code

        for i in range(1, len(ack_file_contents_list)):
            if file_type == 'ACK_JSON':
                ack_file_contents_list[0]['acknowledgements'].extend(
                    ack_file_contents_list[i]['acknowledgements'])
                ack_file_contents_list[0]['fileHeader'].update(
                    ack_file_contents_list[-1]['fileHeader'])
            elif file_type == 'ACK_TSV':
                ack_file_contents_list[0].extend(ack_file_contents_list[i])

        [edi_file_groups[0].extend(edi_file_groups[i])
         for i in range(1, len(edi_file_groups))]
        file = EdiFile(file.file_name, file.header, edi_file_groups[0])

        ack_file_name = self.get_ack_name_with_subcode(ack_file_name_list)

        end = timer()
        time_taken = datetime.timedelta(seconds=end-start)
        self.logger.log_parsed_file(file, ack_file_name, time_taken, None)

        return (ack_file_name, ack_file_contents_list[0], publisher_name_number, agency_code)

    def get_file_splits(self, file_content, file_type):
        limit_length = 5000
        file_list = []
        if isinstance(file_content, str):
            file = json.loads(file_content)
            transaction_type = [key for key, value in file.items() if key not in [
                'fileHeader', 'schema']][0]
            total_length = len(file[transaction_type])
            if total_length > limit_length:
                divisions = math.ceil(total_length/limit_length)
                for i in range(1, divisions+1):
                    split_file = {'fileHeader': {}, '$schema': ''}
                    split_file['fileHeader'].update(file['fileHeader'])
                    if '$schema' in file:
                        split_file['$schema'] = file['$schema']
                    split_file[transaction_type] = file[transaction_type][(
                        i-1)*limit_length:i*limit_length]
                    file_list.append(json.dumps(split_file))
                return file_list
            else:
                file_list.append(file_content)
                return file_list
        elif isinstance(file_content, object) and file_type == 'ACK_TSV':
            file = []
            [file.append(line.strip('\n').strip('\r').split('\t'))
             for line in file_content]
            total_length = len(file)
            if total_length > limit_length:
                divisions = math.ceil(total_length/limit_length)
                for i in range(1, divisions+1):
                    split_file = file[(i-1)*limit_length:i*limit_length]
                    file_list.append(split_file)
                return file_list
            else:
                file_list.append(file)
                return file_list
        else:
            file_list.append(file_content)
            return file_list

    def get_ack_name_with_subcode(self, temp_ack_file_name_list):
        file_extension = temp_ack_file_name_list[0].split('.')[1]
        ack_file_name_list = [element.split('.')[0]
                              for element in temp_ack_file_name_list]
        if ack_file_name_list[-1][-4] == '_':
            submitter_code = '_'
            for i in range(len(ack_file_name_list)-1):
                if ack_file_name_list[i][-4] != '_':
                    submitter_code = ack_file_name_list[i][-5:-3]
            ack_file_name_list[-1] = ack_file_name_list[-1].replace(
                '_', submitter_code)
            return '{}.{}'.format(ack_file_name_list[-1], file_extension)
        else:
            return '{}.{}'.format(ack_file_name_list[-1], file_extension)

    def validate_file(self, file_name: str, file_type: str, file_contents: str):
        ack_file_contents = self.validate_json_schema(file_name, file_contents)
        if not ack_file_contents:
            ack_file_contents = self.validate_submission_ids(file_contents)
        if not ack_file_contents and file_name.__contains__("FromLocalRange"):
            ack_file_contents = self.validate_locally_allocated_iswcs(
                file_contents)

        if not ack_file_contents:
            return None

        ack_file_name = EdiFile(
            file_name, ack_file_contents['fileHeader'], []).get_ack_file_name(None, JsonSerializerService.FILE_EXTENSION, file_type)

        return (ack_file_name, ack_file_contents, None, None)

    def validate_json_schema(self, file_name: str, file_contents: str):
        file_contents_json = {}
        try:
            file_contents_json = json.loads(file_contents)

            if 'iswc3' in file_name.lower():
                validate(instance=file_contents_json, schema=thirdPartySchema)
            elif 'iswcp2' in file_name.lower() or 'iswc2' in file_name.lower():
                validate(instance=file_contents_json, schema=schemaV2)
            else:
                validate(instance=file_contents_json, schema=schema)
        except JSONDecodeError as e:
            return {
                "fileHeader": {"filename": file_name},
                "fileLevelError": {
                    "errorMessage": e.args[0],
                    "errorType": "JSON Format Error"}
            }

        except Exception as e:
            try:
                path_list = list(e.path)
                submission_id = file_contents_json[path_list[0]][path_list[1]].get("submissionId")
            except:
                submission_id = None
            
            return {
                "fileHeader": file_contents_json.get('fileHeader', {"filename": file_name}),
                "fileLevelError": {
                    "path": list(e.path),
                    "SubmissionID": submission_id,
                    "errorMessage": e.message,
                    "errorType": "Validation Error"}
            }

    def validate_submission_ids(self, file_contents):
        file_contents_json = json.loads(file_contents)

        submissionIds = []
        key = None

        for k in {
            'addSubmissions',
            'findSubmissions',
            'updateSubmissions',
            'mergeSubmissions',
            'searchByIswcSubmissions',
            'searchByAgencyWorkCodeSubmissions',
            'deleteSubmissions',
            'publisherContextSearch'
        }:
            if k in file_contents_json:
                key = k
                break

        if key is not None:
            for submission in file_contents_json[key]:
                submission["submissionId"] += 1
                submissionIds.append(submission["submissionId"])

            if len(submissionIds) != len(set(submissionIds)):
                return {
                    "fileHeader": file_contents_json.get('fileHeader'),
                    "fileLevelError": {
                        "errorMessage": 'There are duplicate SubmissionId values in the file.',
                        "errorType": "Validation Error"}
                }

    def validate_locally_allocated_iswcs(self, file_contents):
        file_contents_json = json.loads(file_contents)

        iswcs = []

        if "addSubmissions" in file_contents_json:
            for submission in file_contents_json["addSubmissions"]:
                if "locallyAllocatedIswc" in submission:
                    iswcs.append(submission["locallyAllocatedIswc"])

            if len(iswcs) != len(set(iswcs)):
                return {
                    "fileHeader": file_contents_json.get('fileHeader'),
                    "fileLevelError": {
                        "errorMessage": 'There are duplicate locallyAllocatedIswc values in the file.',
                        "errorType": "Validation Error"}
                }
