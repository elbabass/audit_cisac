using SpanishPoint.Azure.Iswc.Api.Label.Integration.Tests.V1;
using SpanishPoint.Azure.Iswc.Api.Label.Integration.Tests.V1.Data;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using IISWC_Allocation_and_ResolutionClient = SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.IISWC_Allocation_and_ResolutionClient;
using ISWC_Allocation_and_ResolutionClient = SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.ISWC_Allocation_and_ResolutionClient;
using IISWC_SearchClient = SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.IISWC_SearchClient;
using ISWC_SearchClient = SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.ISWC_SearchClient;
using System.Data;

namespace SpanishPoint.Azure.Iswc.Api.Integration.Tests.Label.V1.Tests.ResolutionServiceTests
{
    public class ResolveWorkBatchTests : TestBase, IAsyncLifetime
    {
        public IISWC_Label_SubmissionClient client;
        public IISWC_SearchClient searchClient;
        public IISWC_Allocation_and_ResolutionClient resolutionClient;
        public HttpClient httpClient;
        public HttpClient httpSearchClient;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await GetClient();
            httpSearchClient = await GetAgencyClient();
            client = new ISWC_Label_SubmissionClient(httpClient);
            searchClient = new ISWC_SearchClient(httpSearchClient);
            resolutionClient = new ISWC_Allocation_and_ResolutionClient(httpSearchClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            httpSearchClient.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async void ProvisionalIswcIgnoredByResolutionService()
        {
            var submission = Submissions.EligibleLabelSubmissionSESAC;

            List<SubmissionBatch> batch = new List<SubmissionBatch>
            {
                new SubmissionBatch
                {
                    SubmissionId = 1,
                    Submission =  submission,
                }
            };

            var submitRes = await client.AddLabelSubmissionBatchAsync(batch);
            await WaitForSubmission(submission.Agency, submission.Workcode, httpSearchClient);

            var resSubmission = new Api.Agency.Integration.Tests.V1.Submission
            {
                Sourcedb = 315,
                Agency = "312",
                Workcode = CreateNewWorkCode(),
                PreferredIswc = submitRes.First().Submission.VerifiedSubmission.Iswc.ToString(),
                OriginalTitle = submission.OriginalTitle,
                InterestedParties = new List<Api.Agency.Integration.Tests.V1.InterestedParty>
                {
                    new Api.Agency.Integration.Tests.V1.InterestedParty { Name = "FREDDIE", LastName = "MERCURY", NameNumber = 00077406269, Role = Api.Agency.Integration.Tests.V1.InterestedPartyRole.CA }
                }
            };

            List<Api.Agency.Integration.Tests.V1.SubmissionBatch> resBatch = new List<Agency.Integration.Tests.V1.SubmissionBatch>
            {
                new Agency.Integration.Tests.V1.SubmissionBatch
                {
                    SubmissionId = 1,
                    Submission =  resSubmission,
                }
            };

            var resolutionResult = await resolutionClient.AddResolutionBatchAsync(resBatch);

            Assert.NotNull(resolutionResult.First().Rejection);
            Assert.Equal("163", resolutionResult.First().Rejection.Code);
        }
    }
}
