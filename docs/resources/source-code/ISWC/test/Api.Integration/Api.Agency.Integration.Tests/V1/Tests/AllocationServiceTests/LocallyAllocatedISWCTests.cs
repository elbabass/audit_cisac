using Azure.Search.Documents.Indexes;
using Microsoft.Rest.Azure;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Tests.AllocationServiceTests
{
    public class LocallyAllocatedISWCTests : TestBase, IAsyncLifetime
    {
        private IISWC_SubmissionClient submissionClient;
        private HttpClient httpClient;
        private string iswcForBatchSub1;
        private string iswcForBatchSub2;
        private string iswcForBatchSub4;

        public async Task InitializeAsync()
        {
            httpClient = await GetClient();
            submissionClient = new ISWC_SubmissionClient(httpClient);

            var addSub1 = Submissions.EligibleSubmissionSACEM;
            var addSub2 = Submissions.EligibleSubmissionSACEM;
            var addSub3 = Submissions.EligibleSubmissionSACEM;

            iswcForBatchSub1 = (await submissionClient.AddSubmissionAsync(addSub1)).VerifiedSubmission.Iswc.ToString();
            iswcForBatchSub2 = (await submissionClient.AddSubmissionAsync(addSub2)).VerifiedSubmission.Iswc.ToString();
            iswcForBatchSub4 = (await submissionClient.AddSubmissionAsync(addSub3)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(addSub3.Agency, addSub3.Workcode, httpClient);
            await WaitForSubmission(addSub1.Agency, addSub1.Workcode, httpClient);
            await Task.Delay(4000);

            await submissionClient.DeleteSubmissionAsync(iswcForBatchSub1, addSub1.Agency, addSub1.Workcode, addSub1.Sourcedb, "IT_21 deleting for resuse of ISWC");
            await submissionClient.DeleteSubmissionAsync(iswcForBatchSub4, addSub3.Agency, addSub3.Workcode, addSub3.Sourcedb, "IT_21 deleting for resuse of ISWC");
        }

        public Task DisposeAsync()
        {
            httpClient.Dispose();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Submit Work Batch with locally assigned Iswcs, Partial Success
        /// </summary>
        [Fact]
        public async void LocallyAllocatedISWCTests_01()
        {
            var matchExistingIswcSub = Submissions.EligibleSubmissionSACEM;
            var addedSubmissionResponse = await submissionClient.AddSubmissionAsync(matchExistingIswcSub);
            await WaitForSubmission(matchExistingIswcSub.Agency, matchExistingIswcSub.Workcode, httpClient);

            matchExistingIswcSub.Workcode = CreateNewWorkCode();
            matchExistingIswcSub.Iswc = iswcForBatchSub1;
            matchExistingIswcSub.AllowProvidedIswc = true;
            matchExistingIswcSub.DisableAddUpdateSwitching = true;

            var locallyAllocatedIswcAlreadyExistsSub = Submissions.EligibleSubmissionSACEM;
            locallyAllocatedIswcAlreadyExistsSub.AllowProvidedIswc = true;
            locallyAllocatedIswcAlreadyExistsSub.DisableAddUpdateSwitching = true;
            locallyAllocatedIswcAlreadyExistsSub.Iswc = iswcForBatchSub2;

            var workCodeAlreadyExistsSub = Submissions.EligibleSubmissionSACEM;
            workCodeAlreadyExistsSub.AllowProvidedIswc = true;
            workCodeAlreadyExistsSub.DisableAddUpdateSwitching = true;
            workCodeAlreadyExistsSub.Workcode = addedSubmissionResponse.VerifiedSubmission.Workcode;

            var newIswcSub = Submissions.EligibleSubmissionSACEM;
            newIswcSub.AllowProvidedIswc = true;
            newIswcSub.DisableAddUpdateSwitching = true;
            newIswcSub.Iswc = iswcForBatchSub4;

            var reservedRangeIswcSub = Submissions.EligibleSubmissionSACEM;
            reservedRangeIswcSub.AllowProvidedIswc = true;
            reservedRangeIswcSub.DisableAddUpdateSwitching = true;
            reservedRangeIswcSub.Iswc = "T3990009906";

            List<SubmissionBatch> batch = new List<SubmissionBatch>
            {
                new SubmissionBatch
                {
                    SubmissionId = 1,
                    Submission = matchExistingIswcSub
                },
                new SubmissionBatch
                {
                    SubmissionId = 2,
                    Submission = locallyAllocatedIswcAlreadyExistsSub
                },
                new SubmissionBatch
                {
                    SubmissionId = 3,
                    Submission = workCodeAlreadyExistsSub
                },
                new SubmissionBatch
                {
                    SubmissionId = 4,
                    Submission = newIswcSub
                },
                new SubmissionBatch
                {
                    SubmissionId = 5,
                    Submission = reservedRangeIswcSub
                }
            };

            var res = await submissionClient.AddSubmissionBatchAsync(batch);

            Assert.Collection(res,
                elem1 => Assert.Equal(iswcForBatchSub1, elem1.Submission.VerifiedSubmission.ArchivedIswc),
                elem2 => Assert.Equal("169", elem2.Rejection.Code),
                elem3 => Assert.Equal("168", elem3.Rejection.Code),
                elem4 => Assert.Equal(iswcForBatchSub4, elem4.Submission.VerifiedSubmission.Iswc),
                elem5 => Assert.Equal("171", elem5.Rejection.Code));

        }
    }
}
