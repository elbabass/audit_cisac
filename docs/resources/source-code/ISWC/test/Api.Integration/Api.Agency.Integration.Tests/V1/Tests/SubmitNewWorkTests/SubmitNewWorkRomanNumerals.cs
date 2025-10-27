using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using Xunit;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Tests.SubmitNewWorkTests
{
    public class SubmitNewWorkRomanNumerals : TestBase, IAsyncLifetime
    {
        private IISWC_SubmissionClient client;
        private IISWC_SearchClient searchClient;
        private HttpClient httpClient;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await GetClient();
            client = new ISWC_SubmissionClient(httpClient);
            searchClient = new ISWC_SearchClient(httpClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            return Task.CompletedTask;
        }

        //English, roman numerals filtering should be applied
        [Fact]
        public async void SearchByTitleRomanNumerals_01()
        {
            var subWithNumerals = Submissions.EligibleSubmissionSACEM;
            var subWithoutNumerals = Submissions.EligibleSubmissionBMI;
            subWithoutNumerals.OriginalTitle = "Iterate Challenge Work Negative Isolation Pilot Insect Token Head Power Emperor Empower Gibberish";
            subWithNumerals.OriginalTitle = subWithoutNumerals.OriginalTitle + " IV";

            var searchModel1 = new TitleAndContributorSearchModel
            {
                Titles = new Title[] { new Title { Title1 = subWithNumerals.OriginalTitle, Type = TitleType.OT } },
                InterestedParties = null
            };
            ICollection<ISWCMetadata> res1;
            try
            {
                res1 = await searchClient.SearchByTitleAndContributorAsync(searchModel1);
                if (res1.Where(x => x.OriginalTitle == subWithNumerals.OriginalTitle).Count() == 0)
                {
                    await client.AddSubmissionAsync(subWithNumerals);
                    await WaitForSubmission(subWithNumerals.Agency, subWithNumerals.Workcode, httpClient);
                }
            }
            catch (ApiException)
            {
                await client.AddSubmissionAsync(subWithNumerals);
                await WaitForSubmission(subWithNumerals.Agency, subWithNumerals.Workcode, httpClient);
            }

            var searchModel2 = new TitleAndContributorSearchModel
            {
                Titles = new Title[] { new Title { Title1 = subWithoutNumerals.OriginalTitle, Type = TitleType.OT } },
                InterestedParties = null
            };
            ICollection<ISWCMetadata> res2;
            try
            {
                res2 = await searchClient.SearchByTitleAndContributorAsync(searchModel2);
                if (res2.Where(x => x.OriginalTitle == subWithoutNumerals.OriginalTitle).Count() == 0)
                {
                    await client.AddSubmissionAsync(subWithoutNumerals);
                    await WaitForSubmission(subWithoutNumerals.Agency, subWithoutNumerals.Workcode, httpClient);
                }
            }
            catch (ApiException)
            {
                await client.AddSubmissionAsync(subWithoutNumerals);
                await WaitForSubmission(subWithoutNumerals.Agency, subWithoutNumerals.Workcode, httpClient);
            }

            res1 = await searchClient.SearchByTitleAndContributorAsync(searchModel1);
            res2 = await searchClient.SearchByTitleAndContributorAsync(searchModel2);


            foreach (var work in res1)
            {
                Assert.Equal(work.OriginalTitle, subWithNumerals.OriginalTitle);
            }

            Assert.True(res2.Where(x => x.OriginalTitle == subWithoutNumerals.OriginalTitle).Count() == 1);
            Assert.True(res2.Where(x => x.OriginalTitle == subWithNumerals.OriginalTitle).Count() == 1);

        }
        //French, roman numerals filtering should be applied
        [Fact]
        public async void SearchByTitleRomanNumerals_02()
        {
            var subWithNumerals = Submissions.EligibleSubmissionSACEM;
            var subWithoutNumerals = Submissions.EligibleSubmissionBMI;
            subWithoutNumerals.OriginalTitle = "Itérer Défi Chien de travail Chat de guerre Pouvoir de l’empereur Charabia Signifie Tuer";
            subWithNumerals.OriginalTitle = subWithoutNumerals.OriginalTitle + " IV";

            var searchModel1 = new TitleAndContributorSearchModel
            {
                Titles = new Title[] { new Title { Title1 = subWithNumerals.OriginalTitle, Type = TitleType.OT } },
                InterestedParties = null
            };
            ICollection<ISWCMetadata> res1;
            try
            {
                res1 = await searchClient.SearchByTitleAndContributorAsync(searchModel1);
                if (res1.Where(x => x.OriginalTitle == subWithNumerals.OriginalTitle).Count() == 0)
                {
                    await client.AddSubmissionAsync(subWithNumerals);
                    await WaitForSubmission(subWithNumerals.Agency, subWithNumerals.Workcode, httpClient);
                }
            }
            catch (ApiException)
            {
                await client.AddSubmissionAsync(subWithNumerals);
                await WaitForSubmission(subWithNumerals.Agency, subWithNumerals.Workcode, httpClient);
            }

            var searchModel2 = new TitleAndContributorSearchModel
            {
                Titles = new Title[] { new Title { Title1 = subWithoutNumerals.OriginalTitle, Type = TitleType.OT } },
                InterestedParties = null
            };
            ICollection<ISWCMetadata> res2;
            try
            {
                res2 = await searchClient.SearchByTitleAndContributorAsync(searchModel2);
                if (res2.Where(x => x.OriginalTitle == subWithoutNumerals.OriginalTitle).Count() == 0)
                {
                    await client.AddSubmissionAsync(subWithoutNumerals);
                    await WaitForSubmission(subWithoutNumerals.Agency, subWithoutNumerals.Workcode, httpClient);
                }
            }
            catch (ApiException)
            {
                await client.AddSubmissionAsync(subWithoutNumerals);
                await WaitForSubmission(subWithoutNumerals.Agency, subWithoutNumerals.Workcode, httpClient);
            }

            res1 = await searchClient.SearchByTitleAndContributorAsync(searchModel1);
            res2 = await searchClient.SearchByTitleAndContributorAsync(searchModel2);


            foreach (var work in res1)
            {
                Assert.Equal(work.OriginalTitle, subWithNumerals.OriginalTitle);
            }

            Assert.True(res2.Where(x => x.OriginalTitle == subWithoutNumerals.OriginalTitle).Count() == 1);
            Assert.True(res2.Where(x => x.OriginalTitle == subWithNumerals.OriginalTitle).Count() == 1);
        }
        //Chinese Traditional, roman numerals filtering should not be applied
        [SkippableFact]
        public async void SearchByTitleRomanNumerals_03()
        {
            Skip.If(true);
            var subWithNumerals = Submissions.EligibleSubmissionAEPI;
            var subWithoutNumerals = Submissions.EligibleSubmissionAKKA;
            subWithoutNumerals.OriginalTitle = "迭代挑戰工作負隔離飛行員昆蟲令牌頭權力皇帝授權胡言亂語";
            subWithNumerals.OriginalTitle = subWithoutNumerals.OriginalTitle + " IV";

            var searchModel1 = new TitleAndContributorSearchModel
            {
                Titles = new Title[] { new Title { Title1 = subWithNumerals.OriginalTitle, Type = TitleType.OT } },
                InterestedParties = null
            };
            ICollection<ISWCMetadata> res1;
            try
            {
                res1 = await searchClient.SearchByTitleAndContributorAsync(searchModel1);
                if (res1.Where(x => x.OriginalTitle == subWithNumerals.OriginalTitle).Count() == 0)
                {
                    await client.AddSubmissionAsync(subWithNumerals);
                    await WaitForSubmission(subWithNumerals.Agency, subWithNumerals.Workcode, httpClient);
                }
            }
            catch (ApiException)
            {
                await client.AddSubmissionAsync(subWithNumerals);
                await WaitForSubmission(subWithNumerals.Agency, subWithNumerals.Workcode, httpClient);
            }

            var searchModel2 = new TitleAndContributorSearchModel
            {
                Titles = new Title[] { new Title { Title1 = subWithoutNumerals.OriginalTitle, Type = TitleType.OT } },
                InterestedParties = null
            };
            ICollection<ISWCMetadata> res2;
            try
            {
                res2 = await searchClient.SearchByTitleAndContributorAsync(searchModel2);
                if (res2.Where(x => x.OriginalTitle == subWithoutNumerals.OriginalTitle).Count() == 0)
                {
                    await client.AddSubmissionAsync(subWithoutNumerals);
                    await WaitForSubmission(subWithoutNumerals.Agency, subWithoutNumerals.Workcode, httpClient);
                }
            }
            catch (ApiException)
            {
                await client.AddSubmissionAsync(subWithoutNumerals);
                await WaitForSubmission(subWithoutNumerals.Agency, subWithoutNumerals.Workcode, httpClient);
            }

            res1 = await searchClient.SearchByTitleAndContributorAsync(searchModel1);
            res2 = await searchClient.SearchByTitleAndContributorAsync(searchModel2);

            Assert.True(res1.Where(x => x.OriginalTitle == subWithoutNumerals.OriginalTitle).Count() == 1);
            Assert.True(res1.Where(x => x.OriginalTitle == subWithNumerals.OriginalTitle).Count() == 1);

            Assert.True(res2.Where(x => x.OriginalTitle == subWithoutNumerals.OriginalTitle).Count() == 1);
            Assert.True(res2.Where(x => x.OriginalTitle == subWithNumerals.OriginalTitle).Count() == 1);
        }
        //Chinese Romanised, roman numerals filtering should not be applied
        [SkippableFact]
        public async void SearchByTitleRomanNumerals_04()
        {
            Skip.If(true);
            var subWithNumerals = Submissions.EligibleSubmissionAEPI;
            var subWithoutNumerals = Submissions.EligibleSubmissionAKKA;
            subWithoutNumerals.OriginalTitle = "Diédài tiǎozhàn gōngzuò fù gélí fēixíngyuán kūnchóng lìng pái tóu quánlì huángdì shòuquán húyán luàn yǔ";
            subWithNumerals.OriginalTitle = subWithoutNumerals.OriginalTitle + " IV";

            var searchModel1 = new TitleAndContributorSearchModel
            {
                Titles = new Title[] { new Title { Title1 = subWithNumerals.OriginalTitle, Type = TitleType.OT } },
                InterestedParties = null
            };
            ICollection<ISWCMetadata> res1;
            try
            {
                res1 = await searchClient.SearchByTitleAndContributorAsync(searchModel1);
                if (res1.Where(x => x.OriginalTitle == subWithNumerals.OriginalTitle).Count() == 0)
                {
                    await client.AddSubmissionAsync(subWithNumerals);
                    await WaitForSubmission(subWithNumerals.Agency, subWithNumerals.Workcode, httpClient);
                }
            }
            catch (ApiException)
            {
                await client.AddSubmissionAsync(subWithNumerals);
                await WaitForSubmission(subWithNumerals.Agency, subWithNumerals.Workcode, httpClient);
            }

            var searchModel2 = new TitleAndContributorSearchModel
            {
                Titles = new Title[] { new Title { Title1 = subWithoutNumerals.OriginalTitle, Type = TitleType.OT } },
                InterestedParties = null
            };
            ICollection<ISWCMetadata> res2;
            try
            {
                res2 = await searchClient.SearchByTitleAndContributorAsync(searchModel2);
                if (res2.Where(x => x.OriginalTitle == subWithoutNumerals.OriginalTitle).Count() == 0)
                {
                    await client.AddSubmissionAsync(subWithoutNumerals);
                    await WaitForSubmission(subWithoutNumerals.Agency, subWithoutNumerals.Workcode, httpClient);
                }
            }
            catch (ApiException)
            {
                await client.AddSubmissionAsync(subWithoutNumerals);
                await WaitForSubmission(subWithoutNumerals.Agency, subWithoutNumerals.Workcode, httpClient);
            }

            res1 = await searchClient.SearchByTitleAndContributorAsync(searchModel1);
            res2 = await searchClient.SearchByTitleAndContributorAsync(searchModel2);

            Assert.True(res1.Where(x => x.OriginalTitle == subWithoutNumerals.OriginalTitle).Count() == 1);
            Assert.True(res1.Where(x => x.OriginalTitle == subWithNumerals.OriginalTitle).Count() == 1);

            Assert.True(res2.Where(x => x.OriginalTitle == subWithoutNumerals.OriginalTitle).Count() == 1);
            Assert.True(res2.Where(x => x.OriginalTitle == subWithNumerals.OriginalTitle).Count() == 1);
        }
        //Finnish, roman numerals filtering should not be applied
        [Fact]
        public async void SearchByTitleRomanNumerals_05()
        {
            var subWithNumerals = Submissions.EligibleSubmissionAEPI;
            var subWithoutNumerals = Submissions.EligibleSubmissionAKKA;
            subWithoutNumerals.OriginalTitle = "Toista haaste Työ negatiivinen eristys Pilotti hyönteinen merkkipää Voimakeisari antaa vallan puhetta";
            subWithNumerals.OriginalTitle = subWithoutNumerals.OriginalTitle + " IV";

            var searchModel1 = new TitleAndContributorSearchModel
            {
                Titles = new Title[] { new Title { Title1 = subWithNumerals.OriginalTitle, Type = TitleType.OT } },
                InterestedParties = null
            };
            ICollection<ISWCMetadata> res1;
            try
            {
                res1 = await searchClient.SearchByTitleAndContributorAsync(searchModel1);
                if (res1.Where(x => x.OriginalTitle == subWithNumerals.OriginalTitle).Count() == 0)
                {
                    await client.AddSubmissionAsync(subWithNumerals);
                    await WaitForSubmission(subWithNumerals.Agency, subWithNumerals.Workcode, httpClient);
                }
            }
            catch (ApiException)
            {
                await client.AddSubmissionAsync(subWithNumerals);
                await WaitForSubmission(subWithNumerals.Agency, subWithNumerals.Workcode, httpClient);
            }

            var searchModel2 = new TitleAndContributorSearchModel
            {
                Titles = new Title[] { new Title { Title1 = subWithoutNumerals.OriginalTitle, Type = TitleType.OT } },
                InterestedParties = null
            };
            ICollection<ISWCMetadata> res2;
            try
            {
                res2 = await searchClient.SearchByTitleAndContributorAsync(searchModel2);
                if (res2.Where(x => x.OriginalTitle == subWithoutNumerals.OriginalTitle).Count() == 0)
                {
                    await client.AddSubmissionAsync(subWithoutNumerals);
                    await WaitForSubmission(subWithoutNumerals.Agency, subWithoutNumerals.Workcode, httpClient);
                }
            }
            catch (ApiException)
            {
                await client.AddSubmissionAsync(subWithoutNumerals);
                await WaitForSubmission(subWithoutNumerals.Agency, subWithoutNumerals.Workcode, httpClient);
            }

            res1 = await searchClient.SearchByTitleAndContributorAsync(searchModel1);
            res2 = await searchClient.SearchByTitleAndContributorAsync(searchModel2);

            Assert.True(res1.Where(x => x.OriginalTitle == subWithoutNumerals.OriginalTitle).Count() == 1);
            Assert.True(res1.Where(x => x.OriginalTitle == subWithNumerals.OriginalTitle).Count() == 1);

            Assert.True(res2.Where(x => x.OriginalTitle == subWithoutNumerals.OriginalTitle).Count() == 1);
            Assert.True(res2.Where(x => x.OriginalTitle == subWithNumerals.OriginalTitle).Count() == 1);
        }
    }
}