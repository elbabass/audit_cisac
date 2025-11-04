from datetime import datetime


class HDR():
    def __init__(self, file_header: dict, receivingAgency=None):
        self.file_header = file_header
        self.receivingAgency = receivingAgency

    def get_record(self):
        if self.receivingAgency != None:
            return {
                "fileCreationDateTime": str(datetime.utcnow()),
                "receivingAgency": self.receivingAgency,
                "submittingAgency": "315",
                "submittingSourcedb": 315
            }
        else:
            if self.file_header.get('submittingPublisher') and self.file_header.get('submittingPublisher').get('role'):
                del self.file_header['submittingPublisher']['role']

            if self.file_header.get('submittingPublisher'):
                return {
                        "submittingPublisher": self.file_header['submittingPublisher'],
                        "fileCreationDateTime": str(datetime.utcnow()),
                        "receivingAgency": self.file_header['receivingAgency'],
                        "submittingAgency": self.file_header['submittingAgency'],
                        "submittingSourcedb": self.file_header['submittingSourcedb']
                        } 

            elif self.file_header.get('submittingPartyId'):
                return {
                        "submittingPartyId": self.file_header['submittingPartyId'],
                        "fileCreationDateTime": str(datetime.utcnow())
                       }
            
            else: 
                return  {
                          "fileCreationDateTime": str(datetime.utcnow()),
                          "receivingAgency": self.file_header['submittingAgency'],
                          "submittingAgency": "315",
                          "submittingSourcedb": 315
                        } 