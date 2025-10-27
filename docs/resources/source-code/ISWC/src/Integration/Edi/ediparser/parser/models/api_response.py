class ApiResponse:
    def __init__(self, output: dict, original_transaction: dict, status_code: int):
        self.output = output
        self.original_transaction = original_transaction
        self.status_code = status_code
