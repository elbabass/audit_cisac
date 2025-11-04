using Azure.Search.Documents;
using SpanishPoint.Azure.Iswc.Api.ThirdParty.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.ThirdParty.Integration.Tests.V1;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.ThirdParty.Integration.Tests.V1.Tests.SearchTests
{
    public class SearchByAgencyWorkcodeThirdPartyTests_Fixture : IAsyncLifetime
    {
        public IISWC_SubmissionClient submissionClient;
        public IISWC_Third_Party_SearchClient searchThirdPartyClient;
        public HttpClient httpClient;
        public HttpClient httpSearchClient;
        public Submission submissionOne;
        public Submission submissionTwo;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await TestBase.GetClient();
            httpSearchClient = await TestBase.GetAgencyClient();
            submissionClient = new ISWC_SubmissionClient(httpSearchClient);
            searchThirdPartyClient = new ISWC_Third_Party_SearchClient(httpClient);
            submissionOne = Submissions.EligibleSubmissionIMRO;
            submissionOne.PreferredIswc = (await submissionClient.AddSubmissionAsync(submissionOne)).VerifiedSubmission.Iswc.ToString();
            await TestBase.WaitForSubmission(submissionOne.Agency, submissionOne.Workcode, httpSearchClient);

            submissionTwo = Submissions.EligibleSubmissionIMRO;
            submissionTwo.PreferredIswc = (await submissionClient.AddSubmissionAsync(submissionTwo)).VerifiedSubmission.Iswc.ToString();
            await TestBase.WaitForSubmission(submissionTwo.Agency, submissionTwo.Workcode, httpSearchClient);

            var body = new Body
            {
                Iswcs = new string[] { submissionTwo.PreferredIswc }
            };

            var mergeClient = new ISWC_MergeClient(await TestBase.GetAgencyClient());
            await mergeClient.MergeISWCMetadataAsync(submissionOne.PreferredIswc, submissionOne.Agency, body);
            await Task.Delay(2000);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            httpSearchClient.Dispose();
            return Task.CompletedTask;
        }
    }

    public class SearchByAgencyWorkcodeThirdPartyTests : TestBase, IClassFixture<SearchByAgencyWorkcodeThirdPartyTests_Fixture>
    {
        private readonly IISWC_Third_Party_SearchClient searchThirdPartyClient;
        private readonly Submission submissionOne;
        private readonly Submission submissionTwo;

        public SearchByAgencyWorkcodeThirdPartyTests(SearchByAgencyWorkcodeThirdPartyTests_Fixture fixture)
        {
            submissionOne = fixture.submissionOne;
            submissionTwo = fixture.submissionTwo;
            searchThirdPartyClient = fixture.searchThirdPartyClient;
        }

        /// <summary>
        /// Search by AgencyWorkCode with DetailLevel.CoreAndLinks
        /// </summary>
        [Fact]
        public async void SearchByAgencyWorkcodeThirdPartyTests_01()
        {
            var batch = new AgencyWorkCodeSearchModel[] {
                new AgencyWorkCodeSearchModel { Agency=submissionOne.Agency, WorkCode=submissionOne.Workcode },
                new AgencyWorkCodeSearchModel { Agency=submissionTwo.Agency, WorkCode=submissionTwo.Workcode }
            };

            var res = await searchThirdPartyClient.ThirdPartySearchByAgencyWorkCodeBatchAsync(batch);

            foreach (var item in batch.Select((x, i) => new { Work = x, Index = i }))
            {
                var work = res.ElementAtOrDefault(item.Index);
                Assert.NotNull(work);
                Assert.Contains(work.SearchResults.FirstOrDefault().Agency, item.Work.Agency);
                Assert.Null(work.SearchResults.FirstOrDefault().Works);
                Assert.True(work.SearchResults.FirstOrDefault().InterestedParties.Count.Equals(2));
            }
        }
    }
}
