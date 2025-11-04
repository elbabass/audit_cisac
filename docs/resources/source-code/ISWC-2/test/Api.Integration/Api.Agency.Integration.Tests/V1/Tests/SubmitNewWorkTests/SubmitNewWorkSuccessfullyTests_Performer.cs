using Newtonsoft.Json;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using xRetry;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Tests.SubmitNewWorkTests
{
    public class SubmitNewWorkSuccessfullyTests_Performer_Fixture : IAsyncLifetime
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

    public class SubmitNewWorkSuccessfullyTests_Performer : TestBase, IClassFixture<SubmitNewWorkSuccessfullyTests_Performer_Fixture>
    {
        private IISWC_SubmissionClient client;
        private HttpClient httpClient;

        public SubmitNewWorkSuccessfullyTests_Performer(SubmitNewWorkSuccessfullyTests_Performer_Fixture fixture)
        {
            client = fixture.client;
            httpClient = fixture.httpClient;
        }

        /// <summary>
        /// Submitter is ISWC eligible
        /// Performers can be added to the submission.
        /// One performer exists, the second is new.
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccessfullyTests_Performer_01()
        {
            var noIswcSubmission = Submissions.EligibleSubmissionSACEM;
            var performerOne = new Performer
            {
                Isni = "00001345134",
                Ipn = 172346512,
                FirstName = "Test",
                LastName = "Performer",
                Designation = PerformerDesignation.Main_Artist
            };
            var performerTwo = new Performer
            {
                LastName = CreateNewWorkCode(),
                Designation = PerformerDesignation.Main_Artist
            };
            noIswcSubmission.Performers = new List<Performer>()
            {
                performerOne,
                performerTwo
            };
            var res = await client.AddSubmissionAsync(noIswcSubmission);
            var firstPerformerRes = JsonConvert.SerializeObject(res.VerifiedSubmission.Performers.Where(x => x.LastName == performerOne.LastName).FirstOrDefault());
            var secondPerformerRes = JsonConvert.SerializeObject(res.VerifiedSubmission.Performers.Where(x => x.LastName == performerTwo.LastName).FirstOrDefault());

            Assert.NotNull(res.VerifiedSubmission);
            Assert.Null(res.VerifiedSubmission.Rejection);
            Assert.Equal(JsonConvert.SerializeObject(performerOne), firstPerformerRes);
            Assert.Equal(JsonConvert.SerializeObject(performerTwo), secondPerformerRes);
        }

        /// <summary>
        /// Submitter is not ISWC eligible
        /// Performers added that are not in original submission
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccessfullyTests_Performer_02()
        {
            var noIswcSubmission = Submissions.EligibleSubmissionSACEM;
            await client.AddSubmissionAsync(noIswcSubmission);
            await WaitForSubmission(noIswcSubmission.Agency, noIswcSubmission.Workcode, httpClient);
            noIswcSubmission.Agency = Submissions.EligibleSubmissionAKKA.Agency;
            noIswcSubmission.Sourcedb = Submissions.EligibleSubmissionAKKA.Sourcedb;
            var performer = new Performer
            {
                FirstName = "Test3",
                LastName = "Performer3",
                Designation = PerformerDesignation.Main_Artist
            };
            noIswcSubmission.Performers = new List<Performer>()
            {
                performer
            };
            noIswcSubmission.Workcode = CreateNewWorkCode();
            var res = await client.AddSubmissionAsync(noIswcSubmission);
            var performerStr = JsonConvert.SerializeObject(performer);
            var responseStr = JsonConvert.SerializeObject(res.VerifiedSubmission.Performers.ElementAtOrDefault(0));

            Assert.NotNull(res.VerifiedSubmission);
            Assert.Null(res.VerifiedSubmission.Rejection);
            Assert.Equal(performerStr, responseStr);
        }

        /// <summary>
        /// Submitter is ISWC eligible
        /// Performers list contains duplicate performers.
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccessfullyTests_Performer_03()
        {
            var submission = Submissions.EligibleSubmissionSACEM;
            var performer = new Performer
            {
                Isni = "00001345134",
                Ipn = 172346512,
                FirstName = "Test",
                LastName = "Performer",
                Designation = PerformerDesignation.Main_Artist
            };
            submission.Performers = new List<Performer>()
            {
                performer,
                performer
            };
            var res = await client.AddSubmissionAsync(submission);
            var firstPerformerRes = JsonConvert.SerializeObject(res.VerifiedSubmission.Performers.ElementAtOrDefault(0));

            Assert.NotNull(res.VerifiedSubmission);
            Assert.True(res.VerifiedSubmission.Performers.Count == 1);
            Assert.Equal(JsonConvert.SerializeObject(performer), firstPerformerRes);
        }

        /// <summary>
        /// Submitter is ISWC eligible
        /// Two new performers are added.
        /// </summary>
        [Fact]
        public async void SubmitNewWorkSuccessfullyTests_Performer_04()
        {
            var noIswcSubmission = Submissions.EligibleSubmissionSACEM;
            var performerOne = new Performer
            {
                LastName = CreateNewWorkCode(),
                Designation = PerformerDesignation.Main_Artist
            };
            var performerTwo = new Performer
            {
                LastName = CreateNewWorkCode(),
                Designation = PerformerDesignation.Main_Artist
            };
            noIswcSubmission.Performers = new List<Performer>()
            {
                performerOne,
                performerTwo
            };
            var res = await client.AddSubmissionAsync(noIswcSubmission);
            var firstPerformerRes = JsonConvert.SerializeObject(res.VerifiedSubmission.Performers.Where(x => x.LastName == performerOne.LastName).FirstOrDefault());
            var secondPerformerRes = JsonConvert.SerializeObject(res.VerifiedSubmission.Performers.Where(x => x.LastName == performerTwo.LastName).FirstOrDefault());

            Assert.NotNull(res.VerifiedSubmission);
            Assert.Null(res.VerifiedSubmission.Rejection);
            Assert.Equal(JsonConvert.SerializeObject(performerOne), firstPerformerRes);
            Assert.Equal(JsonConvert.SerializeObject(performerTwo), secondPerformerRes);
        }
    }
}