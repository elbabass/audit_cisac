using Microsoft.AspNetCore.Http;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Tests.DeleteExistingWorkTests
{

    public class DeleteExistingWorkFailureTests_Fixture : IAsyncLifetime
    {
        public IISWC_SubmissionClient client;
        public Submission submission;
        public HttpClient httpClient;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await TestBase.GetClient();
            client = new ISWC_SubmissionClient(httpClient);
            submission = Submissions.EligibleSubmissionAKKA;
            submission.PreferredIswc = (await client.AddSubmissionAsync(submission)).VerifiedSubmission.Iswc.ToString();
            await TestBase.WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            return Task.CompletedTask;
        }
    }

    public class DeleteExistingWorkFailureTests : TestBase, IClassFixture<DeleteExistingWorkFailureTests_Fixture>
    {
        private readonly IISWC_SubmissionClient client;
        private readonly Submission submission;

        public DeleteExistingWorkFailureTests(DeleteExistingWorkFailureTests_Fixture fixture)
        {
            client = fixture.client;
            submission = fixture.submission;
        }

        /// <summary>
        /// Submitter is ISWC eligible
        /// Preferred ISWC does not exist
        /// </summary>
        [Fact]
        public async void DeleteExistingWorkFailureTests_01()
        {
            var response = await Assert.ThrowsAsync<ApiException>(
                async () => await client.DeleteSubmissionAsync("T2010000015", submission.Agency, submission.Workcode, submission.Sourcedb, "reasonCode"));

            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
            Assert.Equal("117", GetErrorCode(response.Response));
        }


        /// <summary>
        /// Submitter is ISWC eligible
        /// Deletion Reason is not provided
        /// </summary>
        [Fact]
        public async void DeleteExistingWorkFailureTests_02()
        {
            var response = await Assert.ThrowsAsync<ApiException>(
                async () => await client.DeleteSubmissionAsync(submission.PreferredIswc, submission.Agency, submission.Workcode, submission.Sourcedb, ""));

            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
            Assert.Equal("133", GetErrorCode(response.Response));
        }
    }
}
