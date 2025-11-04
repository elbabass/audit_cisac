from ediparser.parser.models.input.input_validator import err_106, err_112, err_123, err_125, err_126, err_134, err_135, err_139
import ediparser.parser.models.input.input_validator as input_validator


def get_rejection_message(submission: dict, file_type=None):
    ips = submission.get('interestedParties')
    titles = submission.get('otherTitles')
    performers = submission.get('performers')

    if submission.get('disambiguation') == True:
        if submission.get('disambiguationReason') == 0:
            return create_rejection(err_123)

        if submission.get('bvltr') == 0:
            return create_rejection(err_125)

    if performers != None:
        for performer in performers:
            performer_name = input_validator.validate_field_values(
                'PerformerName', performer, str)
            if performer_name == err_126:
                return create_rejection(err_126)

    if ips != None:
        for ip in ips:
            if file_type == 'JSON' or file_type == 'TSV':
                ip['role'] = input_validator.validate_field_values(
                    'IPRole', ip.get('role'), str)
                ip['nameNumber'] = input_validator.validate_field_values(
                    'IPNameNumber', ip.get('nameNumber'), int)

            if str(ip.get('nameNumber')) == err_134:
                if submission.get('additionalIdentifiers') is not None:
                    for publisherIdentifier in submission['additionalIdentifiers']['publisherIdentifiers']:
                        publisherIdentifier['nameNumber'] = 0
                return create_rejection(err_134)
            elif ip.get('role') == err_106:
                return create_rejection(err_106)
            elif ip.get('role') == err_139:
                return create_rejection(err_139)

    if titles != None:
        for title in titles:
            if file_type == 'JSON' or file_type == 'TSV':
                title['type'] = input_validator.validate_field_values(
                    'TitleType', title.get('type'), str)

            if title.get('type') == err_112:
                return create_rejection(err_112)
            elif title.get('type') == err_135:
                return create_rejection(err_135)

    return None


def create_rejection(code):
    return {
        'code': get_error_code(code),
        'message': get_error_message(code)
    }


def get_error_message(code):
    return {
        '106': 'IP Role Code is required',
        '112': 'Work Title Type is required',
        '123': 'Invalid disambiguation reason code',
        '125': 'BLTVR must be blank or contain the letters B, L, T, V or R',
        '134': 'IP Name Number is invalid, must be numeric',
        '135': 'Work Title Type is invalid',
        '139': 'IP Role Code is invalid',
        '126': 'Performer information must contain a Second Name or a Second Name and a First Name'
    }.get(code)


def get_error_code(code: str):
    return '_'+code
