using SpanishPoint.Azure.Iswc.Api.Label.Integration.Tests.V1;
using SpanishPoint.Azure.Iswc.Api.Label.Integration.Tests.V1.Data;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using ApiException = SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.ApiException;
using IISWC_SearchClient = SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.IISWC_SearchClient;
using ISWC_SearchClient = SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.ISWC_SearchClient;

namespace SpanishPoint.Azure.Iswc.Api.Integration.Tests.Label.V1.Tests.PortalTests
{
    public class PortalSearchTests : TestBase, IAsyncLifetime
    {
        public IISWC_Label_SubmissionClient client;
        public IISWC_SearchClient searchClient;
        public HttpClient httpClient;
        public HttpClient httpSearchClient;
        public readonly List<Submission> submissions = new List<Submission>();

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await GetClient();
            httpSearchClient = await GetAgencyClient();
            client = new ISWC_Label_SubmissionClient(httpClient);
            searchClient = new ISWC_SearchClient(httpSearchClient);

            var submission = Submissions.EligibleLabelSubmissionPRS;

            List<SubmissionBatch> batch = new List<SubmissionBatch>
            {
                new SubmissionBatch
                {
                    SubmissionId = 1,
                    Submission =  submission,
                }
            };

            batch[0].Submission.PreferredIswc = (await client.AddLabelSubmissionBatchAsync(batch)).First().Submission.VerifiedSubmission.Iswc.ToString();
            submissions.Add(batch[0].Submission);

            await TestBase.WaitForSubmission(submission.Agency, submission.Workcode, httpSearchClient);

        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            httpSearchClient.Dispose();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Iswc search through public portal should not return provisional iswc
        /// </summary>
        [Fact]
        public async void CannotSearchProvisionalIswcThroughPublicPortal()
        {

            httpSearchClient.DefaultRequestHeaders.Add("Public-Portal", "true");
            searchClient = new ISWC_SearchClient(httpSearchClient);

            try
            {
                var searchResult = await searchClient.SearchByISWCAsync(submissions[0].PreferredIswc);
            }
            catch (ApiException e)
            {
                Assert.Equal(404, e.StatusCode);
            }
        }
    }
}
