using Azure.Search.Documents;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Tests.SubmitNewWorkTests
{

    public class SubmitNewWork_IsrcMatching_Fixture : IAsyncLifetime
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


    public class SubmitNewWork_IsrcMatching : TestBase, IClassFixture<SubmitNewWork_IsrcMatching_Fixture>
    {
        private IISWC_SubmissionClient client;
        private HttpClient httpClient;

        public SubmitNewWork_IsrcMatching(SubmitNewWork_IsrcMatching_Fixture fixture)
        {
            client = fixture.client;
            httpClient = fixture.httpClient;
        }

        /// <summary>
        /// Scenario:Submitter is ISWC eligible and their submission matches the same ISWC by metadata and ISRC.
        /// Expected: New work is created successfully.
        /// </summary>
        [Fact]
        public async void SubmitNewWork_SameMetadataIsrcMatch_ReturnsSuccess()
        {
            var initialMetadataSubmission = Submissions.EligibleSubmissionPRS;
            var initialMetadataSubResult = await client.AddSubmissionAsync(initialMetadataSubmission);

            await WaitForSubmission(initialMetadataSubmission.Agency, initialMetadataSubmission.Workcode, httpClient);

            var initialIsrcSubmission = Submissions.EligibleSubmissionPRS;
            initialIsrcSubmission.AdditionalIdentifiers = AdditionalIdentifersData.AI_SONY;
            initialIsrcSubmission.OriginalTitle = initialMetadataSubmission.OriginalTitle;
            initialIsrcSubmission.InterestedParties = initialMetadataSubmission.InterestedParties;
            var initialIsrcSubResult = await client.AddSubmissionAsync(initialIsrcSubmission);

            await WaitForSubmission(initialIsrcSubmission.Agency, initialIsrcSubmission.Workcode, httpClient);

            var testSubmission = Submissions.EligibleSubmissionPRS;
            testSubmission.OriginalTitle = initialMetadataSubmission.OriginalTitle;
            testSubmission.InterestedParties = initialMetadataSubmission.InterestedParties;
            testSubmission.AdditionalIdentifiers = initialIsrcSubmission.AdditionalIdentifiers;
            var testSubmissionResult = await client.AddSubmissionAsync(testSubmission);

            Assert.NotNull(initialMetadataSubResult.VerifiedSubmission);
            Assert.NotNull(initialIsrcSubResult.VerifiedSubmission);
            Assert.NotNull(testSubmissionResult.VerifiedSubmission);
            Assert.Null(testSubmissionResult.VerifiedSubmission.Rejection);
            Assert.NotNull(testSubmissionResult.VerifiedSubmission.Iswc);
            Assert.Equal(testSubmissionResult.VerifiedSubmission.Iswc, initialMetadataSubResult.VerifiedSubmission.Iswc);
            Assert.Equal(testSubmissionResult.VerifiedSubmission.Iswc, initialIsrcSubResult.VerifiedSubmission.Iswc);
        }

        /// <summary>
        /// Scenario:Submitter is ISWC eligible and their submission matches different ISWCs by metadata and ISRC.
        /// Expected: Submission is rejected with error code 249.
        /// </summary>
        [Fact]
        public async void SubmitNewWork_DifferentMetadataIsrcMatch_ReturnsRejection()
        {
            var initialMetadataSubmission = Submissions.EligibleSubmissionPRS;
            var initialMetadataSubResult = await client.AddSubmissionAsync(initialMetadataSubmission);

            await WaitForSubmission(initialMetadataSubmission.Agency, initialMetadataSubmission.Workcode, httpClient);

            var initialIsrcSubmission = Submissions.EligibleSubmissionPRS;
            initialIsrcSubmission.AdditionalIdentifiers = AdditionalIdentifersData.AI_SONY;
            var initialIsrcSubResult = await client.AddSubmissionAsync(initialIsrcSubmission);

            await WaitForSubmission(initialIsrcSubmission.Agency, initialIsrcSubmission.Workcode, httpClient);

            var testSubmission = Submissions.EligibleSubmissionPRS;
            testSubmission.OriginalTitle = initialMetadataSubmission.OriginalTitle;
            testSubmission.InterestedParties = initialMetadataSubmission.InterestedParties;
            testSubmission.AdditionalIdentifiers = initialIsrcSubmission.AdditionalIdentifiers;
            var testSubmissionResult = await Assert.ThrowsAsync<ApiException>(async () => await client.AddSubmissionAsync(testSubmission));

            Assert.NotNull(initialMetadataSubResult.VerifiedSubmission);
            Assert.NotNull(initialIsrcSubResult.VerifiedSubmission);
            Assert.Equal(StatusCodes.Status400BadRequest, testSubmissionResult.StatusCode);
            Assert.Equal("249", GetErrorCode(testSubmissionResult.Response));
        }
    }
}
