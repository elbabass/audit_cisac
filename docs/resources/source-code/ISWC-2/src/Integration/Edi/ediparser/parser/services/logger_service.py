import traceback
import json
from logging import INFO, WARN, StreamHandler, getLogger
import datetime

from opencensus.ext.azure.log_exporter import AzureLogHandler

from ediparser.parser.models.edi_file import EdiFile
from ediparser.parser.services.cosmos_service import CosmosService


class LoggerService:
    database = "ISWC"
    container = "FileAudit"

    def __init__(self, ai_connection_string, cosmos_connection_string):
        self.ai_connection_string = ai_connection_string
        self.logger = getLogger()
        self.logger.addHandler(StreamHandler())

        if cosmos_connection_string:
            self.cosmos_service = CosmosService(
                cosmos_connection_string, self.database, self.container)

        if ai_connection_string:
            self.logger.addHandler(AzureLogHandler(
                connection_string=ai_connection_string))

    def log_exception(self, file_name=None):
        message = traceback.format_exc()
        if file_name:
            message = "File: " + file_name + "\n" + message
        self.logger.exception(message)

    def log_parsed_file(self, ediFile: EdiFile, ack_file_name: str, time_taken, transaction_count: int):
        group = ediFile.groups[0]
        custom_dimensions = {"FileName": ediFile.file_name,
                             "AckFileName": ack_file_name,
                             "TimeTaken": str(time_taken),
                             "TransactionCount": transaction_count or sum([len(item.index) for item in group.df_arr]),
                             "TransactionType": group.transaction_type}

        self.logger.setLevel(INFO)
        self.logger.info("FileProcessed", extra={
                         'custom_dimensions': custom_dimensions})
        self.logger.setLevel(WARN)

        print(json.dumps(custom_dimensions, indent=4))

    def log_file_audit_cosmos(self, file_name: str, ack_file_name: str, start_date, end_date, cosmos_id: str, publisher_name_number, status: str):
        start_date = start_date.isoformat()
        if end_date is not None:
            end_date = end_date.isoformat()

        agency_code = file_name[0:3]

        custom_dimensions = {"DatePickedUp": start_date,
                             "AgencyCode": agency_code,
                             "SubmittingPublisherIPNameNumber": publisher_name_number,
                             "FileName": file_name,
                             "Status": status,
                             "AckFileName": ack_file_name,
                             "DateAckGenerated": end_date,
                             "PartitionKey": cosmos_id,
                             "id": cosmos_id}

        self.cosmos_service.set_processed_items(custom_dimensions)

    def log_warning(self, message: str):
        self.logger.warning(message)

    def log_info(self, message: str):
        self.logger.info(message)    

    def log_submission_details(self, submission_details: dict, file_name=None):
        if file_name:
            submission_details['FileName'] = file_name

        self.logger.error('Internal Server Error', extra={
            'custom_dimensions': submission_details})

        print(json.dumps(submission_details, indent=4))
