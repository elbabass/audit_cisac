using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.ThirdParty.Integration.Tests.V1.Tests.SearchTests
{
    public class SearchByTitleContributorThirdPartyTests_Fixture : IAsyncLifetime
    {
        public IISWC_SubmissionClient submissionClient;
        public IISWC_Third_Party_SearchClient searchThirdPartyClient;
        public HttpClient httpClient;
        public HttpClient httpSearchClient;
        public Submission submissionOne;
        public Submission submissionTwo;
        public Submission submissionThree;
        public Submission submissionFour;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await TestBase.GetClient();
            httpSearchClient = await TestBase.GetAgencyClient();
            submissionClient = new ISWC_SubmissionClient(httpSearchClient);
            searchThirdPartyClient = new ISWC_Third_Party_SearchClient(httpClient);

            submissionOne = Submissions.EligibleSubmissionIMRO;
            submissionOne.Performers = new Agency.Integration.Tests.V1.Performer[] 
            { 
                new Agency.Integration.Tests.V1.Performer
                { 
                    FirstName = submissionOne.InterestedParties.First().Name, 
                    LastName = submissionOne.InterestedParties.Last().Name 
                } 
            };
            submissionOne.PreferredIswc = (await submissionClient.AddSubmissionAsync(submissionOne)).VerifiedSubmission.Iswc.ToString();

            submissionTwo = Submissions.EligibleSubmissionBMI;
            submissionTwo.InterestedParties.Add(new Agency.Integration.Tests.V1.InterestedParty { NameNumber = 1091707950, Name = "KNOCKOUT UNIVERSAL MUSIC PUBLISHING", Role = Agency.Integration.Tests.V1.InterestedPartyRole.E, Affiliation = "021" });
            submissionTwo.PreferredIswc = (await submissionClient.AddSubmissionAsync(submissionTwo)).VerifiedSubmission.Iswc.ToString();

            await TestBase.WaitForSubmission(submissionTwo.Agency, submissionTwo.Workcode, httpSearchClient);

            submissionThree = Submissions.EligibleSubmissionBMI;
            submissionThree.InterestedParties = new List<Agency.Integration.Tests.V1.InterestedParty>() { InterestedParties.IP_BMI.Last() };
            submissionThree.PreferredIswc = (await submissionClient.AddSubmissionAsync(submissionThree)).VerifiedSubmission.Iswc.ToString();

            submissionFour = Submissions.EligibleSubmissionASCAP;
            submissionFour.OriginalTitle = submissionThree.OriginalTitle;
            submissionFour.InterestedParties = new List<Agency.Integration.Tests.V1.InterestedParty>() { InterestedParties.IP_ASCAP.Last() };
            submissionFour.PreferredIswc = (await submissionClient.AddSubmissionAsync(submissionFour)).VerifiedSubmission.Iswc.ToString();

            await TestBase.WaitForSubmission(submissionFour.Agency, submissionFour.Workcode, httpSearchClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            return Task.CompletedTask;
        }
    }
    public class SearchByTitleContributorThirdPartyTests : TestBase, IClassFixture<SearchByTitleContributorThirdPartyTests_Fixture>
    {
        private readonly IISWC_Third_Party_SearchClient searchThirdPartyClient;
        private readonly Submission submissionOne;
        private readonly Submission submissionTwo;
        private readonly Submission submissionThree;
        private readonly Submission submissionFour;

        public SearchByTitleContributorThirdPartyTests(SearchByTitleContributorThirdPartyTests_Fixture fixture)
        {
            submissionOne = fixture.submissionOne;
            submissionTwo = fixture.submissionTwo;
            submissionThree = fixture.submissionThree;
            submissionFour = fixture.submissionFour;
            searchThirdPartyClient = fixture.searchThirdPartyClient;
        }

        /// <summary>
        /// Test: Third party provides appropriate title and all interested parties by namenumber
        /// Result: Search returns single ISWC, interested parties stripped of BaseNumber
        /// </summary>
        [Fact]
        public async void SearchByTitleContributorThirdPartyTests_01()
        {
            var title = submissionOne.OriginalTitle;
            var nameNum = submissionOne.InterestedParties.First().NameNumber;
            var nameNum2 = submissionOne.InterestedParties.Last().NameNumber;

            var searchModel = new TitleAndContributorSearchModel[]
            {
                new TitleAndContributorSearchModel
                {
                    Titles = new Title[] { new Title { Title1 = title, Type = TitleType.OT } },
                    InterestedParties = new InterestedParty[] { new InterestedParty { NameNumber = nameNum }, new InterestedParty { NameNumber = nameNum2 } }
                }
            };

            var res = await searchThirdPartyClient.ThirdPartySearchByTitleAndContributorBatchAsync(searchModel);
            var work = res.ElementAtOrDefault(0);

            Assert.Contains(res, x => x.SearchResults.FirstOrDefault().Iswc.Equals(submissionOne.PreferredIswc));
            Assert.Null(work.SearchResults.FirstOrDefault().Works);
            var interestedParties = work.SearchResults.FirstOrDefault().InterestedParties;
            Assert.True(interestedParties.Count.Equals(2));
        }

        /// <summary>
        /// Test: Third party provides appropriate title and all interested parties by Last Name
        /// Result: Search returns single ISWC, interested parties stripped of BaseNumber
        /// </summary>
        [Fact]
        public async void SearchByTitleContributorThirdPartyTests_02()
        {
            var title = submissionOne.OriginalTitle;
            var lastName1 = submissionOne.InterestedParties.First().LastName;
            var lastName2 = submissionOne.InterestedParties.Last().LastName;

            var searchModel = new TitleAndContributorSearchModel[]
            {
                new TitleAndContributorSearchModel
                {
                    Titles = new Title[] { new Title { Title1 = title, Type = TitleType.OT } },
                    InterestedParties = new InterestedParty[] { new InterestedParty { LastName = lastName1 }, new InterestedParty { LastName = lastName2 } }
                }
            };

            var res = await searchThirdPartyClient.ThirdPartySearchByTitleAndContributorBatchAsync(searchModel);
            var work = res.ElementAtOrDefault(0);

            Assert.Contains(res, x => x.SearchResults.FirstOrDefault().Iswc.Equals(submissionOne.PreferredIswc));
            Assert.Null(work.SearchResults.FirstOrDefault().Works);
            var interestedParties = work.SearchResults.FirstOrDefault().InterestedParties;
            Assert.True(interestedParties.Count.Equals(2));
        }

        /// <summary>
        /// Test: Third party does not provide all interested parties
        /// Result: No results returned, specific 180 status code returned
        /// </summary>
        [Fact]
        public async void SearchByTitleContributorThirdPartyTests_03()
        {
            var title = submissionOne.OriginalTitle;
            var nameNum = submissionOne.InterestedParties.First().NameNumber;

            var searchModel = new TitleAndContributorSearchModel[]
            {
                new TitleAndContributorSearchModel
                {
                    Titles = new Title[] { new Title { Title1 = title, Type = TitleType.OT } },
                    InterestedParties = new InterestedParty[] { new InterestedParty { NameNumber = nameNum } }
                }
            };

            var res = await searchThirdPartyClient.ThirdPartySearchByTitleAndContributorBatchAsync(searchModel);
            var rejection = res.ElementAtOrDefault(0);

            Assert.Null(rejection.SearchResults);
            Assert.True(rejection.Rejection.Code.Equals("180"));
            Assert.True(rejection.Rejection.Message.Equals("ISWC not found."));
        }

        /// <summary>
        /// Test: Valid search for ISWC with associated E type interested Party
        /// Result: ISWC returned, interested parties does not include the E type party, should be stripped out
        /// </summary>
        [Fact]
        public async void SearchByTitleContributorThirdPartyTests_04()
        {
            var title = submissionTwo.OriginalTitle;
            var nameNum = submissionTwo.InterestedParties.ElementAt(0).NameNumber;
            var nameNum2 = submissionTwo.InterestedParties.ElementAt(1).NameNumber;

            var searchModel = new TitleAndContributorSearchModel[]
            {
                new TitleAndContributorSearchModel
                {
                    Titles = new Title[] { new Title { Title1 = title, Type = TitleType.OT } },
                    InterestedParties = new InterestedParty[] { new InterestedParty { NameNumber = nameNum }, new InterestedParty { NameNumber = nameNum2 } }
                }
            };

            var res = await searchThirdPartyClient.ThirdPartySearchByTitleAndContributorBatchAsync(searchModel);
            var work = res.ElementAtOrDefault(0);

            Assert.True(work.SearchResults.FirstOrDefault().InterestedParties.Count().Equals(2));
            Assert.Empty(work.SearchResults.FirstOrDefault().InterestedParties.Where(x => x.Role == InterestedPartyRole.E));
        }

        /// <summary>
        /// Test: Valid search that returns multiple ISWCs
        /// Result: Rejected with multiple matching results
        /// </summary>
        [Fact]
        public async void SearchByTitleContributorThirdPartyTests_05()
        {
            var title = submissionThree.OriginalTitle;
            var lastName = submissionThree.InterestedParties.First().LastName;

            var searchModel = new TitleAndContributorSearchModel[]
            {
                new TitleAndContributorSearchModel
                {
                    Titles = new Title[] { new Title { Title1 = title, Type = TitleType.OT } },
                    InterestedParties = new InterestedParty[] { new InterestedParty { LastName = lastName } }
                }
            };

            var res = await searchThirdPartyClient.ThirdPartySearchByTitleAndContributorBatchAsync(searchModel);
            var rejection = res.ElementAtOrDefault(0);

            Assert.Null(rejection.SearchResults);
            Assert.True(rejection.Rejection.Code.Equals("164"));
            Assert.True(rejection.Rejection.Message.Equals("Multiple matching ISWCs have been found"));
        }

        /// <summary>
        /// Test: Solution for Bug 28682. Third party provides appropriate title and provides some interested parties by name and the rest by name number.
        /// Result: Search returns single ISWC, interested parties stripped of BaseNumber
        /// </summary>
        [Fact]
        public async void SearchByTitleContributorThirdPartyTests_06()
        {
            var title = submissionOne.OriginalTitle;
            var nameNum = submissionOne.InterestedParties.First().NameNumber;
            var lastName = submissionOne.InterestedParties.Last().LastName;

            var searchModel = new TitleAndContributorSearchModel[]
            {
                new TitleAndContributorSearchModel
                {
                    Titles = new Title[] { new Title { Title1 = title, Type = TitleType.OT } },
                    InterestedParties = new InterestedParty[] { new InterestedParty { NameNumber = nameNum }, new InterestedParty { LastName = lastName } }
                }
            };

            var res = await searchThirdPartyClient.ThirdPartySearchByTitleAndContributorBatchAsync(searchModel);
            var work = res.ElementAtOrDefault(0);

            Assert.Contains(res, x => x.SearchResults.FirstOrDefault().Iswc.Equals(submissionOne.PreferredIswc));
            Assert.Null(work.SearchResults.FirstOrDefault().Works);
            var interestedParties = work.SearchResults.FirstOrDefault().InterestedParties;
            Assert.True(interestedParties.Count.Equals(2));
        }
    }
}
