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
    public class ProvisionalIswcUpgradeTests : TestBase, IAsyncLifetime
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
        /// Scenario: Submit Label submission with unique title and creators (excluding name numbers) to create provisional ISWC.
        ///           Then Submit Agency submission with same metadata including name numbers.
        /// Expected: Matches existing provisional ISWC, upgrading to preferred and updating creator name numbers.
        /// </summary>
        [Fact]
        public async void ProvisionalIswcUpgradeTests_SameMetadataIncludingNameNumber_MatchesAndUpgradesToPreferred()
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

            Assert.Equal(labelVerifiedSubmission.Iswc, agencyVerifiedSubmission.Iswc);
            Assert.Equal("Preferred", agencyVerifiedSubmission.IswcStatus);
            Assert.Single(agencyVerifiedSubmission.InterestedParties);
            Assert.Equal(agencySubmission.InterestedParties.ElementAt(0).NameNumber, agencyVerifiedSubmission.InterestedParties.ElementAt(0).NameNumber);
            Assert.Equal(agencySubmission.InterestedParties.ElementAt(0).BaseNumber, agencyVerifiedSubmission.InterestedParties.ElementAt(0).BaseNumber);
        }

        /// <summary>
        /// Scenario: Submit Label submission with unique title and creators (excluding name numbers) to create provisional ISWC.
        ///           Then Submit Agency submission with same metadata including only name numbers.
        /// Expected: Matches existing provisional ISWC, upgrading to preferred and updating creator name numbers.
        /// </summary>
        [Fact]
        public async void ProvisionalIswcUpgradeTests_SameMetadataIncludingOnlyNameNumber_MatchesAndUpgradesToPreferred()
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
                    NameNumber = 589238793,
                    Role = agencyDefinition.InterestedPartyRole.C
                }
            };

            var agencyResponse = await agencyClient.AddSubmissionAsync(agencySubmission);
            var agencyVerifiedSubmission = agencyResponse.VerifiedSubmission;

            Assert.Equal(labelVerifiedSubmission.Iswc, agencyVerifiedSubmission.Iswc);
            Assert.Equal("Preferred", agencyVerifiedSubmission.IswcStatus);
            Assert.Single(agencyVerifiedSubmission.InterestedParties);
            Assert.Equal(agencySubmission.InterestedParties.ElementAt(0).NameNumber, agencyVerifiedSubmission.InterestedParties.ElementAt(0).NameNumber);
        }

        /// <summary>
        /// Scenario: Submit Label submission with unique title and creators (excluding name numbers) to create provisional ISWC.
        ///           Then Submit Agency submission with same metadata including name numbers.
        /// Expected: Matches existing provisional ISWC, upgrading to preferred and updating creator name numbers and also updates metadata
        /// </summary>
        [Fact]
        public async void ProvisionalIswcUpgradeTests_SameMetadataIncludingNameNumber_MatchesAndUpdatesMetadata()
        {
            var labelSubmission = Submissions.EligibleLabelSubmissionCISAC;
            labelSubmission.Performers = new List<Performer>();

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
            agencySubmission.Performers = new List<agencyDefinition.Performer>()
            {
                new agencyDefinition.Performer
                {
                    Isni = Performers.Performers_CISAC.ElementAt(0).Isni,
                    Ipn = Performers.Performers_CISAC.ElementAt(0).Ipn,
                    FirstName = Performers.Performers_CISAC.ElementAt(0).FirstName,
                    LastName = Performers.Performers_CISAC.ElementAt(0).LastName,
                    Designation = agencyDefinition.PerformerDesignation.Main_Artist
                }
            };
            agencySubmission.AdditionalIdentifiers.Recordings = new List<agencyDefinition.Recordings>()
            {
                new agencyDefinition.Recordings
                {
                    Isrc = labelSubmission.AdditionalIdentifiers.Recordings.ElementAt(0).Isrc,
                    RecordingTitle = labelSubmission.AdditionalIdentifiers.Recordings.ElementAt(0).RecordingTitle,
                    LabelName = labelSubmission.AdditionalIdentifiers.Recordings.ElementAt(0).LabelName
                },
                new agencyDefinition.Recordings
                {
                    Isrc = TestBase.CreateNewWorkCode(),
                    RecordingTitle = TestBase.CreateNewTitleWithLettersOnly(),
                    LabelName = "Columbia"
                }
            };

            var agencyResponse = await agencyClient.AddSubmissionAsync(agencySubmission);
            var agencyVerifiedSubmission = agencyResponse.VerifiedSubmission;

            Assert.Equal(labelVerifiedSubmission.Iswc, agencyVerifiedSubmission.Iswc);
            Assert.Equal("Preferred", agencyVerifiedSubmission.IswcStatus);
            Assert.Single(agencyVerifiedSubmission.InterestedParties);
            Assert.Single(agencyVerifiedSubmission.Performers);
            Assert.Equal(2, agencyVerifiedSubmission.AdditionalIdentifiers.Recordings.Count);
            Assert.Equal(agencyVerifiedSubmission.AdditionalIdentifiers.Recordings.ElementAt(1).Isrc, agencySubmission.AdditionalIdentifiers.Recordings.ElementAt(1).Isrc);
            Assert.Equal(agencyVerifiedSubmission.Performers.ElementAt(0).Isni, agencySubmission.Performers.ElementAt(0).Isni);

        }

        /// <summary>
        /// Scenario: Submit Label submission with unique title and creators (excluding name numbers) to create provisional ISWC.
        ///           Submit agency submission with same metadata including name numbers but low creator similarity ("Hozier" matching to "Hozier Byrne" is only 50% similar).
        /// Expected: Agency submission will not match to label submission as creator similarity is too low. 
        ///           Agency submission will create a new preferred iswc and label submission will remain provisional.
        /// </summary>
        [Fact]
        public async void ProvisionalIswcUpgradeTests_LowCreatorSimilarity_DoesNotMatch()
        {
            var labelSubmission = Submissions.EligibleLabelSubmissionCISAC;
            labelSubmission.InterestedParties.ElementAt(0).LastName = "Hozier";

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
                    LastName = "Hozier Byrne",
                    NameNumber = 589238793,
                    Role = agencyDefinition.InterestedPartyRole.C
                },
            };

            var agencyResponse = await agencyClient.AddSubmissionAsync(agencySubmission);
            var agencyVerifiedSubmission = agencyResponse.VerifiedSubmission;

            var searchResponse = await searchClient.SearchByISWCAsync(labelVerifiedSubmission.Iswc.ToString());

            Assert.NotEqual(labelVerifiedSubmission.Iswc, agencyVerifiedSubmission.Iswc);
            Assert.Equal("Preferred", agencyVerifiedSubmission.IswcStatus);
            Assert.Equal("Provisional", searchResponse.IswcStatus);
            Assert.Single(agencyVerifiedSubmission.InterestedParties);
        }

        /// <summary>
        /// Scenario: Submit Label submission with unique title and creators (excluding name numbers) to create provisional ISWC.
        ///           Submit agency submission with same metadata except for additional creator.
        /// Expected: Agency submission will not match to label submission as creator counts differ. 
        ///           Agency submission will create a new preferred iswc and label submission will remain provisional.
        /// </summary>
        [Fact]
        public async void ProvisionalIswcUpgradeTests_DifferentCreatorCount_DoesNotMatch()
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
                },
                agencyData.InterestedParties.IP_BMI.ElementAt(0)
            };

            var agencyResponse = await agencyClient.AddSubmissionAsync(agencySubmission);
            var agencyVerifiedSubmission = agencyResponse.VerifiedSubmission;

            var searchResponse = await searchClient.SearchByISWCAsync(labelVerifiedSubmission.Iswc.ToString());

            Assert.NotEqual(labelVerifiedSubmission.Iswc, agencyVerifiedSubmission.Iswc);
            Assert.Equal("Preferred", agencyVerifiedSubmission.IswcStatus);
            Assert.Equal("Provisional", searchResponse.IswcStatus);
            Assert.Equal(2, agencyVerifiedSubmission.InterestedParties.Count);
        }

        /// <summary>
        /// Scenario: Submit Label submission with unique title and creators (excluding name numbers) to create provisional ISWC.
        ///           Submit an agency submission which matches the metadata of the label submission but is missing one creator.
        ///           Because of this a new preferred ISWC is created for the agency submission.
        ///           Next submit a revised agency submission which includes the missing creator.
        /// Expected: The new agency submission will update the original agency submission and now match to the provisional ISWC
        ///           This will cause the provisional ISWC to merge into the preferred ISWC.

        [Fact]
        public async void ProvisionalIswcUpgradeTests_UpdateOfPreferredIswc_TriggersMergeWithProvisionalIswc()
        {
            var labelSubmission = Submissions.EligibleLabelSubmissionCISAC;
            labelSubmission.InterestedParties = new List<InterestedParty>()
            {
                new InterestedParty { Role = InterestedPartyRole.C, LastName = "BOURKE" },
                new InterestedParty { Role = InterestedPartyRole.C, LastName = "O BRIEN" }
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

            agencyDefinition.Submission agencySubmission = agencyData.Submissions.EligibleSubmissionIMRO;
            agencySubmission.InterestedParties = agencySubmission.InterestedParties.Take(1).ToList();
            agencySubmission.OriginalTitle = labelSubmission.OriginalTitle;

            var agencyResponse = await agencyClient.AddSubmissionAsync(agencySubmission);
            var agencyVerifiedSubmission = agencyResponse.VerifiedSubmission;

            agencyDefinition.Submission agencyRevisedSubmission = agencyData.Submissions.EligibleSubmissionIMRO;
            agencyRevisedSubmission.Workcode = agencySubmission.Workcode;
            agencyRevisedSubmission.OriginalTitle = agencySubmission.OriginalTitle;

            var agencyRevisedResponse = await agencyClient.AddSubmissionAsync(agencyRevisedSubmission);
            var agencyRevisedVerifiedSubmission = agencyRevisedResponse.VerifiedSubmission;
            var agencyRevisisedLinkedIswc = agencyRevisedResponse.LinkedIswcs.First().IswcMetadata;

            Assert.Equal("Provisional", labelVerifiedSubmission.IswcStatus);
            Assert.Equal("Preferred", agencyVerifiedSubmission.IswcStatus);
            Assert.Equal("Preferred", agencyRevisedVerifiedSubmission.IswcStatus);
            Assert.Equal(1, agencyVerifiedSubmission.InterestedParties.Count);
            Assert.Equal(2, agencyRevisedVerifiedSubmission.InterestedParties.Count);
            Assert.Single(agencyRevisedVerifiedSubmission.AdditionalIdentifiers.Recordings);
            Assert.Equal(2, agencyRevisedVerifiedSubmission.Performers.Count());
            Assert.Equal(agencyVerifiedSubmission.Iswc, agencyRevisedVerifiedSubmission.Iswc);
            Assert.Equal("Provisional", agencyRevisisedLinkedIswc.IswcStatus);
            Assert.Equal(labelVerifiedSubmission.Iswc, agencyRevisisedLinkedIswc.Iswc);
        }

        /// <summary>
        /// Scenario: Submit Label submission with unique title and creators (excluding name numbers) to create provisional ISWC.
        ///           Submit agency submission with same metadata except it has the same creator twice. One with a "C" role and another with an "A" role (both rolled-up to "C")
        /// Expected: Agency submission will match to label submission. Prior to doing the second phase of matching, the creators will be consolidated to the unique 
        ///           creator / rolled up role combinations so that the creator count is 2 rather than 3 for the society submission when used for matching 
        ///           against the label submission. 
        /// </summary>
        [Fact]
        public async void ProvisionalIswcUpgradeTests_DifferentCreatorCountSameCreators_MatchesAndUpdatesMetadata()
        {
            var labelSubmission = Submissions.EligibleLabelSubmissionCISAC;
            labelSubmission.InterestedParties = new List<InterestedParty>()
            {
                new InterestedParty { Role = InterestedPartyRole.C, LastName = "BOURKE" },
                new InterestedParty { Role = InterestedPartyRole.C, LastName = "O BRIEN" }
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

            agencyDefinition.Submission agencySubmission = agencyData.Submissions.EligibleSubmissionIMRO;
            agencySubmission.InterestedParties.Add(new agencyDefinition.InterestedParty() { Name = "BOURKE CIARAN FRANCIS", NameNumber = 36303314, Role = agencyDefinition.InterestedPartyRole.A, BaseNumber = "I-000380434-8", LastName = "BOURKE" });
            agencySubmission.OriginalTitle = labelSubmission.OriginalTitle;

            var agencyResponse = await agencyClient.AddSubmissionAsync(agencySubmission);
            var agencyVerifiedSubmission = agencyResponse.VerifiedSubmission;

            Assert.Equal("Provisional", labelVerifiedSubmission.IswcStatus);
            Assert.Equal("Preferred", agencyVerifiedSubmission.IswcStatus);
            Assert.Equal(2, labelVerifiedSubmission.InterestedParties.Count);
            Assert.Equal(3, agencyVerifiedSubmission.InterestedParties.Count);
            Assert.Equal(labelVerifiedSubmission.Iswc, agencyVerifiedSubmission.Iswc);
        }

        /// <summary>
        /// Scenario: Submit Label submission with unique title and creators (excluding name numbers) to create provisional ISWC.
        ///           Submit agency submission with same metadata except it has both creators duplicated twice. Each one with a "CA" role and another with an "A" role (both rolled-up to "C")
        /// Expected: Agency submission will match to label submission. Prior to doing the second phase of matching, the creators will be consolidated to the unique 
        ///           creator / rolled up role combinations so that the creator count is 2 rather than 4 for the society submission when used for matching 
        ///           against the label submission. 
        /// </summary>
        [Fact]
        public async void ProvisionalIswcUpgradeTests_DifferentCreatorCountSameCreatorsDuplicatedTwice_MatchesAndUpdatesMetadata()
        {
            var labelSubmission = Submissions.EligibleLabelSubmissionCISAC;
            labelSubmission.InterestedParties = new List<InterestedParty>()
            {
                new InterestedParty { Role = InterestedPartyRole.CA, LastName = "BAKER" },
                new InterestedParty { Role = InterestedPartyRole.C, LastName = "REEVES" }
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

            agencyDefinition.Submission agencySubmission = agencyData.Submissions.EligibleSubmissionBMI;
            agencySubmission.InterestedParties.Add(new agencyDefinition.InterestedParty() { BaseNumber = "I-000938234-5", Role = agencyDefinition.InterestedPartyRole.A, NameNumber = 1856409, Name = "BAKER BILL", LastName = "BAKER" });
            agencySubmission.InterestedParties.Add(new agencyDefinition.InterestedParty() { BaseNumber = "I-000682468-4", Role = agencyDefinition.InterestedPartyRole.A, NameNumber = 1906032, Name = "REEVES KEN", LastName = "REEVES" });
            agencySubmission.OriginalTitle = labelSubmission.OriginalTitle;

            var agencyResponse = await agencyClient.AddSubmissionAsync(agencySubmission);
            var agencyVerifiedSubmission = agencyResponse.VerifiedSubmission;

            Assert.Equal("Provisional", labelVerifiedSubmission.IswcStatus);
            Assert.Equal("Preferred", agencyVerifiedSubmission.IswcStatus);
            Assert.Equal(2, labelVerifiedSubmission.InterestedParties.Count);
            Assert.Equal(4, agencyVerifiedSubmission.InterestedParties.Count);
            Assert.Equal(labelVerifiedSubmission.Iswc, agencyVerifiedSubmission.Iswc);
        }
    }
}
