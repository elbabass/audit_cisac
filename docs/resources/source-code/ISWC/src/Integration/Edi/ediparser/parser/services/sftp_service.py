from time import sleep
import errno
import json
from datetime import datetime
from io import BytesIO, StringIO
from pathlib import Path, PurePosixPath
from posixpath import join
from uuid import uuid4

import magic
import paramiko

from ediparser.parser.models.edi_file import EdiFile
from ediparser.parser.models.input.hdr import HDR


class SftpService():
    IN_FOLDER = 'In'
    OUT_FOLDER = 'Out'
    OUT_CSE_FOLDER = 'OutCSE'
    ARCHIVE_FOLDER = 'Archive'
    ERROR_FOLDER = 'Error'
    ALLOCATION_FOLDER = 'Allocation'
    RESOLUTION_FOLDER = 'Resolution'

    def __init__(self, hostname: str, username: str, password: str):
        self.hostname = hostname
        self.username = username
        self.password = password
        self.open()

    def open(self):
        self.transport = paramiko.Transport(self.hostname, 22)
        self.transport.connect(None, self.username, self.password)
        self.transport.banner_timeout = 100
        self.sftp = paramiko.SFTPClient.from_transport(self.transport)

    def get_file(self, file_path: str, file_type: str):
        if file_type.__contains__('JSON'):
            return (file_path, file_type, self.__get_file_json(file_path))
        elif file_type == 'ACK':
            return (file_path, file_type, self.__get_file_edi(file_path))
        elif file_type.__contains__('TSV'):
            return (file_path, file_type, self.__get_file_tsv(file_path))
        elif file_type.__contains__('CSV'):
            return (file_path, file_type, self.__get_file_csv(file_path))
        else:
            raise Exception('File type not supported: ' + file_type)

    def get_list_of_files(self):
        try:
            for customer in self.sftp.listdir_attr('/'):
                if customer.filename == 'In':
                    continue
                files = self.sftp.listdir_attr(join(customer.filename, self.IN_FOLDER))
                files.sort(key=lambda x: x.st_mtime)
                for file in files:
                    file_type = self.__get_file_type(file.filename)
                    file_name = join(customer.filename, self.IN_FOLDER, file.filename)
                    yield (file_name, file_type)
                for publisher in self.sftp.listdir_attr(join(customer.filename, self.ALLOCATION_FOLDER)):
                    files = self.sftp.listdir_attr(join(customer.filename, self.ALLOCATION_FOLDER, publisher.filename, self.IN_FOLDER))
                    files.sort(key=lambda x: x.st_mtime)
                    for file in files:
                        file_type = self.__get_file_type(file.filename)
                        file_name = join(customer.filename, self.ALLOCATION_FOLDER, publisher.filename, self.IN_FOLDER, file.filename)
                        yield(file_name, file_type)

                if self.__check_directory_exists(join(customer.filename, self.RESOLUTION_FOLDER), raise_exception=False):
                    for publisher in self.sftp.listdir_attr(join(customer.filename, self.RESOLUTION_FOLDER)):
                        files = self.sftp.listdir_attr(join(customer.filename, self.RESOLUTION_FOLDER, publisher.filename, self.IN_FOLDER))
                        files.sort(key=lambda x: x.st_mtime)
                        for file in files:
                            file_type = self.__get_file_type(file.filename)
                            file_name = join(customer.filename, self.RESOLUTION_FOLDER, publisher.filename, self.IN_FOLDER, file.filename)
                            yield(file_name, file_type)
        except FileNotFoundError as error:
            error.filename = f'Agency:{customer.filename}; Publisher:{publisher.filename}'
            raise

    def __get_file_edi(self, file_to_open):
        encoding = self.__get_file_encoding(file_to_open)
        with (self.sftp.open(file_to_open, 'rb')) as file:
            for line in file:
                yield line.decode(encoding)

    def __get_file_json(self, file_to_open):
        encoding = self.__get_file_encoding(file_to_open)
        with (self.sftp.open(file_to_open, 'rb')) as file:
            return file.read().decode(encoding)

    def __get_file_tsv(self, file_to_open):
        encoding = self.__get_file_encoding(file_to_open)
        with (self.sftp.open(file_to_open, 'rb')) as file:
            for line in file:
                yield line.decode(encoding)

    def __get_file_csv(self, file_to_open):
        encoding = self.__get_file_encoding(file_to_open)
        with (self.sftp.open(file_to_open, 'rb')) as file:
            for line in file:
                yield line.decode(encoding)

    def __get_file_type(self, file_name: str):
        file_extension = Path(file_name).suffix
        if file_name.startswith('UP'):
            return 'ACK_CSV'
        elif file_extension in ['.010', '.030']:
            return 'ACK'
        elif file_extension in ['.json']:
            return 'ACK_JSON'
        elif file_extension in ['.txt', '.tsv']:
            return 'ACK_TSV'
        else:
            return 'File extension not supported: ' + file_extension

    def __get_file_encoding(self, file_to_open):
        encoding = 'utf-8-sig'
        m = magic.Magic(mime_encoding=True)
        if m.from_buffer(self.sftp.open(file_to_open, 'rb').read()) in ['iso-8859-1', 'unknown-8bit']:
            encoding = 'latin_1'
        return encoding

    def archive_input_file(self, file_name: str):
        archive_file_name = file_name.replace(
            self.IN_FOLDER, self.ARCHIVE_FOLDER)
        self.__move_file(file_name, archive_file_name)

    def error_input_file(self, file_name: str):
        error_file_name = file_name.replace(
            self.IN_FOLDER, self.ERROR_FOLDER)
        self.__move_file(file_name, error_file_name)

    def __move_file(self, oldfile: str, newfile: str):
        if not self.transport.is_active():
            self.open()

        with BytesIO() as fl:
            self.sftp.getfo(oldfile, fl)
            fl.seek(0)
            self.sftp.putfo(fl, newfile)
        self.sftp.remove(oldfile)

    def put_ack_file(self, input_file_name: str, file_name: str, file_contents: list):
        customer = PurePosixPath(input_file_name).parts[0]
        output_path = join(customer, self.OUT_FOLDER, file_name)
        if customer == '300':
            self.__write_file(output_path, file_contents)
            return file_name
        else:
            return self.__write_and_rename(
                customer, file_name, file_contents, input_file_name)

    def put_csn_file(self, customer: str, file_name: str, file_contents: list):
        output_path = join(customer, self.OUT_FOLDER, file_name)
        self.__write_file(output_path, file_contents)

    def put_cse_file(self, customer: str, file_name: str, file_contents: list):
        directory = join(customer, self.OUT_CSE_FOLDER)
        self.__check_directory_exists(directory)

        output_path = join(customer, self.OUT_CSE_FOLDER, file_name)
        self.__write_file(output_path, file_contents)

    def customer_directory_exists(self, customer: str, raise_exception: bool = True):
        directory = join(customer, self.OUT_FOLDER)
        return self.__check_directory_exists(directory, raise_exception)

    def __write_file(self, file_path: str, file_contents: list):
        if not self.transport.is_active():
            self.open()

        if file_path.__contains__('.json'):
            contents = json.dumps(file_contents, ensure_ascii=False)
            self.sftp.putfo(StringIO(contents), file_path, 0, None, False)
        else:
            self.sftp.putfo(StringIO('\n'.join(file_contents)),
                            file_path, 0, None, False)

    def __check_directory_exists(self, directory: str, raise_exception: bool = True) -> bool:
        try:
            self.sftp.stat(directory)
            return True
        except IOError as e:
            if raise_exception and e.errno == errno.ENOENT:
                raise Exception('Directory does not exist: ' + directory)
            return False

    def close(self):
        if self.sftp:
            self.sftp.close()
        if self.transport:
            self.transport.close()

    def __write_and_rename(self, customer, file_name, file_contents, input_file_name):
        path = join(customer, self.OUT_FOLDER)
        input_file_folder = "/".join(PurePosixPath(input_file_name).parts[:-1])
        if input_file_folder.__contains__(self.ALLOCATION_FOLDER) or input_file_folder.__contains__(self.RESOLUTION_FOLDER):
            submission_type = PurePosixPath(input_file_name).parts[1]
            publisher = PurePosixPath(input_file_name).parts[2]
            path = join(customer, submission_type,
                        publisher, self.OUT_FOLDER)

        temp_file_name = str(uuid4()) + file_name[file_name.rfind('.'):]
        temp_output_path = join(path, temp_file_name)

        self.__write_file(temp_output_path, file_contents)

        return self.regenerate_name_if_file_exists(
            temp_output_path, file_name, customer, file_contents, path, input_file_name)

    def regenerate_name_if_file_exists(self, temp_output_path: str, file_name: str, customer: str,
                                       file_contents, path: str, input_file_name: str):
        file_extension = file_name[file_name.rfind('.'):]
        if file_extension == '.json':
            header = file_contents.get('fileHeader')
            file_type = 'ACK_JSON'
        elif file_extension == '.csv':
            header = {}
            file_type = 'ACK_CSV'
        elif file_extension == '.txt':
            header = {}
            file_type = 'ACK_TSV'
        else:
            header = HDR({'HDR': file_contents[0]}).get_body()
            file_type = 'ACK'

        try:
            self.sftp.rename(temp_output_path, join(path, file_name))
        except:
            sleep(1)
            file_name = EdiFile(input_file_name, header, []
                                ).get_ack_file_name(customer, file_extension, file_type)
            file_name = self.regenerate_name_if_file_exists(
                temp_output_path, file_name, customer, file_contents, path, input_file_name)
        finally:
            return file_name
