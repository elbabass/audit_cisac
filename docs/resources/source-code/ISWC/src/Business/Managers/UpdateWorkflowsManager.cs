using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstanceStatus = SpanishPoint.Azure.Iswc.Bdo.Work.InstanceStatus;

namespace SpanishPoint.Azure.Iswc.Business.Managers
{
    public interface IUpdateWorkflowsManager
    {
        Task CompleteWorkflows(int minimumAgeInDays);
    }

    public class UpdateWorkflowsManager : IUpdateWorkflowsManager
    {
        private readonly IWorkflowManager workflowManager;
        private readonly IWorkflowInstanceRepository workflowInstancesRepository;

        public UpdateWorkflowsManager(IWorkflowManager workflowManager, IWorkflowInstanceRepository workflowInstancesRepository)
        {
            this.workflowManager = workflowManager;
            this.workflowInstancesRepository = workflowInstancesRepository;
        }

        public async Task CompleteWorkflows(int minimumAgeInDays)
        {
            var unitOfWork = workflowInstancesRepository.UnitOfWork;

            var eligibleWorkflows = await FindWorkflowsDueUpdate(minimumAgeInDays);

            foreach (var workflow in eligibleWorkflows)
            {
                var finalStatus = workflow.WorkflowTask.Any(x => x.TaskStatus == (int)InstanceStatus.Rejected) ? InstanceStatus.Rejected
                    : workflow.WorkflowTask.Any(x => x.TaskStatus == (int)InstanceStatus.Cancelled) ? InstanceStatus.Cancelled
                    : InstanceStatus.Approved;

                workflow.InstanceStatus = (int)finalStatus;
                foreach (var task in workflow.WorkflowTask)
                {
                    task.TaskStatus = (int)finalStatus;
                }

                if (workflow.WorkflowType == (int)Bdo.Work.WorkflowType.MergeApproval)
                    workflow.MergeRequest.MergeStatus = (int)Bdo.Work.MergeStatus.Complete;
                else if (workflow.WorkflowType == (int)Bdo.Work.WorkflowType.DemergeApproval)
                    workflow.DeMergeRequest.DeMergeStatus = (int)Bdo.Work.MergeStatus.Complete;
            }

            await unitOfWork.Save();

            async Task<IEnumerable<WorkflowInstance>> FindWorkflowsDueUpdate(int minWorkflowAge)
            {
                return await workflowInstancesRepository
                    .FindManyAsyncOptimizedByPath(x => x.CreatedDate.AddDays(minWorkflowAge) <= DateTime.UtcNow
                    && x.WorkflowTask.Any(y => (InstanceStatus)y.TaskStatus == InstanceStatus.Outstanding),
                    $"{nameof(WorkflowTask)}", $"{nameof(MergeRequest)}", $"{nameof(DeMergeRequest)}");
            }
        }
    }
}
