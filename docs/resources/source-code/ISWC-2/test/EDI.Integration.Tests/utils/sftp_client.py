import json
import paramiko
import os
from utils.utils import Settings


class SftpClient():
    def __init__(self):
        self.settings = Settings()
        self.transport = paramiko.Transport((self.settings.sftp_host, 22))
        self.transport.connect(
            None,
            self.settings.sftp_username,
            self.settings.sftp_password
        )
        self.client = paramiko.SFTPClient.from_transport(self.transport)

    def upload(self, agency, filename):
        remote_path = f'{agency}/In/{filename}'
        local_path = f'files/{filename}'
        self.client.put(local_path, remote_path)
        return remote_path

    def upload_to_publisher_allocation_folder(self, agency, publisher, filename):
        remote_path = f'{agency}/Allocation/{publisher}/In/{filename}'
        local_path = f'files/{filename}'
        self.client.put(local_path, remote_path)
        return remote_path

    def get_ack_file_from_publisher_allocation_directory(self, filename, agency, publisher):
        os.makedirs('output', exist_ok=True)
        self.client.get(
            f'{agency}/Allocation/{publisher}/Out/{filename}', f'output/{filename}')
        return f'output/{filename}'

    def upload_to_publisher_resolution_folder(self, agency, publisher, filename):
        remote_path = f'{agency}/Resolution/{publisher}/In/{filename}'
        local_path = f'files/{filename}'
        self.client.put(local_path, remote_path)
        return remote_path

    def get_ack_file_from_publisher_resolution_directory(self, filename, agency, publisher):
        os.makedirs('output', exist_ok=True)
        self.client.get(
            f'{agency}/Resolution/{publisher}/Out/{filename}', f'output/{filename}')
        return f'output/{filename}'

    def get_ack_file(self, filename, agency):
        os.makedirs('output', exist_ok=True)
        self.client.get(
            f'{agency}/Out/{filename}', f'output/{filename}')
        return f'output/{filename}'

    def is_file_in_publisher_resolution_error_directory(self, filename, agency, publisher):
        return self.__does_file_exist(f'{agency}/Resolution/{publisher}/Error', filename)

    def is_file_in_publisher_resolution_archive_directory(self, filename, agency, publisher):
        return self.__does_file_exist(f'{agency}/Resolution/{publisher}/Archive', filename)

    def is_file_in_publisher_allocation_error_directory(self, filename, agency, publisher):
        return self.__does_file_exist(f'{agency}/Allocation/{publisher}/Error', filename)

    def is_file_in_publisher_allocation_archive_directory(self, filename, agency, publisher):
        return self.__does_file_exist(f'{agency}/Allocation/{publisher}/Archive', filename)

    def is_file_in_agency_archive_directory(self, filename, agency):
        return self.__does_file_exist(f'{agency}/Archive', filename)

    def is_file_in_agency_error_directory(self, filename, agency):
        return self.__does_file_exist(f'{agency}/Error', filename)

    def __does_file_exist(self, path, filename):
        return filename in self.client.listdir(path)

    def close_client(self):
        self.client.close()
        self.transport.close()
