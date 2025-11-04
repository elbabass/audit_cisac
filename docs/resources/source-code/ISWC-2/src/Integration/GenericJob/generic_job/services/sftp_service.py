import paramiko
from posixpath import join
import errno
from posixpath import join
from io import BytesIO, StringIO


class SftpService():
    IN_FOLDER = 'In'
    OUT_FOLDER = 'Out'
    OUT_CSE_FOLDER = 'OutCSE'
    ARCHIVE_FOLDER = 'Archive'
    ERROR_FOLDER = 'Error'
    REPORTS_FOLDER = 'Reports'

    def __init__(self, hostname: str, username: str, password: str):
        self.hostname = hostname
        self.username = username
        self.password = password
        self.open()

    def open(self):
        self.transport = paramiko.Transport(self.hostname, 22)
        self.transport.connect(None, self.username, self.password)
        self.sftp = paramiko.SFTPClient.from_transport(self.transport)

    def close(self):
        if self.sftp:
            self.sftp.close()
        if self.transport:
            self.transport.close()

    def put_file(self, customer: str, file_name: str, file_contents):  # :str
        directory = join(customer, self.REPORTS_FOLDER)
        self.__check_directory_exists(directory)
        output_path = join(customer, self.REPORTS_FOLDER, file_name)
        self.__write_file(output_path, file_contents)

    def __check_directory_exists(self, directory: str):
        try:
            self.sftp.stat(directory)
        except IOError as e:
            if e.errno == errno.ENOENT:
                raise Exception('Directory does not exist: ' + directory)

    def __write_file(self, file_path: str, file_contents):
        self.sftp.putfo(file_contents, file_path, 0, None, False)
