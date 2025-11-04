from ediparser.parser.mixins.transaction_mixin import TransactionMixin
from ediparser.parser.models.edi_file import EdiFileVersion
from ediparser.parser.models.input.input_row_definition import InputRowDefinition
from ediparser.parser.models.input.input_transaction import InputTransaction
from ediparser.parser.models.input.nat import NAT
from ediparser.parser.models.input.rejection import get_rejection_message


class CUR(TransactionMixin, InputTransaction):
    def __init__(self, transaction, header):
        super().__init__(transaction, header)

    def get_http_verb(self):
        return 'PUT'

    def get_url(self):
        return '/submission/batch'

    def get_parameters(self):
        return {
            'preferredIswc': self.fields.get_field('PreferredISWC')
        }

    def get_body(self):
        body = {
            'agency': self.get_field('AgencyCode'),
            'sourcedb': self.get_field('SourceDBCode'),
            'workcode': self.fields.get_field('AgencyWorkCode'),
            'category': 'DOM',
            'disambiguation': False,
            'originalTitle': self.fields.get_field('WorkTitle'),
            'preferredIswc': self.fields.get_field('PreferredISWC')
        }

        if body['sourcedb'] == '':
            body['sourcedb'] = 0

        body['otherTitles'] = self.get_children('CTL')

        body['interestedParties'] = self.get_children('CIP')

        if self.header['EdiFileVersion'] == EdiFileVersion.THREE:

            body['otherTitles'] = body['otherTitles']+self.get_children('NAT')

            body['performers'] = self.get_children('PER')
            body['instrumentation'] = self.get_children('INS')

            body['disambiguation'] = self.fields.get_field(
                'Disambiguation') == 'T'

            if body['disambiguation']:
                body['disambiguationReason'] = self.get_disambiguation_reason(
                    'DisambiguationReason')
                body['disambiguateFrom'] = self.get_children('DIS')
                if self.get_field('bvltr') != '':
                    body['bvltr'] = self.get_field_label('bvltr')

            if self.get_field('DerivedWorkType') != '':
                body['derivedWorkType'] = self.get_field(
                    'DerivedWorkType')
                body['derivedFromIswcs'] = self.get_children('DER')

        return body

    def get_rejection(self):
        rejection = get_rejection_message(self.get_body())
        return rejection

    def get_field_label(self, fieldname: str):
        key = self.fields.get_field(fieldname)

        return {
            'B': 'Background',
            'L': 'Logo',
            'T': 'Theme',
            'V': 'Visual',
            'R': 'RolledUpCue'
        }.get(key, 0)

    def get_disambiguation_reason(self, field_name: str):
        key = self.fields.get_field(field_name)

        if key in ['DIT', 'DIA', 'DIE', 'DIC', 'DIP', 'DIV']:
            return key
        else:
            return 0
