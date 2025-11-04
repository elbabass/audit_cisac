class KeyVaultConfiguration:
    prefix = 'AzureKeyVaultSecret-EDI'

    def __init__(self, dbutils):
        self.dbutils = dbutils

    def get_secret(self, key: str, exclude_prefix: bool = False):
        prefix = '' if exclude_prefix else self.prefix + '-'
        return self.dbutils.secrets.get(scope="keyvault", key=prefix + key)
