using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using xRetry;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Tests.SubmitNewWorkTests
{
    public class SubmitNewWorkSuccess_SpecialCharacters_Fixture : IAsyncLifetime
    {
        public IISWC_SubmissionClient client;
        public HttpClient httpClient;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await TestBase.GetClient();
            client = new ISWC_SubmissionClient(httpClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            return Task.CompletedTask;
        }
    }

    public class SubmitNewWorkSuccess_SpecialCharacters : TestBase, IClassFixture<SubmitNewWorkSuccess_SpecialCharacters_Fixture>
    {
        private IISWC_SubmissionClient client;
        private HttpClient httpClient;

        public SubmitNewWorkSuccess_SpecialCharacters(SubmitNewWorkSuccess_SpecialCharacters_Fixture fixture)
        {
            client = fixture.client;
            httpClient = fixture.httpClient;
        }

        /// <summary>
        /// Strip the specified special characters from the submission titles.
        /// Testing AT's.
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccess_SpecialCharacters_01()
        {
            IEnumerable<string> charactersToReplace = new List<string>
        {
            "\u0001","\u0002","\u0003","\u0004","\u0005","\u0006","\u0007","\u0008","\u0009","\u000A",
            "\u000B","\u000C","\u000D","\u000E","\u000F","\u0010","\u0011","\u0012","\u0013","\u0014",
            "\u0015","\u0016","\u0017","\u0018","\u0019","\u0009","\u001A","\u001B","\u001C","\u001D",
            "\u001E","\u001F"
        };
            var submission = Submissions.EligibleSubmissionSACEM;
            submission.OtherTitles = new List<Title>();
            foreach (var charToReplace in charactersToReplace)
            {
                submission.OtherTitles.Add(new Title
                {
                    Title1 = $"{CreateNewTitleWithLettersOnly()} {charToReplace}",
                    Type = TitleType.AT
                });
            }
            var res = await client.AddSubmissionAsync(submission);

            for (int i = 0; i < charactersToReplace.Count(); i++)
            {
                var returnedTitle = res.VerifiedSubmission.OtherTitles.FirstOrDefault(
                    x => x.Title1.Equals(submission.OtherTitles.ElementAt(i).Title1.Replace(charactersToReplace.ElementAt(i), string.Empty)));
                Assert.NotNull(returnedTitle);
            }
        }

        /// <summary>
        /// Strip the specified special characters from the submission OT.
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccess_SpecialCharacters_02()
        {
            var submission = Submissions.EligibleSubmissionSACEM;
            submission.OriginalTitle = $"{submission.OriginalTitle} \u0001";
            var res = await client.AddSubmissionAsync(submission);

            Assert.Equal(submission.OriginalTitle.Replace("\u0001", string.Empty), res.VerifiedSubmission.OriginalTitle);
        }

        /// <summary>
        /// New submission matches to existing ISWC based on title without special characters.
        /// EnableWorkTitleContainsNumberExactMatch will require an exact title match since there is a number.
        /// </summary>
        [SkippableFact]
        public async void SubmitNewWorkSuccess_SpecialCharacters_03()
        {
            Xunit.Skip.IfNot(await IsMatchingRuleEnabled("EnableWorkTitleContainsNumberExactMatch"));
            var submission = Submissions.EligibleSubmissionASCAP;
            submission.OriginalTitle += " 1";
            var existingIswc = (await client.AddSubmissionAsync(submission)).VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            submission.OriginalTitle += " \u0001 \u0006 \u001B";
            submission.Workcode = CreateNewWorkCode();
            var res = await client.AddSubmissionAsync(submission);

            Assert.Equal(existingIswc, res.VerifiedSubmission.Iswc.ToString());
        }

        /// <summary>
        /// Strip the specified special characters from the workcode
        /// </summary>
        [RetryFact]
        public async void SubmitNewWorkSuccess_SpecialCharacters_04()
        {
            var submission = Submissions.EligibleSubmissionSACEM;
            submission.Workcode = $"\u001B{submission.Workcode.Substring(0, 10)}\u0001";
            var res = await client.AddSubmissionAsync(submission);

            Assert.Equal(submission.Workcode.Replace("\u0001", string.Empty).Replace("\u001B", string.Empty),
                res.VerifiedSubmission.Workcode);
        }
    }
}