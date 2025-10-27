using Azure.Search.Documents;
using SpanishPoint.Azure.Iswc.Api.ThirdParty.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.ThirdParty.Integration.Tests.V1.Tests.SearchTests
{
    public class SearchByISWCThirdPartyTests_Fixture : IAsyncLifetime
    {
        public IISWC_SubmissionClient submissionClient;
        public IISWC_Third_Party_SearchClient searchThirdPartyClient;
        public HttpClient httpClient;
        public HttpClient httpSearchClient;
        public readonly List<Submission> submissions = new List<Submission>();

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await TestBase.GetClient();
            httpSearchClient = await TestBase.GetAgencyClient();
            submissionClient = new ISWC_SubmissionClient(httpSearchClient);
            searchThirdPartyClient = new ISWC_Third_Party_SearchClient(httpClient);
            var subOne = Submissions.EligibleSubmissionAEPI;
            subOne.InterestedParties.Add(new Agency.Integration.Tests.V1.InterestedParty
            {
                NameNumber = 568562507,
                Role = Agency.Integration.Tests.V1.InterestedPartyRole.CA,
            });
            var subTwo = Submissions.EligibleSubmissionAEPI;

            var subThree = Submissions.EligibleSubmissionAEPI;
            subThree.InterestedParties.Add(new Agency.Integration.Tests.V1.InterestedParty
            {
                NameNumber = 277016857,
                Role = Agency.Integration.Tests.V1.InterestedPartyRole.CA,
            });
            subOne.PreferredIswc = (await submissionClient.AddSubmissionAsync(subOne)).VerifiedSubmission.Iswc.ToString();
            subTwo.PreferredIswc = (await submissionClient.AddSubmissionAsync(subTwo)).VerifiedSubmission.Iswc.ToString();
            subThree.PreferredIswc = (await submissionClient.AddSubmissionAsync(subThree)).VerifiedSubmission.Iswc.ToString();
            submissions.Add(subOne);
            submissions.Add(subTwo);
            submissions.Add(subThree);

            await TestBase.WaitForSubmission(subThree.Agency, subThree.Workcode, httpSearchClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            httpSearchClient.Dispose();
            return Task.CompletedTask;
        }
    }

    public class SearchByISWCThirdPartyTests : TestBase, IClassFixture<SearchByISWCThirdPartyTests_Fixture>
    {
        private readonly IISWC_Third_Party_SearchClient searchThirdPartyClient;
        private readonly List<Submission> submissions;

        public SearchByISWCThirdPartyTests(SearchByISWCThirdPartyTests_Fixture fixture)
        {
            submissions = fixture.submissions;
            searchThirdPartyClient = fixture.searchThirdPartyClient;
        }

        [Fact]
        public async void SearchByISWCByThirdPartyTests_01()
        {

            var iswcOne = submissions[0].PreferredIswc;
            var agencyOne = submissions[0].Agency;
            var originalTitleOne = submissions[0].OriginalTitle;
            var iswcTwo = submissions[1].PreferredIswc;
            var agencyTwo = submissions[1].Agency;
            var originalTitleTwo = submissions[1].OriginalTitle;
            var batch = new IswcSearchModel[] {
                new IswcSearchModel { Iswc = iswcOne },
                new IswcSearchModel { Iswc = iswcTwo },
                new IswcSearchModel { Iswc = "sdf67" }
            };

            var res = await searchThirdPartyClient.ThirdPartySearchByISWCBatchAsync(batch);

            Assert.Collection(res,
                elem1 =>
                {
                    Assert.Equal(batch[0].Iswc, elem1.SearchResults.FirstOrDefault().Iswc);
                    Assert.Equal(agencyOne, elem1.SearchResults.FirstOrDefault().Agency);
                    Assert.Equal(originalTitleOne, elem1.SearchResults.FirstOrDefault().OriginalTitle);
                    Assert.Null(elem1.SearchResults.FirstOrDefault().Works);
                },
                elem2 =>
                {
                    Assert.Equal(batch[1].Iswc, elem2.SearchResults.FirstOrDefault().Iswc);
                    Assert.Equal(agencyTwo, elem2.SearchResults.FirstOrDefault().Agency);
                    Assert.Equal(originalTitleTwo, elem2.SearchResults.FirstOrDefault().OriginalTitle);
                    Assert.Null(elem2.SearchResults.FirstOrDefault().Works);
                },
                elem3 =>
                {
                    Assert.NotNull(elem3.Rejection);
                });
        }
    }
}
