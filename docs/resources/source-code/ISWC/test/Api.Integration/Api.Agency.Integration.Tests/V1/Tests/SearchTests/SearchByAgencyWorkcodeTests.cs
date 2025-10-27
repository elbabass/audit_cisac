using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using xRetry;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Tests.SearchTests
{

    public class SearchByAgencyWorkcodeTests_Fixture : IAsyncLifetime
    {
        public IISWC_SubmissionClient submissionClient;
        public IISWC_SearchClient searchClient;
        public HttpClient httpClient;
        public Submission submissionOne;
        public Submission submissionTwo;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await TestBase.GetClient();
            submissionClient = new ISWC_SubmissionClient(httpClient);
            searchClient = new ISWC_SearchClient(httpClient);
            submissionOne = Submissions.EligibleSubmissionIMRO;
            submissionOne.PreferredIswc = (await submissionClient.AddSubmissionAsync(submissionOne)).VerifiedSubmission.Iswc.ToString();
            await TestBase.WaitForSubmission(submissionOne.Agency, submissionOne.Workcode, httpClient);

            submissionTwo = Submissions.EligibleSubmissionIMRO;
            submissionTwo.PreferredIswc = (await submissionClient.AddSubmissionAsync(submissionTwo)).VerifiedSubmission.Iswc.ToString();
            await TestBase.WaitForSubmission(submissionTwo.Agency, submissionTwo.Workcode, httpClient);

            var body = new Body
            {
                Iswcs = new string[] { submissionTwo.PreferredIswc }
            };

            var mergeClient = new ISWC_MergeClient(await TestBase.GetClient());
            await mergeClient.MergeISWCMetadataAsync(submissionOne.PreferredIswc, submissionOne.Agency, body);
            await Task.Delay(2000);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            return Task.CompletedTask;
        }
    }

    public class SearchByAgencyWorkcodeTests : TestBase, IClassFixture<SearchByAgencyWorkcodeTests_Fixture>
    {
        private readonly IISWC_SearchClient searchClient;
        private readonly Submission submissionOne;
        private readonly Submission submissionTwo;

        public SearchByAgencyWorkcodeTests(SearchByAgencyWorkcodeTests_Fixture fixture)
        {
            submissionOne = fixture.submissionOne;
            submissionTwo = fixture.submissionTwo;
            searchClient = fixture.searchClient;
        }

        /// <summary>
        /// Search by AgencyWorkCode with DetailLevel.Full
        /// </summary>
        [Fact]
        public async void SearchByAgencyWorkcodeTests_01()
        {
            var agency = submissionOne.Agency;
            var workcode = submissionOne.Workcode;
            var res = await searchClient.SearchByAgencyWorkCodeAsync(agency, workcode, DetailLevel.Full);

            Assert.Equal(submissionOne.PreferredIswc, res.First().Iswc);
            Assert.Equal(submissionTwo.OriginalTitle, res.First().OriginalTitle);
            Assert.NotNull(res.FirstOrDefault().InterestedParties.FirstOrDefault(
                x => x.NameNumber == submissionOne.InterestedParties.First().NameNumber));
            Assert.NotNull(res.FirstOrDefault().InterestedParties.FirstOrDefault(
                x => x.NameNumber == submissionOne.InterestedParties.ElementAt(1).NameNumber));
            Assert.Equal(submissionOne.Workcode, res.First().Works.First().Workcode);
            Assert.Equal(submissionTwo.PreferredIswc, res.First().LinkedISWC.First());
        }

        /// <summary>
        /// Search by AgencyWorkCode with DetailLevel.Minimal
        /// </summary>
        [RetryFact]
        public async void SearchByAgencyWorkcodeTests_02()
        {
            var agency = submissionOne.Agency;
            var workcode = submissionOne.Workcode;
            var res = await searchClient.SearchByAgencyWorkCodeAsync(agency, workcode, DetailLevel.Minimal);

            Assert.Equal(submissionOne.PreferredIswc, res.First().Iswc);
            Assert.Equal(string.Empty, res.First().OriginalTitle);
            Assert.Null(res.First().Works);
            Assert.Null(res.First().LinkedISWC);
            Assert.Equal(0, res.First().InterestedParties.Count);
        }

        /// <summary>
        /// Search by AgencyWorkCode with DetailLevel.Core
        /// </summary>
        [Fact]
        public async void SearchByAgencyWorkcodeTests_03()
        {
            var agency = submissionOne.Agency;
            var workcode = submissionOne.Workcode;
            var res = await searchClient.SearchByAgencyWorkCodeAsync(agency, workcode, DetailLevel.Core);

            Assert.Equal(submissionOne.PreferredIswc, res.First().Iswc);
            Assert.Equal(submissionTwo.OriginalTitle, res.First().OriginalTitle);
            Assert.NotNull(res.FirstOrDefault().InterestedParties.FirstOrDefault(
                x => x.NameNumber == submissionOne.InterestedParties.First().NameNumber));
            Assert.NotNull(res.FirstOrDefault().InterestedParties.FirstOrDefault(
                x => x.NameNumber == submissionOne.InterestedParties.ElementAt(1).NameNumber));
            Assert.Null(res.First().Works);
            Assert.Null(res.First().LinkedISWC);
        }

        /// <summary>
        /// Search by AgencyWorkCode with DetailLevel.CoreAndLinks
        /// </summary>
        [Fact]
        public async void SearchByAgencyWorkcodeTests_04()
        {
            var agency = submissionOne.Agency;
            var workcode = submissionOne.Workcode;
            var res = await searchClient.SearchByAgencyWorkCodeAsync(agency, workcode, DetailLevel.CoreAndLinks);

            Assert.Equal(submissionOne.PreferredIswc, res.First().Iswc);
            Assert.Equal(submissionTwo.OriginalTitle, res.First().OriginalTitle);
            Assert.NotNull(res.FirstOrDefault().InterestedParties.FirstOrDefault(
                x => x.NameNumber == submissionOne.InterestedParties.First().NameNumber));
            Assert.NotNull(res.FirstOrDefault().InterestedParties.FirstOrDefault(
                x => x.NameNumber == submissionOne.InterestedParties.ElementAt(1).NameNumber));
            Assert.Null(res.First().Works);
            Assert.Equal(1, res.First().LinkedISWC.Count);
            Assert.Equal(submissionTwo.PreferredIswc, res.First().LinkedISWC.First());
        }

        [RetryFact]
        public async void SearchByAgencyWorkcodeTests_05()
        {
            var batch = new AgencyWorkCodeSearchModel[] {
                new AgencyWorkCodeSearchModel { Agency=submissionOne.Agency, WorkCode=submissionOne.Workcode },
                new AgencyWorkCodeSearchModel { Agency=submissionTwo.Agency, WorkCode=submissionTwo.Workcode }
            };

            var res = await searchClient.SearchByAgencyWorkCodeBatchAsync(batch);

            foreach (var item in batch.Select((x, i) => new { Work = x, Index = i }))
            {
                var work = res.ElementAtOrDefault(item.Index);
                Assert.NotNull(work);
                Assert.Contains(work.SearchResults.FirstOrDefault().Works, x => x.Agency == item.Work.Agency);
                Assert.Contains(work.SearchResults.FirstOrDefault().Works, x => x.Workcode == item.Work.WorkCode);
            }
        }
    }
}
