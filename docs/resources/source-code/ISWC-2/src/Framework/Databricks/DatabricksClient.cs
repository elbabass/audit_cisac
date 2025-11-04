using Microsoft.Extensions.Options;
using SpanishPoint.Azure.Iswc.Framework.Databricks.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Framework.Databricks
{
    [ExcludeFromCodeCoverage]
    public class DatabricksClient : IDatabricksClient
    {
        private readonly HttpClient httpClient;
        private readonly DatabricksClientOptions options;

        public DatabricksClient(IHttpClientFactory httpClientFactory, IOptions<DatabricksClientOptions> options)
        {
            this.options = options.Value;
            httpClient = httpClientFactory.CreateClient("DatabricksJobApi");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.options.BearerToken);
        }

        public async Task<IEnumerable<Run>> GetActiveRuns() => (await (await httpClient
                .GetAsync($"runs/list?active_only=true"))
                .EnsureSuccessStatusCode()
                .Content
                .ReadAsAsync<RunsListResponseModel>())
                .Runs;

        public async Task SubmitJob(SubmitJobRequestModel jobRequestModel)
        {
            jobRequestModel.JobID = options.JobID;

            (await httpClient.PostAsJsonAsync("run-now", jobRequestModel)).EnsureSuccessStatusCode();
        }
    }
}
