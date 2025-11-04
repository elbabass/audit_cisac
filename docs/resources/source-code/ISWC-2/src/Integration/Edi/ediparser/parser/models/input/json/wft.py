from ediparser.parser.models.input.input_transaction import InputTransaction


class WFT(InputTransaction):
    def __init__(self, transaction, header):
        self.transaction = transaction
        self.header = header

    def get_http_verb(self):
        return 'PATCH'

    def get_url(self):
        return '/iswc/workflowTasks'

    def get_submission(self):
        return {
            'submissionId': self.transaction.get('submissionId'),
            'parameters': {
                'agency': self.header.get('submittingAgency')
            },
            'body': {
                'taskId': self.transaction.get('taskId'),
                'workflowType': self.get_api_workflow_type(self.transaction.get('workflowType')),
                'status': self.transaction.get('status')
            },
        }

    def get_body(self):
        return self.transaction.get('submission').get('body')

    def get_parameters(self):
        return self.transaction.get('submission').get('parameters')

    def get_api_workflow_type(self, workflow_type: int):
        if workflow_type == 1:
            return "UpdateApproval"
        elif workflow_type == 2:
            return "MergeApproval"
        elif workflow_type == 3:
            return "DemergeApproval"
