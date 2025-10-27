using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using SpanishPoint.Azure.Iswc.Api.Label.Integration.Tests.V1.Data;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using agencyDefinition = SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using agencyData = SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using IdentityServer4.Models;

namespace SpanishPoint.Azure.Iswc.Api.Label.Integration.Tests.V1.Tests.UpdateTests
{
    public class UpdateExistingWorkBatchTests_ConsolidatedMetadata : TestBase, IAsyncLifetime
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
        /// Scenario: Search for a Preferred Iswc which has a Label submission. 
        /// Expected: Label metadata not shown in the consolidated metadata.
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkBatchTests_SearchForPreferredIswc_ReturnsPreferredMetadataOnly()
        {
            agencyDefinition.Submission agencySubmission = agencyData.Submissions.EligibleSubmissionBMI;

            var agencyResponse = await agencyClient.AddSubmissionAsync(agencySubmission);

            await WaitForSubmission(agencySubmission.Agency, agencySubmission.Workcode, httpAgencyClient);

            var labelSubmission = Submissions.EligibleLabelSubmissionSESAC;
            labelSubmission.OriginalTitle = agencySubmission.OriginalTitle;
            labelSubmission.OtherTitles = new List<Title> { new Title { Title1 = CreateNewTitle(), Type = TitleType.AT } };
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

            var labelResponse = await labelClient.AddLabelSubmissionBatchAsync(batch);

            await WaitForSubmission(labelSubmission.Agency, labelSubmission.Workcode, httpAgencyClient);

            var updateLabelSubmission = labelSubmission;
            updateLabelSubmission.InterestedParties.Add(new InterestedParty() { Name = "KORKEJIAN AZNIV", LastName = "KORKEJIAN", Role = InterestedPartyRole.CA });

            batch = new List<SubmissionBatch>
            {
                new SubmissionBatch
                {
                    SubmissionId = 1,
                    Submission = updateLabelSubmission
                }
            };

            var updateLabelResponse = await labelClient.AddLabelSubmissionBatchAsync(batch);

            await WaitForSubmission(updateLabelSubmission.Agency, updateLabelSubmission.Workcode, httpAgencyClient);

            var searchResponse = await searchClient.SearchByISWCAsync(agencyResponse.VerifiedSubmission.Iswc.ToString());

            Assert.Equal("Preferred", searchResponse.IswcStatus);
            Assert.Equal(searchResponse.Iswc, updateLabelResponse.FirstOrDefault().Submission.VerifiedSubmission.Iswc);
            Assert.Equal(0, searchResponse.OtherTitles.Count);
            Assert.Equal(searchResponse.InterestedParties.Count, agencyResponse.VerifiedSubmission.InterestedParties.Count);
        }

        /// <summary>
        /// Scenario: Search for a Provisional Iswc. 
        /// Expected: Label metadata shown in the consolidated metadata.
        /// </summary>
        [Fact]
        public async void UpdateExistingWorkBatchTests_SearchForProvisionalIswc_ReturnsProvisionalMetadata()
        {
            var labelSubmission = Submissions.EligibleLabelSubmissionSESAC;
            labelSubmission.OtherTitles = new List<Title> { new Title { Title1 = CreateNewTitle(), Type = TitleType.AT } };

            List<SubmissionBatch> batch = new List<SubmissionBatch>
            {
                new SubmissionBatch
                {
                    SubmissionId = 1,
                    Submission = labelSubmission
                }
            };

            var labelResponse = await labelClient.AddLabelSubmissionBatchAsync(batch);

            await WaitForSubmission(labelSubmission.Agency, labelSubmission.Workcode, httpAgencyClient);

            var searchResponse = await searchClient.SearchByISWCAsync(labelResponse.FirstOrDefault().Submission.VerifiedSubmission.Iswc.ToString());

            Assert.Equal("Provisional", searchResponse.IswcStatus);
            Assert.Equal(labelSubmission.OriginalTitle, searchResponse.OriginalTitle);
            Assert.Equal(labelSubmission.OtherTitles.FirstOrDefault().Title1, searchResponse.OtherTitles.FirstOrDefault().Title1);
            Assert.Equal(labelSubmission.InterestedParties.Count, searchResponse.InterestedParties.Count);
        }
    }
}
