class TransactionMixinTsv:
    def __init__(self, transaction: list):
        self.transaction = transaction
        self.fields = {
            "recordType": 0,
            "submissionId": 1,
            "submittingAgency": 2,
            "submittingSourcedb": 3,
            "submittingPublisher / name": 4,
            "submittingPublisher / nameNumber": 5,
            "submittingPublisher / role": 6,
            "submittingPublisher / email": 7,
            "workcode": 8,
            "disambiguation": 9,
            "disambiguationReason": 10,
            "disambiguateFrom": 11,
            "bvltr": 12,
            "Performers / firstName": 13,
            "Performers / lastName": 14,
            "Instrumentation": 15,
            "derivedWorkType": 16,
            "derivedFromIswcs": 17,
            "originalTitle": 18,
            "ISRCs": 19,
            "interestedParty1 / name": 20,
            "interestedParty1 / nameNumber": 21,
            "interestedParty1 / role": 22,
        } if transaction[0] == 'addSubmissions' else {
            "recordType": 0,
            "submissionId": 1,
            "submittingAgency": 2,
            "submittingSourcedb": 3,
            "submittingPublisher / name": 4,
            "submittingPublisher / nameNumber": 5,
            "submittingPublisher / role": 6,
            "submittingPublisher / email": 7,
            "workcode": 8,
            "disambiguation": 9,
            "disambiguationReason": 10,
            "disambiguateFrom": 11,
            "derivedWorkType": 12,
            "derivedFromIswcs": 13,
            "originalTitle": 14,
            "additionalIdentifiers / ISRCs": 15,
            "additionalIdentifiers / AgencyWorkCodes": 16,
            "interestedParty1 / name": 17,
            "interestedParty1 / nameNumber": 18,
            "interestedParty1 / role": 19,

        }

    def get_tsv_field(self, field_name):
        if self.fields.get(field_name) is not None:
            field = self.transaction[self.fields.get(field_name)]
            field_list = []
            if field_name in ['originalTitle']:
                return field

            if field_name == "additionalIdentifiers / publisherIdentifiers":
                if (field.__contains__('[') and field.__contains__(',')):
                    for x in field.split('|'):
                        publisherIdentifier = x.split(',')

                        if publisherIdentifier[1] != '':
                            i = 2
                            workcodes = []
                            while i < len(publisherIdentifier):
                                if publisherIdentifier[i] != '':
                                    workcodes.append(publisherIdentifier[i].replace(
                                        '(', '').replace(')', '').replace(']', ''))
                                i = i + 1

                        field_list.append({
                            "nameNumber": publisherIdentifier[0].replace('[', ''),
                            "submitterCode": publisherIdentifier[1].replace(',', ''),
                            "workCode": workcodes}),

                return field_list

            if field != '' and field_name != "additionalIdentifiers / publisherIdentifiers":
                if (field.__contains__('[') == False and field.__contains__('(') and field.__contains__(',')):
                    if field.__contains__('|'):
                        for x in field.split('|'):
                            agency_array = x.split(',')
                            field_list.append({
                                "agency": agency_array[0].replace('(', ''),
                                "workCode": agency_array[1].replace(')', '')})
                        return field_list
                    else:
                        agency_array = field.split(',')
                        field_list.append({
                            "agency": agency_array[0].replace('(', ''),
                            "workCode": agency_array[1].replace(')', '')})
                    return field_list
                if field.__contains__('[') == False and field.__contains__('|'):
                    return field.split('|')
                else:
                    if field_name in ['disambiguateFrom', 'Instrumentation', 'derivedFromIswcs', 'ISRCs', 'additionalIdentifiers / ISRCs', 'Performers / firstName', 'Performers / lastName']:
                        field_list.append(field)
                        return (field_list)
                    else:
                        return field

    def get_IProle(self, ip_list):
        try:
            pub_role = list(
                filter(lambda ip_role: ip_role['role'] in ['AM', 'E'], ip_list))
            return pub_role
        except:
            raise Exception(
                'Error occurred while filtering by role : {0}. Ensure the latest schema version is being used'.format(str(ip_list)))

    def get_tsv_body(self):
        input_body = {
            "submittingPublisher": {
                "name": self.get_tsv_field('submittingPublisher / name'),
                "nameNumber": self.get_tsv_field('submittingPublisher / nameNumber'),
                "email": self.get_tsv_field('submittingPublisher / email'),
                "role": self.get_tsv_field('submittingPublisher / role')
            },
            "submissionId": self.get_tsv_field('submissionId'),
            "submission": {
                "agency": self.get_tsv_field('submittingAgency'),
                "sourcedb": self.get_tsv_field('submittingSourcedb'),
                "workcode": self.get_tsv_field('workcode'),
                "category": 'DOM',
                "additionalIdentifiers": {
                    "publisherIdentifiers": [{
                        "nameNumber": self.get_tsv_field('submittingPublisher / nameNumber'),
                        "workCode": [
                            self.get_tsv_field(
                                'workcode')
                        ]
                    }]
                },
                "interestedParties": [
                    {
                        "name": self.get_tsv_field('interestedParty1 / name'),
                        "nameNumber": self.get_tsv_field('interestedParty1 / nameNumber'),
                        "role": self.get_tsv_field('interestedParty1 / role')
                    }
                ]

            }
        }

        titles = self.get_tsv_field(
            'originalTitle')
        if isinstance(titles, str):
            input_body["submission"]["originalTitle"] = titles
        elif isinstance(titles, list):
            input_body["submission"]["otherTitles"] = []
            for i in range(0, len(titles)):

                if i == 0:
                    input_body["submission"]["originalTitle"] = titles[i]

                else:
                    input_body["submission"]["otherTitles"].append(
                        {
                            "title": titles[i],
                            "type": 'AT'
                        })

        if self.check_for_multiple_ips():
            range_value = 23
            if self.transaction[0] == 'findSubmissions':
                range_value = 20

            for i in range(range_value, len(self.transaction), 3):
                if self.transaction[i] != '':
                    if i+1 > (len(self.transaction)-1):
                        input_body["submission"]["interestedParties"].append({
                            "name": self.transaction[i]
                        })
                    elif i+2 > (len(self.transaction)-1):
                        input_body["submission"]["interestedParties"].append({
                            "name": self.transaction[i],
                            "nameNumber":  self.transaction[i+1]
                        })
                    else:
                        input_body["submission"]["interestedParties"].append({
                            "name": self.transaction[i],
                            "nameNumber":  self.transaction[i+1],
                            "role":  self.transaction[i+2]
                        })

        pub_info = {k: v for k, v in input_body['submittingPublisher'].items(
        ) if not k.startswith('email')}
        if not 'role' in list(pub_info.keys()) or not pub_info['role'] in ['AM', 'E']:
            pub_info['role'] = 'AM'
            input_body['submittingPublisher']["role"] = 'AM'
        if input_body.get('submission').get('interestedParties') != None:
            trans_record_ips = input_body.get('submission').get(
                'interestedParties')
            if not self.get_IProle(trans_record_ips):
                trans_record_ips.append(pub_info)

        if self.get_tsv_field('disambiguation') != None:
            input_body["submission"]["disambiguation"] = self.get_tsv_field(
                'disambiguation')

        if self.get_tsv_field('disambiguationReason') != None:
            input_body["submission"]["disambiguationReason"] = self.get_tsv_field(
                'disambiguationReason')

        if self.get_tsv_field('disambiguateFrom') != None:
            input_body["submission"]["disambiguateFrom"] = []
            [input_body["submission"]["disambiguateFrom"].append(
                {"iswc": iswc_string}) for iswc_string in self.get_tsv_field('disambiguateFrom')]

        if self.get_tsv_field('bvltr') != None:
            input_body["submission"]["bvltr"] = self.get_tsv_field(
                'bvltr')

        if self.get_tsv_field('derivedWorkType') != None:
            input_body["submission"]["derivedWorkType"] = self.get_tsv_field(
                'derivedWorkType')

        if self.get_tsv_field('derivedFromIswcs') != None:
            input_body["submission"]["derivedFromIswcs"] = []
            [input_body["submission"]["derivedFromIswcs"].append(
                {"iswc": iswc_string}) for iswc_string in self.get_tsv_field('derivedFromIswcs')]

        if self.get_tsv_field('Performers / firstName') != None or self.get_tsv_field('Performers / lastName') != None:
            input_body["submission"]["performers"] = []
            firstname_list = self.get_tsv_field(
                'Performers / firstName')
            lastname_list = self.get_tsv_field(
                'Performers / lastName')
            if firstname_list != None and lastname_list == None:
                lastname_list = ['NoLastName']
            value_flag = 0

            if firstname_list != None:
                list_length = len(firstname_list)
            elif lastname_list != None and firstname_list == None:
                list_length = len(lastname_list)
            else:
                list_length = 0

            for i in range(list_length):
                if firstname_list != None:
                    input_body["submission"]["performers"].append(
                        {"firstName": firstname_list[i]})
                else:
                    input_body["submission"]["performers"].append({})

                if lastname_list != None and i <= (len(lastname_list)-1):
                    input_body["submission"]["performers"][i].update(
                        {"lastName": lastname_list[i]})
                    if firstname_list != None and (len(firstname_list) < len(lastname_list)):
                        if i == (len(firstname_list)-1):
                            if i == 0:
                                value_flag = -2
                            elif i > 0:
                                value_flag = i
                elif lastname_list != None and i > (len(lastname_list)-1):
                    pass

                if value_flag > 0 or value_flag == -2:
                    if value_flag == -2:
                        value_flag = 1
                    else:
                        value_flag = value_flag+1
                    for j in range(value_flag, len(lastname_list)):
                        input_body["submission"]["performers"].append(
                            {"lastName": lastname_list[j]})

        if self.get_tsv_field('Instrumentation') != None:
            input_body["submission"]["instrumentation"] = []
            [input_body["submission"]["instrumentation"].append(
                {"code": code_string}) for code_string in self.get_tsv_field('Instrumentation')]

        if self.get_tsv_field('ISRCs') != None:
            input_body["submission"]["additionalIdentifiers"]["isrcs"] = self.get_tsv_field(
                'ISRCs')
        elif self.get_tsv_field('additionalIdentifiers / ISRCs') != None:
            input_body["submission"]["additionalIdentifiers"]["isrcs"] = self.get_tsv_field(
                'additionalIdentifiers / ISRCs')

        if self.get_tsv_field('additionalIdentifiers / AgencyWorkCodes') != None:
            input_body["submission"]["additionalIdentifiers"]["agencyWorkCodes"] = self.get_tsv_field(
                'additionalIdentifiers / AgencyWorkCodes')

        if self.get_tsv_field('additionalIdentifiers / publisherIdentifiers') != None:

            additionalPub = self.get_tsv_field(
                'additionalIdentifiers / publisherIdentifiers')

            if additionalPub != None:
                for pub in additionalPub:
                    input_body["submission"]["additionalIdentifiers"]["publisherIdentifiers"].append(
                        pub)
                    input_body["submission"]["interestedParties"].append({
                        "nameNumber":  pub["nameNumber"],
                        "role": "AM"
                    })

        return input_body

    def check_for_multiple_ips(self):
        if self.transaction[0] == 'addSubmissions' and len(self.transaction) > 23 and self.transaction[23] != '':
            return True
        elif self.transaction[0] == 'findSubmissions' and len(self.transaction) > 20 and self.transaction[20] != '':
            return True
        else:
            return False
