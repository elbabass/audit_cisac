from ediparser.parser.mixins.transaction_mixin import TransactionMixin
from ediparser.parser.models.edi_file import EdiFileVersion
from ediparser.parser.models.input.input_row_definition import InputRowDefinition
from ediparser.parser.models.input.input_transaction import InputTransaction


class CIQEXT(TransactionMixin, InputTransaction):
    def __init__(self, transaction, header):
        self.transaction = transaction
        self.header = header

    def get_http_verb(self):
        return 'POST'

    def get_url(self):
        return '/iswc/searchByAgencyWorkCode/batch'
    
    def get_thirdParty_url(self, group_id):
        if group_id.__eq__('searchByTitleAndContributorsSubmissions') :
            return '/thirdParty/searchByTitleAndContributor/batch'
        else :
            return '/thirdParty/searchByAgencyWorkCode/batch'

    def get_publisherContextSearch_url(self):
        return '/publisher/context/search/batch'       

    def get_submission(self):
        return {
            'submissionId': self.transaction.get('submissionId'),
            'body': {
                'workCode': self.transaction.get('workcode'),
                'agency': self.transaction.get('agency')
            }

        }
    
    def get_title_submission(self):
        return {
            'submissionId': self.transaction.get('submissionId'),
            'body': {
                'titles': self.transaction.get('titles'),
                'interestedParties': self.transaction.get('interestedParties')
            }

        }
    
    def get_context_submission(self):
        body_fields = [
            'firstName', 'lastName', 'nameNumber', 'dateOfBirth', 'dateOfDeath',
            'age', 'ageTolerance', 'works', 'affiliations',
            'additionalIdentifiers', 'societyWorkCodes', 'additionalIPNames'
        ]

        body = {
            field: self.transaction.get(field)
            for field in body_fields
            if self.transaction.get(field) is not None
        }

        return {
            'submissionId': self.transaction.get('submissionId'),
            'body': body
    }

    def get_parameters(self):
        None

    def get_body(self):
        return self.transaction.get('submission').get('body')
