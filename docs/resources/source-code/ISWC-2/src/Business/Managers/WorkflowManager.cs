using AutoMapper;
using LinqKit;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.Repositories;
using SpanishPoint.Azure.Iswc.Data.Services.Notifications;
using SpanishPoint.Azure.Iswc.Data.Services.Notifications.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Data.Services.UpdateWorkflowHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Business.Managers
{
    public interface IWorkflowManager
    {
        Task<IEnumerable<Bdo.Work.WorkflowTask>> UpdateWorkflowTasks(IEnumerable<Bdo.Work.WorkflowTask> tasks, string agency);
        Task<IEnumerable<Bdo.Work.WorkflowTask>> FindWorkflows(WorkflowSearchModel searchModel);
    }

    public class WorkflowManager : IWorkflowManager
    {
        private readonly IWorkflowRepository workflowRepository;
        private readonly IIswcRepository iswcRepository;
        private readonly IWorkRepository workRepository;
        private readonly IIswcLinkedToRepository iswcLinkedToRepository;
        private readonly IMessagingManager messagingManager;
        private readonly IWorkManager workManager;
        private readonly IMapper mapper;
        private readonly IUpdateWorkflowHistoryService updateWorkflowsHistoryService;
        private readonly INotificationService notificationService;
        private readonly IWorkflowInstanceRepository workflowInstancesRepository;

        public WorkflowManager(IWorkflowRepository workflowRepository, IIswcRepository iswcRepository, IWorkRepository workRepository,
            IIswcLinkedToRepository iswcLinkedToRepository, IMessagingManager messagingManager, IWorkManager workManager, IMapper mapper,
            IUpdateWorkflowHistoryService updateWorkflowsHistoryService, IWorkflowInstanceRepository workflowInstancesRepository, INotificationService notificationService)
        {
            this.workflowRepository = workflowRepository;
            this.iswcRepository = iswcRepository;
            this.workRepository = workRepository;
            this.iswcLinkedToRepository = iswcLinkedToRepository;
            this.messagingManager = messagingManager;
            this.workManager = workManager;
            this.mapper = mapper;
            this.updateWorkflowsHistoryService = updateWorkflowsHistoryService;
            this.notificationService = notificationService;
            this.workflowInstancesRepository = workflowInstancesRepository;
        }

        public async Task<IEnumerable<Bdo.Work.WorkflowTask>> FindWorkflows(WorkflowSearchModel searchModel)
        {
            var predicate = BuildPredicate(searchModel);

            var workflows = await workflowInstancesRepository.FindWorkflows(predicate, searchModel.StartIndex, searchModel.PageLength, includes: new string[] {
                $"{nameof(WorkflowTask)}", $"{nameof(WorkInfo)}",
                $"{nameof(MergeRequest)}", $"{nameof(MergeRequest)}.{nameof(IswclinkedTo)}.{nameof(Iswc.Data.DataModels.Iswc)}",
                $"{nameof(DeMergeRequest)}",$"{nameof(DeMergeRequest)}.{nameof(IswclinkedTo)}.{nameof(Iswc.Data.DataModels.Iswc)}"});

            var updateIswcs = workflows.Where(w => w.WorkInfoId != null).ToDictionary(w => w.WorkflowInstanceId, w => w.WorkInfo!.IswcId);
            var mergeIswcs = workflows.Where(w => w.MergeRequestId != null).ToDictionary(w => w.WorkflowInstanceId, w => w.MergeRequest!.IswclinkedTo?.FirstOrDefault()?.IswcId ?? 0L);
            var demergeIswcs = workflows.Where(w => w.DeMergeRequestId != null).ToDictionary(w => w.WorkflowInstanceId, w => w.DeMergeRequest!.IswclinkedTo?.FirstOrDefault()?.IswcId ?? 0L);

            var workflowIswcs = updateIswcs.Union(mergeIswcs).Union(demergeIswcs).ToDictionary(x => x.Key, x => x.Value);
            var iswcModels = (await iswcRepository.GetIswcModels(workflowIswcs.Select(p => p.Value).Distinct(), true, DetailLevel.Core))
                .Where(m => m.IswcId.HasValue).ToDictionary(m => m.IswcId!.Value, m => m);

            ICollection<Bdo.Work.WorkflowTask> workflowTasks = new List<Bdo.Work.WorkflowTask>();
            foreach (var task in workflows.SelectMany(x => x.WorkflowTask)
                .Where(x => searchModel.ShowWorkflows == Bdo.Work.ShowWorkflows.AssignedToMe
                ? x.AssignedAgencyId == searchModel.Agency : x.AssignedAgencyId != searchModel.Agency))
            {
                var workflowTask = mapper.Map<Bdo.Work.WorkflowTask>(task);
                workflowTask.IswcMetadata = iswcModels.TryGetValue(workflowIswcs[task.WorkflowInstanceId], out var iswcModel) ? iswcModel : default;
                workflowTask.CreatedDate = task.WorkflowInstance.CreatedDate;

                if (task.WorkflowInstance.WorkInfo != null) workflowTask.OriginatingSociety = task.WorkflowInstance.WorkInfo.AgencyId;
                else if (task.WorkflowInstance.MergeRequestId != null) workflowTask.OriginatingSociety = task.WorkflowInstance.MergeRequest.AgencyId;
                else if (task.WorkflowInstance.DeMergeRequestId != null) workflowTask.OriginatingSociety = task.WorkflowInstance.DeMergeRequest.AgencyId;

                workflowTasks.Add(workflowTask);
            }

            return workflowTasks.Where(x => searchModel.Statuses.ToList().Contains(x.TaskStatus)).OrderByDescending(w => w.CreatedDate);
        }

        private ExpressionStarter<WorkflowInstance> BuildPredicate(WorkflowSearchModel searchModel)
        {
            var predicate = PredicateBuilder.New<WorkflowInstance>();

            predicate.Start(x => x.WorkflowTask.Any(x => searchModel.Statuses.Cast<int>().ToList().Contains(x.TaskStatus))
               && !x.IsDeleted);

            if (searchModel.ShowWorkflows == Bdo.Work.ShowWorkflows.AssignedToMe)
                predicate.And(x => x.WorkflowTask.Any(wt => wt.AssignedAgencyId == searchModel.Agency));
            else
            {
                predicate.And(x => x.WorkInfoId != null && x.WorkInfo.Agency.AgencyId == searchModel.Agency);
                predicate.Or(x => x.MergeRequest != null && x.MergeRequest.Agency.AgencyId == searchModel.Agency);
                predicate.Or(x => x.DeMergeRequest != null && x.DeMergeRequest.AgencyId == searchModel.Agency);
            }

            if (searchModel.FromDate.HasValue)
                predicate.And(x => x.CreatedDate >= searchModel.FromDate);

            if (searchModel.ToDate.HasValue)
                predicate.And(x => x.CreatedDate >= searchModel.ToDate);

            if (!string.IsNullOrWhiteSpace(searchModel.Iswc))
            {
                predicate.And(x => x.WorkInfoId != null && x.WorkInfo.Iswc.Iswc1 == searchModel.Iswc);
                predicate.Or(x => x.MergeRequest != null && x.MergeRequest.IswclinkedTo.Any(y => y.Iswc.Iswc1 == searchModel.Iswc));
                predicate.Or(x => x.DeMergeRequest != null && x.DeMergeRequest.IswclinkedTo.Any(y => !string.IsNullOrWhiteSpace(y.Iswc.Iswc1) && y.Iswc.Iswc1 == searchModel.Iswc));
            }

            if (!string.IsNullOrWhiteSpace(searchModel.OriginatingAgency))
                predicate.And(x => x.WorkInfo.Agency.AgencyId == searchModel.OriginatingAgency || x.MergeRequest.Agency.AgencyId == searchModel.OriginatingAgency || x.DeMergeRequest.AgencyId == searchModel.OriginatingAgency);

            if (searchModel.AgencyWorkCodes.Any())
                predicate.And(x => searchModel.AgencyWorkCodes.Any(y => y == x.WorkInfo.AgencyWorkCode));

            if (searchModel.WorkflowType.HasValue)
                predicate.And(x => (int)searchModel.WorkflowType == x.WorkflowType);

            return predicate;
        }

        public async Task<IEnumerable<Bdo.Work.WorkflowTask>> UpdateWorkflowTasks(IEnumerable<Bdo.Work.WorkflowTask> tasks, string agency)
        {
            var unitOfWork = workflowRepository.UnitOfWork;

            var workflowTasks = (await workflowRepository.FindManyAsyncOptimizedByPath(x => x.AssignedAgencyId == agency,
                $"{nameof(WorkflowInstance)}", $"{nameof(WorkflowInstance)}.{nameof(MergeRequest)}", $"{nameof(WorkflowInstance)}.{nameof(DeMergeRequest)}",
                $"{nameof(WorkflowInstance)}.{nameof(WorkInfo)}", $"{nameof(WorkflowInstance)}.{nameof(WorkInfo)}.{nameof(Data.DataModels.Iswc)}")).ToList();

            var updatedTasks = new List<WorkflowTask>() { };

            foreach (var task in tasks)
            {
                var taskToUpdate = workflowTasks.FirstOrDefault(x => x.WorkflowTaskId == task.TaskId &&
                    (Bdo.Work.WorkflowType)x.WorkflowInstance.WorkflowType == task.WorkflowTaskType);

                if (taskToUpdate == null)
                    task.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._152);
                else
                {
                    updatedTasks.Add(taskToUpdate);
                    taskToUpdate.TaskStatus = (int)task.TaskStatus;
                    taskToUpdate.Message = task.Message;
                    var newStatus = CheckWorkflowInstanceStatus(taskToUpdate.WorkflowInstance);

                    if (newStatus != Bdo.Work.InstanceStatus.Outstanding)
                    {
                        taskToUpdate.WorkflowInstance.InstanceStatus = (int)newStatus;

                        if (taskToUpdate.WorkflowInstance.MergeRequestId != null)
                            taskToUpdate.WorkflowInstance.MergeRequest.MergeStatus = (int)Bdo.Work.MergeStatus.Complete;
                    }

                    await workflowRepository.UpdateAsync(taskToUpdate);
                    task.IswcMetadata = await GetIswcMetadataForTask(taskToUpdate);
                }
            }

            await unitOfWork.Save();

            foreach (WorkflowInstance workflowInstance in updatedTasks.Select(x => x.WorkflowInstance).Distinct())
            {
                if (workflowInstance.InstanceStatus == (int)Bdo.Work.InstanceStatus.Rejected
                    && workflowInstance.WorkflowType == (int)Bdo.Work.WorkflowType.UpdateApproval)
                    await RollBackWorkinfoRecord(workflowInstance, agency);

                if (workflowInstance.InstanceStatus == (int)Bdo.Work.InstanceStatus.Rejected
                    && (workflowInstance.WorkflowType == (int)Bdo.Work.WorkflowType.MergeApproval
                    || workflowInstance.WorkflowType == (int)Bdo.Work.WorkflowType.DemergeApproval))
                    await RollBackMerge(workflowInstance, agency);
            }

            return tasks;

            static Bdo.Work.InstanceStatus CheckWorkflowInstanceStatus(WorkflowInstance instance)
            {
                if (instance.WorkflowTask.All(x => x.TaskStatus == (int)Bdo.Work.InstanceStatus.Approved))
                    return Bdo.Work.InstanceStatus.Approved;

                else if (instance.WorkflowTask.Any(x => x.TaskStatus == (int)Bdo.Work.InstanceStatus.Rejected))
                    return Bdo.Work.InstanceStatus.Rejected;

                else if (instance.WorkflowTask.Any(x => x.TaskStatus == (int)Bdo.Work.InstanceStatus.Cancelled))
                    return Bdo.Work.InstanceStatus.Cancelled;

                else return Bdo.Work.InstanceStatus.Outstanding;
            }

        }

        async Task RollBackWorkinfoRecord(WorkflowInstance workflowInstance, string agency)
        {
            var mergedIswc = await iswcLinkedToRepository.FindAsync(x => x.LinkedToIswc == workflowInstance.WorkInfo.Iswc.Iswc1 && x.Status
            && x.MergeRequestNavigation.MergeStatus == (int)Bdo.Work.MergeStatus.Pending);

            if (mergedIswc == null)
            {
                await UpdateWorkIntoToPreviousState(workflowInstance.WorkInfo.Iswc.Iswc1, workflowInstance.WorkInfoId);
            }
            else
            {
                var childIswc = await iswcRepository.FindAsyncOptimizedByPath(x => x.IswcId == mergedIswc.IswcId, "WorkInfo", "IswcLinkedTo");

                var workUpdatedBeforeAutoMerge = childIswc.WorkInfo.FirstOrDefault(x => x.AgencyWorkCode == workflowInstance.WorkInfo.AgencyWorkCode
                && x.AgencyId == workflowInstance.WorkInfo!.AgencyId);

                await UpdateWorkIntoToPreviousState(childIswc.Iswc1, workUpdatedBeforeAutoMerge?.WorkInfoId);

                if (mergedIswc != null)
                {
                    var worksToBeRemovedFromParent = await UndoReplaceWorks(childIswc);

                    if (worksToBeRemovedFromParent.Any())
                        await RemoveChildFromParent(mergedIswc.LinkedToIswc, worksToBeRemovedFromParent);
                }
            }

            async Task UpdateWorkIntoToPreviousState(string iswc, long? workInfoRecord)
            {
                var submission = await updateWorkflowsHistoryService.GetModel(iswc, workInfoRecord);
                submission.Model.Agency = agency;
                submission.Model.WorkflowTasks = mapper.Map<ICollection<Bdo.Work.WorkflowTask>>(workflowInstance.WorkflowTask);
                await workManager.UpdateAsync(submission, true);
            }
        }

        async Task<List<(string workCode, string agency)>> UndoReplaceWorks(Data.DataModels.Iswc? childIswc)
        {
            var unitOfWork = iswcRepository.UnitOfWork;
            var now = DateTime.UtcNow;
            var worksToBeRemovedFromParent = new List<(string workCode, string agency)>();

            if (childIswc == null) return worksToBeRemovedFromParent;

            foreach (var childWork in childIswc.WorkInfo)
            {
                childWork.LastModifiedDate = now;
                childWork.IsReplaced = false;

                worksToBeRemovedFromParent.Add((childWork.AgencyWorkCode, childWork.AgencyId));
            }

            foreach (var linkedTo in childIswc.IswclinkedTo.Where(x => x.Status && x.MergeRequestNavigation.MergeStatus == (int)Bdo.Work.MergeStatus.Pending))
                linkedTo.Status = false;


            await iswcRepository.UpdateAsync(childIswc);
            await unitOfWork.Save();

            return worksToBeRemovedFromParent;
        }

        async Task RollBackMerge(WorkflowInstance workflowInstance, string agency)
        {
            var unitOfWork = workRepository.UnitOfWork;
            IList<CsnNotifications> notifications = new List<CsnNotifications>();
            var worksToBeUpdatedOnParent = new List<(string workCode, string agency)>();
            var now = DateTime.UtcNow;

            if (workflowInstance.WorkflowType == (int)Bdo.Work.WorkflowType.MergeApproval)
            {
                notifications = await GetCsnNotifications(workflowInstance, TransactionType.MER, agency);
                foreach (var wi in workflowInstance.MergeRequest.IswclinkedTo)
                {
                    wi.Status = false;
                    foreach (var child in wi.Iswc.WorkInfo.Where(x => x.IsReplaced))
                    {
                        child.IsReplaced = false;
                        child.LastModifiedDate = now;
                        worksToBeUpdatedOnParent.Add((child.AgencyWorkCode, child.AgencyId));
                    }

                    await RemoveChildFromParent(wi.LinkedToIswc, worksToBeUpdatedOnParent);
                };

            }
            else if (workflowInstance.WorkflowType == (int)Bdo.Work.WorkflowType.DemergeApproval)
            {
                notifications = await GetCsnNotifications(workflowInstance, TransactionType.DMR, agency);
                foreach (var wi in workflowInstance.DeMergeRequest.IswclinkedTo)
                {
                    wi.Status = true;

                    foreach (var child in wi.Iswc.WorkInfo.Where(x => !x.IsReplaced))
                    {
                        child.IsReplaced = true;
                        child.LastModifiedDate = now;
                        worksToBeUpdatedOnParent.Add((child.AgencyWorkCode, child.AgencyId));
                    }

                    await ReAddChildToParent(wi.LinkedToIswc, worksToBeUpdatedOnParent);
                };
            }

            await notificationService.AddCsnNotifications(notifications, workflowInstance);
            await unitOfWork.Save();
        }

        async Task RemoveChildFromParent(string iswc, IList<(string workCode, string agency)> worksToBeUpdatedOnParent)
        {
            var unitOfWork = iswcRepository.UnitOfWork;
            var parentIswc = await iswcRepository.FindAsyncOptimizedByPath(x => x.Iswc1 == iswc, "WorkInfo");
            var now = DateTime.UtcNow;

            foreach (var work in parentIswc.WorkInfo.Where(x => worksToBeUpdatedOnParent.Any(y => y.workCode == x.AgencyWorkCode && y.agency == x.AgencyId)))
            {
                work.Status = false;
                work.LastModifiedDate = now;
            }

            await iswcRepository.UpdateAsync(parentIswc);
            await unitOfWork.Save();
        }

        async Task ReAddChildToParent(string iswc, IList<(string workCode, string agency)> worksToBeUpdatedOnParent)
        {
            var unitOfWork = iswcRepository.UnitOfWork;
            var parentIswc = await iswcRepository.FindAsyncOptimizedByPath(x => x.Iswc1 == iswc, "WorkInfo", "WorkInfo.Title", "WorkInfo.Creator",
                "WorkInfo.Title", "WorkInfo.Publisher", "WorkInfo.WorkInfoPerformer", "WorkInfo.DerivedFrom", "WorkInfo.DisambiguationIswc");
            var now = DateTime.UtcNow;

            foreach (var work in parentIswc.WorkInfo.Where(x => worksToBeUpdatedOnParent.Any(y => y.workCode == x.AgencyWorkCode && y.agency == x.AgencyId)))
            {
                work.Status = true;
                work.LastModifiedDate = now;

                foreach (var title in work.Title) { title.Status = true; title.LastModifiedDate = now; }
                foreach (var creator in work.Creator) { creator.Status = true; creator.LastModifiedDate = now; }
                foreach (var p in work.Publisher) { p.Status = true; p.LastModifiedDate = now; }
                foreach (var p in work.WorkInfoPerformer) { p.Status = true; p.LastModifiedDate = now; }
                foreach (var df in work.DerivedFrom) { df.Status = true; df.LastModifiedDate = now; }
                foreach (var di in work.DisambiguationIswc) { di.Status = true; di.LastModifiedDate = now; }
            }

            await iswcRepository.UpdateAsync(parentIswc);
            await unitOfWork.Save();
        }

        async Task<IswcModel> GetIswcMetadataForTask(WorkflowTask workflowTask)
        {
            long iswcId = 0;
            if ((Bdo.Work.WorkflowType)workflowTask.WorkflowInstance.WorkflowType == Bdo.Work.WorkflowType.UpdateApproval && workflowTask.WorkflowInstance.WorkInfoId != null)
            {
                iswcId = (await workRepository.FindAsync(x => x.WorkInfoId == workflowTask.WorkflowInstance.WorkInfoId)).IswcId;
            }
            else if ((Bdo.Work.WorkflowType)workflowTask.WorkflowInstance.WorkflowType == Bdo.Work.WorkflowType.MergeApproval
                && workflowTask.WorkflowInstance.MergeRequestId != null)
            {
                iswcId = (await iswcLinkedToRepository.FindAsync(x => x.MergeRequest == workflowTask.WorkflowInstance.MergeRequestId)).IswcId;
            }
            else if ((Bdo.Work.WorkflowType)workflowTask.WorkflowInstance.WorkflowType == Bdo.Work.WorkflowType.DemergeApproval
                && workflowTask.WorkflowInstance.DeMergeRequestId != null)
            {
                iswcId = (await iswcLinkedToRepository.FindAsync(x => x.DeMergeRequest == workflowTask.WorkflowInstance.DeMergeRequestId)).IswcId;
            }

            var result = await workManager.FindAsync(iswcId);

            if (workflowTask.WorkflowInstance?.WorkInfoId != null && result.VerifiedSubmissions.Any())
            {
                var selected = result.VerifiedSubmissions.FirstOrDefault(x => x.WorkInfoID == workflowTask.WorkflowInstance.WorkInfoId);

                var works = new List<Bdo.Submissions.VerifiedSubmissionModel>();
                if (selected != null)
                    works.Add(selected);

                result.VerifiedSubmissions = works;
            }

            if (result.ParentIswc == null)
                result.ParentIswc = mapper.Map<IswcModel>(result.LinkedIswc.FirstOrDefault(x => x.Status));

            else if (result.ParentIswc != null)
                result.ParentIswc.LinkedIswc = new List<IswcModel>();

            result.LinkedIswc = mapper.Map<ICollection<IswcModel>>(await iswcRepository.FindManyAsyncOptimized(
                x => x.IswclinkedTo.Any(y => y.LinkedToIswc == result.Iswc && y.Status),
                i => i.LastModifiedUser));

            return result;
        }

        async Task<IList<CsnNotifications>> GetCsnNotifications(WorkflowInstance workflowInstance, TransactionType transactionType, string agency)
        {
            var csnRecords = new List<CsnNotifications>();

            var iswcs = transactionType == TransactionType.MER ? workflowInstance.MergeRequest.IswclinkedTo.Select(x => new LinkedIswc { IswcId = x.IswcId, LinkedToIswc = x.LinkedToIswc }) :
                workflowInstance.DeMergeRequest.IswclinkedTo.Select(x => new LinkedIswc { IswcId = x.IswcId, LinkedToIswc = x.LinkedToIswc });

            foreach (var linkedIswc in iswcs)
            {
                var workInfos = await workRepository.FindManyAsyncOptimized(x => x.IswcId == linkedIswc.IswcId && x.Status, i => i.WorkflowInstance);
                var parentWorkInfos = await workRepository.FindManyAsyncOptimized(x => x.Iswc.Iswc1 == linkedIswc.LinkedToIswc && x.Status && !workInfos.Any(w => w.AgencyId == x.AgencyId), i => i.Iswc, i => i.WorkflowInstance);
                var combinedWorkInfos = (workInfos ?? Enumerable.Empty<WorkInfo>()).Concat(parentWorkInfos ?? Enumerable.Empty<WorkInfo>());
                
                foreach (var workInfo in combinedWorkInfos)
                {
                    csnRecords.Add(new CsnNotifications(workInfo, agency, DateTime.UtcNow, transactionType, workInfo.Iswc.Iswc1));
                }
            }
            return csnRecords;
        }
    }
}
