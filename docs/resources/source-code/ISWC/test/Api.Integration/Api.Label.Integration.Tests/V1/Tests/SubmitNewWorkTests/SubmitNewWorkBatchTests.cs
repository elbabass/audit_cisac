using SpanishPoint.Azure.Iswc.Api.Label.Integration.Tests.V1.Data;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Label.Integration.Tests.V1.Tests.SubmitNewWorkTests
{
    public class ProvisionalIswcMatchingTests: TestBase, IAsyncLifetime
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
        /// Scenario: Submit Label submission with multiple label identifiers. 
        /// Expected: Returns rejection 248.
        /// </summary>
        [Fact]
        public async void SubmitNewWorkBatchTest_WithMultipleIdentifiers_ReturnsRejection()
        {
            var submission = Submissions.EligibleLabelSubmissionPRS;
            var multipleLabelIdentifiersSubmission = Submissions.EligibleLabelSubmissionPRS;
            multipleLabelIdentifiersSubmission.AdditionalIdentifiers.LabelIdentifiers.Add(new LabelIdentifiers
            {
                SubmitterDPID = "DPID",
                WorkCode = new List<string>{ TestBase.CreateNewWorkCode() }
            });

            List<SubmissionBatch> batch = new List<SubmissionBatch>
            {
                new SubmissionBatch
                {
                    SubmissionId = 1,
                    Submission = submission
                },
                new SubmissionBatch
                {
                    SubmissionId = 2,
                    Submission = multipleLabelIdentifiersSubmission
                }
                
            };

            var response = await client.AddLabelSubmissionBatchAsync(batch);

            Assert.Collection(response,
                elem1 => Assert.NotNull(elem1.Submission.VerifiedSubmission),
                elem2 => Assert.Equal("248", elem2.Rejection.Code));
        }

        /// <summary>
        /// Scenario: Submit Label submission without additional identifiers. 
        /// Expected: Returns rejection 250.
        /// </summary>
        [Fact]
        public async void SubmitNewWorkBatchTest_WithoutAdditionalIdentifiers_ReturnsRejection()
        {
            var submission = Submissions.EligibleLabelSubmissionPRS;
            submission.AdditionalIdentifiers = null;

            List<SubmissionBatch> batch = new List<SubmissionBatch>
            {
                new SubmissionBatch
                {
                    SubmissionId = 1,
                    Submission = submission
                }
            };

            var response = await client.AddLabelSubmissionBatchAsync(batch);

            Assert.Collection(response, elem1 => Assert.Equal("250", elem1.Rejection.Code));
        }

        /// <summary>
        /// Scenario: Submit Label submission with empty SubmitterDPID. 
        /// Expected: Returns rejection 250.
        /// </summary>
        [Fact]
        public async void SubmitNewWorkBatchTest_WithEmptySubmitterDpId_ReturnsRejection()
        {
            var submission = Submissions.EligibleLabelSubmissionPRS;
            submission.AdditionalIdentifiers.LabelIdentifiers = new List<LabelIdentifiers>
            {
                new LabelIdentifiers
                {
                    SubmitterDPID = string.Empty,
                    WorkCode = new List<string>{ TestBase.CreateNewWorkCode() }
                }
            };

            List<SubmissionBatch> batch = new List<SubmissionBatch>
            {
                new SubmissionBatch
                {
                    SubmissionId = 1,
                    Submission = submission
                }
            };

            var response = await client.AddLabelSubmissionBatchAsync(batch);

            Assert.Collection(response, elem1 => Assert.Equal("250", elem1.Rejection.Code));
        }

        /// <summary>
        /// Scenario: Submit Label submission without IP name. 
        /// Expected: Returns rejection 251.
        /// </summary>
        [Fact]
        public async void SubmitNewWorkBatchTest_WithoutIpName_ReturnsRejection()
        {
            var submission = Submissions.EligibleLabelSubmissionPRS;
            submission.InterestedParties = new List<InterestedParty>
            {
                new InterestedParty() { LastName = "North", Role = InterestedPartyRole.CA }
            };

            List<SubmissionBatch> batch = new List<SubmissionBatch>
            {
                new SubmissionBatch
                {
                    SubmissionId = 1,
                    Submission = submission
                }
            };

            var response = await client.AddLabelSubmissionBatchAsync(batch);

            Assert.Collection(response, elem1 => Assert.Equal("251", elem1.Rejection.Code));
        }

        /// <summary>
        /// Scenario: Submit Label submission without IP last name. 
        /// Expected: Returns rejection 251.
        /// </summary>
        [Fact]
        public async void SubmitNewWorkBatchTest_WithoutIpLastName_ReturnsRejection()
        {
            var submission = Submissions.EligibleLabelSubmissionPRS;
            submission.InterestedParties = new List<InterestedParty>
            {
                new InterestedParty() { Name = "Hy", Role = InterestedPartyRole.CA }
            };

            List<SubmissionBatch> batch = new List<SubmissionBatch>
            {
                new SubmissionBatch
                {
                    SubmissionId = 1,
                    Submission = submission
                }
            };

            var response = await client.AddLabelSubmissionBatchAsync(batch);

            Assert.Collection(response, elem1 => Assert.Equal("251", elem1.Rejection.Code));
        }
    }
}
