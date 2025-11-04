using SpanishPoint.Azure.Iswc.Api.Label.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.Label.Integration.Tests.V1;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Label.Integration.Tests.V1.Tests.SubmitNewWorkTests
{
    /// <summary>
    /// 7. Submit Work Batch
    /// </summary>
    public class SubmitExistingWorkBatchTests : TestBase, IAsyncLifetime
    {
        private IISWC_Label_SubmissionClient client;
        private HttpClient httpClient;
        private HttpClient httpSearchClient;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await GetClient();
            httpSearchClient = await GetAgencyClient();
            client = new ISWC_Label_SubmissionClient(httpClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            httpSearchClient.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async void SubmitNewWorkBatchTests_01()
        {
            var submission = Submissions.EligibleLabelSubmissionPRS;

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

            Assert.Null(submitRes.FirstOrDefault().Rejection);
            batch.FirstOrDefault().Submission.Performers = new List<Performer>
            {
                new Performer { FirstName = "Test", LastName = "Performer" }
            };

            var submitRes2 = await client.AddLabelSubmissionBatchAsync(batch);
            await WaitForSubmission(submission.Agency, submission.Workcode, httpSearchClient);

            Assert.NotNull(submitRes2);
            Assert.Null(submitRes2.FirstOrDefault().Rejection);

            Assert.Equal("Test", submitRes2.FirstOrDefault().Submission.VerifiedSubmission.Performers.FirstOrDefault().FirstName);
            Assert.Equal("Performer", submitRes2.FirstOrDefault().Submission.VerifiedSubmission.Performers.FirstOrDefault().LastName);
        }
    }
}
