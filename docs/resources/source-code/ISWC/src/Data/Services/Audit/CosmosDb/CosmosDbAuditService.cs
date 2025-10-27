using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LinqKit;
using Microsoft.Extensions.Logging;
using SpanishPoint.Azure.Iswc.Bdo.Audit;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Reports;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.Repositories;
using SpanishPoint.Azure.Iswc.Data.Services.Audit;
using SpanishPoint.Azure.Iswc.Data.Services.Audit.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Framework.CosmosDb.Repositories;
using SpanishPoint.Azure.Iswc.Framework.Extensions;

namespace SpanishPoint.Azure.Iswc.Data.Services.CosmosDb
{
    internal class CosmosDbAuditService : IAuditService
    {
        private readonly ICosmosDbRepository<AuditModel> auditContainer;
        private readonly ICosmosDbRepository<AuditRequestModel> auditRequestContainer;
        private readonly ICosmosDbRepository<FileAuditModel> fileAuditContainer;
        private readonly ICosmosDbRepository<AgencyStatisticsModel> agencyStatisticsContainer;
        private readonly IIswcLinkedToRepository iswcLinkedToRepository;
        private readonly IWorkflowInstanceRepository workflowInstanceRepository;
        private readonly IWorkRepository workRepository;
        private readonly IMapper mapper;
        private readonly IHighWatermarkRepository highWatermarkRepository;
        private readonly ILogger<CosmosDbAuditService> _logger;

        public CosmosDbAuditService(
            ICosmosDbRepository<AuditModel> auditContainer,
            ICosmosDbRepository<AuditRequestModel> auditRequestContainer,
            ICosmosDbRepository<FileAuditModel> fileAuditContainer,
            ICosmosDbRepository<AgencyStatisticsModel> agecnyStatisticsContainer,
            IIswcLinkedToRepository iswcLinkedToRepository,
            IWorkflowInstanceRepository workflowInstanceRepository,
            IWorkRepository workRepository,
            IMapper mapper, IHighWatermarkRepository highWatermarkRepository, ILogger<CosmosDbAuditService> logger)
        {
            this.auditContainer = auditContainer;
            this.auditRequestContainer = auditRequestContainer;
            this.fileAuditContainer = fileAuditContainer;
            this.agencyStatisticsContainer = agecnyStatisticsContainer;
            this.iswcLinkedToRepository = iswcLinkedToRepository;
            this.workflowInstanceRepository = workflowInstanceRepository;
            this.workRepository = workRepository;
            this.mapper = mapper;
            this.highWatermarkRepository = highWatermarkRepository;
            _logger = logger;
        }

        public async Task LogSubmissions(IEnumerable<Submission> submissions)
        {
            try
            {
                submissions = submissions.Where(x => !new TransactionType[] { TransactionType.CIQ, TransactionType.CMQ, TransactionType.COR }.Contains(x.TransactionType));

                if (submissions.Count() == 0) return;

                var firstSubmission = submissions.First();
                var now = DateTime.UtcNow;

                if (firstSubmission.ToBeProcessed)
                {
                    await auditContainer.UpsertItemAsync(new AuditModel
                    {
                        AuditId = firstSubmission.AuditId,
                        PartitionKey = firstSubmission.AuditId.ToString(),
                        AgencyCode = firstSubmission.Model.Agency,
                        CreatedDate = now,
                        BatchSize = submissions.Count()
                    });
                }

                foreach (var submission in submissions)
                {
                    long? workId = null;
                    long? workflowInstanceId = null;
                    TransactionSource transactionSource = GetTransactionSource(submission.RequestType);

                    if (submission.TransactionType == TransactionType.MER || submission.TransactionType == TransactionType.DMR)
                    {
                        if (submission.TransactionType == TransactionType.MER)
                        {
                            if (string.IsNullOrEmpty(submission.Model.PreferredIswc))
                                submission.Model.PreferredIswc = submission.IswcModel.Iswc;

                            if (submission.Model.IswcsToMerge.Count() > 0)
                            {
                                workflowInstanceId = (await iswcLinkedToRepository.FindManyAsyncOptimizedByPath(
                                    x => x.Iswc.Iswc1 == submission.Model.IswcsToMerge.First() && x.LinkedToIswc == submission.Model.PreferredIswc && x.Status,
                                    "Iswc", "MergeRequestNavigation", "MergeRequestNavigation.WorkflowInstance"))?.OrderByDescending(x => x.CreatedDate)?.FirstOrDefault()?
                                    .MergeRequestNavigation?.WorkflowInstance?.FirstOrDefault()?.WorkflowInstanceId;
                            }
                            else if (submission.Model.WorkNumbersToMerge.Count() > 0)
                            {
                                var worknumber = submission.Model.WorkNumbersToMerge.First();
                                var childIswc = (await workRepository.FindAsyncOptimizedByPath(
                                    x => x.AgencyWorkCode == worknumber.Number && x.AgencyId == worknumber.Type, "Iswc"))?.Iswc;

                                if (childIswc != null)
                                    workflowInstanceId = (await iswcLinkedToRepository.FindManyAsyncOptimizedByPath(
                                        x => x.IswcId == childIswc.IswcId && x.LinkedToIswc == submission.IswcModel.Iswc && x.Status,
                                        "Iswc", "MergeRequestNavigation", "MergeRequestNavigation.WorkflowInstance"))?.OrderByDescending(x => x.CreatedDate)?.FirstOrDefault()?
                                        .MergeRequestNavigation?.WorkflowInstance?.FirstOrDefault()?.WorkflowInstanceId;
                            }
                        }
                        else
                        {
                            var iswc = (await workRepository.FindAsyncOptimizedByPath(x => x.AgencyWorkCode == submission.Model.WorkNumber.Number
                                     && x.AgencyId == submission.Model.WorkNumber.Type, "Iswc", "Iswc.IswclinkedTo", "Iswc.IswclinkedTo.DeMergeRequestNavigation",
                                     "Iswc.IswclinkedTo.DeMergeRequestNavigation.WorkflowInstance"))?.Iswc;
                            workflowInstanceId = iswc?.IswclinkedTo?.OrderByDescending(x => x.CreatedDate).FirstOrDefault(
                                      x => x.LinkedToIswc == submission.Model.PreferredIswc)?.DeMergeRequestNavigation?.WorkflowInstance?
                                      .FirstOrDefault()?.WorkflowInstanceId;
                        }
                    }
                    else if (submission.IswcModel != null && submission.IswcModel.VerifiedSubmissions.Any())
                    {
                        if (string.IsNullOrEmpty(submission.Model.PreferredIswc))
                            submission.Model.PreferredIswc = submission.IswcModel.Iswc;

                        workId = submission.IswcModel.VerifiedSubmissions
                         .FirstOrDefault(x => x.WorkNumber.Number == submission.Model.WorkNumber.Number)?.WorkInfoID;

                        workflowInstanceId = submission.IswcModel.VerifiedSubmissions
                                .FirstOrDefault(x => x.WorkNumber.Number == submission.Model.WorkNumber.Number)?.WorkflowInstance
                                .FirstOrDefault(w => w.InstanceStatus == Bdo.Work.InstanceStatus.Outstanding)?.WorkflowInstanceId;
                    }
                    if (submission.Model.AdditionalAgencyWorkNumbers.Any())
                    {
                        var sub = submission.Model.Copy();

                        foreach (var worknum in submission.Model.AdditionalAgencyWorkNumbers)
                        {
                            sub.Agency = worknum.WorkNumber.Type;
                            sub.WorkNumber = worknum.WorkNumber;

                            await auditRequestContainer.UpsertItemAsync(new AuditRequestModel
                            {
                                AuditId = submission.AuditId,
                                AuditRequestId = worknum.AuditRequestId,
                                PartitionKey = $"{worknum.WorkNumber.Type}{worknum.WorkNumber.Number}",
                                RecordId = submission.SubmissionId,
                                AgencyCode = worknum.WorkNumber.Type,
                                IsProcessingFinished = !submission.ToBeProcessed,
                                IsProcessingError = submission.Rejection != null,
                                ProcessingCompletionDate = now,
                                CreatedDate = now,
                                RulesApplied = submission.RulesApplied,
                                TransactionType = submission.TransactionType,
                                TransactionError = submission.Rejection,
                                RequestSource = submission.RequestSource,
                                AgentVersion = submission.AgentVersion,
                                WorkIdAfter = workId,
                                Work = sub,
                                IswcStatus = submission.IswcModel.IswcStatusId == null ? "" : ((Bdo.Iswc.IswcStatus)submission.IswcModel.IswcStatusId).ToString(),
                                WorkflowInstanceId = workflowInstanceId,
                                TransactionSource = transactionSource,
                                IsEligible = worknum.IsEligible,
                                RelatedSubmissionIncludedIswc = submission.Model.RelatedSubmissionIncludedIswc
                            }); ;
                        }
                    }
                    else
                    {
                        await auditRequestContainer.UpsertItemAsync(new AuditRequestModel
                        {
                            AuditId = submission.AuditId,
                            AuditRequestId = submission.AuditRequestId,
                            PartitionKey = GetPartitionKey(submission),
                            RecordId = submission.SubmissionId,
                            AgencyCode = submission.Model.Agency,
                            IsProcessingFinished = !submission.ToBeProcessed,
                            IsProcessingError = submission.Rejection != null,
                            ProcessingCompletionDate = now,
                            CreatedDate = now,
                            RulesApplied = submission.RulesApplied,
                            TransactionType = submission.TransactionType,
                            TransactionError = submission.Rejection,
                            RequestSource = submission.RequestSource,
                            AgentVersion = submission.AgentVersion,
                            WorkIdAfter = workId,
                            Work = submission.Model,
                            IswcStatus = submission.IswcModel.IswcStatusId == null ? "" : ((Bdo.Iswc.IswcStatus)submission.IswcModel.IswcStatusId).ToString(),
                            UpdateAllocatedIswc = submission.UpdateAllocatedIswc,
                            WorkNumberToReplaceIasWorkNumber = submission.WorkNumberToReplaceIasWorkNumber,
                            WorkflowInstanceId = workflowInstanceId,
                            MultipleAgencyWorkCodes = submission.MultipleAgencyWorkCodes,
                            MultipleAgencyWorkCodesChild = submission.MultipleAgencyWorkCodesChild,
                            TransactionSource = transactionSource,
                            IsEligible = submission.IsEligible,
                            RelatedSubmissionIncludedIswc = submission.Model.RelatedSubmissionIncludedIswc

                        });
                    }
                    if (submission.IsProcessed && submission.Rejection == null)
                    {
                        var submittedPreferredIswc = submission.Model.PreferredIswc;
                        if (submission.TransactionType == TransactionType.MER)
                        {
                            foreach (var iswc in submission.Model.IswcsToMerge)
                            {
                                var merSubmission = mapper.Map<Submission>(submission);
                                merSubmission.Model.PreferredIswc = iswc;

                                merSubmission.Model.WorkNumber.Number = (await workRepository.FindManyAsyncOptimizedByPath(x => x.AgencyId == submission.Model.Agency
                                && x.Iswc.Iswc1 == iswc, "Iswc"))?.OrderByDescending(x => x.LastModifiedDate)?.FirstOrDefault()?.AgencyWorkCode;

                                var mergeWorkflowInstanceId = (await iswcLinkedToRepository.FindManyAsyncOptimizedByPath(
                                    x => x.Iswc.Iswc1 == iswc && x.LinkedToIswc == submission.IswcModel.Iswc && x.Status,
                                    "Iswc", "MergeRequestNavigation", "MergeRequestNavigation.WorkflowInstance"))?.OrderByDescending(x => x.CreatedDate)?.FirstOrDefault()?
                                    .MergeRequestNavigation?.WorkflowInstance?.FirstOrDefault()?.WorkflowInstanceId;

                                await auditRequestContainer.UpsertItemAsync(new AuditRequestModel
                                {
                                    AuditId = submission.AuditId,
                                    AuditRequestId = Guid.NewGuid(),
                                    PartitionKey = iswc,
                                    RecordId = submission.SubmissionId,
                                    AgencyCode = submission.Model.Agency,
                                    IsProcessingFinished = !submission.ToBeProcessed,
                                    IsProcessingError = submission.Rejection != null,
                                    ProcessingCompletionDate = now,
                                    CreatedDate = now,
                                    RulesApplied = submission.RulesApplied,
                                    TransactionType = submission.TransactionType,
                                    TransactionError = submission.Rejection,
                                    WorkIdAfter = null,
                                    Work = merSubmission.Model,
                                    WorkflowInstanceId = mergeWorkflowInstanceId,
                                    TransactionSource = transactionSource,
                                    IsEligible = submission.IsEligible
                                });
                            }
                            foreach (var number in submission.Model.WorkNumbersToMerge)
                            {
                                var merSubmission = mapper.Map<Submission>(submission);
                                long? mergeWorkflowInstanceId = null;

                                var childIswc = (await workRepository.FindAsyncOptimizedByPath(
                                    x => x.AgencyWorkCode == number.Number && x.AgencyId == number.Type, "Iswc"))?.Iswc;

                                if (childIswc != null)
                                    mergeWorkflowInstanceId = (await iswcLinkedToRepository.FindManyAsyncOptimizedByPath(
                                        x => x.IswcId == childIswc.IswcId && x.LinkedToIswc == submission.IswcModel.Iswc && x.Status,
                                            "Iswc", "MergeRequestNavigation", "MergeRequestNavigation.WorkflowInstance"))?.OrderByDescending(x => x.CreatedDate)?.FirstOrDefault()?
                                            .MergeRequestNavigation?.WorkflowInstance?.FirstOrDefault()?.WorkflowInstanceId;

                                if (childIswc != null)
                                    merSubmission.Model.PreferredIswc = childIswc.Iswc1;
                                else
                                    merSubmission.Model.PreferredIswc = null;

                                merSubmission.Model.WorkNumber = number;

                                await auditRequestContainer.UpsertItemAsync(new AuditRequestModel
                                {
                                    AuditId = submission.AuditId,
                                    AuditRequestId = Guid.NewGuid(),
                                    PartitionKey = GetPartitionKey(merSubmission),
                                    RecordId = submission.SubmissionId,
                                    AgencyCode = submission.Model.Agency,
                                    IsProcessingFinished = !submission.ToBeProcessed,
                                    IsProcessingError = submission.Rejection != null,
                                    ProcessingCompletionDate = now,
                                    CreatedDate = now,
                                    RulesApplied = submission.RulesApplied,
                                    TransactionType = submission.TransactionType,
                                    TransactionError = submission.Rejection,
                                    WorkIdAfter = null,
                                    Work = merSubmission.Model,
                                    WorkflowInstanceId = mergeWorkflowInstanceId,
                                    TransactionSource = transactionSource,
                                    IsEligible = submission.IsEligible
                                });
                            }
                        }
                        else if (submission.TransactionType == TransactionType.DMR)
                        {
                            var dmrSubmission = submission.DeepCopy();
                            dmrSubmission.Model.PreferredIswc = null;

                            if (!string.IsNullOrWhiteSpace(dmrSubmission.Model.WorkNumber.Number) && !string.IsNullOrWhiteSpace(dmrSubmission.Model.WorkNumber.Type))
                            {
                                var demergedParent = (await workRepository.FindAsyncOptimizedByPath(
                                        x => x.AgencyWorkCode == dmrSubmission.Model.WorkNumber.Number && x.AgencyId == dmrSubmission.Model.WorkNumber.Type && x.Status, "Iswc"))?.Iswc;

                                if (demergedParent != null)
                                    dmrSubmission.Model.PreferredIswc = demergedParent.Iswc1;
                            }

                            await auditRequestContainer.UpsertItemAsync(new AuditRequestModel
                            {
                                AuditId = submission.AuditId,
                                AuditRequestId = Guid.NewGuid(),
                                PartitionKey = GetPartitionKey(dmrSubmission),
                                RecordId = submission.SubmissionId,
                                AgencyCode = submission.Model.Agency,
                                IsProcessingFinished = !submission.ToBeProcessed,
                                IsProcessingError = submission.Rejection != null,
                                ProcessingCompletionDate = now,
                                CreatedDate = now,
                                RulesApplied = submission.RulesApplied,
                                TransactionType = submission.TransactionType,
                                TransactionError = submission.Rejection,
                                WorkIdAfter = null,
                                Work = dmrSubmission.Model,
                                WorkflowInstanceId = workflowInstanceId,
                                TransactionSource = transactionSource,
                                IsEligible = submission.IsEligible
                            });
                        }

                        submission.Model.PreferredIswc = submittedPreferredIswc;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, $"Failed to log submissions in {nameof(CosmosDbAuditService)} - {nameof(LogSubmissions)}");
                throw;
            }

            static TransactionSource GetTransactionSource(RequestType requestType)
            {
                return requestType switch
                {
                    RequestType.Agency => TransactionSource.Agency,
                    RequestType.Label => TransactionSource.Label,
                    RequestType.Publisher => TransactionSource.Publisher,
                    RequestType.ThirdParty => TransactionSource.ThirdParty,
                    _ => TransactionSource.Agency
                };
            }

            static string GetPartitionKey(Submission submission)
            {
                var model = submission.Model;

                if (!string.IsNullOrWhiteSpace(model.PreferredIswc)
                    && new TransactionType[] { TransactionType.MER, TransactionType.DMR, TransactionType.CDR }.Contains(submission.TransactionType))
                    return $"{model.PreferredIswc}";

                if (!string.IsNullOrWhiteSpace(model.WorkNumber.Number))
                    if(submission.UpdateAllocatedIswc)
                        return $"{submission.AgencyToReplaceIasAgency}{model.WorkNumber.Number}";
                    else
                    return $"{model.Agency}{model.WorkNumber.Number}";

                return $"{model.Agency}";
            }
        }

        public async Task<IEnumerable<AuditHistoryResult>> Search(string preferredIswc, IEnumerable<WorkInfo> submissions)
        {
            IEnumerable<AuditHistoryResult> results = new List<AuditHistoryResult>();

            foreach (var submission in submissions)
            {
                var agencyId = submission.AgencyId;

                results = results.Union(mapper.Map<IEnumerable<AuditHistoryResult>>(await auditRequestContainer
                    .GetItemsAsync(x =>
                        x.Work.PreferredIswc == preferredIswc
                        && x.Work.WorkNumber.Number == submission.AgencyWorkCode || x.Work.AdditionalAgencyWorkNumbers.Any(x => x.WorkNumber.Number == submission.AgencyWorkCode)
                        && x.Work.Agency == agencyId
                        && x.TransactionError == null
                        && x.TransactionType != TransactionType.FSQ,
                        $"{agencyId}{submission.AgencyWorkCode}")));
            }

            if (results.Any(x => x.UpdateAllocatedIswc))
            {
                var allocatedSub = (await auditRequestContainer
                    .GetItemsAsync(x => x.Work.PreferredIswc == preferredIswc && x.TransactionError == null && x.TransactionType == TransactionType.CAR && x.Work.AdditionalIdentifiers.Any(), $""));

                if (allocatedSub != null)
                {
                    results.Select(x => { x.SubmittingAgency = submissions.FirstOrDefault(y => y.AgencyWorkCode == x.WorkNumber)?.AgencyId; return x; }).ToList();
                    results = results.Union(mapper.Map<IEnumerable<AuditHistoryResult>>(allocatedSub));
                }
            }

            results = results.Union(mapper.Map<IEnumerable<AuditHistoryResult>>(await auditRequestContainer
                    .GetItemsAsync(x => x.Work.PreferredIswc == preferredIswc && x.TransactionError == null && x.TransactionType != TransactionType.FSQ, $"{preferredIswc}")));

            return mapper.Map<IEnumerable<AuditHistoryResult>>(results);
        }

        public async Task<IEnumerable<AuditReportResult>> Search(AuditReportSearchParameters searchParameters)
        {
            IEnumerable<AuditReportResult> results = new List<AuditReportResult>();

            var predicate = GetPredicate();

            results = results.Union(mapper.Map<List<AuditReportResult>>(await auditRequestContainer.GetItemsAsync(predicate, 1000)));

            return mapper.Map<IEnumerable<AuditReportResult>>(results);


            ExpressionStarter<AuditRequestModel> GetPredicate()
            {
                var predicate = PredicateBuilder.New<AuditRequestModel>();

                predicate.Start(x => (x.CreatedDate >= searchParameters.FromDate && x.CreatedDate < searchParameters.ToDate.AddDays(1)));

                if (searchParameters.Report.Equals(ReportType.SubmissionAudit))
                {
                    if (!searchParameters.AgencyName.Contains("All"))
                        predicate.And(x => x.AgencyCode == searchParameters.AgencyName);

                    if (!string.IsNullOrWhiteSpace(searchParameters.AgencyWorkCode))
                        predicate.And(x => x.Work.WorkNumber.Number == searchParameters.AgencyWorkCode);

                    if (searchParameters.Status != StatusType.All)
                        predicate.And(x => x.IsProcessingError == (searchParameters.Status == StatusType.Error ? true : false));

                    if (searchParameters.TransactionSource != TransactionSource.All)
                    {
                        if (searchParameters.TransactionSource == TransactionSource.Publisher)
                            predicate.And(x => x.TransactionSource == TransactionSource.Publisher || x.Work.AdditionalIdentifiers.Any());

                        else
                            predicate.And(x => x.TransactionSource == TransactionSource.Agency || !x.Work.AdditionalIdentifiers.Any());
                    }

                    predicate.And(x => (x.IsProcessingError && x.IsProcessingFinished && x.TransactionError != null)
                    || (!x.IsProcessingError && x.IsProcessingFinished && x.TransactionError == null));
                }

                return predicate;
            }
        }

        public async Task<IEnumerable<FileAuditReportResult>> SearchFileAudit(AuditReportSearchParameters searchParameters)
        {
            var predicate = GetPredicate();

            return mapper.Map<IEnumerable<FileAuditReportResult>>(await fileAuditContainer
                .GetItemsAsync(predicate, 1000));

            ExpressionStarter<FileAuditModel> GetPredicate()
            {
                var predicate = PredicateBuilder.New<FileAuditModel>();

                predicate.Start(x => (x.DatePickedUp >= searchParameters.FromDate && x.DatePickedUp < searchParameters.ToDate.AddDays(1)));

                if (searchParameters.Report.Equals(ReportType.FileSubmissionAudit))
                {
                    if (!searchParameters.AgencyName.Contains("All"))
                        predicate.And(x => x.AgencyCode == searchParameters.AgencyName);

                    if (searchParameters.TransactionSource != TransactionSource.All)
                    {
                        if (searchParameters.TransactionSource == TransactionSource.Publisher)
                            predicate.And(x => x.SubmittingPublisherIPNameNumber != null);

                        else
                            predicate.And(x => x.SubmittingPublisherIPNameNumber == null);
                    }
                }

                return predicate;
            }
        }

        public async Task<IEnumerable<AgencyStatisticsResult>> GetAgencyStatistics(AgencyStatisticsSearchParameters searchParameters)
        {
            if (searchParameters.TimePeriod == TimePeriod.Year)
            {
                var results = new List<AgencyStatisticsResult>();
                for (int i = 1; i <= 12; i++)
                {
                    results.AddRange(await GetStatisticsPerMonth(i));
                }
                return results;
            }
            else
            {
                return await GetStatisticsPerMonth(searchParameters.Month);
            }

            async Task<IEnumerable<AgencyStatisticsResult>> GetStatisticsPerMonth(int? month)
            {
                var agency = searchParameters.AgencyName;
                var year = searchParameters.Year;

                var predicate = PredicateBuilder.New<AgencyStatisticsModel>(x => x.Year == year && x.Month == month);
                string partitionKey = null;

                if (searchParameters.AgencyName != "All")
                    predicate.And(x => x.AgencyCode == searchParameters.AgencyName);

                if (searchParameters.TransactionSource != TransactionSource.All)
                    predicate.And(x => x.TransactionSource == searchParameters.TransactionSource);

                if (searchParameters.AgencyName != "All" && searchParameters.TransactionSource != TransactionSource.All)
                {
                    var transActionSource = searchParameters.TransactionSource.ToFriendlyString().ToLower();
                    predicate.And(x => x.AgencyCode == searchParameters.AgencyName);
                    partitionKey = $"{agency}_{month}_{year}_{transActionSource}";
                }
                return mapper.Map<List<AgencyStatisticsResult>>(
                await agencyStatisticsContainer.GetItemsAsync(predicate, partitionKey));
            }
        }

        public async Task GenerateAgencyStatistics(HighWatermark highWatermark)
        {
            var feedIterator = auditRequestContainer.GetChangeFeedIterator(highWatermark.Value);
            var latestChanges = new List<AuditRequestModel>();

            while (feedIterator.HasMoreResults)
            {
                foreach (var auditRequest in await feedIterator.ReadNextAsync())
                {
                    latestChanges.Add(auditRequest);
                }

                if (latestChanges.Count > 50_000 || !feedIterator.HasMoreResults)
                {
                    if (!latestChanges.Any())
                        continue;

                    var changes = latestChanges
                         .Where(x => !string.IsNullOrWhiteSpace(x.AgencyCode))
                         .GroupBy(x => x.AgencyCode).ToDictionary(x => x.Key, x => x.ToList());

                    foreach (var agency in changes.Keys)
                    {

                        var agencyChanges = changes.Where(x => x.Key == agency).SelectMany(y => y.Value);

                        if (!agencyChanges.Any()) continue;

                        foreach (var agencyChangesByTransSource in agencyChanges.GroupBy(x => x.TransactionSource))
                        {
                            foreach (var monthlyAgencyChanges in agencyChangesByTransSource.GroupBy(x => x.CreatedDate.Day))
                            {
                                var statistics = new AgencyStatisticsModel(monthlyAgencyChanges);
                                var transactionSource = monthlyAgencyChanges.FirstOrDefault().TransactionSource.ToFriendlyString().ToLower();

                                var existingStatistics = (await agencyStatisticsContainer
                                    .GetItemsAsync(x => x.Day == statistics.Day, $"{agency}_{statistics.Month}_{statistics.Year}_{transactionSource}"))
                                    .FirstOrDefault();

                                if (existingStatistics != null)
                                {
                                    existingStatistics = UpdateExistingAgencyStats(existingStatistics);
                                    await agencyStatisticsContainer.UpdateItemAsync(existingStatistics.ID.ToString(), existingStatistics);
                                }
                                else
                                    await agencyStatisticsContainer.UpsertItemAsync(statistics);

                                AgencyStatisticsModel UpdateExistingAgencyStats(AgencyStatisticsModel existing)
                                {
                                    existing.InEligibleIswcSubmissions += statistics.InEligibleIswcSubmissionsWithDisambiguation;
                                    existing.InEligibleIswcSubmissionsWithDisambiguation += statistics.InEligibleIswcSubmissionsWithDisambiguation;
                                    existing.ValidSubmissions += statistics.ValidSubmissions;
                                    existing.InvalidSubmissions += statistics.InvalidSubmissions;
                                    existing.EligibleIswcSubmissions += statistics.EligibleIswcSubmissions;
                                    existing.EligibleIswcSubmissionsWithDisambiguation += statistics.EligibleIswcSubmissionsWithDisambiguation;
                                    existing.WorkflowTasksAssignedPending += 1;
                                    return existing;
                                }
                            }
                        }

                    }

                    await UpdateHighWatermark(highWatermark, latestChanges.Last().CreatedDate);
                    latestChanges.Clear();
                }
            }

            async Task UpdateHighWatermark(HighWatermark highWatermark, DateTime dateTime)
            {
                highWatermark.Value = dateTime;
                await highWatermarkRepository.UpdateAsync(highWatermark);
                await highWatermarkRepository.UnitOfWork.Save();
            }
        }

        public async Task GenerateWorkflowStatistics()
        {
            var iterator = agencyStatisticsContainer.GetItemsFeedIterator(x => x.WorkflowTasksAssignedPending > 0);

            while (iterator.HasMoreResults)
            {
                var pendingStats = await iterator.ReadNextAsync();

                var relevantAgencies = pendingStats.Select(x => x.AgencyCode).Distinct();
                var relevantMonths = pendingStats.Select(x => x.Month).Distinct();
                var relevantYears = pendingStats.Select(x => x.Year).Distinct();

                var workflows = await workflowInstanceRepository
                    .FindManyAsyncOptimizedByPath(x => x.WorkflowTask.Any(x =>
                    relevantAgencies.Contains(x.AssignedAgency.AgencyId))
                    && relevantMonths.Contains(x.CreatedDate.Month) && relevantYears.Contains(x.CreatedDate.Year)
                    && !x.IsDeleted, $"{nameof(DataModels.WorkflowTask)}");

                foreach (var stat in pendingStats)
                {
                    var workflowTasks = workflows
                        .Where(x => x.CreatedDate.Day == stat.Day && x.CreatedDate.Month == stat.Month && x.CreatedDate.Year == stat.Year)
                        .SelectMany(x => x.WorkflowTask.Where(y => y.AssignedAgencyId == stat.AgencyCode))
                        .GroupBy(x => x.TaskStatus)
                        .ToDictionary(x => (InstanceStatus)x.Key, y => y.Count());

                    stat.WorkflowTasksAssigned = workflowTasks.Values.Sum();
                    stat.WorkflowTasksAssignedPending = workflowTasks.TryGetValue(InstanceStatus.Outstanding, out var outstanding) ? outstanding : default;
                    stat.WorkflowTasksAssignedApproved = workflowTasks.TryGetValue(InstanceStatus.Approved, out var approved) ? approved : default;
                    stat.WorkflowTasksAssignedRejected = workflowTasks.TryGetValue(InstanceStatus.Rejected, out var rejected) ? rejected : default;

                    await agencyStatisticsContainer.UpdateItemAsync(stat.ID.ToString(), stat);
                }
            }
        }
    }
}
