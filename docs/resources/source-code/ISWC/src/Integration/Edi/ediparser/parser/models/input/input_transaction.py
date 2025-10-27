import random
from datetime import datetime
from time import sleep
from abc import ABC, abstractmethod
import hashlib


class InputTransaction(ABC):

    @abstractmethod
    def get_http_verb():
        pass

    @abstractmethod
    def get_url():
        pass

    @abstractmethod
    def get_parameters():
        pass

    @abstractmethod
    def get_body():
        pass

    def get_field(self, field_name):
        return self.fields.get_field(field_name)

    def generate_workcode(self, submitted_workcode, submission):

        def generate_hash(submitted_workcode, submission):
            string_to_hash = "{}{}{}".format(submitted_workcode, str(submission), get_ticks())
            blake_hash = hashlib.blake2b(string_to_hash.encode(), digest_size=9).hexdigest()
            return blake_hash.upper()

        def get_ticks():
            dt = datetime.utcnow()
            return int((dt - datetime(1, 1, 1)).total_seconds() * 10000000)

        return "AS{}".format(generate_hash(submitted_workcode, submission))
