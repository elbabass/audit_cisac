import datetime


class ReportOutput():
    iso8601_format = 'yyyy-MM-dd HH:mm:ss'

    def get_output_file_name(self, parameters: dict):
        current_datetime = datetime.datetime.utcnow()
        sub_agency = parameters['SubmittingAgencyCode']
        report_type = parameters['ReportType']
        ip_number = parameters.get('IpNumber', '') 
        file_name = ''

        if report_type in ['SubmissionAudit', 'AgencyInterestExtract', 'PublisherIswcTracking', 'PotentialDuplicates']:
            file_extension = '.csv'
            file_name = '{}{}{}{}'.format(
                report_type,
                current_datetime.strftime('%Y%m%d%H%M%S'),
                sub_agency,
                file_extension)
            
        if report_type in ['CreatorReport']:
            file_extension = '.json'
            file_name = '{}{}{}{}{}'.format(
                report_type,
                current_datetime.strftime('%Y%m%d%H%M%S'),
                sub_agency,
                ip_number,
                file_extension)

        return file_name
