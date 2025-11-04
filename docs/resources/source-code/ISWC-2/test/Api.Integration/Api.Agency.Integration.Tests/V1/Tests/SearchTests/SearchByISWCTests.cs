using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.Configuration;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Tests.SearchTests
{

    public class SearchByISWCTests_Fixture : IAsyncLifetime
    {
        public IISWC_SubmissionClient submissionClient;
        public IISWC_SearchClient searchclient;
        public HttpClient httpClient;
        public readonly List<Submission> submissions = new List<Submission>();
        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await TestBase.GetClient();
            submissionClient = new ISWC_SubmissionClient(httpClient);
            searchclient = new ISWC_SearchClient(httpClient);
            var subOne = Submissions.EligibleSubmissionAEPI;
            subOne.InterestedParties.Add(new InterestedParty
            {
                NameNumber = 568562507,
                Role = InterestedPartyRole.CA,
                BaseNumber = "I-001651337-2"
            });
            var subTwo = Submissions.EligibleSubmissionAEPI;

            var subThree = Submissions.EligibleSubmissionAEPI;
            subThree.InterestedParties.Add(new InterestedParty
            {
                NameNumber = 277016857,
                Role = InterestedPartyRole.CA,
                BaseNumber = "I-000039884-3"
            });
            subOne.PreferredIswc = (await submissionClient.AddSubmissionAsync(subOne)).VerifiedSubmission.Iswc.ToString();
            subTwo.PreferredIswc = (await submissionClient.AddSubmissionAsync(subTwo)).VerifiedSubmission.Iswc.ToString();
            subThree.PreferredIswc = (await submissionClient.AddSubmissionAsync(subThree)).VerifiedSubmission.Iswc.ToString();
            submissions.Add(subOne);
            submissions.Add(subTwo);
            submissions.Add(subThree);

            await TestBase.WaitForSubmission(subThree.Agency, subThree.Workcode, httpClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            return Task.CompletedTask;
        }
    }

    public class SearchByISWCTests : TestBase, IClassFixture<SearchByISWCTests_Fixture>
    {
        public readonly IISWC_SearchClient searchclient;
        private readonly List<Submission> submissions;

        public SearchByISWCTests(SearchByISWCTests_Fixture fixture)
        {
            submissions = fixture.submissions;
            searchclient = fixture.searchclient;
        }

        [Fact]
        public async void SearchByISWCTests_01()
        {
            var iswc = submissions[0].PreferredIswc;
            var res = await searchclient.SearchByISWCAsync(iswc);

            Assert.Equal(iswc, res.Iswc);
            Assert.True(res.Works.Count > 0);
            Assert.Equal(submissions[0].Workcode, res.Works.First().Workcode);
        }

        /// <summary>
        /// IP's within PG group are not returned.
        /// </summary>
        [Fact]
        public async void SearchByISWCTests_02()
        {
            var iswc = submissions[0].PreferredIswc;
            var res = await searchclient.SearchByISWCAsync(iswc);

            Assert.Equal(iswc, res.Iswc);
            Assert.True(res.Works.Count > 0);
            Assert.Equal(3, res.Works.First().InterestedParties.Count);
            Assert.DoesNotContain(res.Works.First().InterestedParties, x => x.NameNumber == 45529476 || x.NameNumber == 251745079);
            Assert.Contains(res.Works.First().InterestedParties, x => x.NameNumber == 568562507);
        }

        /// <summary>
        /// PA is returned when PA is submitted
        /// </summary>
        [Fact]
        public async void SearchByISWCTests_03()
        {
            var iswc = submissions[2].PreferredIswc;
            var res = await searchclient.SearchByISWCAsync(iswc);

            Assert.Equal(iswc, res.Iswc);
            Assert.True(res.Works.Count > 0);
            Assert.Equal(3, res.Works.First().InterestedParties.Count);
            Assert.DoesNotContain(res.Works.First().InterestedParties, x => x.NameNumber == 276396620); // PA is not returned
            Assert.Contains(res.Works.First().InterestedParties, x => x.NameNumber == 277016857);
        }

        [Fact]
        public async void SearchByISWCTests_04()
        {
            var iswcOne = submissions[0].PreferredIswc;
            var iswcTwo = submissions[1].PreferredIswc;
            var batch = new IswcSearchModel[] {
                new IswcSearchModel { Iswc = iswcOne },
                new IswcSearchModel { Iswc = iswcTwo }
            };

            var res = await searchclient.SearchByISWCBatchAsync(batch);

            Assert.Collection(res,
                elem1 => Assert.Equal(batch[0].Iswc, elem1.SearchResults.FirstOrDefault().Iswc),
                elem2 => Assert.Equal(batch[1].Iswc, elem2.SearchResults.FirstOrDefault().Iswc));
        }
    }
}
