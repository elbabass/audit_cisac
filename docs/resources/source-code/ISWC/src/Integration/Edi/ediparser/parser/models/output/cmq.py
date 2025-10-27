from ediparser.parser.models.input.input_transaction import InputTransaction
from ediparser.parser.models.output.cwi import CWI
from ediparser.parser.models.output.mli import MLI
from ediparser.parser.models.output.output_row_definition import \
    OutputRowDefinition
from ediparser.parser.models.output.output_transaction import OutputTransaction


class CMQ(OutputTransaction):
    def __init__(self, api_response: dict, original_transaction: InputTransaction, header: dict):
        super().__init__(api_response, original_transaction)
        self.header = header
        search_results = self.api_response.get('searchResults')
        search_result = search_results[0] if search_results else self.api_response.get('searchResult')
        self.iswc = search_result
        self.original_transaction = original_transaction

    def get_record(self):
        if self.iswc != '' and self.iswc != None:
            return {
                'WorkTitle': self.iswc.get('originalTitle')[0:60],
                'AgencyCode': self.iswc.get('agency').rjust(3, '0'),
                'SourceDBCode': self.iswc.get('agency').rjust(3, '0'),
                'PreferredISWC': self.iswc.get('iswc'),
            }            
        elif self.original_transaction.get('OriginalTransactionType') == 'CDR':
            return {
                'PreferredISWC': self.original_transaction.get('ParentIswc')
            }

    def get_child_records(self):
        records = []
        if self.iswc == '' or self.iswc == None:
            return None
        for work in self.iswc.get('works'):
            cwi = CWI(work, self.header)
            records.append(cwi.get_record())
            for child in cwi.get_child_records():
                records.append(child)
        if self.original_transaction.get('ParentIswc') != '' and self.original_transaction.get('OriginalTransactionType') != 'CDR':
            mli = MLI(self.original_transaction.get('ParentIswc'))
            records.append(mli.get_record())

        return records
