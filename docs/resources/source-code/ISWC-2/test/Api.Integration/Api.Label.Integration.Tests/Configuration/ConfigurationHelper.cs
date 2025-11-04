using Microsoft.Extensions.Configuration;
using System.IO;

namespace SpanishPoint.Azure.Iswc.Api.Label.Integration.Tests.Configuration
{
    public static class ConfigurationHelper
    {
        public static TestConfig GetConfig()
        {
            var conf = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Label.json")
            .AddEnvironmentVariables()
            .AddUserSecrets<TestConfig>(optional: true)
            .Build();

            return new TestConfig
            {
                IswcApiUrl = conf["AzureKeyVaultSecret-ISWC-BaseAddress-IswcApi"],
                IswcAgencyApiUrl = conf["AzureKeyVaultSecret-ISWC-BaseAddress-IswcAgencyApi"],
                MatchingEngineUrl = conf["AzureKeyVaultSecret-ISWC-BaseAddress-SpanishPointMatchingEngine"],
                IswcSecret = conf["AzureKeyVaultSecret-ISWC-Secret-IswcApiManagement"],
                MatchingEngineMicrosoftEntraIDClientID = conf["AzureKeyVaultSecret-ISWC-MicrosoftEntraID-ClientID"],
                MatchingEngineMicrosoftEntraIDClientSecret = conf["AzureKeyVaultSecret-ISWC-Secret-MicrosoftEntraIDClient"],
                MatchingEngineMicrosoftEntraIDScope = conf["AzureKeyVaultSecret-ISWC-MicrosoftEntraID-Scope"],
                CosmosConnectionString = conf["AzureKeyVaultSecret-ISWC-ConnectionString-ISWCCosmosDb"],
                SearchServiceKey = conf["AzureKeyVaultSecret-ISWC-ApiKey-AzureSearch"],
                SearchServiceName = conf["AzureKeyVaultSecret-ISWC-Name-AzureSearch"],
                WorkNumbersIndexer = "work-numbers-indexer-worknumbers",
                WorkNameContributorsPerformersIndexer = "worknames-contributors-performers-indexer-creators"
            };
        }
    }
}