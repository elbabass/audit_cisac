using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Tests.DemergeTests
{
    public class DemergeIswcsSuccessfullyTests_Fixture : IAsyncLifetime
    {
        public IISWC_SubmissionClient submissionClient;
        public IISWC_SearchClient searchclient;
        public IISWC_MergeClient mergeclient;
        public HttpClient httpClient;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await TestBase.GetClient();
            mergeclient = new ISWC_MergeClient(httpClient);
            searchclient = new ISWC_SearchClient(httpClient);
            submissionClient = new ISWC_SubmissionClient(httpClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            return Task.CompletedTask;
        }
    }

    public class DemergeIswcsSuccessfullyTests : TestBase, IClassFixture<DemergeIswcsSuccessfullyTests_Fixture>
    {
        private IISWC_MergeClient mergeclient;
        private IISWC_SearchClient searchclient;
        private IISWC_SubmissionClient submissionClient;
        private HttpClient httpClient;

        public DemergeIswcsSuccessfullyTests(DemergeIswcsSuccessfullyTests_Fixture fixture)
        {
            mergeclient = fixture.mergeclient;
            searchclient = fixture.searchclient;
            submissionClient = fixture.submissionClient;
            httpClient = fixture.httpClient;
        }

        /// <summary>
        /// Submitter is ISWC eligible
        /// Preferred ISWCs exist
        /// </summary>
        [Fact]
        public async void DemergeIswcsSuccessfullyTests_01()
        {
            var parentSubmission = Submissions.EligibleSubmissionBMI;
            var mergeSubmission = Submissions.EligibleSubmissionBMI;
            parentSubmission.PreferredIswc = (await submissionClient.AddSubmissionAsync(parentSubmission))
                .VerifiedSubmission.Iswc.ToString();
            mergeSubmission.PreferredIswc = (await submissionClient.AddSubmissionAsync(mergeSubmission))
                .VerifiedSubmission.Iswc.ToString();
            var body = new Body
            {
                Iswcs = new string[] { mergeSubmission.PreferredIswc }
            };
            await WaitForSubmission(mergeSubmission.Agency, mergeSubmission.Workcode, httpClient);
            await WaitForSubmission(parentSubmission.Agency, parentSubmission.Workcode, httpClient);

            await mergeclient.MergeISWCMetadataAsync(parentSubmission.PreferredIswc, parentSubmission.Agency, body);
            await Task.Delay(2000);

            await mergeclient.DemergeISWCMetadataAsync(parentSubmission.PreferredIswc, parentSubmission.Agency, mergeSubmission.Workcode);
            await Task.Delay(2000);

            var child = await searchclient.SearchByISWCAsync(mergeSubmission.PreferredIswc);
            var parent = await searchclient.SearchByISWCAsync(parentSubmission.PreferredIswc);

            Assert.Null(child.ParentISWC);
            Assert.Equal(parentSubmission.OriginalTitle, parent.OriginalTitle);
        }

        /// <summary>
        /// Demerge is submitted by parent agency under IncludeAgenciesInEligibilityCheck.
        /// IP's belong to a child agency.
        /// </summary>
        [Fact]
        public async void DemergeIswcsSuccessfullyTests_02()
        {
            var parentSubmission = Submissions.EligibleSubmissionECAD;
            var mergeSubmission = Submissions.EligibleSubmissionECAD;
            parentSubmission.PreferredIswc = (await submissionClient.AddSubmissionAsync(parentSubmission))
                .VerifiedSubmission.Iswc.ToString();
            mergeSubmission.PreferredIswc = (await submissionClient.AddSubmissionAsync(mergeSubmission))
                .VerifiedSubmission.Iswc.ToString();
            var body = new Body
            {
                Iswcs = new string[] { mergeSubmission.PreferredIswc }
            };
            await WaitForSubmission(mergeSubmission.Agency, mergeSubmission.Workcode, httpClient);
            await WaitForSubmission(parentSubmission.Agency, parentSubmission.Workcode, httpClient);
            await mergeclient.MergeISWCMetadataAsync(parentSubmission.PreferredIswc, parentSubmission.Agency, body);
            await Task.Delay(2000);

            await mergeclient.DemergeISWCMetadataAsync(parentSubmission.PreferredIswc, parentSubmission.Agency, mergeSubmission.Workcode);
            await Task.Delay(2000);

            var response = await searchclient.SearchByISWCAsync(mergeSubmission.PreferredIswc);

            Assert.Null(response.ParentISWC);
        }

        /// <summary>
        /// Merges IswcTwo into IswcOne and then updates the child work.
        /// Demerges and ensures that the child is unchanged.
        /// </summary>
        [Fact]
        public async void DemergeIswcsSuccessfullyTests_03()
        {
            var sub = Submissions.EligibleSubmissionASCAP;
            var iswcOne = (await submissionClient.AddSubmissionAsync(sub)).VerifiedSubmission.Iswc.ToString();
            sub.Workcode = CreateNewWorkCode();
            sub.OriginalTitle = CreateNewTitle();
            var originalChildOt = sub.OriginalTitle;
            var iswcTwo = (await submissionClient.AddSubmissionAsync(sub)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(sub.Agency, sub.Workcode, httpClient);

            var body = new Body
            {
                Iswcs = new string[] { iswcTwo }
            };

            await mergeclient.MergeISWCMetadataAsync(iswcOne, sub.Agency, body);
            await Task.Delay(2000);

            sub.OriginalTitle += "123";
            sub.InterestedParties.Add(InterestedParties.IP_BMI[0]);
            var res = await submissionClient.UpdateSubmissionAsync(iswcOne, sub);
            await WaitForUpdate(sub.Agency, sub.Workcode, sub.OriginalTitle, httpClient);

            await mergeclient.DemergeISWCMetadataAsync(iswcOne, sub.Agency, sub.Workcode);
            await Task.Delay(2000);

            var searchRes = await searchclient.SearchByISWCAsync(iswcTwo);

            Assert.Equal(originalChildOt, searchRes.OriginalTitle);
            Assert.DoesNotContain(searchRes.InterestedParties, x => x.NameNumber.Equals(InterestedParties.IP_BMI[0].NameNumber));
        }

        /// <summary>
        /// Merges IswcTwo into IswcOne.
        /// Demerges and ensures that the child can be updated.
        /// </summary>
        [Fact]
        public async void DemergeIswcsSuccessfullyTests_04()
        {
            var sub = Submissions.EligibleSubmissionASCAP;
            var iswcOne = (await submissionClient.AddSubmissionAsync(sub)).VerifiedSubmission.Iswc.ToString();
            sub.Workcode = CreateNewWorkCode();
            sub.OriginalTitle = CreateNewTitle();
            var iswcTwo = (await submissionClient.AddSubmissionAsync(sub)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(sub.Agency, sub.Workcode, httpClient);

            var body = new Body
            {
                Iswcs = new string[] { iswcTwo }
            };

            await mergeclient.MergeISWCMetadataAsync(iswcOne, sub.Agency, body);
            await Task.Delay(2000);


            await mergeclient.DemergeISWCMetadataAsync(iswcOne, sub.Agency, sub.Workcode);
            await Task.Delay(2000);

            sub.OriginalTitle += "123";
            sub.InterestedParties.Add(InterestedParties.IP_BMI[0]);
            var res = await submissionClient.UpdateSubmissionAsync(iswcTwo, sub);
            var searchRes = await WaitForUpdate(sub.Agency, sub.Workcode, sub.OriginalTitle, httpClient);

            Assert.Equal(sub.OriginalTitle, searchRes.OriginalTitle);
            Assert.Contains(searchRes.InterestedParties, x => x.NameNumber.Equals(InterestedParties.IP_BMI[0].NameNumber));
        }


        /// <summary>
        /// ParentWork agency is eligible on both works.
        /// ChildWork agency is eligible on child work only.
        /// Can demerge as ChildWork agency.
        /// </summary>
        [Fact]
        public async void DemergeIswcsSuccessfullyTests_05()
        {
            var parentWork = Submissions.EligibleSubmissionASCAP;
            var parentIswc = (await submissionClient.AddSubmissionAsync(parentWork)).VerifiedSubmission.Iswc.ToString();
            var childWork = Submissions.EligibleSubmissionBMI;
            childWork.InterestedParties = Submissions.EligibleSubmissionASCAP.InterestedParties;
            childWork.InterestedParties.Add(InterestedParties.IP_BMI.First());
            var childIswc = (await submissionClient.AddSubmissionAsync(childWork)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(childWork.Agency, childWork.Workcode, httpClient);

            var body = new Body
            {
                Iswcs = new string[] { childIswc }
            };

            await mergeclient.MergeISWCMetadataAsync(parentIswc, childWork.Agency, body);
            await Task.Delay(2000);

            await mergeclient.DemergeISWCMetadataAsync(parentIswc, childWork.Agency, childWork.Workcode);
            await Task.Delay(2000);

            var res = await searchclient.SearchByISWCAsync(parentIswc);

            Assert.Equal(0, res.LinkedISWC.Count);
        }

        /// <summary>
        /// ParentWork agency is elligible on parent work only.
        /// ChildWork agency is eligible on child work only.
        /// ChildWork agency has an inelligible submission on parent.
        /// Demerge as ChildWork agency is eligible.
        /// </summary>
        [Fact]
        public async void DemergeIswcsSuccessfullyTests_06()
        {
            var parentWork = Submissions.EligibleSubmissionBMI;
            var parentIswc = (await submissionClient.AddSubmissionAsync(parentWork)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(parentWork.Agency, parentWork.Workcode, httpClient);
            parentWork.Agency = Submissions.EligibleSubmissionASCAP.Agency;
            parentWork.Sourcedb = Submissions.EligibleSubmissionASCAP.Sourcedb;
            parentWork.Workcode = CreateNewWorkCode();
            await submissionClient.AddSubmissionAsync(parentWork);
            await WaitForSubmission(parentWork.Agency, parentWork.Workcode, httpClient);
            var childWork = Submissions.EligibleSubmissionASCAP;
            var childIswc = (await submissionClient.AddSubmissionAsync(childWork)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(childWork.Agency, childWork.Workcode, httpClient);

            var body = new Body
            {
                Iswcs = new string[] { childIswc }
            };

            await mergeclient.MergeISWCMetadataAsync(parentIswc, childWork.Agency, body);
            await Task.Delay(2000);

            await mergeclient.DemergeISWCMetadataAsync(parentIswc, childWork.Agency, childWork.Workcode);
            await Task.Delay(2000);

            var res = await searchclient.SearchByISWCAsync(parentIswc);

            Assert.Equal(0, res.LinkedISWC.Count);
        }
    }
}
