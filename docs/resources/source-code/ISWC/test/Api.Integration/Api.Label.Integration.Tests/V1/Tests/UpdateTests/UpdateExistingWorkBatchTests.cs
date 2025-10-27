using SpanishPoint.Azure.Iswc.Api.Label.Integration.Tests.V1.Data;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Label.Integration.Tests.V1.Tests.UpdateTests
{
    public class UpdateExistingWorkBatchTests : TestBase, IAsyncLifetime
    {
        private IISWC_Label_SubmissionClient client;
        private HttpClient httpClient;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await GetClient();
            client = new ISWC_Label_SubmissionClient(httpClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Scenario: Label revision of Provisional Iswc. 
        /// Expected: First submission should create a provisional Iswc. Second submission should update that provisional Iswc to add the extra creator.
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkBatchTest_AddAddtionalCreator_ReturnsSuccess()
        {
            var addSubmission = Submissions.EligibleLabelSubmissionCISAC;

            List<SubmissionBatch> batch = new List<SubmissionBatch>
            {
                new SubmissionBatch
                {
                    SubmissionId = 1,
                    Submission = addSubmission
                }
            };

            var addResponse = await client.AddLabelSubmissionBatchAsync(batch);

            var updateSubmission = Submissions.EligibleLabelSubmissionCISAC;
            updateSubmission.Workcode = addSubmission.Workcode;
            updateSubmission.OriginalTitle = addSubmission.OriginalTitle;
            updateSubmission.InterestedParties.Add(new InterestedParty() { Name = "KORKEJIAN AZNIV", LastName = "KORKEJIAN", Role = InterestedPartyRole.CA });
            updateSubmission.AdditionalIdentifiers.LabelIdentifiers = addSubmission.AdditionalIdentifiers.LabelIdentifiers;
            updateSubmission.AdditionalIdentifiers.Recordings = addSubmission.AdditionalIdentifiers.Recordings;

            batch = new List<SubmissionBatch>
            {
                new SubmissionBatch
                {
                    SubmissionId = 1,
                    Submission = updateSubmission
                }
            };

            var updateResponse = await client.AddLabelSubmissionBatchAsync(batch);

            Assert.NotNull(addResponse.FirstOrDefault().Submission.VerifiedSubmission);
            Assert.NotNull(updateResponse.FirstOrDefault().Submission.VerifiedSubmission);
            Assert.Equal("Provisional", addResponse.FirstOrDefault().Submission.VerifiedSubmission.IswcStatus);
            Assert.Equal("Provisional", updateResponse.FirstOrDefault().Submission.VerifiedSubmission.IswcStatus);
            Assert.Equal(addResponse.FirstOrDefault().Submission.VerifiedSubmission.Iswc, updateResponse.FirstOrDefault().Submission.VerifiedSubmission.Iswc);
            Assert.Equal(1, addResponse.FirstOrDefault().Submission.VerifiedSubmission.InterestedParties.Count);
            Assert.Equal(2, updateResponse.FirstOrDefault().Submission.VerifiedSubmission.InterestedParties.Count);
        }
    }
}
