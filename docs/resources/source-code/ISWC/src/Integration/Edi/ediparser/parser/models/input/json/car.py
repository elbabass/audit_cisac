from ediparser.parser.models.input.input_transaction import InputTransaction
from ediparser.parser.models.input.rejection import get_rejection_message


class CAR(InputTransaction):
    def __init__(self, transaction, header):
        self.transaction = transaction
        self.header = header

    def get_http_verb(self):
        return 'POST'

    def get_url(self):
        if self.header.get('submittingPublisher') != None:
            return '/allocation/batch'
        else:
            return '/submission/batch'

    def get_submission(self):
        if self.header.get('submittingPublisher') != None:
            self.transaction['agency'] = self.header.get(
                'submittingAgency')
            self.transaction['sourcedb'] = self.header.get(
                'submittingSourcedb')
            if not 'category' in self.transaction.keys():
                self.transaction['category'] = "DOM"

            if 'additionalIdentifiers' in self.transaction.keys():
                if 'publisherIdentifiers' in self.transaction['additionalIdentifiers'].keys() and self.transaction['additionalIdentifiers']['publisherIdentifiers'] != []:
                    self.transaction['additionalIdentifiers']['publisherIdentifiers'].append({
                        "nameNumber": self.header.get('submittingPublisher').get('nameNumber'),
                        "workCode": [self.transaction.get('workcode')]})
                else:
                    self.transaction['additionalIdentifiers'].update({"publisherIdentifiers": [{
                        "nameNumber": self.header.get('submittingPublisher').get('nameNumber'),
                        "workCode": [self.transaction.get('workcode')]}]})
            else:
                self.transaction['additionalIdentifiers'] = {"publisherIdentifiers": [{
                    "nameNumber":  self.header.get('submittingPublisher').get('nameNumber'),
                    "workCode": [self.transaction.get('workcode')]}]}

            self.transaction['workcode'] = self.generate_workcode(
                self.transaction.get('workcode'), self.transaction)

        if 'multipleAgencyWorkCodes' in self.transaction.keys():
            return {
                'body': {
                    'submissionId': self.transaction.get('submissionId'),
                    'submission': self.transaction,
                    'multipleAgencyWorkCodes': self.transaction.get('multipleAgencyWorkCodes')
                }
            }
        else:
            return {
                'body': {
                    'submissionId': self.transaction.get('submissionId'),
                    'submission': self.transaction
                }
            }

    def get_parameters(self):
        return None

    def get_IProle(self, ip_list):
        pub_role = list(
            filter(lambda ip_role: ip_role['role'] in ['AM', 'E'], ip_list))
        return pub_role

    def get_body(self):
        if self.header.get('submittingPublisher') != None:
            transaction_record = self.transaction.get('submission').get('body')
            sub_publisher = self.header.get('submittingPublisher')

            pub_info = {'name': sub_publisher.get('name'),
                        'nameNumber': sub_publisher.get('nameNumber')}

            if not 'role' in list(sub_publisher.keys()) or not sub_publisher['role'] in ['AM', 'E']:
                pub_info['role'] = 'AM'
                self.header['submittingPublisher']['role'] = 'AM'
            else:
                pub_info['role'] = sub_publisher.get('role')

            if transaction_record.get('submission').get('interestedParties') != None:
                trans_record_ips = transaction_record.get('submission').get(
                    'interestedParties')
                if not self.get_IProle(trans_record_ips):
                    trans_record_ips.append(pub_info)
            return(transaction_record)
        else:
            return (self.transaction.get('submission').get('body'))

    def get_children(self, transaction_type: str):
        def isNaN(num):
            return num != num

        children = self.transaction.get(transaction_type)

        if children == None or isNaN(children):
            return []

        results = []

        for child in children:
            results.append(globals()[transaction_type](
                {transaction_type: child}, self.header).get_body())

        return results

    def get_rejection(self):
        rejection = get_rejection_message(
            self.transaction.get('submission').get('body').get('submission'), 'JSON')
        return rejection

    def get_from_local_range_data(self):
        if 'locallyAllocatedIswc' in self.transaction.keys():
            return {
                'iswc': self.transaction['locallyAllocatedIswc'],
                'disableAddUpdateSwitching': True,
                'allowProvidedIswc': True
            }
        else:
            return {}
