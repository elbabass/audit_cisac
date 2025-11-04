import requests
import zeep
from datetime import datetime
from ediparser.parser.services.logger_service import LoggerService

class SoapService:

    def __init__(self,
                 logger: LoggerService, 
                 soap_service_url: str, 
                 soap_service_binding: str,
                 soap_wsdl_url: str, 
                 soap_client_name: str, 
                 soap_client_version: str, 
                 soap_client_location_name: str, 
                 soap_client_api_key: str):
        self.logger = logger
        self.soap_service_url = soap_service_url 
        self.soap_service_binding = soap_service_binding 
        self.soap_wsdl_url = soap_wsdl_url 
        self.soap_client_name = soap_client_name 
        self.soap_client_version = soap_client_version
        self.soap_client_location_name = soap_client_location_name
        self.soap_client_api_key = soap_client_api_key    

    def send_file(self, file_name: str, file_contents: list, agency_code: str):     
        header = zeep.xsd.Element(
            'clientIdentifier',
            zeep.xsd.ComplexType(
                [
                    zeep.xsd.Element('name', zeep.xsd.String()),
                    zeep.xsd.Element('version', zeep.xsd.String()),
                    zeep.xsd.Element('location', zeep.xsd.ComplexType([
                            zeep.xsd.Element('name', zeep.xsd.String())
                        ])
                    ),
                    zeep.xsd.Element('apiKey', zeep.xsd.String()) 
                ]
            ),
        )

        header_value = header(name=self.soap_client_name, version=self.soap_client_version, 
                              location={'name':self.soap_client_location_name}, apiKey=self.soap_client_api_key)

        client = zeep.Client(wsdl=self.soap_wsdl_url)
        
        service = client.create_service(
        self.soap_service_binding,
        self.soap_service_url
        )
        
        file_transfer = {
            'name': file_name,
            'contentType': 'text/csv',
            'dataHandler': '\n'.join(file_contents).encode('utf8')
        }

        comment = f'Package generated on {datetime.now()}'

        response = service.downloadPackage(file_transfer, agency_code, 'iswc.support@cisac.org',
                                             comment, 'DNL', 'en', ',', _soapheaders=[header_value])
        
        self.logger.log_info(
                            f"File has been sent to MWI RTF Download SOAP service. Valid Items: {response.numberOfValidItems}, Invalid Items: {response.numberOfInvalidItems}")

        return response


