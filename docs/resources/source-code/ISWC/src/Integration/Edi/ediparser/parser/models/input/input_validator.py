from enum import Enum

err_106, err_112, err_123, err_125, err_126, err_134, err_135, err_139 = '106', '112', '123', '125', '126', '134', '135', '139'


def validate_field_values(field_name, field_value, of_type):
    # IV_20
    if field_name == 'IPNameNumber':
        return iv_20(field_value, of_type)

    # IV_22
    elif field_name == 'IPRole':
        return iv_22(field_value, of_type)

    # IV_31
    elif field_name == 'TitleType':
        return iv_31(field_value, of_type)

    # IV_40
    elif field_name == 'PerformerName':
        return iv_40(field_value, of_type)
    else:
        return of_type(field_value)


def iv_20(field_value, of_type):
    try:
        if field_value != None:
            of_type(field_value)
            return of_type(field_value)
        raise ValueError
    except ValueError:
        return of_type(err_134)


def iv_22(field_value, of_type):
    if field_value == None or field_value == '':
        return of_type(err_106)
    elif field_value not in ip_roles_enum.__members__ and field_value not in performer_roles_enum.__members__:
        return of_type(err_139)
    else:
        return of_type(field_value)


def iv_31(field_value, of_type):
    if field_value == None or field_value == '':
        return of_type(err_112)
    elif field_value not in title_types_enum.__members__:
        return of_type(err_135)
    else:
        return of_type(field_value)


def iv_40(field_value, of_type):
    if field_value.get('firstName') != None and field_value.get('lastName') == 'NoLastName':
        return of_type(err_126)


class title_types_enum(Enum):
    CT = 1,
    OT = 2,
    RT = 3,
    AT = 4,
    ET = 5,
    ST = 6,
    TO = 7,
    OA = 8,
    TE = 9,
    FT = 10,
    IT = 11,
    TT = 12,
    PT = 13,
    OL = 14,
    AL = 15


class ip_roles_enum(Enum):
    SR = 1,
    SA = 2,
    TR = 3,
    PA = 4,
    ES = 5,
    CA = 6,
    AD = 7,
    AR = 8,
    SE = 9,
    C = 10,
    A = 11,
    E = 12,
    AQ = 13,
    AM = 14,
    MA = 15

class performer_roles_enum(Enum):
    IN = 1,
    PR = 2,
    PER = 3