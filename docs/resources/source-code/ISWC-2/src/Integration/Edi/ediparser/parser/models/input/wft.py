from ediparser.parser.models.input.input_row_definition import InputRowDefinition
from ediparser.parser.models.input.input_transaction import InputTransaction


class WFT(InputTransaction):
    def __init__(self, transaction: object, header: dict):
        super().__init__()
        self.header = header
        self.fields = InputRowDefinition(transaction, self.__class__.__name__, {
            'RecordType': (1, 3),
            'TransactionSequence#': (4, 8),
            'RecordSequence#': (12, 8),
            'WorkflowTaskId': (20, 10),
            'WorkflowType': (30, 1),
            'Status': (31, 1)
        })

    def get_http_verb(self):
        return 'PATCH'

    def get_url(self):
        return '/iswc/workflowTasks'

    def get_parameters(self):
        return {
            'agency': self.header['SenderID'][-3:]
        }

    def get_body(self):
        body = {
            "taskId": self.fields.get_field('WorkflowTaskId'),
            "workflowType": self.get_api_workflow_type(self.fields.get_field('WorkflowType')),
            "status": (self.fields.get_field('Status'))
        }

        return body

    def get_api_workflow_type(self, workflow_type: str):
        if workflow_type == "1":
            return "UpdateApproval"
        else:
            return "MergeApproval"
