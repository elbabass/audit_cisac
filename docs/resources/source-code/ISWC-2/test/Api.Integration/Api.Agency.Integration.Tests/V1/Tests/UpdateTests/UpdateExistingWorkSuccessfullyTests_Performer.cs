using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using xRetry;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Tests.UpdateTests
{
    public class UpdateExistingWorkSuccessfullyTests_Performer_Fixture : IAsyncLifetime
    {
        public IISWC_SubmissionClient submissionClient;
        public HttpClient httpClient;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await TestBase.GetClient();
            submissionClient = new ISWC_SubmissionClient(httpClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// 2. Update existing ISWC eligible work successfully
    /// </summary>
    public class UpdateExistingWorkSuccessfullyTests_Performer : TestBase, IAsyncLifetime, IClassFixture<UpdateExistingWorkSuccessfullyTests_Performer_Fixture>
    {
        private IISWC_SubmissionClient client;
        private Submission currentSubmission;
        private readonly HttpClient httpClient;

        public UpdateExistingWorkSuccessfullyTests_Performer(UpdateExistingWorkSuccessfullyTests_Performer_Fixture fixture)
        {
            client = fixture.submissionClient;
            httpClient = fixture.httpClient;
        }

        async Task IAsyncLifetime.InitializeAsync()
        {
            currentSubmission = Submissions.EligibleSubmissionBMI;
            var res = await client.AddSubmissionAsync(currentSubmission);
            currentSubmission.PreferredIswc = res.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(currentSubmission.Agency, currentSubmission.Workcode, httpClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Update contains performers that were not in original submission.
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_Performer_01()
        {
            currentSubmission.Performers = new List<Performer>() {
                new Performer
                {
                    FirstName="Test",
                    LastName = "Performer"
                }
            };
            var updateRes = await client.UpdateSubmissionAsync(currentSubmission.PreferredIswc, currentSubmission);

            Assert.True(updateRes.VerifiedSubmission.Performers.Count == 1);
            Assert.Equal(currentSubmission.Performers.ElementAt(0).LastName, updateRes.VerifiedSubmission.Performers.ElementAt(0).LastName);
        }

        /// <summary>
        /// Updating a work that has existing performers to add extra performers.
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_Performer_02()
        {
            currentSubmission.Performers = new List<Performer>() {
                new Performer
                {
                    FirstName="Test",
                    LastName = "Performer"
                }
            };
            currentSubmission.Workcode = CreateNewWorkCode();
            await client.AddSubmissionAsync(currentSubmission);
            await WaitForSubmission(currentSubmission.Agency, currentSubmission.Workcode, httpClient);

            currentSubmission.Performers.Add(new Performer
            {
                LastName = "Second Performer"
            });
            var updateRes = await client.UpdateSubmissionAsync(currentSubmission.PreferredIswc, currentSubmission);

            Assert.True(updateRes.VerifiedSubmission.Performers.Count == 2);
            Assert.Equal(currentSubmission.Performers.ElementAt(1).LastName,
                updateRes.VerifiedSubmission.Performers.ElementAt(1).LastName);
        }

        /// <summary>
        /// Updating a work that has existing performers to remove all performers.
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkSuccessfullyTests_Performer_03()
        {
            currentSubmission.Performers = new List<Performer>() {
                new Performer
                {
                    FirstName="Test",
                    LastName = "Performer"
                }
            };
            currentSubmission.Workcode = CreateNewWorkCode();
            await client.AddSubmissionAsync(currentSubmission);
            await WaitForSubmission(currentSubmission.Agency, currentSubmission.Workcode, httpClient);

            currentSubmission.Performers = new List<Performer>() { };
            var updateRes = await client.UpdateSubmissionAsync(currentSubmission.PreferredIswc, currentSubmission);

            Assert.True(updateRes.VerifiedSubmission.Performers.Count == 0);
        }
    }
}
