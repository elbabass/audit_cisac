using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Tests.SubmitNewWorkTests
{
    /// <summary>
    /// 7. Submit Work Batch
    /// </summary>
    public class SubmitNewWorkBatchTests : TestBase, IAsyncLifetime
    {
        private IISWC_SubmissionClient client;
        private HttpClient httpClient;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await GetClient();
            client = new ISWC_SubmissionClient(httpClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            return Task.CompletedTask;
        }

        /// <summary>
        /// 7.Submit Work Batch, Partial Success
        /// </summary>
        [Fact]
        public async void SubmitNewWorkBatchTests_01()
        {
            var noIPSub = Submissions.EligibleSubmissionSACEM;
            noIPSub.InterestedParties = new List<InterestedParty>();
            var disambigSub = Submissions.EligibleSubmissionSACEM;
            disambigSub.Disambiguation = true;
            var sub = Submissions.EligibleSubmissionSACEM;

            List<SubmissionBatch> batch = new List<SubmissionBatch>{
            new SubmissionBatch {
                SubmissionId = 1,
                Submission =  noIPSub,
            },
            new SubmissionBatch {
                SubmissionId = 2,
                Submission = disambigSub
            },
            new SubmissionBatch {
                SubmissionId = 3,
                Submission = sub
            },
            new SubmissionBatch
            {
                SubmissionId = 4,
                Submission = sub
            }
        };

            var updateRes = await client.AddSubmissionBatchAsync(batch);

            Assert.Collection(updateRes,
                elem1 => Assert.Equal("104", elem1.Rejection.Code),
                elem2 => Assert.Equal("124", elem2.Rejection.Code),
                elem3 => Assert.NotNull(elem3.Submission.VerifiedSubmission),
                elem4 => Assert.Equal("170", elem4.Rejection.Code));

        }
    }
}
