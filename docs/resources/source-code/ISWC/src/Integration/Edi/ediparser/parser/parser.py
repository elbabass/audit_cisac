import itertools

from pyspark.sql import SparkSession

from ediparser.parser.models.edi_file import EdiFile
from ediparser.parser.services.deserializer_service import DeserializerService
from ediparser.parser.services.json_deserializer_service import \
    DeserializerJsonService
from ediparser.parser.services.json_serializer_service import \
    JsonSerializerService
from ediparser.parser.services.csv_serializer_service import \
    CsvSerializerService    
from ediparser.parser.services.logger_service import LoggerService
from ediparser.parser.services.request_service import RequestService
from ediparser.parser.services.serializer_service import SerializerService
from ediparser.parser.services.tsv_deserializer_service import \
    DeserializerTsvService
from ediparser.parser.services.tsv_serializer_service import \
    TsvSerializerService


class Parser():
    def __init__(self,
                 api_base_address: str,
                 api_subscription_key: str,
                 logger: LoggerService,
                 spark: SparkSession,
                 partition_count: int,
                 ):
        self.api_base_address = api_base_address
        self.api_subscription_key = api_subscription_key
        self.logger = logger
        self.spark = spark
        self.partition_count = partition_count

    def run(self, file_name: str, file_type: str, file_contents: object):
        # 1. download file & parse file into dataframe incl. field for json/parameters

        if file_type.__contains__('JSON'):
            file = DeserializerJsonService(
                file_name, file_contents)
        elif file_type.__contains__('TSV'):
            file = DeserializerTsvService(file_name, file_contents)
        else:
            file = DeserializerService(file_name, file_contents)

        file = file.deserialize_file()

        # 2. send to Api
        api_responses = RequestService(
            file, self.spark, self.api_base_address, self.api_subscription_key, self.partition_count, file_type, self.logger).run()

        # 3. construct Ack/Csn from response
        if file_type.__contains__('JSON'):
            serializer = JsonSerializerService(
                file, api_responses, file_type, self.logger)
        elif file_type.__contains__('TSV'):
            serializer = TsvSerializerService(
                file, api_responses, file_type, self.logger)
        elif file_type.__contains__('CSV'):
            serializer = CsvSerializerService(
                file, api_responses, file_type, self.logger)
        else:
            serializer = SerializerService(
                file, api_responses, file_type, self.logger)

        (ack_file_name, ack_file_contents, file) = serializer.serialize_file()
        return (ack_file_name, ack_file_contents, file)

    def run_csn(self, file_type: str, file: EdiFile, api_responses: dict):
        ack_file_count = 0
        if file_type.__contains__('JSON'):
            serializer = JsonSerializerService(
                file, api_responses, file_type, self.logger)
        else:
            serializer = SerializerService(
                file, api_responses, file_type, self.logger)

        (ack_file_name, ack_file_contents, file) = serializer.serialize_file()

        if file_type.__contains__('JSON'):
            ack_file_count = len(ack_file_contents['notifications'])
        else:
            ack_file_count = len(
                list(filter(lambda x: x.startswith('CSN'),  ack_file_contents)))

        return (ack_file_name, ack_file_contents, ack_file_count)

    def run_cse(self, file_type: str, file_name: str, file_header: dict, api_responses: dict):
        file = EdiFile(file_name, file_header, None)
        serializer = SerializerService(
            file, api_responses, 'CSE', self.logger)

        (ack_file_name, ack_file_contents, file) = serializer.serialize_file()

        return (ack_file_name, ack_file_contents)
