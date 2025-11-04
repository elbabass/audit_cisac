class Ip():
    def __init__(self, name, name_number, role, base_number, affiliation, last_name):
        self.name = name
        self.name_number = name_number
        self.role = role
        self.affiliation = affiliation
        self.base_number = base_number
        self.last_name = last_name


class ImroIps:
    ips = [
        Ip('BOURKE CIARAN FRANCIS', 36303314,
           'C', 'I-000380434-8', '128', 'BOURKE'),
        Ip('O BRIEN LIAM PATRICK', 159837032, 'C',
           'I-000139657-0', '128', 'O BRIEN'),
        Ip('SCHWEPPE EDWARD JOHN', 46932859, 'C',
           'I-001221086-3', '128', 'SCHWEPPE')
    ]


class AkkaIps:
    ips = [
        Ip('KARKLINS ANDRIS', 254535957, 'C', 'I-001626505-5', '122', 'KARKLINS')
    ]


class SonyPublisherIp:
    name = 'SONY/ATV MUSIC PUBLISHING (UK) LIMITED'
    name_number = 269137346
    role = 'E'
    base_number = 'I-001197764-1'
    affiliation = 'Multiple'
