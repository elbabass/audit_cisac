using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.Cosmos;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Integration.Tests.V1.Tests.CosmosTests
{
    public class ComosAuditRequestRecordTests_Fixture : IAsyncLifetime
    {
        public IISWC_SubmissionClient submissionClient;
        public HttpClient httpClient;
        public ISWC_CosmosClient cosmosClient;
        public IISWC_MergeClient mergeClient;
        public IISWC_Workflow_TasksClient workflow_TasksClient;
        public ISWC_Allocation_and_ResolutionClient allocResClient;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await TestBase.GetClient();
            submissionClient = new ISWC_SubmissionClient(httpClient);
            cosmosClient = new ISWC_CosmosClient();
            mergeClient = new ISWC_MergeClient(httpClient);
            workflow_TasksClient = new ISWC_Workflow_TasksClient(httpClient);
            allocResClient = new ISWC_Allocation_and_ResolutionClient(httpClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            return Task.CompletedTask;
        }
    }
    public class ComosAuditRequestRecordTests : TestBase, IClassFixture<ComosAuditRequestRecordTests_Fixture>
    {
        private IISWC_SubmissionClient submissionClient;
        private ISWC_CosmosClient cosmosClient;
        private IISWC_MergeClient mergeClient;
        private IISWC_Workflow_TasksClient workflow_TasksClient;
        private ISWC_Allocation_and_ResolutionClient allocResClient;
        private HttpClient httpClient;

        public ComosAuditRequestRecordTests(ComosAuditRequestRecordTests_Fixture fixture)
        {
            submissionClient = fixture.submissionClient;
            cosmosClient = fixture.cosmosClient;
            mergeClient = fixture.mergeClient;
            workflow_TasksClient = fixture.workflow_TasksClient;
            allocResClient = fixture.allocResClient;
            httpClient = fixture.httpClient;
        }

        /// <summary>
        /// Adding a new submission creates an AuditRequest record.
        /// </summary>
        [Fact]
        public async void ComosAuditRequestRecordTests_01()
        {
            var submission = Submissions.EligibleSubmissionIMRO;
            var res = await submissionClient.AddSubmissionAsync(submission);
            submission.PreferredIswc = res.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);

            var record = await cosmosClient.GetAuditRequestRecord(submission.Workcode, TransactionTypes.CAR);

            Assert.NotNull(record);
            Assert.Equal(submission.PreferredIswc, record.Work.PreferredIswc);
            Assert.Equal(submission.Agency, record.AgencyCode);
            Assert.Collection(record.Work.InterestedParties,
                elem1 =>
                {
                    Assert.Equal(submission.InterestedParties.ElementAt(0).NameNumber, elem1.IPNameNumber);
                    Assert.Equal(submission.InterestedParties.ElementAt(0).BaseNumber, elem1.IpBaseNumber);
                    Assert.Equal(submission.InterestedParties.ElementAt(0).Name, elem1.Name);
                },
                elem2 =>
                {
                    Assert.Equal(submission.InterestedParties.ElementAt(1).NameNumber, elem2.IPNameNumber);
                    Assert.Equal(submission.InterestedParties.ElementAt(1).BaseNumber, elem2.IpBaseNumber);
                    Assert.Equal(submission.InterestedParties.ElementAt(1).Name, elem2.Name);
                });
            Assert.False(record.IsProcessingError);
            Assert.Equal("Agency", record.TransactionSource);
        }

        /// <summary>
        /// Adding a new submission creates an AuditRequest record.
        /// </summary>
        [Fact]
        public async void ComosAuditRequestRecordTests_02()
        {
            var submission = Submissions.EligibleSubmissionIMRO;
            var res = await submissionClient.AddSubmissionAsync(submission);
            submission.PreferredIswc = res.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            submission.OriginalTitle += "123";
            var updateRes = await submissionClient.UpdateSubmissionAsync(submission.PreferredIswc, submission);

            var record = await cosmosClient.GetAuditRequestRecord(submission.Workcode, TransactionTypes.CUR);

            Assert.NotNull(record);
            Assert.Equal(submission.PreferredIswc, record.Work.PreferredIswc);
            Assert.Equal(submission.Agency, record.AgencyCode);
            Assert.Equal(submission.OriginalTitle, record.Work.Titles.FirstOrDefault().Name);
            Assert.Collection(record.Work.InterestedParties,
                elem1 =>
                {
                    Assert.Equal(submission.InterestedParties.ElementAt(0).NameNumber, elem1.IPNameNumber);
                    Assert.Equal(submission.InterestedParties.ElementAt(0).BaseNumber, elem1.IpBaseNumber);
                    Assert.Equal(submission.InterestedParties.ElementAt(0).Name, elem1.Name);
                },
                elem2 =>
                {
                    Assert.Equal(submission.InterestedParties.ElementAt(1).NameNumber, elem2.IPNameNumber);
                    Assert.Equal(submission.InterestedParties.ElementAt(1).BaseNumber, elem2.IpBaseNumber);
                    Assert.Equal(submission.InterestedParties.ElementAt(1).Name, elem2.Name);
                });
            Assert.False(record.IsProcessingError);
            Assert.Equal("Agency", record.TransactionSource);
        }

        /// <summary>
        /// Merge request creates an AuditRequest record for parent.
        /// </summary>
        [Fact]
        public async void ComosAuditRequestRecordTests_03()
        {
            var submission = Submissions.EligibleSubmissionIMRO;
            var res = await submissionClient.AddSubmissionAsync(submission);
            submission.PreferredIswc = res.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);

            var subTwo = Submissions.EligibleSubmissionIMRO;
            var re = await submissionClient.AddSubmissionAsync(subTwo);
            await WaitForSubmission(subTwo.Agency, subTwo.Workcode, httpClient);
            subTwo.PreferredIswc = re.VerifiedSubmission.Iswc.ToString();

            var body = new Body
            {
                Iswcs = new string[] { subTwo.PreferredIswc }
            };
            await mergeClient.MergeISWCMetadataAsync(submission.PreferredIswc, subTwo.Agency, body);

            var record = await cosmosClient.GetAuditRequestRecord(submission.Workcode, TransactionTypes.MER);

            Assert.Equal(subTwo.PreferredIswc, record.Work.IswcsToMerge.First());
            Assert.Equal(submission.PreferredIswc, record.Work.PreferredIswc);
            Assert.Equal(subTwo.OriginalTitle, record.Work.Titles.First().Name);
            Assert.False(record.IsProcessingError);
            Assert.Equal("Agency", record.TransactionSource);
        }

        /// <summary>
        /// Merge request creates an AuditRequest record for child.
        /// </summary>
        [Fact]
        public async void ComosAuditRequestRecordTests_04()
        {
            var submission = Submissions.EligibleSubmissionIMRO;
            var res = await submissionClient.AddSubmissionAsync(submission);
            submission.PreferredIswc = res.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);

            var subTwo = Submissions.EligibleSubmissionIMRO;
            var re = await submissionClient.AddSubmissionAsync(subTwo);
            await WaitForSubmission(subTwo.Agency, subTwo.Workcode, httpClient);
            subTwo.PreferredIswc = re.VerifiedSubmission.Iswc.ToString();

            var body = new Body
            {
                Iswcs = new string[] { subTwo.PreferredIswc }
            };
            await mergeClient.MergeISWCMetadataAsync(submission.PreferredIswc, subTwo.Agency, body);

            var record = await cosmosClient.GetAuditRequestRecord(subTwo.Workcode, TransactionTypes.MER);

            Assert.Equal(subTwo.PreferredIswc, record.Work.IswcsToMerge.First());
            Assert.Equal(subTwo.PreferredIswc, record.Work.PreferredIswc);
            Assert.Equal(subTwo.OriginalTitle, record.Work.Titles.First().Name);
            Assert.False(record.IsProcessingError);
            Assert.Equal("Agency", record.TransactionSource);
        }

        /// <summary>
        /// Demerge request creates an AuditRequest record for child.
        /// </summary>
        [Fact]
        public async void ComosAuditRequestRecordTests_05()
        {
            var submission = Submissions.EligibleSubmissionIMRO;
            var res = await submissionClient.AddSubmissionAsync(submission);
            submission.PreferredIswc = res.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);

            var subTwo = Submissions.EligibleSubmissionIMRO;
            var re = await submissionClient.AddSubmissionAsync(subTwo);
            await WaitForSubmission(subTwo.Agency, subTwo.Workcode, httpClient);
            subTwo.PreferredIswc = re.VerifiedSubmission.Iswc.ToString();

            var body = new Body
            {
                Iswcs = new string[] { subTwo.PreferredIswc }
            };
            await mergeClient.MergeISWCMetadataAsync(submission.PreferredIswc, subTwo.Agency, body);
            await Task.Delay(2000);
            await mergeClient.DemergeISWCMetadataAsync(submission.PreferredIswc, subTwo.Agency, subTwo.Workcode);

            var record = (await cosmosClient.GetAuditRequestRecords(subTwo.Workcode, TransactionTypes.DMR))
                .FirstOrDefault(x => x.Work.PreferredIswc == subTwo.PreferredIswc);

            Assert.Equal(subTwo.PreferredIswc, record.Work.PreferredIswc);
            Assert.Equal(subTwo.OriginalTitle, record.Work.Titles.First().Name);
            Assert.False(record.IsProcessingError);
            Assert.Equal("Agency", record.TransactionSource);
        }

        /// <summary>
        /// Demerge request creates an AuditRequest record for parent.
        /// </summary>
        [Fact]
        public async void ComosAuditRequestRecordTests_06()
        {
            var submission = Submissions.EligibleSubmissionIMRO;
            var res = await submissionClient.AddSubmissionAsync(submission);
            submission.PreferredIswc = res.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);

            var subTwo = Submissions.EligibleSubmissionIMRO;
            var re = await submissionClient.AddSubmissionAsync(subTwo);
            await WaitForSubmission(subTwo.Agency, subTwo.Workcode, httpClient);
            subTwo.PreferredIswc = re.VerifiedSubmission.Iswc.ToString();

            var body = new Body
            {
                Iswcs = new string[] { subTwo.PreferredIswc }
            };
            await mergeClient.MergeISWCMetadataAsync(submission.PreferredIswc, subTwo.Agency, body);
            await Task.Delay(2000);
            await mergeClient.DemergeISWCMetadataAsync(submission.PreferredIswc, subTwo.Agency, subTwo.Workcode);

            var record = (await cosmosClient.GetAuditRequestRecords(subTwo.Workcode, TransactionTypes.DMR))
                .FirstOrDefault(x => x.Work.PreferredIswc == submission.PreferredIswc);

            Assert.Equal(submission.PreferredIswc, record.Work.PreferredIswc);
            Assert.Equal(subTwo.OriginalTitle, record.Work.Titles.First().Name);
            Assert.False(record.IsProcessingError);
            Assert.Equal("Agency", record.TransactionSource);
        }

        /// <summary>
        /// Deleting a work creates an AuditRequest record.
        /// </summary>
        [Fact]
        public async void ComosAuditRequestRecordTests_07()
        {
            var submission = Submissions.EligibleSubmissionIMRO;
            var res = await submissionClient.AddSubmissionAsync(submission);
            submission.PreferredIswc = res.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);

            await submissionClient.DeleteSubmissionAsync(submission.PreferredIswc, submission.Agency, submission.Workcode, submission.Sourcedb, "Deleted");
            var record = await cosmosClient.GetAuditRequestRecord(submission.Workcode, TransactionTypes.CDR);

            Assert.NotNull(record);
            Assert.Equal(submission.PreferredIswc, record.Work.PreferredIswc);
            Assert.Equal(submission.Agency, record.AgencyCode);
            Assert.False(record.IsProcessingError);
            Assert.Equal("Agency", record.TransactionSource);

        }

        /// <summary>
        /// Allocating a work creates an AuditRequest record.
        /// </summary>
        [Fact]
        public async void ComosAuditRequestRecordTests_08()
        {
            var sub = Submissions.EligibleSubmissionIMRO;
            sub.AdditionalIdentifiers = AdditionalIdentifersData.AI_SONY;

            List<SubmissionBatch> batch = new List<SubmissionBatch>{
                new SubmissionBatch {
                    SubmissionId = 1,
                    Submission = sub
                }
            };

            var res = await allocResClient.AddAllocationBatchAsync(batch);

            var record = await cosmosClient.GetAuditRequestRecord(sub.Workcode, TransactionTypes.CAR);


            Assert.NotNull(record);
            Assert.Equal(sub.Agency, record.AgencyCode);
            Assert.Equal("Publisher", record.TransactionSource);
            Assert.False(String.IsNullOrWhiteSpace(record.Work.PreferredIswc));
            Assert.False(record.IsProcessingError);
            Assert.Equal(sub.AdditionalIdentifiers.Isrcs.First(), record.Work.AdditionalIdentifiers.Where(x => x.SubmitterCode.Equals("ISRC")).First().WorkCode);
            Assert.Equal(sub.AdditionalIdentifiers.PublisherIdentifiers.FirstOrDefault().NameNumber, record.Work.AdditionalIdentifiers.Where(x => x.SubmitterCode.Equals("SA")).First().NameNumber);
        }

        /// <summary>
        /// Resolving a work creates an AuditRequest record.
        /// </summary>
        [Fact]
        public async void ComosAuditRequestRecordTests_09()
        {
            var sub = Submissions.EligibleSubmissionIMRO;
            var workcode = sub.Workcode;
            var res = await submissionClient.AddSubmissionAsync(sub);
            sub.PreferredIswc = res.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(sub.Agency, sub.Workcode, httpClient);

            List<SubmissionBatch> batch = new List<SubmissionBatch>{
                new SubmissionBatch {
                    SubmissionId = 1,
                    Submission = sub
                }
            };
            sub.Agency = "312";
            sub.Sourcedb = 315;
            sub.Workcode = CreateNewWorkCode();
            sub.AdditionalIdentifiers = AdditionalIdentifersData.AI_SONY;

            await allocResClient.AddResolutionBatchAsync(batch);

            var record = await cosmosClient.GetAuditRequestRecord(sub.Workcode, TransactionTypes.FSQ);

            Assert.NotNull(record);
            Assert.Equal(sub.Agency, record.AgencyCode);
            Assert.Equal("Publisher", record.TransactionSource);
            Assert.Equal(sub.PreferredIswc, record.Work.PreferredIswc);
            Assert.Equal(sub.OriginalTitle, record.Work.Titles.First().Name);
            Assert.False(record.IsProcessingError);
            Assert.Equal(sub.AdditionalIdentifiers.PublisherIdentifiers.FirstOrDefault().NameNumber, record.Work.AdditionalIdentifiers.Where(x => x.SubmitterCode.Equals("SA")).First().NameNumber);
        }

        /// <summary>
        /// request-source is saved cosmos.
        /// </summary>
        [Fact]
        public async void ComosAuditRequestRecordTests_10()
        {
            httpClient.DefaultRequestHeaders.Add("request-source", "PORTAL");
            var submission = Submissions.EligibleSubmissionIMRO;
            await submissionClient.AddSubmissionAsync(submission);

            var record = await cosmosClient.GetAuditRequestRecord(submission.Workcode, TransactionTypes.CAR);

            Assert.NotNull(record);
            Assert.Equal(1, record.RequestSource);
        }
    }
}
