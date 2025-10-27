from ediparser.parser.models.input.input_validator import validate_field_values


class InputRowDefinition:

    def __init__(self, row: object, transaction_type: str, fields: dict):
        self.row = row
        self.transaction_type = transaction_type
        self.fields = fields

    def get_field(self,  field_name: str, of_type=str):
        (start_position, length) = self.fields[field_name]
        start_position = start_position - 1

        field_value = validate_field_values(
            field_name, self.row[self.transaction_type][start_position: start_position + length].strip(), of_type)

        return field_value
