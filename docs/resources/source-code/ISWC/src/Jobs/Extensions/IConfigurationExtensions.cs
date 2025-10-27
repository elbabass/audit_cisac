using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using SpanishPoint.Azure.Iswc.Framework.Configuration.KeyVault;

namespace SpanishPoint.Azure.Iswc.Jobs.Extensions
{
    internal static class IConfigurationExtensions
    {
        public static IConfigurationRoot AddKeyVaultSecrets(this IConfigurationRoot configuration, IConfigurationBuilder configurationBuilder)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(
                    azureServiceTokenProvider.KeyVaultTokenCallback));

            configurationBuilder.AddAzureKeyVault(
                   configuration["AzureKeyVaultBaseURL"],
                    keyVaultClient,
                    new PrefixKeyVaultSecretManager("AzureKeyVaultSecret-ISWC"));

            configuration = configurationBuilder.Build();

            return configuration;

        }
    }
}
