import traceback
from logging import StreamHandler, getLogger

from opencensus.ext.azure.log_exporter import AzureLogHandler


class LoggerService:

    def __init__(self, ai_connection_string):
        self.ai_connection_string = ai_connection_string
        self.logger = getLogger()
        self.logger.addHandler(StreamHandler())

        if ai_connection_string:
            self.logger.addHandler(AzureLogHandler(
                connection_string=ai_connection_string))

    def log_exception(self, file_name=None):
        message = traceback.format_exc()
        if file_name:
            message = "File: " + file_name + "\n" + message
        self.logger.exception(message)

    def log_warning(self, message: str):
        self.logger.warning(message)
