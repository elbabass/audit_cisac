import json
import traceback
from datetime import datetime
from logging import StreamHandler, getLogger, INFO, WARN

class LoggerService:
    def __init__(self):
        self.logger = getLogger()
        self.logger.addHandler(StreamHandler())
        self.logger.setLevel(INFO)

    def log_audit_job_details(self, total_processed: int, total_iswc_changes: int, new_hwm: datetime):
        custom_dimensions = {"Audit Records Processed": total_processed,
                             "Iswc Changes Processed": total_iswc_changes,  
                             "New Highwatermark": str(new_hwm)}

        self.logger.info("Job Finished Successfully", extra={
                         'custom_dimensions': custom_dimensions})

        print(json.dumps(custom_dimensions, indent=4))

    def log_change_tracker_job_details(self, iswc_changes: int, new_hwm: datetime):
        custom_dimensions = {"Total Iswcs that have changed": iswc_changes,  
                             "New Highwatermark": str(new_hwm)}

        self.logger.info("Job Finished Successfully", extra={
                         'custom_dimensions': custom_dimensions})

        print(json.dumps(custom_dimensions, indent=4))

    def log_exception(self, file_name=None):
        message = traceback.format_exc()
        if file_name:
            message = "File: " + file_name + "\n" + message
        self.logger.exception(message)

    def log_warning(self, message: str):
        self.logger.warning(message)

    def log_info(self, message: str):
        self.logger.info(message)
