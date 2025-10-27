using Azure.Search.Documents;
using IdentityServer4.Models;
using Microsoft.AspNetCore.SignalR;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using SpanishPoint.Azure.Iswc.Api.Label.Integration.Tests.V1.Data;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using agencyData = SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using agencyDefinition = SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;

namespace SpanishPoint.Azure.Iswc.Api.Label.Integration.Tests.V1.Tests.ProvisionalIswcMatchingTests
{
    public class ProvisionalIswcUpgradeTests_IswcStatus : TestBase, IAsyncLifetime
    {
        private IISWC_Label_SubmissionClient labelClient;
        private IISWC_SubmissionClient agencyClient;
        private IISWC_SearchClient searchClient;
        private HttpClient httpLabelClient, httpAgencyClient;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpLabelClient = await GetClient();
            httpAgencyClient = await GetAgencyClient();
            labelClient = new ISWC_Label_SubmissionClient(httpLabelClient);
            agencyClient = new ISWC_SubmissionClient(httpAgencyClient);
            searchClient = new ISWC_SearchClient(httpAgencyClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpLabelClient.Dispose();
            httpAgencyClient.Dispose();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Scenario: Submit Label submission to create provisional ISWC.
        ///           Then Submit Agency submission with same metadata.           
        /// Expected: Iswc matches and upgrades to preferred status.
        /// </summary>
        [Fact]
        public async void ProvisionalIswcUpgradeTests_SameMetadata_MatchesAndUpgradesToPreferredStatus()
        {
            var labelSubmission = Submissions.EligibleLabelSubmissionCISAC;

            var labelResponse = await labelClient.AddLabelSubmissionBatchAsync(new List<SubmissionBatch>
            {
                new SubmissionBatch
                {
                    SubmissionId = 1,
                    Submission = labelSubmission
                }
            });

            await WaitForSubmission(labelSubmission.Agency, labelSubmission.Workcode, httpAgencyClient);

            var labelVerifiedSubmission = labelResponse.ElementAt(0).Submission.VerifiedSubmission;

            agencyDefinition.Submission agencySubmission = agencyData.Submissions.EligibleSubmissionBMI;

            agencySubmission.OriginalTitle = labelSubmission.OriginalTitle;
            agencySubmission.InterestedParties = new List<agencyDefinition.InterestedParty>()
            {
                new agencyDefinition.InterestedParty
                {
                    Name = InterestedParties.IP_CISAC.ElementAt(0).Name,
                    LastName = InterestedParties.IP_CISAC.ElementAt(0).LastName,
                    NameNumber = 589238793,
                    BaseNumber = "I-002985150-9",
                    Role = agencyDefinition.InterestedPartyRole.C
                }
            };

            var agencyResponse = await agencyClient.AddSubmissionAsync(agencySubmission);
            var agencyVerifiedSubmission = agencyResponse.VerifiedSubmission;

            Assert.Equal(labelVerifiedSubmission.Iswc, agencyVerifiedSubmission.Iswc);
            Assert.Equal("Preferred", agencyVerifiedSubmission.IswcStatus);
        }

        /// <summary>
        /// Scenario: Submit Agency submission to create preferred ISWC.
        ///           Then Submit Label submission with same metadata.           
        /// Expected: Iswc remains with preferred status.
        /// </summary>
        [Fact]
        public async void ProvisionalIswcUpgradeTests_SameMetadata_IswcRemainsWithPreferredStatus()
        {
            agencyDefinition.Submission agencySubmission = agencyData.Submissions.EligibleSubmissionBMI;

            var agencyResponse = await agencyClient.AddSubmissionAsync(agencySubmission);
            var agencyVerifiedSubmission = agencyResponse.VerifiedSubmission;

            var labelSubmission = Submissions.EligibleLabelSubmissionCISAC;
            labelSubmission.OriginalTitle = agencySubmission.OriginalTitle;
            labelSubmission.InterestedParties = new List<InterestedParty>()
            {
                new InterestedParty
                {
                    Role = InterestedPartyRole.CA,
                    Name = "BAKER BILL",
                    LastName = "BAKER"
                },
                new InterestedParty
                {
                    Role = InterestedPartyRole.CA,
                    LastName = "REEVES"
                }
            };

            var labelResponse = await labelClient.AddLabelSubmissionBatchAsync(new List<SubmissionBatch>
            {
                new SubmissionBatch
                {
                    SubmissionId = 1,
                    Submission = labelSubmission
                }
            });

            var labelVerifiedSubmission = labelResponse.ElementAt(0).Submission.VerifiedSubmission;

            Assert.Equal(labelVerifiedSubmission.Iswc, agencyVerifiedSubmission.Iswc);
            Assert.Equal("Preferred", agencyVerifiedSubmission.IswcStatus);
        }

        /// <summary>
        /// Scenario: Submit Label submission to create provisional ISWC.
        ///           Submit Agency submission with same metadata.
        ///           Delete Label submission.
        /// Expected: Iswc remains with preferred status.
        /// </summary>
        [Fact]
        public async void ProvisionalIswcUpgradeTests_SameMetadataDeleteLabelSubmission_IswcRemainsWithPreferredStatus()
        {
            var labelSubmission = Submissions.EligibleLabelSubmissionCISAC;

            var labelResponse = await labelClient.AddLabelSubmissionBatchAsync(new List<SubmissionBatch>
            {
                new SubmissionBatch
                {
                    SubmissionId = 1,
                    Submission = labelSubmission
                }
            });

            var labelVerifiedSubmission = labelResponse.ElementAt(0).Submission.VerifiedSubmission;

            agencyDefinition.Submission agencySubmission = agencyData.Submissions.EligibleSubmissionBMI;

            agencySubmission.OriginalTitle = labelSubmission.OriginalTitle;
            agencySubmission.InterestedParties = new List<agencyDefinition.InterestedParty>()
            {
                new agencyDefinition.InterestedParty
                {
                    Name = InterestedParties.IP_CISAC.ElementAt(0).Name,
                    LastName = InterestedParties.IP_CISAC.ElementAt(0).LastName,
                    NameNumber = 589238793,
                    BaseNumber = "I-002985150-9",
                    Role = agencyDefinition.InterestedPartyRole.C
                }
            };

            var agencyResponse = await agencyClient.AddSubmissionAsync(agencySubmission);
            var agencyVerifiedSubmission = agencyResponse.VerifiedSubmission;

            await WaitForSubmission(agencySubmission.Agency, agencySubmission.Workcode, httpAgencyClient);
            await agencyClient.DeleteSubmissionAsync(labelVerifiedSubmission.Iswc.ToString(), labelSubmission.Agency, labelSubmission.Workcode, 315, "Integration test delete");

            var iswc = await searchClient.SearchByISWCAsync(agencyVerifiedSubmission.Iswc.ToString());

            Assert.Equal(labelVerifiedSubmission.Iswc, agencyVerifiedSubmission.Iswc);
            Assert.Equal("Preferred", iswc.IswcStatus);
        }

        /// <summary>
        /// Scenario: Submit Label submission to create provisional ISWC.
        ///           Submit Agency submission with same metadata.
        ///           Delete Agency submission.
        /// Expected: Iswc downgraded to provisional status.
        /// </summary>
        [Fact]
        public async void ProvisionalIswcUpgradeTests_SameMetadataDeleteLabelSubmission_IswcDowngradedtoProvisionalStatus()
        {
            var labelSubmission = Submissions.EligibleLabelSubmissionCISAC;

            var labelResponse = await labelClient.AddLabelSubmissionBatchAsync(new List<SubmissionBatch>
            {
                new SubmissionBatch
                {
                    SubmissionId = 1,
                    Submission = labelSubmission
                }
            });

            var labelVerifiedSubmission = labelResponse.ElementAt(0).Submission.VerifiedSubmission;

            agencyDefinition.Submission agencySubmission = agencyData.Submissions.EligibleSubmissionBMI;

            agencySubmission.OriginalTitle = labelSubmission.OriginalTitle;
            agencySubmission.InterestedParties = new List<agencyDefinition.InterestedParty>()
            {
                new agencyDefinition.InterestedParty
                {
                    Name = InterestedParties.IP_CISAC.ElementAt(0).Name,
                    LastName = InterestedParties.IP_CISAC.ElementAt(0).LastName,
                    NameNumber = 589238793,
                    BaseNumber = "I-002985150-9",
                    Role = agencyDefinition.InterestedPartyRole.C
                }
            };

            var agencyResponse = await agencyClient.AddSubmissionAsync(agencySubmission);
            var agencyVerifiedSubmission = agencyResponse.VerifiedSubmission;

            await WaitForSubmission(labelSubmission.Agency, labelSubmission.Workcode, httpAgencyClient);
            await agencyClient.DeleteSubmissionAsync(agencyVerifiedSubmission.Iswc.ToString(), agencySubmission.Agency, agencySubmission.Workcode, agencySubmission.Sourcedb, "Integration test delete");

            var iswc = await searchClient.SearchByISWCAsync(labelVerifiedSubmission.Iswc.ToString());

            Assert.Equal(labelVerifiedSubmission.Iswc, agencyVerifiedSubmission.Iswc);
            Assert.Equal("Provisional", iswc.IswcStatus);
        }

        /// <summary>
        /// Scenario: Submit Label submission to create provisional ISWC.
        ///           Submit two Agency submissions with same metadata.
        ///           Delete one Agency submission.
        /// Expected: Iswc remains with preferred status.
        /// </summary>
        [Fact]
        public async void ProvisionalIswcUpgradeTests_SameMetadataDeleteOneAgencySubmission_IswcRemainsWithPreferredStatus()
        {
            var labelSubmission = Submissions.EligibleLabelSubmissionCISAC;

            var labelResponse = await labelClient.AddLabelSubmissionBatchAsync(new List<SubmissionBatch>
            {
                new SubmissionBatch
                {
                    SubmissionId = 1,
                    Submission = labelSubmission
                }
            });

            var labelVerifiedSubmission = labelResponse.ElementAt(0).Submission.VerifiedSubmission;

            agencyDefinition.Submission firstAgencySubmission = agencyData.Submissions.EligibleSubmissionBMI;

            firstAgencySubmission.OriginalTitle = labelSubmission.OriginalTitle;
            firstAgencySubmission.InterestedParties = new List<agencyDefinition.InterestedParty>()
            {
                new agencyDefinition.InterestedParty
                {
                    Name = InterestedParties.IP_CISAC.ElementAt(0).Name,
                    LastName = InterestedParties.IP_CISAC.ElementAt(0).LastName,
                    NameNumber = 589238793,
                    BaseNumber = "I-002985150-9",
                    Role = agencyDefinition.InterestedPartyRole.C
                }
            };

            var firstAgencyResponse = await agencyClient.AddSubmissionAsync(firstAgencySubmission);
            var firstAgencyVerifiedSubmission = firstAgencyResponse.VerifiedSubmission;

            agencyDefinition.Submission secondAgencySubmission = agencyData.Submissions.EligibleSubmissionBMI;

            secondAgencySubmission.OriginalTitle = labelSubmission.OriginalTitle;
            secondAgencySubmission.InterestedParties = new List<agencyDefinition.InterestedParty>()
            {
                new agencyDefinition.InterestedParty
                {
                    Name = InterestedParties.IP_CISAC.ElementAt(0).Name,
                    LastName = InterestedParties.IP_CISAC.ElementAt(0).LastName,
                    NameNumber = 589238793,
                    BaseNumber = "I-002985150-9",
                    Role = agencyDefinition.InterestedPartyRole.C
                }
            };

            var secondAgencyResponse = await agencyClient.AddSubmissionAsync(secondAgencySubmission);
            var secondAgencyVerifiedSubmission = secondAgencyResponse.VerifiedSubmission;

            await WaitForSubmission(firstAgencySubmission.Agency, firstAgencySubmission.Workcode, httpAgencyClient);
            await agencyClient.DeleteSubmissionAsync(firstAgencyVerifiedSubmission.Iswc.ToString(), firstAgencySubmission.Agency, firstAgencySubmission.Workcode, firstAgencySubmission.Sourcedb, "Integration test delete");

            var iswc = await searchClient.SearchByISWCAsync(firstAgencyVerifiedSubmission.Iswc.ToString());

            Assert.Equal(labelVerifiedSubmission.Iswc, firstAgencyVerifiedSubmission.Iswc);
            Assert.Equal(labelVerifiedSubmission.Iswc, secondAgencyVerifiedSubmission.Iswc);
            Assert.Equal(firstAgencyVerifiedSubmission.Iswc, secondAgencyVerifiedSubmission.Iswc);
            Assert.Equal("Preferred", iswc.IswcStatus);
        }
    }
}
