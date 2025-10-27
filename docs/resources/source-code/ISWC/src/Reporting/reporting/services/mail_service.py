import requests


class MailService():

    def __init__(self, mail_service_uri: str, mail_service_key: str):
        self.mail_service_uri = mail_service_uri
        self.mail_service_key = mail_service_key

    def send(self, email: str, report_type: str):
        (requests
         .post(self.mail_service_uri + '?code={}'.format(self.mail_service_key), json={
             "recipient": email,
             "reportType": report_type
         })
         .raise_for_status())
