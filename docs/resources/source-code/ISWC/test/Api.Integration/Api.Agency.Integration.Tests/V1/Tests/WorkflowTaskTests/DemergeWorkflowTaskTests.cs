using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using xRetry;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Tests.WorkflowTaskTests
{
    public class DemergeWorkflowTaskTests_Fixture : IAsyncLifetime
    {
        public IISWC_SubmissionClient submissionClient;
        public IISWC_SearchClient searchclient;
        public IISWC_Workflow_TasksClient workflow_TasksClient;
        public IISWC_MergeClient mergeClient;
        public HttpClient httpClient;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await TestBase.GetClient();
            submissionClient = new ISWC_SubmissionClient(httpClient);
            searchclient = new ISWC_SearchClient(httpClient);
            workflow_TasksClient = new ISWC_Workflow_TasksClient(httpClient);
            mergeClient = new ISWC_MergeClient(httpClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            return Task.CompletedTask;
        }
    }

    public class DemergeWorkflowTaskTests : TestBase, IClassFixture<DemergeWorkflowTaskTests_Fixture>
    {
        private readonly IISWC_Workflow_TasksClient workflow_TasksClient;
        private readonly IISWC_SubmissionClient submissionClient;
        private readonly IISWC_SearchClient searchClient;
        private readonly IISWC_MergeClient mergeClient;
        private readonly HttpClient httpClient;

        public DemergeWorkflowTaskTests(DemergeWorkflowTaskTests_Fixture fixture)
        {
            workflow_TasksClient = fixture.workflow_TasksClient;
            searchClient = fixture.searchclient;
            submissionClient = fixture.submissionClient;
            mergeClient = fixture.mergeClient;
            httpClient = fixture.httpClient;
        }

        /// <summary>
        /// Reject DemergeApprovalTask
        /// </summary>
        [Fact]
        public async void DemergeWorkflowTaskTests_01()
        {
            var parentSubmission = Submissions.EligibleSubmissionSACEM;
            var res = await submissionClient.AddSubmissionAsync(parentSubmission);
            parentSubmission.PreferredIswc = res.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(parentSubmission.Agency, parentSubmission.Workcode, httpClient);

            var childSubmission = Submissions.EligibleSubmissionPRS;
            var re = await submissionClient.AddSubmissionAsync(childSubmission);
            await WaitForSubmission(childSubmission.Agency, childSubmission.Workcode, httpClient);
            childSubmission.PreferredIswc = re.VerifiedSubmission.Iswc.ToString();

            var body = new Body
            {
                Iswcs = new string[] { childSubmission.PreferredIswc }
            };

            await mergeClient.MergeISWCMetadataAsync(parentSubmission.PreferredIswc, childSubmission.Agency, body);
            await Task.Delay(2000);


            await mergeClient.DemergeISWCMetadataAsync(parentSubmission.PreferredIswc, childSubmission.Agency, childSubmission.Workcode);

            IEnumerable<WorkflowStatus> statuses = new List<WorkflowStatus>() { WorkflowStatus.Outstanding };
            var workflowTaskToReject = (await workflow_TasksClient.FindWorkflowTasksAsync(parentSubmission.Agency, ShowWorkflows.AssignedToMe, WorkflowType.DemergeApproval, statuses, 0, 500,
                string.Empty, string.Empty, string.Empty, string.Empty, string.Empty))
                .FirstOrDefault(x => x.IswcMetadata.Iswc == childSubmission.PreferredIswc);

            ICollection<WorkflowTask> rejectDemerge = await workflow_TasksClient.UpdateWorkflowTaskAsync(parentSubmission.Agency, new List<WorkflowTaskUpdate>() {
                    new WorkflowTaskUpdate
                    {
                        TaskId = workflowTaskToReject.WorkflowTaskId,
                        Status = WorkflowTaskUpdateStatus.Rejected,
                        WorkflowType = WorkflowTaskUpdateWorkflowType.DemergeApproval,
                        WorkflowMessage = "REJECT DEMERGE"
                    }
                });
            await Task.Delay(2000);
            var result = await searchClient.SearchByISWCAsync(parentSubmission.PreferredIswc);

            Assert.Equal(1, result.LinkedISWC.Count);
            Assert.Equal(childSubmission.OriginalTitle, result.OriginalTitle);
            Assert.Equal(2, result.Works.Count);
        }

        /// <summary>
        /// Approve DemergeApprovalTask
        /// </summary>
        [RetryFact]
        public async void DemergeWorkflowTaskTests_02()
        {
            var parentSubmission = Submissions.EligibleSubmissionSACEM;
            var res = await submissionClient.AddSubmissionAsync(parentSubmission);
            parentSubmission.PreferredIswc = res.VerifiedSubmission.Iswc.ToString();
            await WaitForSubmission(parentSubmission.Agency, parentSubmission.Workcode, httpClient);

            var childSubmission = Submissions.EligibleSubmissionPRS;
            var re = await submissionClient.AddSubmissionAsync(childSubmission);
            await WaitForSubmission(childSubmission.Agency, childSubmission.Workcode, httpClient);
            childSubmission.PreferredIswc = re.VerifiedSubmission.Iswc.ToString();

            var body = new Body
            {
                Iswcs = new string[] { childSubmission.PreferredIswc }
            };

            await mergeClient.MergeISWCMetadataAsync(parentSubmission.PreferredIswc, childSubmission.Agency, body);
            await Task.Delay(2000);

            await mergeClient.DemergeISWCMetadataAsync(parentSubmission.PreferredIswc, childSubmission.Agency, childSubmission.Workcode);

            IEnumerable<WorkflowStatus> statuses = new List<WorkflowStatus>() { WorkflowStatus.Outstanding };
            var workflowTaskToApprove = (await workflow_TasksClient.FindWorkflowTasksAsync(parentSubmission.Agency, ShowWorkflows.AssignedToMe, WorkflowType.DemergeApproval, statuses, 0, 500,
                string.Empty, string.Empty, string.Empty, string.Empty, string.Empty))
                .FirstOrDefault(x => x.IswcMetadata.Iswc == childSubmission.PreferredIswc);

            ICollection<WorkflowTask> approveDemerge = await workflow_TasksClient.UpdateWorkflowTaskAsync(parentSubmission.Agency, new List<WorkflowTaskUpdate>() {
                    new WorkflowTaskUpdate
                    {
                        TaskId = workflowTaskToApprove.WorkflowTaskId,
                        Status = WorkflowTaskUpdateStatus.Approved,
                        WorkflowType = WorkflowTaskUpdateWorkflowType.DemergeApproval
                    }
                });
            var result = await searchClient.SearchByISWCAsync(parentSubmission.PreferredIswc);

            Assert.True(result.LinkedISWC.Count == 0);
            Assert.Equal(0, result.OtherTitles.Count);
            Assert.Equal(1, result.Works.Count);
        }
    }
}
