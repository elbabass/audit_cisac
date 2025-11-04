class OutputRowDefinition:
    def __init__(self, fields: dict):
        self.fields = fields

    def get_field(self,  field_name: str, of_type=str):
        (start_position, length) = self.fields[field_name]
        start_position = start_position - 1
        return of_type(self.row[self.transaction_type][start_position: start_position + length].strip())
