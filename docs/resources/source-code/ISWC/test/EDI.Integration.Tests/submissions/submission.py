from submissions.interested_parties import AkkaIps, ImroIps, SonyPublisherIp
from faker import Faker

fake = Faker()


class _Submission():
    def __init__(self):
        self.submission = None

    def add_performer(self, firstName, lastName):
        if 'performers' not in self.submission.keys():
            self.submission['performers'] = []
        self.submission['performers'].append({
            'firstName': firstName,
            'lastName': lastName
        })

    def add_disambiguated_works(self, iswc_list, disambiguation_reason='DIT'):
        self.submission['disambiguation'] = True
        self.submission['disambiguationReason'] = disambiguation_reason
        self.submission['disambiguateFrom'] = [{"iswc": i} for i in iswc_list]

    def add_derived_from_works(self, iswc_list, derived_work_type='Excerpt'):
        self.submission['derivedWorkType'] = derived_work_type
        self.submission['derivedFromIswcs'] = [{"iswc": i} for i in iswc_list]

    def add_isrc(self, isrc):
        if 'additionalIdentifiers' not in self.submission.keys():
            self.submission['additionalIdentifiers'] = {
                'isrcs': []
            }
        elif 'isrcs' not in self.submission['additionalIdentifiers'].keys():
            self.submission['additionalIdentifiers']['isrcs'] = []
        self.submission['additionalIdentifiers']['isrcs'].append(isrc)

    def add_publisher_identifier(self, publisher_name_number, workcode):
        if 'additionalIdentifiers' not in self.submission.keys():
            self.submission['additionalIdentifiers'] = {}
        self.submission['additionalIdentifiers']['publisherIdentifiers'] = {
            'nameNumber': publisher_name_number,
            "workcode": [workcode]
        }

    def add_instrumentation(self, code):
        if 'instrumentation' not in self.submission.keys():
            self.submission['instrumentation'] = []
        self.submission['instrumentation'].append({
            'code': code
        })

    def add_bvltr(self, bvltr):
        self.submission['bvltr'] = bvltr

    def add_other_title(self, title, type):
        if 'otherTitles' not in self.submission.keys():
            self.submission['otherTitles'] = []
        self.submission['otherTitles'].append({
            'title': title,
            'type': type
        })

    def add_deletion_reason(self):
        self.submission['reasonCode'] = 'Integration Tests'

    def add_merge_iswcs(self, iswcs):
        self.submission['mergeIswcs'] = iswcs

    def get_tsv_allocation_submission(self):
        sub = [
            'addSubmissions',
            str(self.submission['submissionId']),
            '128',
            '128',
            'SONY/ATV MUSIC PUBLISHING (UK) LIMITED',
            '269137346',
            'AM',
            'info@spanishpoint.ie',
            self.submission['workcode'],
            str(self.submission['disambiguation']),
            '' if 'disambiguationReason' not in self.submission.keys()
            else '0',
            '' if 'disambiguateFrom' not in self.submission.keys()
            else '|'.join([x['iswc'] for x in self.submission['disambiguateFrom']]),
            '' if 'bvltr' not in self.submission.keys()
            else self.submission['bvltr'],
            '' if 'performers' not in self.submission.keys()
            else '|'.join([x['firstName'] for x in self.submission['performers']]),
            '' if 'performers' not in self.submission.keys()
            else '|'.join([x['lastName'] for x in self.submission['performers']]),
            '' if 'instrumentation' not in self.submission.keys()
            else '|'.join([x['code'] for x in self.submission['instrumentation']]),
            '' if 'derivedWorkType' not in self.submission.keys()
            else '0', '' if 'derivedFromIswcs' not in self.submission.keys()
            else '|'.join([x['iswc'] for x in self.submission['derivedFromIswcs']]),
            self.submission['originalTitle'] if 'otherTitles' not in self.submission.keys()
            else f"{self.submission['originalTitle']}|{'|'.join([x['title'] for x in self.submission['otherTitles']])}",
            '' if 'additionalIdentifiers' not in self.submission.keys()
            else '|'.join([x for x in self.submission['additionalIdentifiers']['isrcs']])
        ]
        for ip in self.submission['interestedParties']:
            sub.append(ip['name'])
            sub.append(str(ip['nameNumber']))
            sub.append(ip['role'])
        return '\t'.join(sub)

    def get_tsv_resolution_submission(self, submittingAgency='312'):
        sub = [
            'findSubmissions',
            str(self.submission['submissionId']),
            submittingAgency,
            '315',
            'SONY/ATV MUSIC PUBLISHING (UK) LIMITED',
            '269137346',
            'AM',
            'info@spanishpoint.ie',
            self.submission['workcode'],
            str(self.submission['disambiguation']),
            '' if 'disambiguationReason' not in self.submission.keys()
            else '0',
            '' if 'disambiguateFrom' not in self.submission.keys()
            else '|'.join([x['iswc'] for x in self.submission['disambiguateFrom']]),
            '',
            '',
            self.submission['originalTitle'] if 'otherTitles' not in self.submission.keys()
            else f"{self.submission['originalTitle']}|{'|'.join([x['title'] for x in self.submission['otherTitles']])}",
            '' if 'additionalIdentifiers' not in self.submission.keys()
            else '|'.join([x for x in self.submission['additionalIdentifiers']['isrcs']]),
            '',
            ''
            ]
        for ip in self.submission['interestedParties']:
            sub.append(ip['name'])
            sub.append(str(ip['nameNumber']))
            sub.append(ip['role'])
        return '\t'.join(sub)

    def get_json_resolution_submission(self):
        return {
            'workcode': self.submission['workcode'],
            'interestedParties': self.submission['interestedParties'],
            'originalTitle': self.submission['originalTitle'],
            'submissionId': self.submission['submissionId'],
            'additionalIdentifiers': { "publisherIdentifiers" : [] } if 'additionalIdentifiers' not in self.submission.keys() else self.submission['additionalIdentifiers']
        }

    def get_json_allocation_submission(self):
        return {i: self.submission[i] for i in self.submission.keys() if i not in ['agency', 'sourcedb']}

    def get_json_merge_submission(self):
        return {i: self.submission[i] for i in self.submission.keys() if i in ['agency', 'sourcedb', 'workcode', 'submissionId', 'preferredIswc', 'mergeIswcs']}

    def get_json_workcode_search_submission(self):
        return {i: self.submission[i] for i in self.submission.keys() if i in ['submissionId', 'agency', 'sourcedb', 'workcode']}


class EligibleSubmission(_Submission):
    def __init__(self, submission_id):
        _Submission.__init__(self)
        self.submission = new_eligible_iswc_submission(submission_id)


class IneligibleSubmission(_Submission):
    def __init__(self, submission_id):
        _Submission.__init__(self)
        self.submission = new_ineligible_iswc_submission(submission_id)


def new_eligible_iswc_submission(submission_id):
    return {
        "agency": "128",
        "sourcedb": 128,
        "workcode": get_new_workcode(),
        "category": "DOM",
        "disambiguation": False,
        "interestedParties": [{'name': x.name, 'nameNumber': x.name_number, 'role': x.role} for x in ImroIps.ips],
        "originalTitle": get_new_title(),
        "submissionId": submission_id
    }


def new_ineligible_iswc_submission(submission_id):
    return {
        "agency": "128",
        "sourcedb": 128,
        "workcode": get_new_workcode(),
        "category": "DOM",
        "disambiguation": False,
        "interestedParties": [{'name': x.name, 'nameNumber': x.name_number, 'role': x.role} for x in AkkaIps.ips],
        "originalTitle": get_new_title(),
        "submissionId": submission_id
    }


def get_new_title():
    return f"EDI TEST {fake.pystr(min_chars=8, max_chars=10)} {fake.pystr(min_chars=8, max_chars=10)} {fake.pystr(min_chars=8, max_chars=10)}"


def get_new_workcode():
    return fake.pystr(min_chars=12, max_chars=20)


def trim_ip_name_to_lastname(ipName):
    ipNamesSplit = ipName.split()
    return ipNamesSplit[len(ipNamesSplit) - 1]
