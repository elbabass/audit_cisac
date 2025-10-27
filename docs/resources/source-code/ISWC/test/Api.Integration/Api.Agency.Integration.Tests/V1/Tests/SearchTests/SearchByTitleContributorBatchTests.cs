using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using xRetry;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Tests.SearchTests
{

    public class SearchByTitleContributorBatchTests_Fixture : IAsyncLifetime
    {
        public IISWC_SubmissionClient submissionClient;
        public IISWC_SearchClient searchClient;
        public HttpClient httpClient;
        public Submission submissionOne;
        public Submission submissionTwo;
        public Submission submissionThree;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await TestBase.GetClient();
            submissionClient = new ISWC_SubmissionClient(httpClient);
            searchClient = new ISWC_SearchClient(httpClient);
            submissionOne = Submissions.EligibleSubmissionIMRO;
            submissionOne.PreferredIswc = (await submissionClient.AddSubmissionAsync(submissionOne)).VerifiedSubmission.Iswc.ToString();
            await TestBase.WaitForSubmission(submissionOne.Agency, submissionOne.Workcode, httpClient);
            submissionTwo = Submissions.EligibleSubmissionBMI;
            submissionTwo.InterestedParties.Add(new InterestedParty { BaseNumber = "I-005941427-4", NameNumber = 1091707950, Name = "KNOCKOUT UNIVERSAL MUSIC PUBLISHING", Role = InterestedPartyRole.E, Affiliation = "021" });
            submissionTwo.PreferredIswc = (await submissionClient.AddSubmissionAsync(submissionTwo)).VerifiedSubmission.Iswc.ToString();
            await TestBase.WaitForSubmission(submissionTwo.Agency, submissionTwo.Workcode, httpClient);
            submissionThree = Submissions.EligibleSubmissionPRS;
            submissionThree.PreferredIswc = (await submissionClient.AddSubmissionAsync(submissionThree)).VerifiedSubmission.Iswc.ToString();
            await TestBase.WaitForSubmission(submissionThree.Agency, submissionThree.Workcode, httpClient);

        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            return Task.CompletedTask;
        }
    }

    public class SearchByTitleContributorBatchTests : TestBase, IClassFixture<SearchByTitleContributorBatchTests_Fixture>
    {
        private readonly IISWC_SearchClient searchClient;
        private readonly Submission submissionOne;
        private readonly Submission submissionTwo;
        private readonly Submission submissionThree;

        public SearchByTitleContributorBatchTests(SearchByTitleContributorBatchTests_Fixture fixture)
        {
            submissionOne = fixture.submissionOne;
            submissionTwo = fixture.submissionTwo;
            submissionThree = fixture.submissionThree;
            searchClient = fixture.searchClient;
        }

        [RetryFact]
        public async void SearchByTitleContributorBatchTests_01()
        {
            List<TitleAndContributorSearchModel> searchModel = new List<TitleAndContributorSearchModel>{
                new TitleAndContributorSearchModel {
                    Titles =  new List<Title> { new Title { Title1 = submissionOne.OriginalTitle, Type = TitleType.OT } },
                    InterestedParties = submissionOne.InterestedParties
                },
                new TitleAndContributorSearchModel {
                    Titles =  new List<Title> { new Title { Title1 = submissionTwo.OriginalTitle, Type = TitleType.OT } },
                    InterestedParties = submissionTwo.InterestedParties
                },
                new TitleAndContributorSearchModel {
                    Titles =  new List<Title> { new Title { Title1 = submissionThree.OriginalTitle, Type = TitleType.OT } },
                    InterestedParties = submissionThree.InterestedParties
                }
            };

            var res = await searchClient.SearchByTitleAndContributorBatchAsync(searchModel);

            Assert.Collection(res,
                elem1 =>
                {
                    Assert.Contains(elem1.SearchResults.FirstOrDefault().Works, x => x.Iswc.Equals(submissionOne.PreferredIswc));
                },
                elem2 =>
                {
                    Assert.Contains(elem2.SearchResults.FirstOrDefault().Works, x => x.Iswc.Equals(submissionTwo.PreferredIswc));
                },
                elem3 =>
                {
                    Assert.Contains(elem3.SearchResults.FirstOrDefault().Works, x => x.Iswc.Equals(submissionThree.PreferredIswc));
                });
        }

        [RetryFact]
        public async void SearchByTitleContributorBatchTests_02()
        {
            List<TitleAndContributorSearchModel> searchModel = new List<TitleAndContributorSearchModel>{
                new TitleAndContributorSearchModel {
                    Titles =  new List<Title> { new Title { Title1 = submissionOne.OriginalTitle, Type = TitleType.OT } },
                    InterestedParties = new InterestedParty[] { new InterestedParty { NameNumber = submissionOne.InterestedParties.First().NameNumber } }
                },
                new TitleAndContributorSearchModel {
                    Titles =  new List<Title> { new Title { Title1 = submissionTwo.OriginalTitle, Type = TitleType.OT } },
                    InterestedParties = new InterestedParty[] { new InterestedParty { NameNumber = submissionTwo.InterestedParties.First().NameNumber } }
                },
                new TitleAndContributorSearchModel {
                    Titles =  new List<Title> { new Title { Title1 = submissionThree.OriginalTitle, Type = TitleType.OT } },
                    InterestedParties = new InterestedParty[] { new InterestedParty { NameNumber = submissionThree.InterestedParties.First().NameNumber } }
                }
            };

            var res = await searchClient.SearchByTitleAndContributorBatchAsync(searchModel);

            Assert.Collection(res,
                elem1 =>
                {
                    Assert.Contains(elem1.SearchResults.FirstOrDefault().Works, x => x.Iswc.Equals(submissionOne.PreferredIswc));
                },
                elem2 =>
                {
                    Assert.Contains(elem2.SearchResults.FirstOrDefault().Works, x => x.Iswc.Equals(submissionTwo.PreferredIswc));
                },
                elem3 =>
                {
                    Assert.Contains(elem3.SearchResults.FirstOrDefault().Works, x => x.Iswc.Equals(submissionThree.PreferredIswc));
                });
        }

        [RetryFact]
        public async void SearchByTitleContributorBatchTests_03()
        {
            List<TitleAndContributorSearchModel> searchModel = new List<TitleAndContributorSearchModel>{
                new TitleAndContributorSearchModel {
                    Titles =  new List<Title> { new Title { Title1 = submissionOne.OriginalTitle, Type = TitleType.OT } },
                    InterestedParties = new InterestedParty[] { new InterestedParty { BaseNumber = submissionOne.InterestedParties.First().BaseNumber } }
                },
                new TitleAndContributorSearchModel {
                    Titles =  new List<Title> { new Title { Title1 = submissionTwo.OriginalTitle, Type = TitleType.OT } },
                    InterestedParties = new InterestedParty[] { new InterestedParty { BaseNumber = submissionTwo.InterestedParties.First().BaseNumber } }
                },
                new TitleAndContributorSearchModel {
                    Titles =  new List<Title> { new Title { Title1 = submissionThree.OriginalTitle, Type = TitleType.OT } },
                    InterestedParties = new InterestedParty[] { new InterestedParty { BaseNumber = submissionThree.InterestedParties.First().BaseNumber } }
                }
            };

            var res = await searchClient.SearchByTitleAndContributorBatchAsync(searchModel);

            Assert.Collection(res,
                elem1 =>
                {
                    Assert.Contains(elem1.SearchResults.FirstOrDefault().Works, x => x.Iswc.Equals(submissionOne.PreferredIswc));
                },
                elem2 =>
                {
                    Assert.Contains(elem2.SearchResults.FirstOrDefault().Works, x => x.Iswc.Equals(submissionTwo.PreferredIswc));
                },
                elem3 =>
                {
                    Assert.Contains(elem3.SearchResults.FirstOrDefault().Works, x => x.Iswc.Equals(submissionThree.PreferredIswc));
                });
        }

        [RetryFact]
        public async void SearchByTitleContributorBatchTests_04()
        {
            List<TitleAndContributorSearchModel> searchModel = new List<TitleAndContributorSearchModel>{
                new TitleAndContributorSearchModel {
                    Titles =  new List<Title> { new Title { Title1 = submissionOne.OriginalTitle, Type = TitleType.OT } },
                    InterestedParties = new InterestedParty[] {}
                },
                new TitleAndContributorSearchModel {
                    Titles =  new List<Title> { new Title { Title1 = submissionTwo.OriginalTitle, Type = TitleType.OT } },
                    InterestedParties = new InterestedParty[] {}
                },
                new TitleAndContributorSearchModel {
                    Titles =  new List<Title> { new Title { Title1 = submissionThree.OriginalTitle, Type = TitleType.OT } },
                    InterestedParties = new InterestedParty[] {}
                }
            };

            var res = await searchClient.SearchByTitleAndContributorBatchAsync(searchModel);

            Assert.Collection(res,
                elem1 =>
                {
                    Assert.Contains(elem1.SearchResults.FirstOrDefault().Works, x => x.Iswc.Equals(submissionOne.PreferredIswc));
                },
                elem2 =>
                {
                    Assert.Contains(elem2.SearchResults.FirstOrDefault().Works, x => x.Iswc.Equals(submissionTwo.PreferredIswc));
                },
                elem3 =>
                {
                    Assert.Contains(elem3.SearchResults.FirstOrDefault().Works, x => x.Iswc.Equals(submissionThree.PreferredIswc));
                });
        }

        [Fact]
        public async void SearchByTitleContributorBatchTests_05()
        {
            List<TitleAndContributorSearchModel> searchModel = new List<TitleAndContributorSearchModel>{
                new TitleAndContributorSearchModel {
                    Titles =  new List<Title> { new Title { Title1 = submissionOne.OriginalTitle, Type = TitleType.OT } },
                    InterestedParties = new InterestedParty[] {}
                },
                new TitleAndContributorSearchModel {
                    Titles =  new List<Title> { new Title { Title1 = submissionTwo.OriginalTitle, Type = TitleType.OT } },
                    InterestedParties = new InterestedParty[] {}
                },
                new TitleAndContributorSearchModel {
                    Titles =  new List<Title> { new Title { Title1 = submissionThree.OriginalTitle, Type = TitleType.OT } },
                    InterestedParties = new InterestedParty[] {}
                }
            };

            var res = await searchClient.SearchByTitleAndContributorBatchAsync(searchModel);

            Assert.Collection(res,
                elem1 =>
                {
                    Assert.True(elem1.SearchResults.FirstOrDefault().InterestedParties.Count().Equals(submissionOne.InterestedParties.Count));
                },
                elem2 =>
                {
                    Assert.True(elem2.SearchResults.FirstOrDefault().InterestedParties.Count().Equals(submissionTwo.InterestedParties.Count));
                },
                elem3 =>
                {
                    Assert.True(elem3.SearchResults.FirstOrDefault().InterestedParties.Count().Equals(submissionThree.InterestedParties.Count));
                });
        }
    }
}
