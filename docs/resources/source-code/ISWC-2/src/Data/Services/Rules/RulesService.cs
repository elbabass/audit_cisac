using SpanishPoint.Azure.Iswc.Data.Services.Rules.Entities;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Data.Services.Rules
{
    public interface IRulesService
    {
        Task<IEnumerable<ParameterModel>> GetEnabledRules();
    }

    public class RulesService : IRulesService
    {
        private readonly HttpClient httpClient;

        public RulesService(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient("MatchingApi");
        }

        public async Task<IEnumerable<ParameterModel>> GetEnabledRules()
        {
            var response = await (await httpClient.GetAsync("Settings?sourceName=Global&settingType=Validation")).EnsureSuccessStatusCodeAsync();

            return await response.Content.ReadAsAsync<IEnumerable<ParameterModel>>();
        }
    }
}
