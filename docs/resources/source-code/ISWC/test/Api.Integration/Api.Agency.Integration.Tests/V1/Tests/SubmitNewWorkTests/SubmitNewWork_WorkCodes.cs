using Azure.Search.Documents;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Http;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Tests.SubmitNewWorkTests
{

    public class SubmitNewWork_WorkCodes_Fixture : IAsyncLifetime
    {
        public IISWC_SubmissionClient client;
        public HttpClient httpClient;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await TestBase.GetClient();
            client = new ISWC_SubmissionClient(httpClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            return Task.CompletedTask;
        }
    }


    public class SubmitNewWork_WorkCodes : TestBase, IClassFixture<SubmitNewWork_WorkCodes_Fixture>
    {
        private IISWC_SubmissionClient client;
        private HttpClient httpClient;

        public SubmitNewWork_WorkCodes(SubmitNewWork_WorkCodes_Fixture fixture)
        {
            client = fixture.client;
            httpClient = fixture.httpClient;
        }

        /// <summary>
        /// Scenario:Submitter is ISWC eligible and provides society work code. 
        /// Expected: New work is created successfully.
        /// </summary>
        [Fact]
        public async void SubmitNewWork_WithSocietyWorkCode_ReturnsSuccess()
        {
            var submission = Submissions.EligibleSubmissionBMI;

            var result = await client.AddSubmissionAsync(submission);

            Assert.NotNull(result.VerifiedSubmission);
            Assert.Null(result.VerifiedSubmission.Rejection);
            Assert.NotNull(result.VerifiedSubmission.Iswc);
        }

        /// <summary>
        /// Scenario:Submitter is ISWC eligible and does not provide society work code. 
        /// Expected: Submission is rejected with error code 136.
        /// </summary>
        [Fact]
        public async void SubmitNewWork_WithoutSocietyWorkCode_ReturnsRejection()
        {
            var submission = Submissions.EligibleSubmissionBMI;
            submission.Workcode = string.Empty;
            var result = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(submission));

            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal("136", GetErrorCode(result.Response));
        }
    }
}
