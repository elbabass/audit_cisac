using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.Cosmos;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using xRetry;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Tests.CosmosTests
{
    public class CosmosCSNTests_Fixture : IAsyncLifetime
    {
        public IISWC_SubmissionClient submissionClient;
        public ISWC_CosmosClient cosmosClient;
        public IISWC_MergeClient mergeClient;
        public IISWC_Workflow_TasksClient workflow_TasksClient;
        public HttpClient httpClient;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await TestBase.GetClient();
            submissionClient = new ISWC_SubmissionClient(httpClient);
            cosmosClient = new ISWC_CosmosClient();
            mergeClient = new ISWC_MergeClient(httpClient);
            workflow_TasksClient = new ISWC_Workflow_TasksClient(httpClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            return Task.CompletedTask;
        }
    }
    public class CosmosCSNTests : TestBase, IClassFixture<CosmosCSNTests_Fixture>
    {
        private IISWC_SubmissionClient submissionClient;
        private ISWC_CosmosClient cosmosClient;
        private IISWC_MergeClient mergeClient;
        private IISWC_Workflow_TasksClient workflow_TasksClient;
        private HttpClient httpClient;

        public CosmosCSNTests(CosmosCSNTests_Fixture fixture)
        {
            submissionClient = fixture.submissionClient;
            cosmosClient = fixture.cosmosClient;
            mergeClient = fixture.mergeClient;
            workflow_TasksClient = fixture.workflow_TasksClient;
            httpClient = fixture.httpClient;
        }

        /// <summary>
        /// Adding a new submission creates a CSN for submitting agency.
        /// </summary>
        [Fact]
        public async void CosmosCSNTests_01()
        {
            var submission = Submissions.EligibleSubmissionAKKA;
            var res = await submissionClient.AddSubmissionAsync(submission);
            submission.PreferredIswc = res.VerifiedSubmission.Iswc.ToString();

            var csn = await cosmosClient.GetCsnEntry(submission.Workcode, TransactionTypes.CAR);

            Assert.NotNull(csn);
            Assert.Equal(submission.PreferredIswc, csn.ToIswc);
            Assert.Null(csn.WorkflowTaskID);
            Assert.Equal(csn.ReceivingAgencyCode, csn.SubmittingAgencyCode);
        }

        /// <summary>
        /// Updating a submission creates a CSN for submitting agency.
        /// </summary>
        [RetryFact]
        public async void CosmosCSNTests_02()
        {
            var submission = Submissions.EligibleSubmissionAKKA;
            var res = await submissionClient.AddSubmissionAsync(submission);
            submission.PreferredIswc = res.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);

            submission.OriginalTitle = CreateNewTitle();
            await submissionClient.UpdateSubmissionAsync(submission.PreferredIswc, submission);
            var csn = await cosmosClient.GetCsnEntry(submission.Workcode, TransactionTypes.CUR);

            Assert.NotNull(csn);
            Assert.Equal(submission.PreferredIswc, csn.ToIswc);
            Assert.Null(csn.WorkflowTaskID);
            Assert.Equal(csn.ReceivingAgencyCode, csn.SubmittingAgencyCode);
        }

        /// <summary>
        /// Updating a split copyright ISWC creates a CSN entry for the other agency.
        /// </summary>
        [RetryFact]
        public async void CosmosCSNTests_03()
        {
            var submission = Submissions.EligibleSubmissionIMRO;
            var receivingWorkcode = submission.Workcode;
            var receivingAgency = submission.Agency;
            submission.InterestedParties.Add(InterestedParties.IP_BMI[0]);
            var res = await submissionClient.AddSubmissionAsync(submission);
            submission.PreferredIswc = res.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);

            submission.Agency = Submissions.EligibleSubmissionBMI.Agency;
            submission.Sourcedb = Submissions.EligibleSubmissionBMI.Sourcedb;
            submission.Workcode = CreateNewWorkCode();
            await submissionClient.AddSubmissionAsync(submission);
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            submission.OriginalTitle = CreateNewTitle();
            await submissionClient.UpdateSubmissionAsync(submission.PreferredIswc, submission);
            var csn = await cosmosClient.GetCsnEntry(receivingWorkcode, TransactionTypes.CUR);
            IEnumerable<WorkflowStatus> statuses = new List<WorkflowStatus>() { WorkflowStatus.Outstanding };
            var workflowTask = (await workflow_TasksClient.FindWorkflowTasksAsync(receivingAgency, ShowWorkflows.AssignedToMe, WorkflowType.UpdateApproval, statuses, 0, 500,
                string.Empty, string.Empty, string.Empty, string.Empty, string.Empty))
                .FirstOrDefault(x => x.IswcMetadata.Iswc == submission.PreferredIswc);

            Assert.NotNull(csn);
            Assert.Equal(submission.PreferredIswc, csn.ToIswc);
            Assert.Equal(workflowTask.WorkflowTaskId, Convert.ToInt32(csn.WorkflowTaskID));
            Assert.Equal(submission.Agency, csn.SubmittingAgencyCode);
        }

        /// <summary>
        /// Merge CSN is generated for submitting agency.
        /// </summary>
        [Fact]
        public async void CosmosCSNTests_04()
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
            var csn = await cosmosClient.GetCsnEntry(subTwo.Workcode, TransactionTypes.MER);

            Assert.NotNull(csn);
            Assert.Equal(subTwo.Workcode, csn.ReceivingAgencyWorkCode);
            Assert.Equal(submission.PreferredIswc, csn.ToIswc);
            Assert.Equal(subTwo.PreferredIswc, csn.FromIswc);
        }

        /// <summary>
        /// Merge CSN is generated for other agency on submission.
        /// Workflow task ID is included in CSN.
        /// </summary>
        [Fact]
        public async void CosmosCSNTests_05()
        {
            var submission = Submissions.EligibleSubmissionSACEM;
            submission.InterestedParties.Add(InterestedParties.IP_PRS[0]);
            var submittingAgency = submission.Agency;
            var receivingWorkcode = submission.Workcode;

            var res = await submissionClient.AddSubmissionAsync(submission);
            submission.PreferredIswc = res.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);

            submission.Workcode = CreateNewWorkCode();
            submission.Agency = Submissions.EligibleSubmissionPRS.Agency;
            submission.Sourcedb = Submissions.EligibleSubmissionPRS.Sourcedb;
            await submissionClient.AddSubmissionAsync(submission);

            var subTwo = Submissions.EligibleSubmissionPRS;
            subTwo.InterestedParties.Add(InterestedParties.IP_SACEM[0]);
            var re = await submissionClient.AddSubmissionAsync(subTwo);
            await WaitForSubmission(subTwo.Agency, subTwo.Workcode, httpClient);
            subTwo.PreferredIswc = re.VerifiedSubmission.Iswc.ToString();

            var body = new Body
            {
                Iswcs = new string[] { subTwo.PreferredIswc }
            };
            await mergeClient.MergeISWCMetadataAsync(submission.PreferredIswc, subTwo.Agency, body);
            var csn = await cosmosClient.GetCsnEntry(receivingWorkcode, TransactionTypes.MER);
            IEnumerable<WorkflowStatus> statuses = new List<WorkflowStatus>() { WorkflowStatus.Outstanding };
            var workflowTask = (await workflow_TasksClient.FindWorkflowTasksAsync(submittingAgency, ShowWorkflows.AssignedToMe, WorkflowType.MergeApproval, statuses, 0, 500,
                string.Empty, string.Empty, string.Empty, string.Empty, string.Empty))
                .FirstOrDefault(x => x.IswcMetadata.Iswc == subTwo.PreferredIswc);

            Assert.NotNull(csn);
            Assert.Equal(receivingWorkcode, csn.ReceivingAgencyWorkCode);
            Assert.Equal(workflowTask.WorkflowTaskId, Convert.ToInt32(csn.WorkflowTaskID));
        }

        /// <summary>
        /// Demerge CSN is generated for other agency on submission.
        /// Workflow task ID is included in CSN.
        /// </summary>
        [Fact]
        public async void CosmosCSNTests_06()
        {
            var submission = Submissions.EligibleSubmissionSACEM;
            submission.InterestedParties.Add(InterestedParties.IP_PRS[0]);
            var submittingAgency = submission.Agency;
            var receivingWorkcode = submission.Workcode;

            var res = await submissionClient.AddSubmissionAsync(submission);
            submission.PreferredIswc = res.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);

            submission.Workcode = CreateNewWorkCode();
            submission.Agency = Submissions.EligibleSubmissionPRS.Agency;
            submission.Sourcedb = Submissions.EligibleSubmissionPRS.Sourcedb;
            await submissionClient.AddSubmissionAsync(submission);

            var subTwo = Submissions.EligibleSubmissionPRS;
            subTwo.InterestedParties.Add(InterestedParties.IP_SACEM[0]);
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
            var csn = await cosmosClient.GetCsnEntry(receivingWorkcode, TransactionTypes.DMR);
            IEnumerable<WorkflowStatus> statuses = new List<WorkflowStatus>() { WorkflowStatus.Outstanding };
            var workflowTask = (await workflow_TasksClient.FindWorkflowTasksAsync(submittingAgency, ShowWorkflows.AssignedToMe, WorkflowType.DemergeApproval, statuses, 0, 500,
                string.Empty, string.Empty, string.Empty, string.Empty, string.Empty))
                .FirstOrDefault(x => x.IswcMetadata.Iswc == subTwo.PreferredIswc);

            Assert.NotNull(csn);
            Assert.Equal(receivingWorkcode, csn.ReceivingAgencyWorkCode);
            Assert.Equal(workflowTask.WorkflowTaskId, Convert.ToInt32(csn.WorkflowTaskID));
        }

        /// <summary>
        /// Demerge CSN is generated for submitting agency.
        /// </summary>
        [Fact]
        public async void CosmosCSNTests_07()
        {
            var submission = Submissions.EligibleSubmissionSACEM;
            var res = await submissionClient.AddSubmissionAsync(submission);
            submission.PreferredIswc = res.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);

            var subTwo = Submissions.EligibleSubmissionSACEM;
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
            var csn = await cosmosClient.GetCsnEntry(subTwo.Workcode, TransactionTypes.DMR);

            Assert.NotNull(csn);
            Assert.Equal(subTwo.Workcode, csn.ReceivingAgencyWorkCode);
            Assert.Equal(subTwo.PreferredIswc, csn.ToIswc);
            Assert.Equal(submission.PreferredIswc, csn.FromIswc);
        }

        /// <summary>
        /// Deleting a submission creates a CSN for submitting agency.
        /// </summary>
        [Fact]
        public async void CosmosCSNTests_08()
        {
            var submission = Submissions.EligibleSubmissionAKKA;
            var res = await submissionClient.AddSubmissionAsync(submission);
            submission.PreferredIswc = res.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);

            await submissionClient.DeleteSubmissionAsync(
                submission.PreferredIswc, submission.Agency, submission.Workcode, submission.Sourcedb, "IT_18_08_Cosmos_CSN");

            var csn = await cosmosClient.GetCsnEntry(submission.Workcode, TransactionTypes.CDR);

            Assert.NotNull(csn);
            Assert.Equal(submission.PreferredIswc, csn.ToIswc);
            Assert.Equal(submission.Workcode, csn.ReceivingAgencyWorkCode);
            Assert.Null(csn.WorkflowTaskID);
            Assert.Equal(csn.ReceivingAgencyCode, csn.SubmittingAgencyCode);
        }
    }
}
