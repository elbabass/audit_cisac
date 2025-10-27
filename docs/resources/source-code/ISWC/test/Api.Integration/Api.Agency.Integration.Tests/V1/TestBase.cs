using Azure;
using Azure.Search.Documents.Indexes;
using IdentityModel.Client;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.Configuration;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1
{
    public abstract class TestBase
    {

        public static async Task<HttpClient> GetAgencyClient() => await GetClient(true);

        public static async Task<HttpClient> GetClient(bool agencyClient = false)
        {
            var config = ConfigurationHelper.GetConfig();
            var baseUrl = agencyClient && !string.IsNullOrEmpty(config.IswcAgencyApiUrl) ? config.IswcAgencyApiUrl : config.IswcApiUrl;
            var client = new HttpClient() { BaseAddress = new Uri(baseUrl), Timeout = TimeSpan.FromSeconds(200) };
            var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = new Uri($"{baseUrl}/connect/token").AbsoluteUri,
                ClientId = "iswcapimanagement",
                ClientSecret = config.IswcSecret,
                Scope = "iswcapi",
                Parameters = new Dictionary<string, string> { { "AgentID", "000" } }
            });
            client.SetBearerToken(response.AccessToken);
            return client;
        }

        public static string GetErrorCode(string json)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return JsonSerializer.Deserialize<Rejection>(json, options).Code;
        }

        public static async Task<string> GetValidationParameterValue(string parameterName)
        {
            var config = ConfigurationHelper.GetConfig();
            using var matchingClient = await GetMatchingEngineClient();
            var parameters = await matchingClient
                .GetAsync<IEnumerable<ValidationSetting>>($"{config.MatchingEngineUrl}Settings?sourceName=Global&settingType=Validation");
            var value = parameters.FirstOrDefault(x => x.Name == parameterName);
            return value.Value;
        }

        public static async Task<string> GetMatchingParameterValue(string parameterName)
        {
            var config = ConfigurationHelper.GetConfig();
            using var matchingClient = await GetMatchingEngineClient();
            var parameters = await matchingClient
                .GetAsync<IEnumerable<ValidationSetting>>($"{config.MatchingEngineUrl}Settings?sourceName=Global&settingType=Matching");
            var value = parameters.FirstOrDefault(x => x.Name == parameterName);
            return value.Value;
        }

        public static async Task<HttpClient> GetMatchingEngineClient()
        {
            var config = ConfigurationHelper.GetConfig();
            var baseUrl = config.MatchingEngineUrl;
            var client = new HttpClient() { BaseAddress = new Uri(baseUrl) };
            var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = new Uri($"{baseUrl}connect/token").AbsoluteUri,
                ClientId = "cisacRestApi",
                ClientSecret = config.MatchingEngineSecret,
                Scope = "matchingEngineApi"
            });
            client.SetBearerToken(response.AccessToken);
            return client;
        }

        public static SearchIndexerClient GetSearchServiceIndexerClient()
        {
            var config = ConfigurationHelper.GetConfig();
            var baseUrl = new Uri($"https://{config.SearchServiceName}.search.windows.net");

            var indexerClient = new SearchIndexerClient(baseUrl, new AzureKeyCredential(config.SearchServiceKey));

            return indexerClient;
        }

        public static async Task<bool> IsValidationRuleEnabled(string parameterName)
        {
            var value = await GetValidationParameterValue(parameterName);
            return value == null ? false : bool.TryParse(value, out bool result) ? result : false;
        }

        public static async Task<bool> IsMatchingRuleEnabled(string parameterName)
        {
            var value = await GetMatchingParameterValue(parameterName);
            return value == null ? false : bool.TryParse(value, out bool result) ? result : false;
        }

        private class ValidationSetting
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        private class Rejection
        {
            public string Code { get; set; }
            public string Message { get; set; }
        }

        public static string CreateNewWorkCode()
        {
            string workCode;
            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
            {
                byte[] codeBuffer = new byte[32];
                byte[] numberBuffer = new byte[4];

                rng.GetBytes(numberBuffer);
                int num = BitConverter.ToInt32(numberBuffer, 0);
                int r = new Random(num).Next(10, 19);
                rng.GetBytes(codeBuffer);
                workCode = Convert.ToBase64String(codeBuffer).Substring(0, r).Replace("+", "").Replace("/", "");
            }
            return workCode;
        }

        public static string CreateNewTitle()
        {
            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
            {
                byte[] tokenData = new byte[32];
                var byteArray = new byte[4];

                rng.GetBytes(tokenData);
                var fullString = Convert.ToBase64String(tokenData).Replace("+", "").Replace("/", "");

                rng.GetBytes(byteArray);
                var firstInt = BitConverter.ToUInt32(byteArray, 0);

                rng.GetBytes(byteArray);
                var secondInt = BitConverter.ToUInt32(byteArray, 0);

                return $"INT TEST {fullString.Substring(0, 10)}{firstInt}{fullString.Substring(10, 5)} {fullString.Substring(15, 10)}{secondInt}{fullString.Substring(25, 5)}";
            }

        }

        public static string CreateNewTitleWithLettersOnly()
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string s = "";
            using (RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider())
            {
                while (s.Length != 45)
                {
                    byte[] oneByte = new byte[1];
                    provider.GetBytes(oneByte);
                    char character = (char)oneByte[0];
                    if (valid.Contains(character))
                    {
                        s += character;
                    }
                }
            }
            return $"INT TEST {s.Substring(0, 15)} {s.Substring(15, 15)} {s.Substring(30, 15)}";
        }

        public static async Task<ISWCMetadata> WaitForSubmission(string agency, string workcode, HttpClient httpClient)
        {
            IISWC_SearchClient searchClient = new ISWC_SearchClient(httpClient);
            ISWCMetadata result = new ISWCMetadata();
            for (int i = 0; i < 15; i++)
            {
                try
                {
                    result = (await searchClient.SearchByAgencyWorkCodeAsync(agency, workcode, DetailLevel.Minimal)).FirstOrDefault();
                    if (result.Iswc != null)
                    {
                        await Task.Delay(2000);
                        break;
                    }
                }
                catch
                {
                    await Task.Delay(2000);
                }
            }
            if (result.Iswc is null) throw new Exception($"Added submission not found \n{agency} : {workcode}");
            return result;
        }

        public static async Task<ISWCMetadata> WaitForUpdate(string agency, string workcode, string newTitle, HttpClient httpClient)
        {
            IISWC_SearchClient searchClient = new ISWC_SearchClient(httpClient);
            ISWCMetadata result = new ISWCMetadata();
            int i = 0;
            bool updated = false;
            while (i < 15 && !updated)
            {
                result = (await searchClient.SearchByAgencyWorkCodeAsync(agency, workcode, DetailLevel.Core)).First();
                if (result.OriginalTitle.Equals(newTitle)) updated = true;
                else await Task.Delay(1000);
                i++;
            }
            if (!updated) throw new Exception($"Updated title not found\n{agency}:{workcode} : {newTitle}");
            return result;
        }
    }
}
