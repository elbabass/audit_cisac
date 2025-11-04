using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using xRetry;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Tests.WorkflowTaskTests
{
    public class UpdateWorkFlowTaskTests_Fixture : IAsyncLifetime
    {
        public IISWC_SubmissionClient submissionClient;
        public IISWC_SearchClient searchclient;
        public IISWC_Workflow_TasksClient workflow_TasksClient;
        public HttpClient httpClient;

        async Task IAsyncLifetime.InitializeAsync()
        {
            httpClient = await TestBase.GetClient();
            submissionClient = new ISWC_SubmissionClient(httpClient);
            searchclient = new ISWC_SearchClient(httpClient);
            workflow_TasksClient = new ISWC_Workflow_TasksClient(httpClient);
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            httpClient.Dispose();
            return Task.CompletedTask;
        }
    }

    public class UpdateWorkFlowTaskTests : TestBase, IClassFixture<UpdateWorkFlowTaskTests_Fixture>
    {
        private readonly IISWC_Workflow_TasksClient workflow_TasksClient;
        private readonly IISWC_SubmissionClient submissionClient;
        private readonly IISWC_SearchClient searchClient;
        private readonly HttpClient httpClient;



        public UpdateWorkFlowTaskTests(UpdateWorkFlowTaskTests_Fixture fixture)
        {
            workflow_TasksClient = fixture.workflow_TasksClient;
            searchClient = fixture.searchclient;
            submissionClient = fixture.submissionClient;
            httpClient = fixture.httpClient;
        }

        /// <summary>
        /// Submitter is ISWC eligible
        /// Works requiring actions by the submitter are present within the system
        /// Submitter can update workflow tasks"
        /// </summary>
        [RetryFact]
        public async void UpdateWorkFlowTaskTests_01()
        {
            var submission = Submissions.EligibleSubmissionAKKA;
            var submittingAgency = submission.Agency;
            submission.InterestedParties.Add(InterestedParties.IP_SACEM[0]);
            var addRes = await submissionClient.AddSubmissionAsync(submission);
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            submission.PreferredIswc = addRes.VerifiedSubmission.Iswc.ToString();

            submission.Workcode = CreateNewWorkCode();
            submission.Agency = Submissions.EligibleSubmissionSACEM.Agency;
            addRes = await submissionClient.AddSubmissionAsync(submission);
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);

            submission.OriginalTitle = CreateNewTitle();
            var update = await submissionClient.UpdateSubmissionAsync(submission.PreferredIswc, submission);
            await WaitForUpdate(submission.Agency, submission.Workcode, submission.OriginalTitle, httpClient);


            IEnumerable<WorkflowStatus> statuses = new List<WorkflowStatus>() { WorkflowStatus.Outstanding };
            var task = (await workflow_TasksClient.FindWorkflowTasksAsync(submittingAgency, ShowWorkflows.AssignedToMe, WorkflowType.UpdateApproval, statuses, 0, 500,
                string.Empty, string.Empty, string.Empty, string.Empty, string.Empty))
                .FirstOrDefault(x => x.IswcMetadata.Iswc == submission.PreferredIswc);

            var res = await workflow_TasksClient.UpdateWorkflowTaskAsync(submittingAgency, new List<WorkflowTaskUpdate>()
            {
                new WorkflowTaskUpdate
                {
                    TaskId = task.WorkflowTaskId,
                    Status = WorkflowTaskUpdateStatus.Approved,
                    WorkflowType = WorkflowTaskUpdateWorkflowType.UpdateApproval
                },
                new WorkflowTaskUpdate
                {
                    TaskId = -5,
                    Status = WorkflowTaskUpdateStatus.Approved,
                    WorkflowType = WorkflowTaskUpdateWorkflowType.UpdateApproval
                },
            });

            Assert.Collection(res,
                elem1 =>
                {
                    Assert.NotNull(elem1.IswcMetadata);
                    Assert.Equal(WorkflowTaskStatus.Approved, elem1.Status);
                },
                elem2 =>
                {
                    Assert.Null(elem2.IswcMetadata);
                    Assert.Equal("152", elem2.Rejection.Code);
                });
        }

        /// <summary>
        /// Split Copyright Work
        /// Submitter is ISWC eligible
        /// Workflow Tasks are created for other agencies associated with an updated Work
        /// </summary>
        [RetryFact]
        public async void UpdateWorkFlowTaskTests_02()
        {
            var submission = Submissions.EligibleSubmissionAKKA;
            var submittingAgency = submission.Agency;
            submission.InterestedParties.Add(InterestedParties.IP_SACEM[0]);
            var addRes = await submissionClient.AddSubmissionAsync(submission);
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            submission.PreferredIswc = addRes.VerifiedSubmission.Iswc.ToString();

            submission.Workcode = CreateNewWorkCode();
            submission.Agency = Submissions.EligibleSubmissionSACEM.Agency;
            submission.Sourcedb = Submissions.EligibleSubmissionSACEM.Sourcedb;
            await submissionClient.AddSubmissionAsync(submission);
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);

            submission.OriginalTitle = CreateNewTitle();
            var update = await submissionClient.UpdateSubmissionAsync(submission.PreferredIswc, submission);
            await WaitForUpdate(submission.Agency, submission.Workcode, submission.OriginalTitle, httpClient);


            IEnumerable<WorkflowStatus> statuses = new List<WorkflowStatus>() { WorkflowStatus.Outstanding };
            var res = (await workflow_TasksClient.FindWorkflowTasksAsync(submittingAgency, ShowWorkflows.AssignedToMe, WorkflowType.UpdateApproval, statuses, 0, 500,
                string.Empty, string.Empty, string.Empty, string.Empty, string.Empty))
                .FirstOrDefault(x => x.IswcMetadata.Iswc == submission.PreferredIswc);

            Assert.NotNull(res.IswcMetadata);
            Assert.Equal(WorkflowTaskWorkflowType.UpdateApproval, res.WorkflowType);

            await workflow_TasksClient.UpdateWorkflowTaskAsync(submittingAgency, new List<WorkflowTaskUpdate>()
            {
                new WorkflowTaskUpdate
                {
                    TaskId = res.WorkflowTaskId,
                    Status = WorkflowTaskUpdateStatus.Approved,
                    WorkflowType = WorkflowTaskUpdateWorkflowType.UpdateApproval
                }
            });
        }


        /// <summary>
        /// Split copyright Work
        /// Submitter is ISWC eligible and Work is updated
        /// Rejecting the Update returns Work to previous state
        ///</summary>
        [RetryFact]
        public async void UpdateWorkFlowTaskTests_03()
        {
            var submission = Submissions.EligibleSubmissionAEPI;
            var submittingAgency = submission.Agency;
            var title = submission.OriginalTitle;
            submission.InterestedParties.Add(InterestedParties.IP_PRS[0]);
            var addRes = await submissionClient.AddSubmissionAsync(submission);
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);
            submission.PreferredIswc = addRes.VerifiedSubmission.Iswc.ToString();

            submission.Workcode = CreateNewWorkCode();
            submission.Agency = Submissions.EligibleSubmissionPRS.Agency;
            addRes = await submissionClient.AddSubmissionAsync(submission);
            await WaitForSubmission(submission.Agency, submission.Workcode, httpClient);

            submission.OriginalTitle = CreateNewTitle();
            var update = await submissionClient.UpdateSubmissionAsync(submission.PreferredIswc, submission);
            await WaitForUpdate(submission.Agency, submission.Workcode, submission.OriginalTitle, httpClient);

            IEnumerable<WorkflowStatus> statuses = new List<WorkflowStatus>() { WorkflowStatus.Outstanding };
            var workflowTaskToReject = (await workflow_TasksClient.FindWorkflowTasksAsync(submittingAgency, ShowWorkflows.AssignedToMe, WorkflowType.UpdateApproval, statuses, 0, 500,
                string.Empty, string.Empty, string.Empty, string.Empty, string.Empty))
                .FirstOrDefault(x => x.IswcMetadata.Iswc == submission.PreferredIswc);

            ICollection<WorkflowTask> res = await workflow_TasksClient.UpdateWorkflowTaskAsync(submittingAgency, new List<WorkflowTaskUpdate>()
            {
                new WorkflowTaskUpdate
                {
                    TaskId = workflowTaskToReject.WorkflowTaskId,
                    Status = WorkflowTaskUpdateStatus.Rejected,
                    WorkflowType = WorkflowTaskUpdateWorkflowType.UpdateApproval
                }
            });
            await WaitForUpdate(submission.Agency, submission.Workcode, title, httpClient);
            var result = await searchClient.SearchByISWCAsync(submission.PreferredIswc);
            Assert.Equal(result.OriginalTitle, title);
        }
    }
}
