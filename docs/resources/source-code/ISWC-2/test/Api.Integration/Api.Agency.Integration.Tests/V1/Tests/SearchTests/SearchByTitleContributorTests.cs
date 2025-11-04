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

    public class SearchByTitleContributorTests_Fixture : IAsyncLifetime
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
            submissionTwo = Submissions.EligibleSubmissionBMI;
            submissionTwo.InterestedParties.Add(new InterestedParty { BaseNumber = "I-005941427-4", NameNumber = 1091707950, Name = "KNOCKOUT UNIVERSAL MUSIC PUBLISHING", Role = InterestedPartyRole.E, Affiliation = "021" });
            submissionTwo.PreferredIswc = (await submissionClient.AddSubmissionAsync(submissionTwo)).VerifiedSubmission.Iswc.ToString();
            await TestBase.WaitForSubmission(submissionTwo.Agency, submissionTwo.Workcode, httpClient);

        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            return Task.CompletedTask;
        }
    }

    public class SearchByTitleContributorTests : TestBase, IClassFixture<SearchByTitleContributorTests_Fixture>
    {
        private readonly IISWC_SearchClient searchClient;
        private readonly Submission submissionOne;
        private readonly Submission submissionTwo;

        public SearchByTitleContributorTests(SearchByTitleContributorTests_Fixture fixture)
        {
            submissionOne = fixture.submissionOne;
            submissionTwo = fixture.submissionTwo;
            searchClient = fixture.searchClient;
        }

        [RetryFact]
        public async void SearchByTitleContributorTests_01()
        {
            var title = submissionOne.OriginalTitle;
            var name = submissionOne.InterestedParties.ElementAt(1).Name;
            var searchModel = new TitleAndContributorSearchModel
            {
                Titles = new Title[] { new Title { Title1 = title, Type = TitleType.OT } },
                InterestedParties = new InterestedParty[] { new InterestedParty { Name = name } }
            };

            var res = await searchClient.SearchByTitleAndContributorAsync(searchModel);

            var work = res.FirstOrDefault().Works.FirstOrDefault(x => x.OriginalTitle == title);
            Assert.Contains(res, x => x.Iswc.Equals(submissionOne.PreferredIswc));
            Assert.Contains(work.InterestedParties, x => x.Name.Contains(name));
        }

        [RetryFact]
        public async void SearchByTitleContributorTests_02()
        {
            var title = submissionOne.OriginalTitle;
            var nameNum = submissionOne.InterestedParties.First().NameNumber;
            var searchModel = new TitleAndContributorSearchModel
            {
                Titles = new Title[] { new Title { Title1 = title, Type = TitleType.OT } },
                InterestedParties = new InterestedParty[] { new InterestedParty { NameNumber = nameNum } }
            };

            var res = await searchClient.SearchByTitleAndContributorAsync(searchModel);

            var work = res.FirstOrDefault().Works.FirstOrDefault(x => x.OriginalTitle == searchModel.Titles.First().Title1);
            Assert.Contains(res, x => x.Iswc.Equals(submissionOne.PreferredIswc));
            Assert.Contains(work.InterestedParties, x => x.NameNumber == nameNum);
        }

        [RetryFact]
        public async void SearchByTitleContributorTests_03()
        {
            var title = submissionOne.OriginalTitle;
            var baseNumber = submissionOne.InterestedParties.First().BaseNumber;
            var searchModel = new TitleAndContributorSearchModel
            {
                Titles = new Title[] { new Title { Title1 = title, Type = TitleType.OT } },
                InterestedParties = new InterestedParty[] { new InterestedParty { BaseNumber = baseNumber } }
            };

            var res = await searchClient.SearchByTitleAndContributorAsync(searchModel);

            var work = res.FirstOrDefault().Works.FirstOrDefault(x => x.OriginalTitle == searchModel.Titles.First().Title1);
            Assert.Contains(res, x => x.Iswc.Equals(submissionOne.PreferredIswc));
            Assert.Contains(work.InterestedParties, x => x.BaseNumber.Equals(baseNumber));
        }

        [RetryFact]
        public async void SearchByTitleContributorTests_04()
        {
            var searchModel = new TitleAndContributorSearchModel
            {
                Titles = new Title[] { new Title { Title1 = submissionOne.OriginalTitle, Type = TitleType.OT } },
                InterestedParties = new InterestedParty[] { }
            };

            var res = await searchClient.SearchByTitleAndContributorAsync(searchModel);

            var work = res.FirstOrDefault().Works.FirstOrDefault(x => x.OriginalTitle == searchModel.Titles.First().Title1);
            Assert.Contains(res, x => x.Iswc.Equals(submissionOne.PreferredIswc));
        }

        [RetryFact]
        public async void SearchByTitleContributorTests_05()
        {
            var http = await GetClient();

            var submissionClient = new ISWC_SubmissionClient(http);

            http.DefaultRequestHeaders.Add("request-source", "PORTAL");
            var clientWithHeader = new ISWC_SearchClient(http);



            var submissionTwo = Submissions.EligibleSubmissionIMRO;
            submissionTwo.Workcode = CreateNewWorkCode();
            submissionTwo.OriginalTitle = submissionOne.OriginalTitle;
            submissionTwo.InterestedParties.FirstOrDefault().Role = InterestedPartyRole.AD;
            submissionTwo.Disambiguation = true;
            submissionTwo.DisambiguationReason = DisambiguationReason.DIT;
            submissionTwo.DisambiguateFrom = new List<DisambiguateFrom>();
            submissionTwo.DisambiguateFrom.Add(new DisambiguateFrom { Iswc = submissionOne.PreferredIswc });

            submissionTwo.PreferredIswc = (await submissionClient.AddSubmissionAsync(submissionTwo)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submissionTwo.Agency, submissionTwo.Workcode, http);

            var submissionThree = Submissions.EligibleSubmissionIMRO;
            submissionThree.Workcode = CreateNewWorkCode();
            submissionThree.OriginalTitle = submissionOne.OriginalTitle;

            submissionThree.Disambiguation = true;
            submissionThree.DisambiguationReason = DisambiguationReason.DIT;
            submissionThree.DisambiguateFrom = new List<DisambiguateFrom>();
            submissionThree.DisambiguateFrom.Add(new DisambiguateFrom { Iswc = submissionOne.PreferredIswc });
            submissionThree.DisambiguateFrom.Add(new DisambiguateFrom { Iswc = submissionTwo.PreferredIswc });


            submissionThree.PreferredIswc = (await submissionClient.AddSubmissionAsync(submissionThree)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submissionThree.Agency, submissionThree.Workcode, http);

            var submissionFour = Submissions.EligibleSubmissionIMRO;
            submissionFour.Workcode = CreateNewWorkCode();
            submissionFour.OriginalTitle = submissionOne.OriginalTitle;

            submissionFour.Disambiguation = true;
            submissionFour.DisambiguationReason = DisambiguationReason.DIT;
            submissionFour.DisambiguateFrom = new List<DisambiguateFrom>();
            submissionFour.DisambiguateFrom.Add(new DisambiguateFrom { Iswc = submissionOne.PreferredIswc });
            submissionFour.DisambiguateFrom.Add(new DisambiguateFrom { Iswc = submissionTwo.PreferredIswc });


            submissionFour.PreferredIswc = (await submissionClient.AddSubmissionAsync(submissionFour)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submissionFour.Agency, submissionFour.Workcode, http);



            var submissionFive = Submissions.EligibleSubmissionIMRO;
            submissionFive.Workcode = CreateNewWorkCode();
            submissionFive.OriginalTitle = submissionOne.OriginalTitle;

            submissionFive.Disambiguation = true;
            submissionFive.DisambiguationReason = DisambiguationReason.DIT;
            submissionFive.DisambiguateFrom = new List<DisambiguateFrom>();
            submissionFive.DisambiguateFrom.Add(new DisambiguateFrom { Iswc = submissionOne.PreferredIswc });
            submissionFive.DisambiguateFrom.Add(new DisambiguateFrom { Iswc = submissionTwo.PreferredIswc });


            submissionFive.PreferredIswc = (await submissionClient.AddSubmissionAsync(submissionFive)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submissionFive.Agency, submissionFive.Workcode, http);

            var searchModel = new TitleAndContributorSearchModel
            {
                Titles = new Title[] { new Title { Title1 = submissionOne.OriginalTitle, Type = TitleType.OT } },
                InterestedParties = new InterestedParty[] { new InterestedParty { NameNumber = submissionOne.InterestedParties.FirstOrDefault().NameNumber } }
            };

            var res = await clientWithHeader.SearchByTitleAndContributorAsync(searchModel);


            Assert.Collection(res,
                elem1 => Assert.Equal(submissionThree.PreferredIswc, elem1.Iswc),
                elem2 => Assert.Equal(submissionOne.PreferredIswc, elem2.Iswc),
                elem3 => Assert.Equal(submissionTwo.PreferredIswc, elem3.Iswc)
                );
        }

        /// <summary>
        /// All contributors returned, creators and publishers
        /// </summary>
        [Fact]
        public async void SearchByTitleContributorTests_06()
        {
            var title = submissionTwo.OriginalTitle;
            var interestedParties = submissionTwo.InterestedParties;

            var searchModel = new TitleAndContributorSearchModel
            {
                Titles = new Title[] { new Title { Title1 = title, Type = TitleType.OT } },
                InterestedParties = interestedParties
            };

            var res = await searchClient.SearchByTitleAndContributorAsync(searchModel);
            Assert.True(res.First().InterestedParties.Count().Equals(interestedParties.Count));
        }
    }
}
