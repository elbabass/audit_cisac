using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Tests.UpdateTests
{
    public class UpdateExistingWorkSuccessfullyTests_AutoMerge_Fixture : IAsyncLifetime
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
    public class UpdateExistingWorkSuccessfullyTests_AutoMerge : TestBase, IAsyncLifetime, IClassFixture<UpdateExistingWorkSuccessfullyTests_AutoMerge_Fixture>
    {
        private IISWC_SubmissionClient client;
        private Submission currentSubmission;
        private IISWC_SearchClient searchClient;
        private HttpClient httpClient;

        public UpdateExistingWorkSuccessfullyTests_AutoMerge(UpdateExistingWorkSuccessfullyTests_AutoMerge_Fixture fixture)
        {
            client = fixture.submissionClient;
            searchClient = fixture.searchClient;
            httpClient = fixture.httpClient;
        }

        async Task IAsyncLifetime.InitializeAsync()
        {
            currentSubmission = Submissions.EligibleSubmissionBMI;
            currentSubmission.InterestedParties.Add(InterestedParties.IP_IMRO.First());
            var res = await client.AddSubmissionAsync(currentSubmission);
            currentSubmission.PreferredIswc = res.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(currentSubmission.Agency, currentSubmission.Workcode, httpClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Preffered ISWC is another matching ISWC.
        /// Submitter is ISWC eligible.
        /// Updated ISWC is merged.
        /// Feature 9119 Update: On auto-merge, oldest existing ISWC is set as parent
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_AutoMerge_01()
        {
            var subTwo = Submissions.EligibleSubmissionBMI;
            subTwo.InterestedParties = currentSubmission.InterestedParties;
            subTwo.PreferredIswc = (await client.AddSubmissionAsync(subTwo)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(subTwo.Agency, subTwo.Workcode, httpClient);

            subTwo.OriginalTitle = currentSubmission.OriginalTitle;
            await client.UpdateSubmissionAsync(subTwo.PreferredIswc, subTwo);
            await WaitForUpdate(subTwo.Agency, subTwo.Workcode, currentSubmission.OriginalTitle, httpClient);

            var searchRes = await searchClient.SearchByISWCAsync(currentSubmission.PreferredIswc);
            Assert.Equal(subTwo.PreferredIswc, searchRes.LinkedISWC.First());
        }

        /// <summary>
        /// Preferred ISWC is another matching ISWC.
        /// Submitter is ISWC eligible.
        /// Updated ISWC is merged and then the child work is updated on the parent ISWC.
        /// Child submission has preferredIswc set in update request.
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_AutoMerge_02()
        {
            var subTwo = Submissions.EligibleSubmissionBMI;
            subTwo.InterestedParties = currentSubmission.InterestedParties;
            var mergeIswc = (await client.AddSubmissionAsync(subTwo)).VerifiedSubmission.Iswc.ToString();
            subTwo.PreferredIswc = mergeIswc;
            await WaitForSubmission(subTwo.Agency, subTwo.Workcode, httpClient);

            subTwo.OriginalTitle = currentSubmission.OriginalTitle;
            await client.UpdateSubmissionAsync(mergeIswc, subTwo);
            await WaitForUpdate(subTwo.Agency, subTwo.Workcode, currentSubmission.OriginalTitle, httpClient);

            subTwo.OriginalTitle += "123";
            await client.UpdateSubmissionAsync(subTwo.PreferredIswc, subTwo);
            await WaitForUpdate(subTwo.Agency, subTwo.Workcode, subTwo.OriginalTitle, httpClient);

            var searchRes = await searchClient.SearchByISWCAsync(currentSubmission.PreferredIswc);
            Assert.Equal(subTwo.OriginalTitle, searchRes.OriginalTitle);
        }

        /// <summary>
        /// Preferred ISWC is another matching ISWC.
        /// Works are split copyright
        /// Submitter is eligible under merge eligibility rules.
        /// Updated ISWC is merged.
        /// Feature 9119: Child will be the younger of the two ISWCs
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_AutoMerge_03()
        {
            var subTwo = Submissions.EligibleSubmissionIMRO;
            subTwo.InterestedParties = currentSubmission.InterestedParties;
            subTwo.PreferredIswc = (await client.AddSubmissionAsync(subTwo)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(subTwo.Agency, subTwo.Workcode, httpClient);

            subTwo.OriginalTitle = currentSubmission.OriginalTitle;
            await client.UpdateSubmissionAsync(subTwo.PreferredIswc, subTwo);
            await WaitForUpdate(subTwo.Agency, subTwo.Workcode, currentSubmission.OriginalTitle, httpClient);

            var searchRes = await searchClient.SearchByISWCAsync(currentSubmission.PreferredIswc);
            Assert.Equal(subTwo.PreferredIswc, searchRes.LinkedISWC.First());
        }

        /// <summary>
        /// Work is updated to remove only eligible creator and add an inelligible creator.
        /// Updated work matches ISWC from another agency.
        /// Inellgibile work is moved to that ISWC instead of merging.
        /// currentSubmission is not applicable in this test.
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_AutoMerge_04()
        {
            var subOne = Submissions.EligibleSubmissionBMI;
            subOne.InterestedParties.Remove(subOne.InterestedParties.Last());
            subOne.PreferredIswc = (await client.AddSubmissionAsync(subOne)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(subOne.Agency, subOne.Workcode, httpClient);

            var subTwo = Submissions.EligibleSubmissionIMRO;
            subTwo.InterestedParties.Remove(subTwo.InterestedParties.Last());
            subTwo.PreferredIswc = (await client.AddSubmissionAsync(subTwo)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(subTwo.Agency, subTwo.Workcode, httpClient);

            subTwo.OriginalTitle = subOne.OriginalTitle;
            subTwo.InterestedParties = subOne.InterestedParties;
            subTwo.PreferredIswc = subOne.PreferredIswc;
            var updateRes = await client.UpdateSubmissionAsync(subTwo.PreferredIswc, subTwo);
            await Task.Delay(2000);

            var searchRes = await searchClient.SearchByISWCAsync(subOne.PreferredIswc);
            Assert.NotNull(searchRes.Works.FirstOrDefault(x => x.Workcode == subTwo.Workcode));
            Assert.Equal(2, searchRes.Works.Count);
        }
        /// <summary>
        /// Feature 9119 - On automerge, oldest ISWC should be parent
        /// Update applied to oldest ISWC
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_AutoMerge_05()
        {
            var subOne = Submissions.EligibleSubmissionBMI;
            subOne.PreferredIswc = (await client.AddSubmissionAsync(subOne)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(subOne.Agency, subOne.Workcode, httpClient);

            var subTwo = Submissions.EligibleSubmissionBMI;
            subTwo.PreferredIswc = (await client.AddSubmissionAsync(subTwo)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(subTwo.Agency, subTwo.Workcode, httpClient);

            subOne.OriginalTitle = subTwo.OriginalTitle;
            var updateRes = await client.UpdateSubmissionAsync(subOne.PreferredIswc, subOne);
            await Task.Delay(2000);

            var searchRes = await searchClient.SearchByISWCAsync(subOne.PreferredIswc);
            Assert.NotNull(searchRes.Works.FirstOrDefault(x => x.Workcode == subTwo.Workcode));
            Assert.Equal(2, searchRes.Works.Count);
            Assert.Equal(subTwo.PreferredIswc, searchRes.LinkedISWC.First());
        }
    }
}
