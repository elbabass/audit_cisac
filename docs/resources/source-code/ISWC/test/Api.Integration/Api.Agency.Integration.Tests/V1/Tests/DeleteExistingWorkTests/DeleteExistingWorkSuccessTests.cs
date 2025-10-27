using Microsoft.AspNetCore.Http;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using xRetry;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Tests.DeleteExistingWorkTests
{
    public class DeleteExistingWorkSuccessTests_Fixture : IAsyncLifetime
    {
        public IISWC_SubmissionClient submissionClient;
        public IISWC_SearchClient searchClient;
        public HttpClient httpClient;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await TestBase.GetClient();
            submissionClient = new ISWC_SubmissionClient(httpClient);
            searchClient = new ISWC_SearchClient(httpClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            return Task.CompletedTask;
        }
    }

    public class DeleteExistingWorkSuccessTests : TestBase, IAsyncLifetime, IClassFixture<DeleteExistingWorkSuccessTests_Fixture>
    {
        private IISWC_SubmissionClient submissionClient;
        private IISWC_SearchClient searchClient;
        private readonly HttpClient httpClient;
        private Submission submission;

        public DeleteExistingWorkSuccessTests(DeleteExistingWorkSuccessTests_Fixture fixture)
        {
            submissionClient = fixture.submissionClient;
            searchClient = fixture.searchClient;
            httpClient = fixture.httpClient;
        }

        async Task IAsyncLifetime.InitializeAsync()
        {
            submission = Submissions.EligibleSubmissionIMRO;
            submission.InterestedParties.Add(InterestedParties.IP_BMI[0]);
            var res = await submissionClient.AddSubmissionAsync(submission);
            submission.PreferredIswc = res.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            await Task.Delay(4000);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Submitter is eligible
        /// Work is deleted.
        /// </summary>
        [RetryFact]
        public async void DeleteExistingWorkSuccessTests_01()
        {
            await submissionClient.DeleteSubmissionAsync(submission.PreferredIswc, submission.Agency, submission.Workcode, submission.Sourcedb, "IT_06 Test");

            var response = await Assert.ThrowsAsync<ApiException>(
                async () => await searchClient.SearchByAgencyWorkCodeAsync(submission.Agency, submission.Workcode, DetailLevel.Minimal));

            Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);
        }

        /// <summary>
        /// Workcode for submission is deleted, re-used and then deleted again.
        /// </summary>
        [RetryFact]
        public async void DeleteExistingWorkSuccessTests_02()
        {
            await submissionClient.DeleteSubmissionAsync(submission.PreferredIswc, submission.Agency, submission.Workcode, submission.Sourcedb, "IT_06 Test");
            submission.PreferredIswc = string.Empty;
            submission.OriginalTitle = CreateNewTitle();
            var res = await submissionClient.AddSubmissionAsync(submission);
            submission.PreferredIswc = res.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            await submissionClient.DeleteSubmissionAsync(submission.PreferredIswc, submission.Agency, submission.Workcode, submission.Sourcedb, "IT_06 Test");

            var response = await Assert.ThrowsAsync<ApiException>(
                async () => await searchClient.SearchByAgencyWorkCodeAsync(submission.Agency, submission.Workcode, DetailLevel.Minimal));
            Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);
        }

        /// <summary>
        /// Only eligible submission is deleted.
        /// ISWC is deleted as only other submissions are non-eligible.
        /// </summary>
        [RetryFact]
        public async void DeleteExistingWorkSuccessTests_03()
        {
            var subTwo = Submissions.EligibleSubmissionIMRO;
            subTwo.Agency = Submissions.EligibleSubmissionPRS.Agency;
            subTwo.Sourcedb = Submissions.EligibleSubmissionPRS.Sourcedb;
            subTwo.OriginalTitle = submission.OriginalTitle;
            subTwo.InterestedParties = submission.InterestedParties;
            await submissionClient.AddSubmissionAsync(subTwo);
            await WaitForSubmission(subTwo.Agency, subTwo.Workcode, httpClient);

            await submissionClient.DeleteSubmissionAsync(submission.PreferredIswc, submission.Agency, submission.Workcode, submission.Sourcedb, "IT_06 Test");

            var response = await Assert.ThrowsAsync<ApiException>(
                async () => await searchClient.SearchByISWCAsync(submission.PreferredIswc));
            Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);
        }

        /// <summary>
        /// Only eligible submission is deleted.
        /// ISWC is deleted as only other submissions are non-eligible.
        /// One sub is previously eligible
        /// </summary>
        [RetryFact]
        public async void DeleteExistingWorkSuccessTests_04()
        {
            var subTwo = Submissions.EligibleSubmissionBMI;
            subTwo.OriginalTitle = submission.OriginalTitle;
            subTwo.InterestedParties = submission.InterestedParties;
            await submissionClient.AddSubmissionAsync(subTwo);
            await WaitForSubmission(subTwo.Agency, subTwo.Workcode, httpClient);
            var eligibleWorkcode = subTwo.Workcode;

            subTwo.Agency = Submissions.EligibleSubmissionPRS.Agency;
            subTwo.Sourcedb = Submissions.EligibleSubmissionPRS.Sourcedb;
            subTwo.Workcode = CreateNewWorkCode();
            await submissionClient.AddSubmissionAsync(subTwo);
            await WaitForSubmission(subTwo.Agency, subTwo.Workcode, httpClient);

            submission.InterestedParties = InterestedParties.IP_BMI;
            await submissionClient.UpdateSubmissionAsync(submission.PreferredIswc, submission);

            subTwo.Agency = Submissions.EligibleSubmissionBMI.Agency;
            subTwo.Sourcedb = Submissions.EligibleSubmissionBMI.Sourcedb;
            subTwo.Workcode = eligibleWorkcode;
            await submissionClient.DeleteSubmissionAsync(submission.PreferredIswc, subTwo.Agency, subTwo.Workcode, subTwo.Sourcedb, "IT_06 Test");

            var response = await Assert.ThrowsAsync<ApiException>(
                async () => await searchClient.SearchByISWCAsync(submission.PreferredIswc));
            Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);
        }

        /// <summary>
        ///  Delete a Derived From work
        ///  Bug 3882
        /// </summary>
        [Fact]
        public async void DeleteExistingWorkSuccessTests_05()
        {
            var subTwo = Submissions.EligibleSubmissionIMRO;
            subTwo.DerivedWorkType = DerivedWorkType.ModifiedVersion;
            subTwo.DerivedFromIswcs = new List<DerivedFrom>()
            {
                new DerivedFrom() {Iswc = submission.PreferredIswc, Title = submission.OriginalTitle }
            };
            var res = await submissionClient.AddSubmissionAsync(subTwo);
            subTwo.PreferredIswc = res.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(subTwo.Agency, subTwo.Workcode, httpClient);

            await submissionClient.DeleteSubmissionAsync(subTwo.PreferredIswc, subTwo.Agency, subTwo.Workcode, subTwo.Sourcedb, "IT_6_5");
            var response = await Assert.ThrowsAsync<ApiException>(
                async () => await searchClient.SearchByISWCAsync(subTwo.PreferredIswc));
            Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);
        }

        /// <summary>
        /// Submitter eligible
        /// Deleting the only work on an ISWC will delete the ISWC.
        /// </summary>
        [RetryFact]
        public async void DeleteExistingWorkSuccessTests_06()
        {
            await submissionClient.DeleteSubmissionAsync(submission.PreferredIswc, submission.Agency, submission.Workcode, submission.Sourcedb, "IT_06 Test");
            submission.Workcode = CreateNewWorkCode();
            var response = await Assert.ThrowsAsync<ApiException>(
                async () => await submissionClient.AddSubmissionAsync(submission));
            Assert.Equal("117", GetErrorCode(response.Response));
        }
    }
}