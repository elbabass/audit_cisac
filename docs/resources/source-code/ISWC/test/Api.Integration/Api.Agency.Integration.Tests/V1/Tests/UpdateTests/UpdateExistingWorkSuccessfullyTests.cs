using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using xRetry;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Tests.UpdateTests
{
    public class UpdateExistingWorkSuccessfullyTests_Fixture : IAsyncLifetime
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
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// 2. Update existing ISWC eligible work successfully
    /// </summary>
    public class UpdateExistingWorkSuccessfullyTests : TestBase, IAsyncLifetime, IClassFixture<UpdateExistingWorkSuccessfullyTests_Fixture>
    {
        private IISWC_SubmissionClient client;
        private Submission currentSubmission;
        private IISWC_SearchClient searchClient;
        private HttpClient httpClient;

        public UpdateExistingWorkSuccessfullyTests(UpdateExistingWorkSuccessfullyTests_Fixture fixture)
        {
            httpClient = fixture.httpClient;
            client = fixture.submissionClient;
            searchClient = fixture.searchClient;
        }

        async Task IAsyncLifetime.InitializeAsync()
        {
            currentSubmission = Submissions.EligibleSubmissionBMI;
            var res = await client.AddSubmissionAsync(currentSubmission);
            currentSubmission.PreferredIswc = res.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(currentSubmission.Agency, currentSubmission.Workcode, httpClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 2.4 Update title on the work
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_01()
        {
            var newTitle = CreateNewTitle();
            currentSubmission.OtherTitles = new List<Title> {
                new Title { Title1 = newTitle, Type = TitleType.AT }
            };

            var updateRes = await client.UpdateSubmissionAsync(currentSubmission.PreferredIswc, currentSubmission);

            Assert.True(updateRes.VerifiedSubmission.OtherTitles.Where(x => x.Title1 == newTitle && x.Type == TitleType.AT).Count() > 0);
        }

        /// <summary>
        /// 2.8 Update work returns matches for Work Number and other possible matches
        /// for updated metadata found within system
        /// </summary>
        [RetryFact]
        public async void UpdateExistingWorkSuccessfullyTests_02()
        {
            var iswc = currentSubmission.PreferredIswc;
            currentSubmission.PreferredIswc = string.Empty;
            var workcode = currentSubmission.Workcode;
            currentSubmission.Workcode = CreateNewWorkCode();
            currentSubmission.InterestedParties = InterestedParties.IP_BMI;
            await client.AddSubmissionAsync(currentSubmission);
            currentSubmission.PreferredIswc = iswc;
            currentSubmission.Workcode = workcode;
            await WaitForSubmission(currentSubmission.Agency, currentSubmission.Workcode, httpClient);
            await Task.Delay(8000);
            var updateRes = await client.UpdateSubmissionAsync(currentSubmission.PreferredIswc, currentSubmission);
            Assert.True(updateRes.PotentialMatches.Count() > 1);
        }

        /// <summary>
        /// 2.9 Update work returns matches for Work Number when no preferred iswc 
        /// added to submission
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_03()
        {
            var iswc = currentSubmission.PreferredIswc;
            currentSubmission.OriginalTitle += "a";
            currentSubmission.PreferredIswc = string.Empty;
            var updateRes = await client.UpdateSubmissionAsync(currentSubmission.PreferredIswc, currentSubmission);
            currentSubmission.PreferredIswc = iswc;
            Assert.True(updateRes.VerifiedSubmission.Workcode.Equals(currentSubmission.Workcode));
            Assert.True(updateRes.VerifiedSubmission.Iswc.ToString().Equals(iswc));
        }

        /// <summary>
        /// Updated metadata matches another ISWC.
        /// Submitter is not iswc eligible but has a work on both ISWC's
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_04()
        {
            var subTwo = Submissions.EligibleSubmissionBMI;
            subTwo.InterestedParties = currentSubmission.InterestedParties;
            await client.AddSubmissionAsync(subTwo);
            await WaitForSubmission(subTwo.Agency, subTwo.Workcode, httpClient);

            currentSubmission.Agency = Submissions.EligibleSubmissionSACEM.Agency;
            currentSubmission.Sourcedb = Submissions.EligibleSubmissionSACEM.Sourcedb;
            currentSubmission.Workcode = CreateNewWorkCode();
            await client.AddSubmissionAsync(currentSubmission);
            await WaitForSubmission(currentSubmission.Agency, currentSubmission.Workcode, httpClient);

            subTwo.Agency = Submissions.EligibleSubmissionSACEM.Agency;
            subTwo.Sourcedb = Submissions.EligibleSubmissionSACEM.Sourcedb;
            subTwo.Workcode = CreateNewWorkCode();
            await client.AddSubmissionAsync(subTwo);
            await WaitForSubmission(subTwo.Agency, subTwo.Workcode, httpClient);

            subTwo.OriginalTitle = currentSubmission.OriginalTitle;
            var updateRes = await client.UpdateSubmissionAsync(string.Empty, subTwo);

            Assert.Equal(currentSubmission.PreferredIswc, updateRes.VerifiedSubmission.Iswc.ToString());
        }

        /// <summary>
        /// Eligible submitter adds an IP affiliated with another agency in update.
        /// Now eligible agency can update their originally non-eligible submission.
        /// Bug 3493
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_05()
        {
            var workCodeOne = currentSubmission.Workcode;

            currentSubmission.Agency = Submissions.EligibleSubmissionAEPI.Agency;
            currentSubmission.Sourcedb = Submissions.EligibleSubmissionAEPI.Sourcedb;
            currentSubmission.Workcode = CreateNewWorkCode();
            var workCodeTwo = currentSubmission.Workcode;
            await client.AddSubmissionAsync(currentSubmission);
            await WaitForSubmission(currentSubmission.Agency, currentSubmission.Workcode, httpClient);

            currentSubmission.Agency = Submissions.EligibleSubmissionBMI.Agency;
            currentSubmission.Sourcedb = Submissions.EligibleSubmissionBMI.Sourcedb;
            currentSubmission.Workcode = workCodeOne;
            currentSubmission.InterestedParties.Add(InterestedParties.IP_AEPI[0]);
            await client.UpdateSubmissionAsync(currentSubmission.PreferredIswc, currentSubmission);
            await Task.Delay(2000);

            currentSubmission.Agency = Submissions.EligibleSubmissionAEPI.Agency;
            currentSubmission.Sourcedb = Submissions.EligibleSubmissionAEPI.Sourcedb;
            currentSubmission.Workcode = workCodeTwo;
            currentSubmission.OriginalTitle = CreateNewTitle();
            var updateRes = await client.UpdateSubmissionAsync(currentSubmission.PreferredIswc, currentSubmission);

            Assert.Null(updateRes.VerifiedSubmission.Rejection);
            Assert.Equal(currentSubmission.OriginalTitle, updateRes.VerifiedSubmission.OriginalTitle);
        }

        /// <summary>
        /// Submitting agency is non-eligible
        /// Submission metadata matches current ISWC level metadata
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_06()
        {
            var eligibleWorkcode = currentSubmission.Workcode;
            currentSubmission.Agency = Submissions.EligibleSubmissionIMRO.Agency;
            currentSubmission.Sourcedb = Submissions.EligibleSubmissionIMRO.Sourcedb;
            currentSubmission.Workcode = CreateNewWorkCode();
            await client.AddSubmissionAsync(currentSubmission);
            await WaitForSubmission(currentSubmission.Agency, currentSubmission.Workcode, httpClient);
            var ineligibleWorkcode = currentSubmission.Workcode;

            currentSubmission.Agency = Submissions.EligibleSubmissionBMI.Agency;
            currentSubmission.Sourcedb = Submissions.EligibleSubmissionBMI.Sourcedb;
            currentSubmission.Workcode = eligibleWorkcode;
            currentSubmission.OriginalTitle = CreateNewTitle();
            await client.UpdateSubmissionAsync(currentSubmission.PreferredIswc, currentSubmission);
            await WaitForUpdate(currentSubmission.Agency, currentSubmission.Workcode, currentSubmission.OriginalTitle, httpClient);

            currentSubmission.Agency = Submissions.EligibleSubmissionIMRO.Agency;
            currentSubmission.Sourcedb = Submissions.EligibleSubmissionIMRO.Sourcedb;
            currentSubmission.Workcode = ineligibleWorkcode;

            var res = await client.UpdateSubmissionAsync(currentSubmission.PreferredIswc, currentSubmission);
            Assert.Null(res.VerifiedSubmission.Rejection);
        }

        /// <summary>
        /// Preffered ISWC is another matching ISWC.
        /// Submitter is ISWC eligible.
        /// Updated ISWC is merged.
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_07()
        {
            var subTwo = Submissions.EligibleSubmissionBMI;
            subTwo.InterestedParties = currentSubmission.InterestedParties;
            subTwo.PreferredIswc = (await client.AddSubmissionAsync(subTwo)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(subTwo.Agency, subTwo.Workcode, httpClient);

            subTwo.PreferredIswc = subTwo.PreferredIswc;
            subTwo.OriginalTitle = currentSubmission.OriginalTitle;
            await client.UpdateSubmissionAsync(subTwo.PreferredIswc, subTwo);
            await WaitForUpdate(subTwo.Agency, subTwo.Workcode, currentSubmission.OriginalTitle, httpClient);

            var searchRes = await searchClient.SearchByISWCAsync(subTwo.PreferredIswc);
            Assert.Equal(currentSubmission.PreferredIswc, searchRes.ParentISWC);
        }

        /// <summary>
        /// CUR is changed to CAR as workcode does not exist
        /// Agency is ISWC eligible
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_08()
        {
            currentSubmission.Workcode = CreateNewWorkCode();
            var res = await client.UpdateSubmissionAsync(currentSubmission.PreferredIswc, currentSubmission);

            Assert.Equal(currentSubmission.PreferredIswc, res.VerifiedSubmission.Iswc.ToString());
            Assert.Equal(currentSubmission.Workcode, res.VerifiedSubmission.Workcode);
        }

        /// <summary>
        /// CUR is changed to CAR as workcode does not exist
        /// Agency is not ISWC eligible
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_09()
        {
            currentSubmission.Workcode = CreateNewWorkCode();
            currentSubmission.Agency = Submissions.EligibleSubmissionSACEM.Agency;
            currentSubmission.Sourcedb = Submissions.EligibleSubmissionSACEM.Sourcedb;

            var res = await client.UpdateSubmissionAsync(currentSubmission.PreferredIswc, currentSubmission);

            Assert.Equal(currentSubmission.PreferredIswc, res.VerifiedSubmission.Iswc.ToString());
            Assert.Equal(currentSubmission.Workcode, res.VerifiedSubmission.Workcode);
        }

        /// <summary>
        /// Updating a submission that is re-using a deleted workcode
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_10()
        {
            var sub = Submissions.EligibleSubmissionBMI;
            var deleteIswc = (await client.AddSubmissionAsync(sub)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(sub.Agency, sub.Workcode, httpClient);
            await client.DeleteSubmissionAsync(deleteIswc, sub.Agency, sub.Workcode, sub.Sourcedb, " IT_2_16_UpdateExistingWorkSuccessfully");

            sub.OriginalTitle = CreateNewTitle();
            var addRes = await client.AddSubmissionAsync(sub);
            sub.PreferredIswc = addRes.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(sub.Agency, sub.Workcode, httpClient);
            sub.OriginalTitle = CreateNewTitle();
            var updateRes = await client.UpdateSubmissionAsync(sub.PreferredIswc, sub);

            Assert.Null(updateRes.VerifiedSubmission.Rejection);
            Assert.NotNull(updateRes.VerifiedSubmission.Iswc);
        }

        /// <summary>
        /// Can update a derived from work
        /// Check that derivedFrom is not duplicated per Bug 4076
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_11()
        {
            var derivedSub = Submissions.EligibleSubmissionBMI;
            derivedSub.DerivedWorkType = DerivedWorkType.Excerpt;
            derivedSub.DerivedFromIswcs = new List<DerivedFrom>()
            {
                new DerivedFrom { Iswc = currentSubmission.PreferredIswc }
            };

            var derivedAddRes = await client.AddSubmissionAsync(derivedSub);
            derivedSub.PreferredIswc = derivedAddRes.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(derivedSub.Agency, derivedSub.Workcode, httpClient);

            derivedSub.OriginalTitle += "a";
            var res = await client.UpdateSubmissionAsync(derivedSub.PreferredIswc, derivedSub);

            Assert.True(res.VerifiedSubmission.DerivedFromIswcs.Count == 1);
        }

        /// <summary>
        /// Updating a work so that it is an exact match on AT to another ISWC.
        /// OT is below 70% similarity.
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_12()
        {
            var submission = Submissions.EligibleSubmissionBMI;
            var at = CreateNewTitleWithLettersOnly();
            var ot = CreateNewTitleWithLettersOnly();
            submission.OriginalTitle = ot;
            submission.OtherTitles = new List<Title>()
            {
                new Title{Title1 = at, Type = TitleType.AT}
            };

            var iswcOne = (await client.AddSubmissionAsync(submission)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            submission.Workcode = CreateNewWorkCode();
            submission.OriginalTitle = CreateNewTitleWithLettersOnly();
            submission.OtherTitles.Clear();

            var iswcTwo = (await client.AddSubmissionAsync(submission)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            submission.OriginalTitle = CreateNewTitleWithLettersOnly();
            submission.OtherTitles = new List<Title>()
            {
                new Title{Title1 = at, Type = TitleType.AT}
            };
            submission.PreferredIswc = iswcTwo;

            var update = await client.UpdateSubmissionAsync(iswcTwo, submission);
            Assert.Equal(0, update.LinkedIswcs.Count);
            Assert.Equal(1, update.PotentialMatches.Count);
            Assert.Equal(iswcTwo, update.VerifiedSubmission.Iswc.ToString());
        }

        /// <summary>
        /// Strip the specified special characters from the submission OT.
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_13()
        {
            var submission = Submissions.EligibleSubmissionSACEM;
            submission.PreferredIswc = (await client.AddSubmissionAsync(submission)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            submission.OriginalTitle += " ab\u0001";
            var res = await client.UpdateSubmissionAsync(submission.PreferredIswc, submission);
            Assert.Equal(submission.OriginalTitle.Replace("\u0001", string.Empty), res.VerifiedSubmission.OriginalTitle);
        }

        /// <summary>
        /// Update is successful if EnableChecksumValidation rule is enabled but EnableChecksumValidation=0 for the Agency.
        /// AEPI (003) should always have EnableChecksumValidation disabled in Lookup.Agency
        /// </summary>
        [SkippableFact]
        public async void UpdateExistingWorkSuccessfullyTests_14()
        {
            Skip.IfNot(await IsValidationRuleEnabled("EnableChecksumValidation"), "EnableChecksumValidation is set to FALSE.");
            var submission = Submissions.EligibleSubmissionAEPI;
            submission.PreferredIswc = (await client.AddSubmissionAsync(submission)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            var res = await client.UpdateSubmissionAsync(submission.PreferredIswc, submission);

            Assert.Equal(submission.PreferredIswc, res.VerifiedSubmission.Iswc.ToString());
        }

        /// <summary>
        /// SubmissionDetailLevel Full returns LinkedISWCs
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_15()
        {
            var mergeClient = new ISWC_MergeClient(httpClient);
            var parentSubmission = Submissions.EligibleSubmissionAEPI;
            var parentIswc = (await client.AddSubmissionAsync(parentSubmission)).VerifiedSubmission.Iswc.ToString();

            var childSubmission = Submissions.EligibleSubmissionAEPI;
            var childIswc = (await client.AddSubmissionAsync(childSubmission)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(childSubmission.Agency, childSubmission.Workcode, httpClient);
            var body = new Body
            {
                Iswcs = new string[] { childIswc }
            };
            await mergeClient.MergeISWCMetadataAsync(parentIswc, childSubmission.Agency, body);
            await Task.Delay(3000);

            parentSubmission.DetailLevel = SubmissionDetailLevel.Full;
            parentSubmission.OriginalTitle = CreateNewTitle();
            parentSubmission.PreferredIswc = parentIswc;
            var res = await client.UpdateSubmissionAsync(parentIswc, parentSubmission);

            Assert.True(res.LinkedIswcs.Count == 1);
        }

        /// <summary>
        /// SubmissionDetailLevel Core retdoes not return LinkedISWCs
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_16()
        {
            var mergeClient = new ISWC_MergeClient(httpClient);
            var parentSubmission = Submissions.EligibleSubmissionAEPI;
            var parentIswc = (await client.AddSubmissionAsync(parentSubmission)).VerifiedSubmission.Iswc.ToString();

            var childSubmission = Submissions.EligibleSubmissionAEPI;
            var childIswc = (await client.AddSubmissionAsync(childSubmission)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(childSubmission.Agency, childSubmission.Workcode, httpClient);
            var body = new Body
            {
                Iswcs = new string[] { childIswc }
            };
            await mergeClient.MergeISWCMetadataAsync(parentIswc, childSubmission.Agency, body);
            await Task.Delay(3000);

            parentSubmission.DetailLevel = SubmissionDetailLevel.Core;
            parentSubmission.OriginalTitle = CreateNewTitle();
            parentSubmission.PreferredIswc = parentIswc;
            var res = await client.UpdateSubmissionAsync(parentIswc, parentSubmission);

            Assert.Empty(res.LinkedIswcs);
        }


    }
}
