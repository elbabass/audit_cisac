using Microsoft.AspNetCore.Http;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Tests.DemergeTests
{
    public class DemergeIswcsFailureTests_Fixture : IAsyncLifetime
    {
        public IISWC_SubmissionClient submissionClient;
        public IISWC_MergeClient mergeclient;
        public Submission parentSubmission;
        public Submission mergeSubmission;
        public HttpClient httpClient;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await TestBase.GetClient();
            mergeclient = new ISWC_MergeClient(httpClient);
            submissionClient = new ISWC_SubmissionClient(httpClient);
            parentSubmission = Submissions.EligibleSubmissionASCAP;
            parentSubmission.PreferredIswc = (await submissionClient.AddSubmissionAsync(parentSubmission)).VerifiedSubmission.Iswc.ToString();

            mergeSubmission = Submissions.EligibleSubmissionASCAP;
            mergeSubmission.PreferredIswc = (await submissionClient.AddSubmissionAsync(mergeSubmission)).VerifiedSubmission.Iswc.ToString();
            await TestBase.WaitForSubmission(mergeSubmission.Agency, mergeSubmission.Workcode, httpClient);
            await TestBase.WaitForSubmission(parentSubmission.Agency, parentSubmission.Workcode, httpClient);
            var body = new Body
            {
                Iswcs = new string[] { mergeSubmission.PreferredIswc }
            };

            await mergeclient.MergeISWCMetadataAsync(parentSubmission.PreferredIswc, parentSubmission.Agency, body);
            await Task.Delay(2000);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            return Task.CompletedTask;
        }
    }

    public class DemergeIswcsFailureTests : TestBase, IClassFixture<DemergeIswcsFailureTests_Fixture>
    {
        private readonly IISWC_MergeClient mergeclient;
        private readonly Submission parentSubmission;
        private readonly Submission mergeSubmission;
        private readonly IISWC_SubmissionClient submissionClient;
        private readonly HttpClient httpClient;

        public DemergeIswcsFailureTests(DemergeIswcsFailureTests_Fixture fixture)
        {
            parentSubmission = fixture.parentSubmission;
            mergeSubmission = fixture.mergeSubmission;
            mergeclient = fixture.mergeclient;
            submissionClient = fixture.submissionClient;
            httpClient = fixture.httpClient;
        }

        /// <summary>
        /// Submitter is ISWC eligible
        /// Preferred ISWCs do not exist
        /// </summary>
        [Fact]
        public async void DemergeIswcsFailureTests_01()
        {
            var response = await Assert.ThrowsAsync<ApiException>(
                async () => await mergeclient.DemergeISWCMetadataAsync("T2010000015", parentSubmission.Agency, parentSubmission.Workcode));

            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
            Assert.Equal("153", GetErrorCode(response.Response));
        }


        /// <summary>
        /// Submitter is not ISWC eligible (for at least one work)
        /// Preferred ISWCs exist
        /// </summary>
        [Fact]
        public async void DemergeIswcsFailureTests_02()
        {
            var response = await Assert.ThrowsAsync<ApiException>(
                async () => await mergeclient.DemergeISWCMetadataAsync(parentSubmission.PreferredIswc, Submissions.EligibleSubmissionAEPI.Agency, mergeSubmission.Workcode));

            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
            Assert.Equal("150", GetErrorCode(response.Response));
        }


        /// <summary>
        /// Demerge agency is elligible on parent but has no submission.
        /// Demerge agency has an inelligible submission on the child.
        /// </summary>
        [Fact]
        public async void DemergeIswcsFailureTests_03()
        {
            var parentWork = Submissions.EligibleSubmissionBMI;
            parentWork.InterestedParties.Add(InterestedParties.IP_IMRO.First());
            var parentIswc = (await submissionClient.AddSubmissionAsync(parentWork)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(parentWork.Agency, parentWork.Workcode, httpClient);
            var childWork = Submissions.EligibleSubmissionBMI;
            var childIswc = (await submissionClient.AddSubmissionAsync(childWork)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(childWork.Agency, childWork.Workcode, httpClient);
            childWork.Agency = Submissions.EligibleSubmissionIMRO.Agency;
            childWork.Sourcedb = Submissions.EligibleSubmissionIMRO.Sourcedb;
            childWork.Workcode = CreateNewWorkCode();
            await submissionClient.AddSubmissionAsync(childWork);
            await WaitForSubmission(childWork.Agency, childWork.Workcode, httpClient);

            var body = new Body
            {
                Iswcs = new string[] { childIswc }
            };

            await mergeclient.MergeISWCMetadataAsync(parentIswc, parentWork.Agency, body);
            await Task.Delay(2000);

            var response = await Assert.ThrowsAsync<ApiException>(
               async () => await mergeclient.DemergeISWCMetadataAsync(parentIswc, Submissions.EligibleSubmissionIMRO.Agency, childWork.Workcode));

            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
            Assert.Equal("150", GetErrorCode(response.Response));
        }
    }
}
