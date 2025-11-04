using Microsoft.AspNetCore.Http;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using xRetry;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Tests.MergeTests
{
    public class MergeTwoIswcsFailureTests_Fixture : IAsyncLifetime
    {
        public IISWC_SubmissionClient submissionClient;
        public IISWC_MergeClient mergeclient;
        public HttpClient httpClient;
        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await TestBase.GetClient();
            submissionClient = new ISWC_SubmissionClient(httpClient);
            mergeclient = new ISWC_MergeClient(httpClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            return Task.CompletedTask;
        }
    }

    public class MergeTwoIswcsFailureTests : TestBase, IClassFixture<MergeTwoIswcsFailureTests_Fixture>
    {
        private readonly IISWC_MergeClient mergeclient;
        private readonly IISWC_SubmissionClient submissionClient;
        private readonly HttpClient httpClient;

        public MergeTwoIswcsFailureTests(MergeTwoIswcsFailureTests_Fixture fixture)
        {
            mergeclient = fixture.mergeclient;
            submissionClient = fixture.submissionClient;
            httpClient = fixture.httpClient;
        }

        /// <summary>
        /// Submitter is ISWC eligible
        /// Preferred ISWCs do not exist
        /// </summary>
        [Fact]
        public async void MergeTwoIswcsFailureTests_01()
        {
            var preferredIswc = "T2010000117";
            var body = new Body
            {
                Iswcs = new string[] { "T2010000117" }
            };

            var res = await Assert.ThrowsAsync<ApiException>(async () => await mergeclient.MergeISWCMetadataAsync(preferredIswc, "003", body));

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal("202", GetErrorCode(res.Response));
        }

        /// <summary>
        /// Submitter is not ISWC eligible for either ISWC.
        /// Preferred ISWCs exist
        /// </summary>
        [RetryFact]
        public async void IT_10_2_MergeTwoIswcsFailure()
        {
            var sub = Submissions.EligibleSubmissionAEPI;
            var iswcOne = (await submissionClient.AddSubmissionAsync(sub)).VerifiedSubmission.Iswc.ToString();
            var subTwo = Submissions.EligibleSubmissionAKKA;
            var iswcTwo = (await submissionClient.AddSubmissionAsync(subTwo)).VerifiedSubmission.Iswc.ToString();
            var body = new Body
            {
                Iswcs = new string[] { iswcOne }
            };
            await WaitForSubmission(sub.Agency, sub.Workcode, httpClient);
            await WaitForSubmission(subTwo.Agency, subTwo.Workcode, httpClient);
            var response = await Assert.ThrowsAsync<ApiException>(
               async () => await mergeclient.MergeISWCMetadataAsync(iswcTwo, "124", body));
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
            Assert.Equal("150", GetErrorCode(response.Response));
        }


        /// <summary>
        /// ParentWork agency is eligible on both works.
        /// ChildWork agency is eligible on child work only.
        /// ChildWork agency does not have a submission on either ISWC.
        /// Merge as ChildWork agency is not eligible.
        /// </summary>
        [Fact]
        public async void MergeTwoIswcsFailureTests_02()
        {
            var parentWork = Submissions.EligibleSubmissionASCAP;
            var iswcOne = (await submissionClient.AddSubmissionAsync(parentWork)).VerifiedSubmission.Iswc.ToString();
            var childWork = Submissions.EligibleSubmissionASCAP;
            childWork.InterestedParties.Add(InterestedParties.IP_BMI.First());
            var iswcTwo = (await submissionClient.AddSubmissionAsync(childWork)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(childWork.Agency, childWork.Workcode, httpClient);

            var body = new Body
            {
                Iswcs = new string[] { iswcTwo }
            };

            var response = await Assert.ThrowsAsync<ApiException>(
                async () => await mergeclient.MergeISWCMetadataAsync(iswcOne, Submissions.EligibleSubmissionBMI.Agency, body));
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
            Assert.Equal("150", GetErrorCode(response.Response));
        }

        /// <summary>
        /// ChildWork agency is eligible on child work only but has no submission.
        /// ChildWork agency has an inelligible submission on the parent ISWC.
        /// Merge as ChildWork agency is not eligible as eligibility is not recalculated.
        /// </summary>
        [Fact]
        public async void MergeTwoIswcsFailureTests_03()
        {
            var parentWork = Submissions.EligibleSubmissionASCAP;
            var iswcOne = (await submissionClient.AddSubmissionAsync(parentWork)).VerifiedSubmission.Iswc.ToString();
            var childWork = Submissions.EligibleSubmissionASCAP;
            childWork.InterestedParties.Add(InterestedParties.IP_BMI.First());
            var iswcTwo = (await submissionClient.AddSubmissionAsync(childWork)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(childWork.Agency, childWork.Workcode, httpClient);

            parentWork.Workcode = CreateNewWorkCode();
            parentWork.Agency = Submissions.EligibleSubmissionBMI.Agency;
            parentWork.Sourcedb = Submissions.EligibleSubmissionBMI.Sourcedb;
            await submissionClient.AddSubmissionAsync(parentWork);
            await WaitForSubmission(parentWork.Agency, parentWork.Workcode, httpClient);

            var body = new Body
            {
                Iswcs = new string[] { iswcTwo }
            };

            var response = await Assert.ThrowsAsync<ApiException>(
                async () => await mergeclient.MergeISWCMetadataAsync(iswcOne, Submissions.EligibleSubmissionBMI.Agency, body));
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
            Assert.Equal("150", GetErrorCode(response.Response));
        }

        /// <summary>
        /// BMI is eligible for both ISWC's but has no submissions on either.
        /// Merge is rejected.
        /// </summary>
        [Fact]
        public async void MergeTwoIswcsFailureTests_04()
        {
            var parentWork = Submissions.EligibleSubmissionASCAP;
            parentWork.InterestedParties.Add(InterestedParties.IP_BMI.First());
            var iswcOne = (await submissionClient.AddSubmissionAsync(parentWork)).VerifiedSubmission.Iswc.ToString();
            var childWork = Submissions.EligibleSubmissionASCAP;
            childWork.InterestedParties.Add(InterestedParties.IP_BMI.First());
            var iswcTwo = (await submissionClient.AddSubmissionAsync(childWork)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(childWork.Agency, childWork.Workcode, httpClient);

            var body = new Body
            {
                Iswcs = new string[] { iswcTwo }
            };

            var response = await Assert.ThrowsAsync<ApiException>(
                async () => await mergeclient.MergeISWCMetadataAsync(iswcOne, Submissions.EligibleSubmissionBMI.Agency, body));
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
            Assert.Equal("150", GetErrorCode(response.Response));
        }
    }
}
