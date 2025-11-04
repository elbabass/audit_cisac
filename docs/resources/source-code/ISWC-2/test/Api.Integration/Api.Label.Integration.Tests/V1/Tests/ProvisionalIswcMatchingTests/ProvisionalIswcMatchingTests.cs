using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using SpanishPoint.Azure.Iswc.Api.Label.Integration.Tests.V1.Data;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using agencyData = SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using agencyDefinition = SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;

namespace SpanishPoint.Azure.Iswc.Api.Label.Integration.Tests.V1.Tests.ProvisionalIswcMatchingTests
{
    public class ProvisionalIswcMatchingTests: TestBase, IAsyncLifetime
    {
        private IISWC_Label_SubmissionClient labelClient;
        private IISWC_SubmissionClient agencyClient;
        private HttpClient httpLabelClient, httpAgencyClient;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpLabelClient = await GetClient();
            httpAgencyClient = await GetAgencyClient();
            labelClient = new ISWC_Label_SubmissionClient(httpLabelClient);
            agencyClient = new ISWC_SubmissionClient(httpAgencyClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpLabelClient.Dispose();
            httpAgencyClient.Dispose();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Scenario: Submit Label submission with unique title (including numbers) and creators. 
        /// Expected: No match found and new Provisional Iswc allocated.
        /// </summary>
        [Fact]
        public async void ProvisionalIswcMatchingTests_WithUniqueTitleIncludingNumbersAndCreators_ReturnsSuccess()
        {
            var submission = Submissions.EligibleLabelSubmissionPRS;

            List<SubmissionBatch> batch = new List<SubmissionBatch>
            {
                new SubmissionBatch
                {
                    SubmissionId = 1,
                    Submission = submission
                }
            };

            var response = await labelClient.AddLabelSubmissionBatchAsync(batch);

            Assert.Collection(response,
                elem1 =>
                {
                    Assert.NotNull(elem1.Submission.VerifiedSubmission);
                    Assert.Equal("Provisional", elem1.Submission.VerifiedSubmission.IswcStatus);
                });
        }

        /// <summary>
        /// Scenario: Submit Label submission with unique title (excluding numbers) and creators. 
        /// Expected: No match found and new Provisional Iswc allocated.
        /// </summary>
        [Fact]
        public async void ProvisionalIswcMatchingTests_WithUniqueTitleExcludingNumbersAndCreators_ReturnsSuccess()
        {
            var submission = Submissions.EligibleLabelSubmissionPRS;
            submission.OriginalTitle = CreateNewTitleWithLettersOnly();

            List<SubmissionBatch> batch = new List<SubmissionBatch>
            {
                new SubmissionBatch
                {
                    SubmissionId = 1,
                    Submission = submission
                }
            };

            var response = await labelClient.AddLabelSubmissionBatchAsync(batch);

            Assert.Collection(response,
                elem1 =>
                {
                    Assert.NotNull(elem1.Submission.VerifiedSubmission);
                    Assert.Equal("Provisional", elem1.Submission.VerifiedSubmission.IswcStatus);
                });
        }

        /// <summary>
        /// Scenario: Submit Label submission with unique title and creators without name numbers. 
        /// Expected: No match found and new Provisional Iswc allocated.
        /// </summary>
        [Fact]
        public async void ProvisionalIswcMatchingTests_WithUniqueTitleAndCreatorsWithoutNameNumbers_ReturnsSuccess()
        {
            var submission = Submissions.EligibleLabelSubmissionSESAC;          

            List<SubmissionBatch> batch = new List<SubmissionBatch>
            {
                new SubmissionBatch
                {
                    SubmissionId = 1,
                    Submission = submission
                }
            };

            var response = await labelClient.AddLabelSubmissionBatchAsync(batch);

            Assert.Collection(response,
                elem1 =>
                {
                    Assert.NotNull(elem1.Submission.VerifiedSubmission);
                    Assert.Equal("Provisional", elem1.Submission.VerifiedSubmission.IswcStatus);
                });
        }

        /// <summary>
        /// Scenario: Submit Label submission with same title and creators (including name numbers) as an existing preferred Iswc. 
        /// Expected: Matches existing Preferred Iswc.
        /// </summary>
        [Fact]
        public async void ProvisionalIswcMatchingTests_SameMetadataIncludingNameNumber_ReturnsMatch()
        {
            agencyDefinition.Submission agencySubmission = agencyData.Submissions.EligibleSubmissionPRS;

            var agencyResponse = await agencyClient.AddSubmissionAsync(agencySubmission);

            await TestBase.WaitForSubmission(agencySubmission.Agency, agencySubmission.Workcode, httpAgencyClient);

            var labelSubmission = Submissions.EligibleLabelSubmissionSESAC;
            labelSubmission.OriginalTitle = agencySubmission.OriginalTitle;
            labelSubmission.InterestedParties = new List<InterestedParty>();
            foreach (var ip in agencySubmission.InterestedParties)
            {
                labelSubmission.InterestedParties.Add(new InterestedParty
                {
                    BaseNumber = ip.BaseNumber,
                    NameNumber = ip.NameNumber,
                    Name = ip.Name,
                    LastName = ip.LastName,
                    Role = InterestedPartyRole.C
                });
            } 

            List<SubmissionBatch> batch = new List<SubmissionBatch>
            {
                new SubmissionBatch
                {
                    SubmissionId = 1,
                    Submission = labelSubmission
                }
            };

            var response = await labelClient.AddLabelSubmissionBatchAsync(batch);

            Assert.Collection(response,
                elem1 =>
                {
                    Assert.NotNull(elem1.Submission.VerifiedSubmission);
                    Assert.Equal("Preferred", elem1.Submission.VerifiedSubmission.IswcStatus);
                    Assert.Equal(agencyResponse.VerifiedSubmission.Iswc, elem1.Submission.VerifiedSubmission.Iswc);
                });
        }

        /// <summary>
        /// Scenario: Submit Label submission with same title and creators (excluding name numbers) as an existing preferred Iswc. 
        /// Expected: Matches existing Preferred Iswc.
        /// </summary>
        [Fact]
        public async void ProvisionalIswcMatchingTests_SameMetadataExcludingNameNumber_ReturnsMatch()
        {
            agencyDefinition.Submission agencySubmission = agencyData.Submissions.EligibleSubmissionBMI;

            var agencyResponse = await agencyClient.AddSubmissionAsync(agencySubmission);

            await TestBase.WaitForSubmission(agencySubmission.Agency, agencySubmission.Workcode, httpAgencyClient);

            var labelSubmission = Submissions.EligibleLabelSubmissionSESAC;
            labelSubmission.OriginalTitle = agencySubmission.OriginalTitle;
            labelSubmission.InterestedParties = new List<InterestedParty>();
            foreach (var ip in agencySubmission.InterestedParties)
            {
                labelSubmission.InterestedParties.Add(new InterestedParty
                {
                    Name = ip.Name,
                    LastName = ip.LastName,
                    Role = InterestedPartyRole.C
                });
            }

            List<SubmissionBatch> batch = new List<SubmissionBatch>
            {
                new SubmissionBatch
                {
                    SubmissionId = 1,
                    Submission = labelSubmission
                }
            };

            var response = await labelClient.AddLabelSubmissionBatchAsync(batch);

            Assert.Collection(response,
                elem1 =>
                {
                    Assert.NotNull(elem1.Submission.VerifiedSubmission);
                    Assert.Equal("Preferred", elem1.Submission.VerifiedSubmission.IswcStatus);
                    Assert.Equal(agencyResponse.VerifiedSubmission.Iswc, elem1.Submission.VerifiedSubmission.Iswc);
                });
        }
    }
}
