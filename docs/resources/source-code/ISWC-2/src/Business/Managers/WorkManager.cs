using AutoMapper;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.Repositories;
using SpanishPoint.Azure.Iswc.Data.Services.CacheIswcs;
using SpanishPoint.Azure.Iswc.Data.Services.Checksum;
using SpanishPoint.Azure.Iswc.Data.Services.Checksum.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Data.Services.IswcService;
using SpanishPoint.Azure.Iswc.Data.Services.Notifications;
using SpanishPoint.Azure.Iswc.Data.Services.Notifications.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Data.Services.Search;
using SpanishPoint.Azure.Iswc.Data.Services.UpdateWorkflowHistory;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using SpanishPoint.Azure.Iswc.Framework.Http.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WorkflowInstance = SpanishPoint.Azure.Iswc.Data.DataModels.WorkflowInstance;

namespace SpanishPoint.Azure.Iswc.Business.Managers
{
    public interface IWorkManager
    {
        Task<bool> Exists(WorkNumber workNumber);
        Task<bool> Exists(string preferredIswc);
        Task<bool> ExistsWithDisambiguation(string preferredIswc, WorkNumber workNumber);
        Task<bool> Exists(string preferredIswc, WorkNumber workNumber);
        Task<bool> IsDeleted(WorkNumber workNumber);
        Task<SubmissionModel> FindAsync(WorkNumber workNumber);
        Task<SubmissionModel> FindAsync(Data.DataModels.AdditionalIdentifier additionalIdentifier);
        Task<SubmissionModel> FindWorkForValidationAsync(WorkNumber workNumber);
        Task<SubmissionModel> FindAsync(string preferredIswc, bool iswcEligibleOnly = false, bool excludeLabelSubmissions = false);
        Task<SubmissionModel> FindAllocatedWorkAsync(string preferredIswc);

        Task<bool> CheckIfArchivedIswcAsync(string preferredIswc);
        Task<IEnumerable<SubmissionModel>> FindManyAsync(IEnumerable<string> preferredIswc, bool iswcEligibleOnly = false, DetailLevel detailLevel = DetailLevel.Full);
        Task<IEnumerable<SubmissionModel>> FindManyAsync(IEnumerable<Data.DataModels.AdditionalIdentifier> additionalIdentifiers, bool iswcEligibleOnly = false, DetailLevel detailLevel = DetailLevel.Full);
        Task<IEnumerable<SubmissionModel>> FindManyAsync(IEnumerable<WorkNumber> workNumbers);
        Task<IEnumerable<SubmissionModel>> FindManyWorksAsync(IEnumerable<long> workInfoIds, bool excludeDeletedWorks = true, bool excludeRelaceWorks = true, bool iswcEligibleOnly = false, DetailLevel detailLevel = DetailLevel.Full);
        Task<IEnumerable<WorkInfo>> FindManyWorkInfosAsync(IEnumerable<WorkNumber> workNumbers);
        Task<long> AddWorkInfoAsync(Submission submission, bool getNewIswc);
        Task<IEnumerable<IswcModel>> FindManyAsync(IEnumerable<long> workInfoIds, bool readOnly = false, DetailLevel detailLevel = DetailLevel.Full);
        Task<IswcModel> FindAsync(long inputIswcId);
        Task<IswcModel> FindIswcModelAsync(WorkNumber workNumber, bool readOnly = false, DetailLevel detailLevel = DetailLevel.Full);
        Task<IswcModel> FindIswcModelAsync(string preferredIswc, bool readOnly = false, DetailLevel detailLevel = DetailLevel.Full);
        Task<IswcModel> FindArchivedIswcModelAsync(string archivedIswc, bool readOnly = false, DetailLevel detailLevel = DetailLevel.Full);
        Task<IEnumerable<IswcModel>> FindManyIswcModelsAsync(IEnumerable<string> preferredIswcs, bool readOnly = false, DetailLevel detailLevel = DetailLevel.Full);
        Task AddIswcLinkedTo(Submission submission);
        Task DeleteIswcLinkedTo(Submission submission);
        Task DeleteAsync(Submission submission);
        Task<VerifiedSubmissionModel> FindVerifiedAsync(WorkNumber workNumber);
        Task<long> UpdateAsync(Submission submission, bool isRollbackUpdate = false);
        Task<VerifiedSubmissionModel> FindVerifiedAsync(string preferredIswc, string agency);
        Task<(WorkflowInstance workflowInstance, IList<CsnNotifications> csnRecords)> AddUpdateApprovalWorkflow(long workinfoId, long iswcId, string submitterAgency);
        Task AddSubmissionAsync(Submission submission);
        Task UpdateLabelSubmissionsAsync(Submission submission);
        Task<bool> HasOtherEligibleWorks(string preferredIswc, string agencyId, bool excludeLabelSubmissions = false);
        Task<bool> AllExistAsync(ICollection<WorkNumber> workNumbers);
        Task<bool> AnyExistAsync(ICollection<WorkNumber> workNumbers);
        Task<SubmissionChecksum> GetChecksum(string agency, string agencyWorkCode);
        Task UpsertChecksum(string agency, string agencyWorkCode, string hash);
        Task<IswcModel?> GetCacheIswcs(string iswc);
    }

    public class WorkManager : IWorkManager
    {
        private readonly IWorkRepository workRepository;
        private readonly IIswcRepository iswcRepository;
        private readonly IMapper mapper;
        private readonly IIswcService iswcService;
        private readonly IInstrumentationRepository instrumentationRepository;
        private readonly IMergeRequestRepository mergeRequestRepository;
        private readonly ISearchService searchService;
        private readonly INotificationService notificationService;
        private readonly IWorkflowRepository workflowRepository;
        private readonly IUpdateWorkflowHistoryService updateWorkflowsHistoryService;
        private readonly IPerformerRepository performerRepository;
        private readonly IPublisherCodeRespository publisherCodeRespository;
        private readonly IRecordingRepository recordingRepository;
        private readonly INumberTypeRepository numberTypeRepository;
        private readonly IHttpContextAccessor httpContext;
        private readonly IIswcLinkedToRepository iswcLinkedToRepository;
        private readonly IChecksumService checksumService;
        private readonly ICacheIswcService cacheIswcService;

        public WorkManager(IWorkRepository workRepository,
            IIswcRepository iswcRepository,
            IMapper mapper,
            IIswcService iswcService,
            IInstrumentationRepository instrumentationRepository,
            IMergeRequestRepository mergeRequestRepository,
            ISearchService searchService,
            INotificationService notificationService,
            IWorkflowRepository workflowRepository,
            IUpdateWorkflowHistoryService updateWorkflowsHistoryService,
            IPerformerRepository performerRepository,
            IPublisherCodeRespository publisherCodeRespository,
            IRecordingRepository recordingRepository,
            INumberTypeRepository numberTypeRepository,
            IHttpContextAccessor httpContext,
            IIswcLinkedToRepository iswcLinkedToRepository,
            IChecksumService checksumService,
            ICacheIswcService cacheIswcService)
        {
            this.workRepository = workRepository;
            this.iswcRepository = iswcRepository;
            this.mapper = mapper;
            this.iswcService = iswcService;
            this.instrumentationRepository = instrumentationRepository;
            this.mergeRequestRepository = mergeRequestRepository;
            this.searchService = searchService;
            this.notificationService = notificationService;
            this.workflowRepository = workflowRepository;
            this.updateWorkflowsHistoryService = updateWorkflowsHistoryService;
            this.performerRepository = performerRepository;
            this.publisherCodeRespository = publisherCodeRespository;
            this.recordingRepository = recordingRepository;
            this.numberTypeRepository = numberTypeRepository;
            this.httpContext = httpContext;
            this.iswcLinkedToRepository = iswcLinkedToRepository;
            this.checksumService = checksumService;
            this.cacheIswcService = cacheIswcService;
        }

        public async Task<bool> Exists(WorkNumber workNumber) =>
            await workRepository.ExistsAsync(x => x.AgencyId == workNumber.Type && x.AgencyWorkCode == workNumber.Number && x.Status);

        public async Task<bool> Exists(string preferredIswc) =>
            await workRepository.ExistsAsync(x => x.Iswc.Iswc1 == preferredIswc && x.Iswc.Status && x.Status);

        public async Task<bool> ExistsWithDisambiguation(string preferredIswc, WorkNumber workNumber) =>
            await workRepository.ExistsAsync(x => x.Iswc.Iswc1 == preferredIswc && x.Iswc.Status && x.Disambiguation == true && !x.IsReplaced
            && (x.IswcEligible || x.AgencyWorkCode == workNumber.Number));

        public async Task<bool> Exists(string preferredIswc, WorkNumber workNumber) =>
            await workRepository.ExistsAsync(
                x => x.Iswc.Iswc1 == preferredIswc && x.AgencyId == workNumber.Type && x.AgencyWorkCode == workNumber.Number && x.Status);

        public async Task<bool> IsDeleted(WorkNumber workNumber) =>
            await workRepository.ExistsAsync(x => x.AgencyId == workNumber.Type && x.AgencyWorkCode == workNumber.Number && !x.Status);


        public async Task<SubmissionModel> FindAsync(WorkNumber workNumber) =>
            mapper.Map<SubmissionModel>(await workRepository.FindAsyncOptimized(
                x => x.AgencyId == workNumber.Type && x.AgencyWorkCode == workNumber.Number && x.Status && !x.IsReplaced, creatorOnly: false));

        public async Task<SubmissionModel> FindAsync(Data.DataModels.AdditionalIdentifier additionalIdentifier) =>
           mapper.Map<SubmissionModel>(await workRepository.FindAsyncOptimized(
               x => x.WorkInfoId == additionalIdentifier.WorkInfoId && x.Status && !x.IsReplaced, creatorOnly: false));

        public async Task<SubmissionModel> FindWorkForValidationAsync(WorkNumber workNumber) =>
            mapper.Map<SubmissionModel>(await workRepository.FindAsyncOptimized(
                x => x.AgencyId == workNumber.Type && x.AgencyWorkCode == workNumber.Number && !x.IsReplaced, creatorOnly: false));

        public async Task<VerifiedSubmissionModel> FindVerifiedAsync(WorkNumber workNumber) =>
            await FindVerifiedAsync(x => x.AgencyId == workNumber.Type && x.AgencyWorkCode == workNumber.Number && x.Status && !x.IsReplaced);


        public async Task<SubmissionModel> FindAllocatedWorkAsync(string preferredIswc) =>
           await FindIasWorkinfoAsync(x => x.Iswc.Iswc1 == preferredIswc && x.AdditionalIdentifier.Any() && x.Status && !x.IsReplaced);

        public async Task<VerifiedSubmissionModel> FindVerifiedAsync(string preferredIswc, string agency) =>
            await FindVerifiedAsync(x => x.AgencyId == agency && x.Iswc.Iswc1 == preferredIswc);

        private async Task<VerifiedSubmissionModel> FindVerifiedAsync(Expression<Func<WorkInfo, bool>> predicate) =>
            mapper.Map<VerifiedSubmissionModel>(
                await workRepository.FindAsyncOptimizedByPath(predicate,
                    "Title", "Title.TitleType", "Agency", "Iswc", "LastModifiedUser", "Publisher", "Publisher.IpnameNumberNavigation",
                    "Creator", "Creator.IpnameNumberNavigation", "Creator.IpnameNumberNavigation.NameReference", "AdditionalIdentifier"));

        private async Task<SubmissionModel> FindIasWorkinfoAsync(Expression<Func<WorkInfo, bool>> predicate) =>
         mapper.Map<SubmissionModel>(await workRepository.FindAsyncOptimizedByPath(predicate,
             "Title", "Title.TitleType", "Agency", "Iswc", "LastModifiedUser", "Publisher", "Publisher.IpnameNumberNavigation",
             "Creator", "Creator.IpnameNumberNavigation", "Creator.IpnameNumberNavigation.NameReference", "AdditionalIdentifier"));




        public async Task<SubmissionModel> FindAsync(string preferredIswc, bool iswcEligibleOnly = false, bool excludeLabelSubmissions = false) =>
            mapper.Map<SubmissionModel>(await workRepository.FindAsyncOptimized(
                x => x.Iswc.Iswc1 == preferredIswc && x.Iswc.Status && (!iswcEligibleOnly || x.IswcEligible) && (!excludeLabelSubmissions || !x.AgencyWorkCode.StartsWith("PRS_")), creatorOnly: true));

        public async Task<bool> CheckIfArchivedIswcAsync(string preferredIswc) =>
            !await Exists(preferredIswc) && await workRepository.IsArchivedIswc(preferredIswc);

        public async Task<IEnumerable<SubmissionModel>> FindManyAsync(IEnumerable<string> preferredIswcs, bool iswcEligibleOnly = false, DetailLevel detailLevel = DetailLevel.Full) =>
            mapper.Map<IEnumerable<SubmissionModel>>(
                await workRepository.FindManyAsyncOptimizedByPath(
                    x => preferredIswcs.Contains(x.Iswc.Iswc1) && (!iswcEligibleOnly || x.IswcEligible),
                    $"{nameof(Creator)}",
                    $"{nameof(Creator)}.IpnameNumberNavigation",
                    $"{nameof(Publisher)}",
                    $"{nameof(Publisher)}.IpnameNumberNavigation",
                    $"{nameof(Data.DataModels.Agency)}",
                    $"{nameof(Data.DataModels.Iswc)}"));

        public async Task<IEnumerable<SubmissionModel>> FindManyAsync(IEnumerable<Data.DataModels.AdditionalIdentifier> additionalIdentifiers, bool iswcEligibleOnly = false, DetailLevel detailLevel = DetailLevel.Full) =>
            mapper.Map<IEnumerable<SubmissionModel>>(
                await workRepository.FindManyAsyncOptimizedByPath(
                    x => additionalIdentifiers.Select(x => x.WorkInfoId).ToList().Contains(x.WorkInfoId) && x.Status && !x.IsReplaced && (!iswcEligibleOnly || x.IswcEligible),
                    $"{nameof(Creator)}",
                    $"{nameof(Creator)}.IpnameNumberNavigation",
                    $"{nameof(Publisher)}",
                    $"{nameof(Publisher)}.IpnameNumberNavigation",
                    $"{nameof(Data.DataModels.Agency)}",
                    $"{nameof(Data.DataModels.Iswc)}"));

        public async Task<IEnumerable<SubmissionModel>> FindManyWorksAsync(IEnumerable<long> workInfoIds, bool excludeDeletedWorks = true, bool excludeRelaceWorks = true, bool iswcEligibleOnly = false,  DetailLevel detailLevel = DetailLevel.Full) =>
            mapper.Map<IEnumerable<SubmissionModel>>(
                await workRepository.FindManyAsyncOptimizedByPath(
                    x => workInfoIds.ToList().Contains(x.WorkInfoId) && (!excludeDeletedWorks || x.Status) && (!excludeRelaceWorks || !x.IsReplaced) && (!iswcEligibleOnly || x.IswcEligible),
                    $"{nameof(Creator)}",
                    $"{nameof(Creator)}.IpnameNumberNavigation",
                    $"{nameof(Publisher)}",
                    $"{nameof(Publisher)}.IpnameNumberNavigation",
                    $"{nameof(Data.DataModels.Agency)}",
                    $"{nameof(Data.DataModels.Iswc)}"));

        public async Task<IEnumerable<SubmissionModel>> FindManyAsync(IEnumerable<WorkNumber> workNumbers) =>
            mapper.Map<IEnumerable<SubmissionModel>>(
                await workRepository.FindManyAsyncUnionAll(workNumbers));

        public async Task<IEnumerable<WorkInfo>> FindManyWorkInfosAsync(IEnumerable<WorkNumber> workNumbers) =>
            await workRepository.FindManyAsyncOptimizedByPath(
                x => workNumbers.Any(wn => wn.Number == x.AgencyWorkCode && wn.Type == x.AgencyId) && x.Status && !x.IsReplaced,
                "Title",
                "Creator",
                "Publisher",
                "WorkflowInstance",
                "WorkInfoPerformer",
                "WorkInfoPerformer.Performer",
                "DerivedFrom",
                "DisambiguationISWC");

        public async Task<bool> HasOtherEligibleWorks(string preferredIswc, string agencyId, bool excludeLabelSubmissions = false) =>
            await workRepository.ExistsAsync(x => x.IswcEligible && x.Iswc.Iswc1 == preferredIswc && x.AgencyId != agencyId && (!excludeLabelSubmissions || !x.AgencyWorkCode.StartsWith("PRS_")));

        public async Task<bool> AnyExistAsync(ICollection<WorkNumber> workNumbers) =>
            await workRepository.ExistAsync(workNumbers, false);

        public async Task<bool> AllExistAsync(ICollection<WorkNumber> workNumbers) =>
            await workRepository.ExistAsync(workNumbers, true);

        public async Task<long> AddWorkInfoAsync(Submission submission, bool getNewIswc)
        {
            var now = DateTime.UtcNow;
            var unitOfWork = workRepository.UnitOfWork;

            var work = new WorkInfo
            {
                LastModifiedDate = now,
                CreatedDate = now,
                CisnetCreatedDate = submission.Model.CisnetCreatedDate.HasValue ? submission.Model.CisnetCreatedDate.Value.UtcDateTime : now,
                CisnetLastModifiedDate = submission.Model.CisnetLastModifiedDate.HasValue ? submission.Model.CisnetLastModifiedDate.Value.UtcDateTime : now,
                LastModifiedUserId = GetRequestSourceUserId()
            };

            if (getNewIswc && submission.Model.AllowProvidedIswc)
                work.Iswc = await GetNewIswc(submission.Model.Iswc, submission.RequestType == RequestType.Label);
            else if (getNewIswc)
                work.Iswc = await GetNewIswc(null, submission.RequestType == RequestType.Label);
            else
            {
                work.IswcId = (await iswcRepository.FindAsync(i => i.Iswc1 == submission.Model.PreferredIswc && i.Status))?.IswcId ??
                    throw new Exception($"{submission.Model.PreferredIswc} could not be found.");
                work.Iswc = await iswcRepository.FindAsync(i => i.IswcId == work.IswcId);
                if (work.Iswc != null && work.Iswc.IswcStatusId == 2 && submission.RequestType != RequestType.Label)
                {
                    work.Iswc.IswcStatusId = 1;
                    foreach (var w in work.Iswc.WorkInfo.Where(x => x.Status && x.AgencyWorkCode.StartsWith("PRS_")))
                        w.LastModifiedDate = now;
                }                   
            }

            if (submission.Model.Instrumentation.Any())
                submission.Model.Instrumentation = await GetInstrumentationByCode(submission.Model.Instrumentation);

            if (submission.Model.AdditionalIdentifiers != null)
                submission.Model.AdditionalIdentifiers = await GetPublisherSubmitterInfo(submission.Model.AdditionalIdentifiers);

            if (submission.Model.InterestedParties.Any(x => x.CisacType == CisacInterestedPartyType.C && string.IsNullOrEmpty(x.IpBaseNumber))
                && submission.MatchedResult != null && submission.MatchedResult.Matches.Any())
            {
                var submissionWithIpData = submission.MatchedResult.Matches.FirstOrDefault(x => x.Contributors.Where(x => x.ContributorType == ContributorType.Creator).All(x => x.IpBaseNumber != null));

                if (submissionWithIpData != null)
                {
                    var updatedIps = new List<InterestedPartyModel>();

                    updatedIps.AddRange(submissionWithIpData.Contributors.Where(x => x.ContributorType == ContributorType.Creator).Select(x =>
                    {
                        x.CisacType = CisacInterestedPartyType.C;
                        x.Names = new List<NameModel>();
                        return x;
                    }));

                    foreach (var ip in submission.Model.InterestedParties)
                    {
                        if (ip.CisacType != CisacInterestedPartyType.C)
                            updatedIps.Add(ip);
                    }

                    submission.Model.InterestedParties = updatedIps;
                }
            }

            var mappedWork = mapper.Map(submission, work);

            if (submission.Model.Performers.Any())
                mappedWork.WorkInfoPerformer = await GetPerformers(mappedWork.WorkInfoPerformer);

            if (mappedWork.AdditionalIdentifier.Any())
            {
                for(var i = 0; i < mappedWork.AdditionalIdentifier.Count; i++)
                {
                    if (mappedWork.AdditionalIdentifier.ElementAt(i).Recording != null
                        && mappedWork.AdditionalIdentifier.ElementAt(i).Recording.RecordingArtist != null
                        && mappedWork.AdditionalIdentifier.ElementAt(i).Recording.RecordingArtist.Count() > 0)
                    {
                        mappedWork.AdditionalIdentifier.ElementAt(i).Recording.RecordingArtist = await GetRecordingArtists(mappedWork.AdditionalIdentifier.ElementAt(i).Recording.RecordingArtist);
                    }
                }
            }

            if (submission.Model.AllowProvidedIswc && !getNewIswc)
            {
                mappedWork.ArchivedIswc = submission.Model.Iswc;
            }

            if (getNewIswc)
            {
                if (mappedWork.Creator.Any())
                {
                    foreach (var creator in mappedWork.Creator)
                    {
                        creator.Iswc = work.Iswc;
                    }
                }

                if (mappedWork.Publisher.Any())
                {
                    foreach (var publisher in mappedWork.Publisher)
                    {
                        publisher.Iswc = work.Iswc;
                    }
                }

                if (mappedWork.Title.Any())
                {
                    foreach (var title in mappedWork.Title)
                    {
                        title.Iswc = work.Iswc;
                    }
                }
            }

            await notificationService.AddCsnNotifications(new List<CsnNotifications>() { new CsnNotifications(mappedWork, mappedWork.AgencyId, now, TransactionType.CAR, string.Empty) }, new WorkflowInstance());


            var result = await workRepository.AddAsync(mappedWork);

            await unitOfWork.Save();

            submission.Model.PreferredIswc = result.Iswc.Iswc1;
            await checksumService.AddChecksum(new SubmissionChecksums(submission.Model.WorkNumber.Type, submission.Model.WorkNumber.Number, submission.Model.HashCode));

            return result.WorkInfoId;

            async Task<Data.DataModels.Iswc> GetNewIswc(string? providedIswc = null, bool isLabel = false)
            {
                return new Data.DataModels.Iswc()
                {
                    Status = true,
                    LastModifiedDate = now,
                    CreatedDate = now,
                    AgencyId = submission.Model.Agency,
                    LastModifiedUserId = GetRequestSourceUserId(),
                    Iswc1 = string.IsNullOrEmpty(providedIswc) ? await GetNextIswc() : providedIswc,
                    IswcStatusId = isLabel ? 2 : 1
                };
            }
        }

        async Task<List<WorkInfo>> GetMappedWorkInfosToAdd(WorkInfo mappedWork, List<AdditionalAgencyWorkNumber>? numbers)
        {
            if (numbers == null) numbers = new List<AdditionalAgencyWorkNumber>();
            var unitOfWork = workRepository.UnitOfWork;
            var workInfos = new List<WorkInfo>();

            foreach (var number in numbers)
            {
                var workInfo = new WorkInfo { };
                workInfo.AgencyId = number.WorkNumber.Type;
                workInfo.AgencyWorkCode = number.WorkNumber.Number;
                workInfo.IswcEligible = number.IsEligible;
                workInfo.Creator = mappedWork?.Creator;
                workInfo.Publisher = mappedWork?.Publisher;
                workInfo.SourceDatabase = int.Parse(number.WorkNumber.Type);
                workInfo.WorkInfoPerformer = mappedWork?.WorkInfoPerformer;
                workInfo.WorkInfoInstrumentation = mappedWork?.WorkInfoInstrumentation;

                if (mappedWork?.Title != null)
                    foreach (var title in mappedWork.Title)
                    {
                        workInfo.Title.Add(new Data.DataModels.Title
                        {
                            LastModifiedDate = title.LastModifiedDate,
                            CreatedDate = title.CreatedDate,
                            Title1 = title.Title1,
                            TitleTypeId = title.TitleTypeId,
                            Status = true,
                            LastModifiedUserId = title.LastModifiedUserId,
                            StandardizedTitle = title.StandardizedTitle
                        });
                    }

                workInfo.Status = true;
                workInfo.ArchivedIswc = mappedWork?.ArchivedIswc;
                workInfo.Bvltr = mappedWork?.Bvltr;
                workInfo.CisnetCreatedDate = mappedWork?.CisnetCreatedDate;

                workInfo.LastModifiedDate = mappedWork?.LastModifiedDate ?? default;
                workInfo.CreatedDate = mappedWork?.CreatedDate ?? default;
                workInfo.DerivedFrom = mappedWork?.DerivedFrom;

                workInfo.DerivedWorkType = mappedWork?.DerivedWorkType;
                workInfo.Disambiguation = mappedWork?.Disambiguation;
                workInfo.DisambiguationIswc = mappedWork?.DisambiguationIswc;

                workInfo.DisambiguationReason = mappedWork?.DisambiguationReason;
                workInfo.Ipcount = mappedWork?.Ipcount ?? 0;
                workInfo.Iswc = mappedWork?.Iswc;
                workInfo.LastModifiedDate = mappedWork?.LastModifiedDate ?? default;

                workInfo.LastModifiedUserId = mappedWork?.LastModifiedUserId ?? GetRequestSourceUserId();

                workInfo.MatchType = mappedWork?.MatchType;
                workInfo.MwiCategory = mappedWork?.MwiCategory;

                workInfo.Creator?.Select(x => { x.WorkInfo = null; return x; }).ToList();
                workInfo.Title.Select(x => { x.WorkInfo = null; return x; }).ToList();

                if (mappedWork?.AdditionalIdentifier != null)
                    foreach (var ai in mappedWork.AdditionalIdentifier)
                        workInfo.AdditionalIdentifier.Add(ai);

                workInfos.Add(workInfo);
            }

            var copiedWorks = mapper.Map<List<WorkInfo>>(workInfos.AsQueryable());

            foreach (var cw in copiedWorks)
            {
                cw.Iswc = mappedWork?.Iswc;
                foreach (var c in cw.Creator)
                    c.Iswc = mappedWork?.Iswc;

                foreach (var p in cw.Publisher)
                    p.Iswc = mappedWork?.Iswc;

                foreach (var t in cw.Title)
                    t.Iswc = mappedWork?.Iswc;

            }

            await workRepository.AddRangeAsync(copiedWorks);
            await unitOfWork.Save();

            return copiedWorks;
        }
        private async Task<ICollection<WorkInfoPerformer>> GetPerformers(IEnumerable<WorkInfoPerformer> workInfoPerformers)
        {
            foreach (var workPerformer in workInfoPerformers)
            {
                var existingPerformer = await performerRepository.FindAsync(p => p.Status && p.FirstName == workPerformer.Performer.FirstName && p.LastName == workPerformer.Performer.LastName);

                if (existingPerformer == null)
                    continue;

                else
                {
                    workPerformer.PerformerId = existingPerformer.PerformerId;
                    workPerformer.Performer = existingPerformer;
                }
            }

            var workInfoPerformersList = new List<WorkInfoPerformer>() { };

            for (var x = 0; x < workInfoPerformers.Count(); x++)
            {
                var performer = workInfoPerformers.ElementAt(x);

                if (performer.PerformerId == 0)
                {
                    workInfoPerformersList.Add(performer);
                }
            }

            workInfoPerformersList.AddRange(workInfoPerformers.Where(a => a.PerformerId != 0).GroupBy(x => x.PerformerId).Select(y => y.First()).ToList());

            return workInfoPerformersList;
        }

        private async Task<ICollection<RecordingArtist>> GetRecordingArtists(IEnumerable<RecordingArtist> recordingArtists)
        {
            foreach (var recordingArtist in recordingArtists)
            {
                var existingPerformer = await performerRepository.FindAsync(p => p.Status && p.FirstName == recordingArtist.Performer.FirstName && p.LastName == recordingArtist.Performer.LastName);

                if (existingPerformer == null)
                    continue;

                else
                {
                    recordingArtist.PerformerId = existingPerformer.PerformerId;
                    recordingArtist.Performer = existingPerformer;
                }
            }

            var performersList = new List<RecordingArtist>() { };

            for (var x = 0; x < recordingArtists.Count(); x++)
            {
                var performer = recordingArtists.ElementAt(x);

                if (performer.PerformerId == 0)
                {
                    performersList.Add(performer);
                }
            }

            performersList.AddRange(recordingArtists.Where(a => a.PerformerId != 0).GroupBy(x => x.PerformerId).Select(y => y.First()).ToList());

            return performersList;
        }

        public async Task<IEnumerable<IswcModel>> FindManyAsync(IEnumerable<long> workInfoIds, bool readOnly = false, DetailLevel detailLevel = DetailLevel.Full) =>
            await GetIswcModels((await workRepository.FindManyAsync(x => workInfoIds.Contains(x.WorkInfoId))).Select(x => x.IswcId).Distinct().ToList(), readOnly, detailLevel);

        public async Task<IswcModel> FindAsync(long inputIswcId) =>
            (await GetIswcModels(new List<long>() { inputIswcId })).FirstOrDefault() ?? new IswcModel();

        public async Task<IswcModel> FindIswcModelAsync(WorkNumber workNumber, bool readOnly = false, DetailLevel detailLevel = DetailLevel.Full) =>
            (await GetIswcModels(new List<long>() { (await workRepository.FindAsync(
                x => x.AgencyWorkCode == workNumber.Number && x.AgencyId == workNumber.Type && x.Status)).IswcId }, readOnly, detailLevel)).FirstOrDefault() ?? new IswcModel();


        public async Task<IswcModel> FindIswcModelAsync(string preferredIswc, bool readOnly = false, DetailLevel detailLevel = DetailLevel.Full) =>
            (await GetIswcModels(new List<long>() { (await workRepository.FindAsync(x => x.Iswc.Iswc1 == preferredIswc && x.Status)).IswcId }, readOnly, detailLevel)).FirstOrDefault() ?? new IswcModel();

        public async Task<IEnumerable<IswcModel>> FindManyIswcModelsAsync(IEnumerable<string> preferredIswcs, bool readOnly = false, DetailLevel detailLevel = DetailLevel.Full) =>
            await GetIswcModels((await workRepository.FindManyAsync(x => preferredIswcs.Contains(x.Iswc.Iswc1) && x.Status)).Select(x => x.IswcId), readOnly, detailLevel);


        public async Task<IswcModel> FindArchivedIswcModelAsync(string archivedIswc, bool readOnly = false, DetailLevel detailLevel = DetailLevel.Full) =>
            (await GetIswcModels(new List<long>() { (await workRepository.FindArchivedIswc(archivedIswc)) }, readOnly, detailLevel)).FirstOrDefault() ?? new IswcModel();

        private async Task<IEnumerable<IswcModel>> GetIswcModels(IEnumerable<long> iswcIds, bool readOnly = false, DetailLevel detailLevel = DetailLevel.Full) =>
            await iswcRepository.GetIswcModels(iswcIds, readOnly, detailLevel);

        public async Task DeleteAsync(Submission submission)
        {
            var unitOfWork = workRepository.UnitOfWork;
            var model = submission.Model;

            var iswc = await iswcRepository.FindAsyncOptimizedByPath(x => x.Iswc1 == model.PreferredIswc && x.Status,
                    "Title", "Creator", "Publisher", "WorkInfo", "WorkInfo.AdditionalIdentifier", "WorkInfo.AdditionalIdentifier.Recording", "WorkInfo.WorkInfoPerformer", "WorkInfo.WorkFlowInstance",
                    "WorkInfo.WorkFlowInstance.WorkflowTask", "WorkInfo.DerivedFrom");

            var now = DateTime.UtcNow;
            if (iswc == null || model.WorkNumber == null) return;

            bool noActiveWorkRemainingOnIswc = false;

            if (!iswc.WorkInfo.Any(x => x.Status && x.AgencyWorkCode != model.WorkNumber.Number))
            {
                noActiveWorkRemainingOnIswc = true;
            }

            if (submission.IsEligible)
            {
                var work = iswc.WorkInfo.First(x => x.AgencyId == model.WorkNumber.Type && x.AgencyWorkCode == model.WorkNumber.Number && x.Status);
                work.Status = false;
                work.LastModifiedDate = now;
                foreach (var title in work.Title) { title.Status = false; title.LastModifiedDate = now; }
                foreach (var creator in work.Creator) { creator.Status = false; creator.LastModifiedDate = now; }
                foreach (var p in work.Publisher) { p.Status = false; p.LastModifiedDate = now; }
                foreach (var p in work.WorkInfoPerformer) { p.Status = false; p.LastModifiedDate = now; }
                foreach (var wf in work.WorkflowInstance) { foreach (var x in wf.WorkflowTask) { x.IsDeleted = true; } }
                foreach (var wf in work.WorkflowInstance) { wf.IsDeleted = true; }
                foreach (var df in work.DerivedFrom) { df.Status = false; df.LastModifiedDate = now; }

                if (noActiveWorkRemainingOnIswc)
                {
                    work.Iswc.LastModifiedDate = now;
                    work.Iswc.LastModifiedUserId = GetRequestSourceUserId();
                    work.Iswc.Status = false;
                }

                if (iswc.WorkInfo.Where(x => x.Status).All(x => x.AgencyWorkCode.StartsWith("PRS_")) && !noActiveWorkRemainingOnIswc)
                {
                    work.Iswc.LastModifiedDate = now;
                    work.Iswc.LastModifiedUserId = GetRequestSourceUserId();
                    work.Iswc.IswcStatusId = 2;

                    foreach (var w in iswc.WorkInfo.Where(x => x.Status)) { w.LastModifiedDate = now; }

                    await searchService.UpdateWorksIswcStatus(iswc);
                }

                await searchService.DeleteByWorkCode(work.WorkInfoId);
                await workRepository.UpdateAsync(work);
                await DeleteChecksum(submission.Model.Agency, submission.Model.WorkNumber.Number);

                var csnRecords = new List<CsnNotifications>();
                foreach (var agencyWork in iswc.WorkInfo.Where(x => !x.IsReplaced))
                    csnRecords.Add(new CsnNotifications(agencyWork, submission.Model.Agency, now, TransactionType.CDR, string.Empty));

                if (csnRecords.Any())
                    await notificationService.AddCsnNotifications(csnRecords, new WorkflowInstance());

            }
            else
            {
                var work = iswc.WorkInfo.First(x => x.AgencyId == model.WorkNumber.Type && x.AgencyWorkCode == model.WorkNumber.Number);
                workRepository.DeleteWork(work);
                await searchService.DeleteByWorkCode(work.WorkInfoId);
                await DeleteChecksum(submission.Model.Agency, submission.Model.WorkNumber.Number);
            }

            if (!iswc.WorkInfo.Any(x => x.IswcEligible && x.Status))
            {
                foreach (var work in iswc.WorkInfo.Where(x => !x.IswcEligible))
                {
                    work.Iswc.LastModifiedDate = now;
                    work.Iswc.LastModifiedUserId = GetRequestSourceUserId();
                    work.Iswc.Status = false;

                    workRepository.DeleteWork(work);
                    await searchService.DeleteByWorkCode(work.WorkInfoId);
                    await DeleteChecksum(submission.Model.Agency, submission.Model.WorkNumber.Number);
                }
            }

            await unitOfWork.Save();
        }

        private async Task DeleteChecksum(string agency, string workcode)
        {
            var existingChecksum = await checksumService.GetChecksum(agency, workcode);
            if (existingChecksum != null)
            {
                await checksumService.DeleteItemAsync(existingChecksum.ID, existingChecksum.PartitionKey);
            }
        }

        public async Task AddIswcLinkedTo(Submission submission)
        {
            var model = submission.Model;
            var now = DateTime.UtcNow;
            var webUserId = GetRequestSourceUserId();

            foreach (WorkNumber workNumber in model.WorkNumbersToMerge)
            {

                var executeStrategy = iswcRepository.UnitOfWork.CreateExecutionStrategy();

                await executeStrategy.ExecuteAsync(async () =>
                {
                    using (var trans = iswcRepository.UnitOfWork.BeginTransaction())
                    {
                        var iswc1 = await iswcRepository.FindAsyncOptimizedByPath(x => x.WorkInfo.Any(x => x.AgencyWorkCode == workNumber.Number && x.AgencyId == workNumber.Type && !x.IsReplaced),
                            "WorkInfo", "IswclinkedTo", "WorkInfo.Creator", "WorkInfo.Title", "WorkInfo.Publisher", "IswclinkedTo.MergeRequestNavigation");

                        var iswc2 = await iswcRepository.FindAsyncOptimizedByPath(x => x.WorkInfo.Any(x => x.Iswc.Iswc1 == submission.Model.PreferredIswc && !x.IsReplaced),
                            "WorkInfo", "IswclinkedTo", "WorkInfo.Creator", "WorkInfo.Title", "WorkInfo.Publisher", "IswclinkedTo.MergeRequestNavigation");


                        var iswcsToMerge = GetOldestIswc(iswc1, iswc2, submission.TransactionType == TransactionType.CUR);
                        if (!iswcsToMerge.childIswc.IswclinkedTo.Any(x => x.Status && x.LinkedToIswc == model.PreferredIswc))
                        {
                            await MergeIswcs(iswcsToMerge.childIswc, iswcsToMerge.parentIswc, model);
                            await trans.CommitAsync();
                        }
                    }
                });
            }

            foreach (string iswc in model.IswcsToMerge)
            {
                var executeStrategy = iswcRepository.UnitOfWork.CreateExecutionStrategy();

                await executeStrategy.ExecuteAsync(async () =>
                {
                    using (var trans = iswcRepository.UnitOfWork.BeginTransaction())
                    {
                        var iswc1 = await iswcRepository.FindAsyncOptimizedByPath(x => x.Iswc1 == iswc && x.WorkInfo.Any(x => x.Status && !x.IsReplaced),
                            "WorkInfo", "WorkInfo.Title", "IswclinkedTo", "IswclinkedTo.MergeRequestNavigation", "WorkInfo.AdditionalIdentifier", "WorkInfo.AdditionalIdentifier.Recording", "WorkInfo.AdditionalIdentifier.Recording.RecordingArtist", "WorkInfo.WorkInfoPerformer", "WorkInfo.WorkInfoPerformer.Performer");

                        var iswc2 = await iswcRepository.FindAsyncOptimizedByPath(x => x.Iswc1 == model.PreferredIswc && x.WorkInfo.Any(x => x.Status && !x.IsReplaced),
                            "WorkInfo", "WorkInfo.Title", "IswclinkedTo", "IswclinkedTo.MergeRequestNavigation", "WorkInfo.AdditionalIdentifier", "WorkInfo.AdditionalIdentifier.Recording", "WorkInfo.AdditionalIdentifier.Recording.RecordingArtist", "WorkInfo.WorkInfoPerformer", "WorkInfo.WorkInfoPerformer.Performer");

                        var (childIswc, parentIswc) = GetOldestIswc(iswc1, iswc2, submission.TransactionType == TransactionType.CUR);
                        if (!childIswc.IswclinkedTo.Any(x => x.Status && x.LinkedToIswc == model.PreferredIswc))
                        {
                            await MergeIswcs(childIswc, parentIswc, model);
                            await trans.CommitAsync();
                        }
                    }
                });
            }


            async Task MergeIswcs(Data.DataModels.Iswc childIswc, Data.DataModels.Iswc parentIswc, SubmissionModel model)
            {
                if (await ReplaceChildWorks(childIswc.WorkInfo))
                    await AddWorksUnderNewParentIswc(childIswc, parentIswc);

                if (childIswc.IswclinkedTo.Any(x => x.Status))
                    await FollowLinkedToChain();

                if (parentIswc.Iswc1 != childIswc.Iswc1)
                {
                    var workflowInstance = await AddMergeApprovalWorkflow(childIswc.IswcId, model.Agency, parentIswc.Iswc1, Bdo.Work.WorkflowType.MergeApproval);

                    if (!workflowInstance.WorkflowTask.Any())
                    {
                        var linkedTo = new IswclinkedTo
                        {
                            CreatedDate = now,
                            LastModifiedDate = now,
                            LastModifiedUserId = webUserId,
                            Status = true,
                            LinkedToIswc = parentIswc.Iswc1,
                            IswcId = childIswc.IswcId
                        };
                        childIswc.IswclinkedTo.Add(linkedTo);
                        await iswcRepository.UnitOfWork.Save();
                    }

                    await workflowRepository.UnitOfWork.Save();
                }

                async Task FollowLinkedToChain()
                {
                    var parent = childIswc.IswclinkedTo.OrderByDescending(x => x.CreatedDate)?.FirstOrDefault(x => x.Status)?.LinkedToIswc;

                    if (!string.IsNullOrWhiteSpace(parent))
                    {
                        var latestParent = await iswcLinkedToRepository.GetLinkedToTree(new IswcModel { Iswc = parent });

                        childIswc = await iswcRepository.FindAsyncOptimizedByPath(x => x.Iswc1 == latestParent.Iswc, "WorkInfo", "IswclinkedTo"
                        , "WorkInfo.Creator", "WorkInfo.Title", "WorkInfo.Publisher", "IswclinkedTo.MergeRequestNavigation");
                    }
                }
            }

            (Data.DataModels.Iswc childIswc, Data.DataModels.Iswc parentIswc) GetOldestIswc(Data.DataModels.Iswc iswc1, Data.DataModels.Iswc iswc2, bool isAutoMerge)
            {
                if (isAutoMerge && iswc2.CreatedDate > iswc1.CreatedDate && iswc1.IswcStatusId != 2)
                    return (iswc2, iswc1);

                return (iswc1, iswc2);
            }
        }

        async Task AddWorksUnderNewParentIswc(Data.DataModels.Iswc childIswc, Data.DataModels.Iswc parentIswc)
        {
            var copiedWorks = mapper.Map<List<WorkInfo>>(childIswc.WorkInfo.Where(x => x.Status));
            var worksToAddToIndex = new List<WorkInfo>();

            if (!copiedWorks.Any())
                return;

            foreach (var copiedWork in copiedWorks.Where(x => x.Status && x.IswcEligible && !parentIswc.WorkInfo.Any(p => p.AgencyWorkCode == x.AgencyWorkCode && p.AgencyId == x.AgencyId && p.Status)))
            {
                copiedWork.IswcId = parentIswc.IswcId;
                copiedWork.IsReplaced = false;
                copiedWork.LastModifiedDate = DateTime.UtcNow;
                if (copiedWork.Creator.Any())
                {
                    foreach (var creator in copiedWork.Creator)
                    {
                        creator.IswcId = parentIswc.IswcId;

                        if (!parentIswc.Creator.Any(c => c.IpbaseNumber == creator.IpbaseNumber))
                            creator.IsExcludedFromIswc = true;
                    }
                }

                if (copiedWork.Publisher.Any())
                {
                    foreach (var publisher in copiedWork.Publisher)
                    {
                        publisher.IswcId = parentIswc.IswcId;
                    }
                }

                if (copiedWork.Title.Any())
                {
                    foreach (var title in copiedWork.Title)
                    {
                        if (title.TitleTypeId == (int)Bdo.Work.TitleType.OT)
                        {
                            var lastModifiedTitle = parentIswc.WorkInfo.OrderByDescending(w => w.LastModifiedDate)
                                .FirstOrDefault(x => x.IswcEligible && x.Status)?.Title
                                .FirstOrDefault(x => x.TitleTypeId == (int)Bdo.Work.TitleType.OT && x.Status)?.Title1;

                            if (lastModifiedTitle != title.Title1)
                            {
                                title.TitleTypeId = (int)Bdo.Work.TitleType.OT;
                                title.TitleType = null;
                            }
                        }

                        title.IswcId = parentIswc.IswcId;
                    }
                }
            }

            workRepository.AddRange(copiedWorks);
            await workRepository.UnitOfWork.Save();

            worksToAddToIndex.AddRange(copiedWorks);

            await AddWorksToSearchIndex();

            async Task AddWorksToSearchIndex()
            {
                foreach (var work in worksToAddToIndex.Where(x => x.IswcEligible))
                {
                    var sub = new Submission
                    {
                        IswcModel = new IswcModel(),
                        Model = new SubmissionModel
                        {
                            WorkNumber = new WorkNumber
                            {
                                Number = work.AgencyWorkCode,
                                Type = work.AgencyId
                            }
                        }
                    };

                    var res = mapper.Map<VerifiedSubmissionModel>(work);

                    if (res.Titles.Any() && res.Titles.Count() > 1)
                        res.Titles = res.Titles.Where(x => x.Type == Bdo.Work.TitleType.OT || x.Type == Bdo.Work.TitleType.TO).ToList();

                    sub.IswcModel.VerifiedSubmissions = new List<VerifiedSubmissionModel>() { res };

                    await searchService.AddSubmission(sub);
                }
            }
        }

        async Task<bool> ReplaceChildWorks(ICollection<WorkInfo> childWorks)
        {
            if (childWorks == null || !childWorks.Any() || childWorks.All(x => x.IsReplaced))
                return false;

            foreach (var childWork in childWorks.Where(x => !x.IsReplaced))
            {
                childWork.IsReplaced = true;
                childWork.ArchivedIswc = null;
                childWork.LastModifiedDate = DateTime.UtcNow;
            }

            workRepository.UpdateRangeAsync(childWorks);
            await workRepository.UnitOfWork.Save();

            return true;
        }

        public async Task DeleteIswcLinkedTo(Submission submission)
        {
            var model = submission.Model;

            var parentIswc = submission.Model.PreferredIswc;
            var childIswc = (await workRepository.FindAsync(x => x.AgencyId == model.WorkNumber.Type && x.AgencyWorkCode == model.WorkNumber.Number && x.IsReplaced && x.Status))?.Iswc.Iswc1;

            if (childIswc == null)
                childIswc = (await workRepository.FindAsync(x => x.AgencyId == model.WorkNumber.Type && x.AgencyWorkCode == model.WorkNumber.Number))?.Iswc.Iswc1;

            var iswc = (await iswcRepository.FindAsyncOptimized(
                x => x.IswclinkedTo.Any(y => y.LinkedToIswc.Equals(parentIswc) && y.Iswc.Iswc1.Equals(childIswc)),
                i => i.IswclinkedTo, i => i.IswclinkedTo.First().MergeRequestNavigation));

            var now = DateTime.UtcNow;

            List<CsnNotifications> csnRecords = new List<CsnNotifications>();

            foreach (var x in iswc.IswclinkedTo.Where(x => x.Status).ToList())
            {
                x.Status = false;
                x.LastModifiedDate = now;
                x.LastModifiedUserId = GetRequestSourceUserId();

                if (x.MergeRequest != null && x.MergeRequestNavigation?.MergeStatus == (int)Bdo.Work.MergeStatus.Pending)
                    await UpdateExistingOutstandingWorkflows(x.MergeRequest);

                var newDemergeWorkflowResult = await CreateDeMergeWorkflow(x.IswcId, x.LinkedToIswc, submission.Model.Agency);
                if (newDemergeWorkflowResult.deMergeRequest.WorkflowInstance.Any())
                {
                    x.DeMergeRequestNavigation = newDemergeWorkflowResult.deMergeRequest;
                }
                csnRecords.AddRange(newDemergeWorkflowResult.csnRecords);
            }

            var result = await iswcRepository.UpdateAsync(iswc);

            await iswcRepository.UnitOfWork.Save();

            var workflowInstance = result.IswclinkedTo?.FirstOrDefault()?.DeMergeRequestNavigation?.WorkflowInstance
                    .FirstOrDefault(x => x.InstanceStatus == (int)Bdo.Work.InstanceStatus.Outstanding);

            await notificationService.AddCsnNotifications(csnRecords, workflowInstance);

            async Task UpdateExistingOutstandingWorkflows(long? mergeRequestId)
            {

                var existingWorkflow = await workflowRepository.FindAsyncOptimizedByPath(w => w.WorkflowInstance.MergeRequestId == mergeRequestId,
                        $"{nameof(WorkflowInstance)}", $"{nameof(WorkflowInstance)}.{nameof(MergeRequest)}");

                if (existingWorkflow != null)
                {
                    existingWorkflow.WorkflowInstance.InstanceStatus = (int)Bdo.Work.InstanceStatus.Cancelled;
                    existingWorkflow.WorkflowInstance.MergeRequest.MergeStatus = (int)Bdo.Work.MergeStatus.Complete;

                    await workflowRepository.UpdateAsync(existingWorkflow);
                    await workflowRepository.UnitOfWork.Save();
                }
            }

            async Task<(DeMergeRequest deMergeRequest, IList<CsnNotifications> csnRecords)> CreateDeMergeWorkflow(long childIswcId, string parentIswc, string agency)
            {
                var userId = GetRequestSourceUserId();
                var now = DateTime.UtcNow;

                var workInfos = await workRepository.FindManyAsyncOptimized(x => x.IswcId == childIswcId && x.Status, i => i.Title, i => i.WorkflowInstance);
                var parentWorkinfos = (await workRepository.FindManyAsyncOptimized(x => x.Iswc.Iswc1 == parentIswc && x.Status && workInfos.All(y => y.AgencyId != x.AgencyId), i => i.WorkflowInstance)).ToList();

                List<CsnNotifications> csnRecords = new List<CsnNotifications>();

                var childWorkInfos = workInfos;
                workInfos = workInfos.Concat(parentWorkinfos).ToList();

                var allParentWorkinfos = (await workRepository.FindManyAsyncOptimized(x => x.Iswc.Iswc1 == parentIswc))?.ToList();

                if (allParentWorkinfos != null)
                {
                    var parentWorkinfosToUpdate = allParentWorkinfos.Where(x => childWorkInfos.Any(y => y.AgencyWorkCode == x.AgencyWorkCode)).ToList();

                    await UndoReplaceChildWorks(childWorkInfos.ToList(), parentWorkinfosToUpdate?.ToList());

                    if (parentWorkinfosToUpdate != null)
                        await DeleteWorksOnParent(parentWorkinfosToUpdate);
                }


                if (workInfos != null && workInfos.Any())
                {

                    var workflowInstance = new WorkflowInstance
                    {
                        WorkflowType = (int)Bdo.Work.WorkflowType.DemergeApproval,
                        CreatedDate = now,
                        LastModifiedDate = now,
                        LastModifiedUserId = userId,
                        InstanceStatus = (int)InstanceStatus.Outstanding,
                        IsDeleted = false
                    };

                    foreach (var workInfo in workInfos)
                    {
                        csnRecords.Add(new CsnNotifications(workInfo, agency, now, TransactionType.DMR, parentIswc));
                        if (workInfo.IswcEligible && workflowInstance != null)
                        {
                            if (workflowInstance.WorkflowTask.Any(x => x.AssignedAgencyId == workInfo.AgencyId) || workInfo.AgencyId == agency)
                                continue;

                            workflowInstance.WorkflowTask.Add(new Data.DataModels.WorkflowTask
                            {
                                AssignedAgencyId = workInfo.AgencyId,
                                LastModifiedDate = now,
                                IsDeleted = false,
                                TaskStatus = (int)InstanceStatus.Outstanding,
                                LastModifiedUserId = userId
                            });
                        }
                    }

                    var deMergeRequest = new DeMergeRequest
                    {
                        AgencyId = agency,
                        LastModifiedUserId = userId,
                        LastModifiedDate = now,
                        CreatedDate = now,
                        Status = true,
                        DeMergeStatus = (int)Bdo.Work.MergeStatus.Pending

                    };

                    if (workflowInstance?.WorkflowTask.Any() ?? false)
                        deMergeRequest.WorkflowInstance.Add(workflowInstance);

                    return (deMergeRequest, csnRecords);
                }

                return (new DeMergeRequest(), csnRecords);
            }
        }

        private static readonly IDictionary<RequestSource, int> RequestSourceUsers = new Dictionary<RequestSource, int> {
            {RequestSource.CISNET, 2 },
            {RequestSource.REST, 5 },
            {RequestSource.PORTAL, 8 },
            {RequestSource.FILE, 9 }
        };

        private int GetRequestSourceUserId() => RequestSourceUsers[httpContext.GetRequestItem<RequestSource>("requestSource")];

        private async Task<string> GetNextIswc()
        {
            var nextIswc = await iswcService.GetNextIswc();

            return nextIswc;
        }

        private async Task<IList<Bdo.Work.Instrumentation>> GetInstrumentationByCode(ICollection<Bdo.Work.Instrumentation> instrumentations)
        {
            var instrumentationIds = new List<Bdo.Work.Instrumentation>();
            var instrumentationsDb = (await instrumentationRepository.FindManyAsync(x => x.Code != null)).ToList();

            foreach (var instrumentation in instrumentationsDb)
            {
                if (instrumentations.Select(x => x.Code).Contains(instrumentation.Code))
                    instrumentationIds.Add(new Bdo.Work.Instrumentation(instrumentation.InstrumentationId.ToString()));
            }

            return instrumentationIds;
        }

        public async Task<long> UpdateAsync(Submission submission, bool isRollbackUpdate = false)
        {
            var unitOfWork = workRepository.UnitOfWork;

            var updatedWorkInfos = new List<WorkInfo>();
            var csnRecords = new List<CsnNotifications>();
            var now = DateTime.UtcNow;
            var IswcWithNoEligibleWorks = long.MinValue;

            var iswc = await iswcRepository.FindAsync(i => i.Iswc1 == submission.Model.PreferredIswc && i.Status);
            if (iswc != null && iswc.IswcStatusId == 2 && submission.RequestType != RequestType.Label)
                iswc.IswcStatusId = 1;

            var workNumbers = new List<WorkNumber>
            {
                new WorkNumber { Number = submission.Model.WorkNumber!.Number, Type = submission.Model.WorkNumber.Type }
            };

            var workInfos = await workRepository.FindManyWorkInfosAsync(workNumbers);

            foreach (var workInfo in workInfos)
            {
                var additionalIdentifiers = new List<Data.DataModels.AdditionalIdentifier>();
                if (workInfo.AdditionalIdentifier.Any())
                    additionalIdentifiers.AddRange(workInfo.AdditionalIdentifier);

                foreach (var ai in additionalIdentifiers)
                {
                    ai.Recording = await recordingRepository.FindAsync(r => r.AdditionalIdentifierId == ai.AdditionalIdentifierId && ai.NumberTypeId == 1);
                    if (ai.Recording != null)
                    {
                        var recording = submission.Model?.AdditionalIdentifiers?.Where(a => a.WorkCode == ai.WorkIdentifier && !(a.RecordingTitle.IsNullOrEmpty())).FirstOrDefault();

                        if (recording != null)
                        {
                            ai.Recording.RecordingTitle = recording.RecordingTitle;
                            ai.Recording.SubTitle = recording.SubTitle;
                            ai.Recording.ReleaseEmbargoDate = (recording.ReleaseEmbargoDate?.DateTime).GetValueOrDefault();
                        }
                    }
                }

                var updatedWorkInfo = workInfo;
                if (iswc != null && iswc.IswcId != workInfo.IswcId && !submission.IsEligible)
                {
                    if (updatedWorkInfo.Iswc.WorkInfo.Where(x => x.Status && x.IswcEligible && x.AgencyWorkCode != submission?.Model?.WorkNumber.Number
                    && x.AgencyId != submission?.Model?.WorkNumber.Type).Count() == 0)
                        IswcWithNoEligibleWorks = updatedWorkInfo.IswcId;

                    await searchService.DeleteByWorkCode(updatedWorkInfo.WorkInfoId);
                    updatedWorkInfo = ChangeWorkInfoIswcDetails(updatedWorkInfo, iswc, now);
                }
                else
                {
                    if (submission.IsEligible && !isRollbackUpdate &&
                        (!(submission.Model?.Titles?.Any(t =>
                            t.Type == Bdo.Work.TitleType.OT &&
                            t.Name == updatedWorkInfo.Title.FirstOrDefault(x => x.TitleTypeId == (int)Bdo.Work.TitleType.OT)?.Title1) ?? false)
                        || (submission.Model?.InterestedParties?.Count(c =>
                            c.CisacType == Bdo.Ipi.CisacInterestedPartyType.C ||
                            c.CisacType == CisacInterestedPartyType.MA ||
                            c.CisacType == CisacInterestedPartyType.TA) ?? 0) != updatedWorkInfo.Creator.Count()))
                    {
                        if (!updatedWorkInfo.WorkflowInstance.Any() || (updatedWorkInfo.WorkflowInstance.Any(x => !x.IsDeleted && x.InstanceStatus != (int)Bdo.Work.InstanceStatus.Outstanding
                        && x.WorkflowType == (int)Bdo.Work.WorkflowType.UpdateApproval)))
                        {
                            var workflowResult = await AddUpdateApprovalWorkflow(updatedWorkInfo.WorkInfoId, updatedWorkInfo.IswcId, submission.Model!.Agency);

                            if (workflowResult.workflowInstance != null && workflowResult.workflowInstance.WorkflowTask.Any())
                            {
                                var submissionModel = mapper.Map<SubmissionModel>(updatedWorkInfo);
                                var submissionToSave = new Submission()
                                {
                                    IsEligible = updatedWorkInfo.IswcEligible,
                                    Model = submissionModel
                                };

                                await updateWorkflowsHistoryService.SaveModel(submissionToSave, updatedWorkInfo.WorkInfoId);

                                updatedWorkInfo.WorkflowInstance.Add(workflowResult.workflowInstance);

                                if (workflowResult.csnRecords.Any())
                                    csnRecords.AddRange(workflowResult.csnRecords);
                            }
                        }
                    }
                    else if (isRollbackUpdate)
                    {
                        var workInfosToRollBack = await workRepository.FindManyAsyncOptimized(
                                x => x.IswcId == workInfo.IswcId && x.IswcEligible && x.Status,
                                i => i.WorkflowInstance);
                        foreach (var w in workInfosToRollBack)
                        {
                            var csnRecord = new CsnNotifications(w, submission?.Model?.Agency, now, TransactionType.CUR, string.Empty)
                            {
                                WorkflowTaskID = submission!.Model!.WorkflowTasks.FirstOrDefault(x => x.AssignedAgencyId == submission.Model.Agency)?.TaskId.ToString(),
                                WorkflowStatus = ((int)InstanceStatus.Rejected).ToString()
                            };

                            csnRecords.Add(csnRecord);
                        }
                    }
                }

                if (submission?.Model?.Instrumentation?.Any() ?? false)
                    submission.Model.Instrumentation = await GetInstrumentationByCode(submission.Model.Instrumentation);

                if (submission?.Model?.AdditionalIdentifiers != null)
                    submission.Model.AdditionalIdentifiers = await GetPublisherSubmitterInfo(submission.Model.AdditionalIdentifiers);

                var workInfoMappingObject = mapper.Map(submission, new WorkInfo()
                {
                    IswcId = updatedWorkInfo.IswcId,
                    WorkInfoId = updatedWorkInfo.WorkInfoId,
                    LastModifiedDate = now,
                    LastModifiedUserId = GetRequestSourceUserId()
                });

                updatedWorkInfo.IswcEligible = !submission!.IsEligibileOnlyForDeletingIps && submission.IsEligible;
                updatedWorkInfo.LastModifiedDate = now;
                updatedWorkInfo.Bvltr = workInfoMappingObject.Bvltr;
                updatedWorkInfo.Disambiguation = workInfoMappingObject.Disambiguation;
                updatedWorkInfo.Ipcount = workInfoMappingObject.Ipcount;
                updatedWorkInfo.LastModifiedUserId = workInfoMappingObject.LastModifiedUserId;
                updatedWorkInfo.MwiCategory = workInfoMappingObject.MwiCategory;
                updatedWorkInfo.SourceDatabase = workInfoMappingObject.SourceDatabase;
                updatedWorkInfo.AdditionalIdentifier = workInfoMappingObject.AdditionalIdentifier;

                updatedWorkInfo.Title = UpdateWorkInfoTitles(workInfo.Title, workInfoMappingObject.Title, now);
                updatedWorkInfo.Publisher = UpdateWorkInfoPublishers(workInfo.Publisher, workInfoMappingObject.Publisher, now);
                updatedWorkInfo.Creator = UpdateWorkInfoCreators(workInfo.Creator, workInfoMappingObject.Creator, now);
                updatedWorkInfo.DerivedFrom = UpdateDerivedFrom(workInfo.DerivedFrom, workInfoMappingObject.DerivedFrom, now);
                updatedWorkInfo.WorkInfoPerformer = await UpdateWorkPerformers(workInfo.WorkInfoPerformer, workInfoMappingObject.WorkInfoPerformer, now);
                updatedWorkInfo.DisambiguationIswc = UpdateDisambiguation(workInfo.DisambiguationIswc, workInfoMappingObject.DisambiguationIswc, now);

                if (submission.UpdateAllocatedIswc)
                {
                    updatedWorkInfo.AgencyWorkCode = submission.WorkNumberToReplaceIasWorkNumber;
                    updatedWorkInfo.AgencyId = submission.AgencyToReplaceIasAgency;
                    submission.Model!.WorkNumber.Number = (submission.WorkNumberToReplaceIasWorkNumber != null) ? submission.WorkNumberToReplaceIasWorkNumber : submission.Model.WorkNumber.Number;
                }

                foreach (var additionalIdentifier in additionalIdentifiers)
                {
                    if (updatedWorkInfo.AdditionalIdentifier.Any(x => x.WorkIdentifier == additionalIdentifier.WorkIdentifier && x.NumberTypeId == additionalIdentifier.NumberTypeId))
                    {
                        var ai = updatedWorkInfo.AdditionalIdentifier.First(x => x.WorkIdentifier == additionalIdentifier.WorkIdentifier && x.NumberTypeId == additionalIdentifier.NumberTypeId);
                        updatedWorkInfo.AdditionalIdentifier.Remove(ai);
                        updatedWorkInfo.AdditionalIdentifier.Add(additionalIdentifier);
                    }
                    else
                    {
                        updatedWorkInfo.AdditionalIdentifier.Add(additionalIdentifier);
                    }
                }

                updatedWorkInfos.Add(updatedWorkInfo);
            }


            workRepository.UpdateRangeAsync(updatedWorkInfos);
            await unitOfWork.Save();

            var workflowInstance = updatedWorkInfos.FirstOrDefault()?.WorkflowInstance.FirstOrDefault(x => x.InstanceStatus == (int)Bdo.Work.InstanceStatus.Outstanding);
            if (workflowInstance == null && isRollbackUpdate)
                workflowInstance = updatedWorkInfos.FirstOrDefault()?.WorkflowInstance.FirstOrDefault(x => x.WorkflowTask.Select(y => y.WorkflowTaskId).Any(t => t == submission?.Model?.WorkflowTasks.First().TaskId));

            if (!csnRecords.Any())
                csnRecords.Add(new CsnNotifications(updatedWorkInfos.FirstOrDefault(), updatedWorkInfos.FirstOrDefault()?.AgencyId, now, TransactionType.CUR, string.Empty));

            await notificationService.AddCsnNotifications(csnRecords, workflowInstance);
            await checksumService.AddChecksum(new SubmissionChecksums(submission?.Model?.WorkNumber.Type, submission?.Model?.WorkNumber.Number, submission?.Model?.HashCode));

            if (IswcWithNoEligibleWorks > 0)
                await DeleteIswcWithNoEligibleWorks(IswcWithNoEligibleWorks);

            var workNumber = submission?.Model?.WorkNumber?.Number;

            return updatedWorkInfos.First(x => x.AgencyWorkCode == workNumber).WorkInfoId;

            async Task DeleteIswcWithNoEligibleWorks(long IswcIdWithNoEligibleWorks)
            {
                var unitOfWorkToDeleteIswc = iswcRepository.UnitOfWork;
                var iswcToDelete = await iswcRepository.FindAsyncOptimizedByPath(x => x.IswcId == IswcIdWithNoEligibleWorks,
                "Title",
                "Creator",
                "Publisher");

                iswcRepository.Delete(iswcToDelete);
                await unitOfWorkToDeleteIswc.Save();
            }

        }

        ICollection<Data.DataModels.DisambiguationIswc> UpdateDisambiguation(ICollection<Data.DataModels.DisambiguationIswc> disambiguationIswcs, ICollection<Data.DataModels.DisambiguationIswc> updatedDisambiguationIswcs, DateTime now)
        {
            if (disambiguationIswcs.Any())
            {
                disambiguationIswcs.Where(t => !updatedDisambiguationIswcs.Any(z => z.Iswc == t.Iswc && t.Status))
                    .Select(d =>
                    {
                        d.Status = false;
                        d.LastModifiedDate = now;
                        d.LastModifiedUserId = GetRequestSourceUserId();
                        return d;
                    }).ToList();

                disambiguationIswcs.Where(t => updatedDisambiguationIswcs.Any(z => z.Iswc == t.Iswc && !t.Status))
                    .Select(d =>
                    {
                        d.Status = true;
                        d.LastModifiedDate = now;
                        d.LastModifiedUserId = GetRequestSourceUserId();
                        return d;
                    }).ToList();
            }

            foreach (var disambiguationIswc in updatedDisambiguationIswcs.Where(x => !disambiguationIswcs.Any(y => y.Iswc == x.Iswc && y.Status)))
            {
                disambiguationIswcs.Add(disambiguationIswc);
            }

            return disambiguationIswcs;
        }

        ICollection<Data.DataModels.DerivedFrom> UpdateDerivedFrom(ICollection<Data.DataModels.DerivedFrom> derivedFroms, ICollection<Data.DataModels.DerivedFrom> updatedDerivedFroms, DateTime now)
        {
            if (derivedFroms.Any())
            {
                derivedFroms.Where(t => !updatedDerivedFroms.Any(z => z.Title == t.Title && z.Iswc == t.Iswc && t.Status))
                    .Select(d =>
                    {
                        d.Status = false;
                        d.LastModifiedDate = now;
                        d.LastModifiedUserId = GetRequestSourceUserId();
                        return d;
                    }).ToList();

                derivedFroms.Where(t => updatedDerivedFroms.Any(z => z.Title == t.Title && z.Iswc == t.Iswc && !t.Status))
                    .Select(d =>
                    {
                        d.Status = true;
                        d.LastModifiedDate = now;
                        d.LastModifiedUserId = GetRequestSourceUserId();
                        return d;
                    }).ToList();
            }

            foreach (var derivedFrom in updatedDerivedFroms.Where(x => !derivedFroms.Any(y => y.Iswc == x.Iswc && y.Title == x.Title && y.Status)))
            {
                derivedFroms.Add(derivedFrom);
            }

            return derivedFroms;
        }

        ICollection<Data.DataModels.Title> UpdateWorkInfoTitles(ICollection<Data.DataModels.Title> titles, ICollection<Data.DataModels.Title> updatedTitles, DateTime now)
        {
            titles.Where(t => !updatedTitles.Any(z => z.Title1.Equals(t.Title1) && z.TitleTypeId == t.TitleTypeId && t.Status))
                .Select(r =>
                {
                    r.Status = false;
                    r.LastModifiedDate = now;
                    r.LastModifiedUserId = GetRequestSourceUserId();
                    return r;
                }).ToList();


            foreach (var title in updatedTitles)
            {
                if (!titles.Any(t => t.Title1.Equals(title.Title1) && t.TitleTypeId == title.TitleTypeId))
                    titles.Add(title);


                else
                {
                    titles.Where(t => t.Title1.Equals(title.Title1) && t.TitleTypeId == title.TitleTypeId)
                         .Select(r =>
                         {
                             r.StandardizedTitle = title.StandardizedTitle;
                             r.LastModifiedDate = now;
                             r.LastModifiedUserId = title.LastModifiedUserId;
                             r.Status = title.Status;
                             r.Title1 = title.Title1;
                             r.TitleTypeId = title.TitleTypeId;
                             return r;
                         }).ToList();
                }
            }

            return titles;
        }

        ICollection<Publisher> UpdateWorkInfoPublishers(ICollection<Publisher> publishers, ICollection<Publisher> updatedPublishers, DateTime now)
        {
            if (publishers == null)
                publishers = new List<Publisher>();

            foreach (var pub in updatedPublishers)
            {
                if (!publishers.Any(p => p.IpnameNumber.Equals(pub.IpnameNumber) && p.RoleTypeId == pub.RoleTypeId))
                {
                    publishers.Add(pub);
                }
                else
                {
                    publishers.Where(t => updatedPublishers.Any(z => z.IpnameNumber != t.IpnameNumber))
                        .Select(r =>
                        {
                            r.LastModifiedDate = now;
                            r.LastModifiedUserId = GetRequestSourceUserId();
                            r.Status = false;
                            return r;
                        }).ToList();

                    publishers.Where(p => p.IpnameNumber.Equals(pub.IpnameNumber) && p.RoleTypeId == pub.RoleTypeId)
                       .Select(r =>
                       {
                           r.IpbaseNumber = pub.IpbaseNumber;
                           r.LastModifiedDate = now;
                           r.LastModifiedUserId = pub.LastModifiedUserId;
                           r.Status = pub.Status;
                           return r;
                       }).ToList();
                }
            }

            return publishers;
        }

        ICollection<Creator> UpdateWorkInfoCreators(ICollection<Creator> creators, ICollection<Creator> updatedCreators, DateTime now)
        {
            foreach (var item in creators)
            {
                if (!updatedCreators.Any(x => x.IpnameNumber == item.IpnameNumber && x.RoleTypeId == item.RoleTypeId) 
                        || updatedCreators.Any(x => x.IpbaseNumber == null))
                {
                    item.LastModifiedDate = now;
                    item.LastModifiedUserId = GetRequestSourceUserId();
                    item.Status = false;
                }
            }

            foreach (var creator in updatedCreators)
            {
                if (!creators.Any(t => t.IpnameNumber.Equals(creator.IpnameNumber) && t.RoleTypeId == creator.RoleTypeId)
                        || updatedCreators.Any(x => x.IpbaseNumber == null))
                    creators.Add(creator);

                else
                {
                    creators.Where(p => p.IpnameNumber.Equals(creator.IpnameNumber) && p.RoleTypeId == creator.RoleTypeId)
                       .Select(r =>
                       {
                           r.IpbaseNumber = creator.IpbaseNumber;
                           r.LastModifiedDate = now;
                           r.LastModifiedUserId = creator.LastModifiedUserId;
                           r.Status = creator.Status;
                           r.IpbaseNumber = creator.IpbaseNumber;
                           r.Authoritative = creator.Authoritative;
                           r.CreatedDate = r.CreatedDate;
                           r.RoleTypeId = creator.RoleTypeId;
                           r.IsExcludedFromIswc = false;

                           return r;
                       }).ToList();
                }
            }

            return creators;
        }

        async Task<ICollection<WorkInfoPerformer>> UpdateWorkPerformers(ICollection<WorkInfoPerformer> workInfoPerformers, ICollection<WorkInfoPerformer> updateWorkInfoPerformers, DateTime now)
        {
            if (updateWorkInfoPerformers.Any())
            {
                var newlyAddedPerformers = new List<WorkInfoPerformer>();
                foreach (var workPerf in workInfoPerformers)
                {

                    if (!updateWorkInfoPerformers.Any(x => x.Performer.LastName == workPerf.Performer.LastName
                    && workPerf.Performer.FirstName == x.Performer.FirstName))
                    {
                        workPerf.LastModifiedDate = now;
                        workPerf.LastModifiedUserId = GetRequestSourceUserId();
                        workPerf.Status = false;    
                    }
                    else
                    {
                        workPerf.LastModifiedDate = now;
                        workPerf.LastModifiedUserId = GetRequestSourceUserId();
                        workPerf.Status = true;
                    }
                }

                foreach (var workPerf in updateWorkInfoPerformers)
                {
                    if (!workInfoPerformers.Any(x => x.Performer.LastName == workPerf.Performer.LastName && x.Performer.FirstName == workPerf.Performer.FirstName))
                        newlyAddedPerformers.Add(workPerf);
                }

                if (newlyAddedPerformers.Any())
                {
                    var performers = await GetPerformers(newlyAddedPerformers);

                    foreach (var perf in performers)
                    {
                        workInfoPerformers.Add(perf);
                    }

                    workInfoPerformers = workInfoPerformers.GroupBy(x => x.PerformerId).Select(y => y.First()).ToList();
                }

                return workInfoPerformers;
            }
            else
            {
                return new List<WorkInfoPerformer>();
            }
        }

        public WorkInfo ChangeWorkInfoIswcDetails(WorkInfo workInfo, Data.DataModels.Iswc iswc, DateTime now)
        {
            workInfo.IswcId = iswc.IswcId;
            workInfo.LastModifiedUserId = GetRequestSourceUserId();
            workInfo.ArchivedIswc = null;

            if (workInfo.Title.Any())
                workInfo.Title.Select(t =>
                {
                    t.IswcId = iswc.IswcId;
                    t.LastModifiedDate = now;
                    t.LastModifiedUserId = GetRequestSourceUserId();
                    return t;
                }).ToList();

            if (workInfo.DisambiguationIswc.Any())
                workInfo.DisambiguationIswc.Select(d =>
                {
                    d.Iswc = iswc.Iswc1;
                    d.LastModifiedDate = now;
                    d.LastModifiedUserId = GetRequestSourceUserId();
                    return d;
                }).ToList();

            if (workInfo.DerivedFrom.Any())
                workInfo.DerivedFrom.Select(d =>
                {
                    d.Iswc = iswc.Iswc1;
                    d.LastModifiedDate = now;
                    d.LastModifiedUserId = GetRequestSourceUserId();
                    return d;
                }).ToList();

            if (workInfo.Creator.Any())
                workInfo.Creator.Select(c =>
                {
                    c.IswcId = iswc.IswcId;
                    c.LastModifiedDate = now;
                    c.LastModifiedUserId = GetRequestSourceUserId();
                    return c;
                }).ToList();

            if (workInfo.Publisher.Any())
                workInfo.Publisher.Select(p =>
                {
                    p.IswcId = iswc.IswcId;
                    p.LastModifiedDate = now;
                    p.LastModifiedUserId = GetRequestSourceUserId();
                    return p;
                }).ToList();

            return workInfo;
        }

        public async Task<(WorkflowInstance workflowInstance, IList<CsnNotifications> csnRecords)> AddUpdateApprovalWorkflow(long workinfoId, long iswcId, string submitterAgency)
        {
            var now = DateTime.UtcNow;
            var workInfos = await workRepository.FindManyAsyncOptimized(
                x => x.IswcId == iswcId && x.Status, i => i.WorkflowInstance);

            if (workInfos != null && workInfos.Any())
            {
                var workflowInstance = new WorkflowInstance
                {
                    WorkflowType = (int)Bdo.Work.WorkflowType.UpdateApproval,
                    CreatedDate = now,
                    LastModifiedDate = now,
                    LastModifiedUserId = GetRequestSourceUserId(),
                    InstanceStatus = (int)Bdo.Work.InstanceStatus.Outstanding,
                    IsDeleted = false
                };

                List<CsnNotifications> csnRecords = new List<CsnNotifications>();
                foreach (var workInfo in workInfos)
                {
                    csnRecords.Add(new CsnNotifications(workInfo, submitterAgency, now, TransactionType.CUR, string.Empty));
                    if (workInfo.IswcEligible)
                    {
                        if (workflowInstance.WorkflowTask.Any(x => x.AssignedAgencyId == workInfo.AgencyId) || workInfo.AgencyId == submitterAgency)
                            continue;

                        workflowInstance.WorkflowTask.Add(new Data.DataModels.WorkflowTask
                        {
                            AssignedAgencyId = workInfo.AgencyId,
                            LastModifiedDate = now,
                            IsDeleted = false,
                            TaskStatus = (int)Bdo.Work.InstanceStatus.Outstanding,
                            LastModifiedUserId = GetRequestSourceUserId()
                        });
                    }
                }

                return (workflowInstance, csnRecords);
            }

            return (new WorkflowInstance(), new List<CsnNotifications>());
        }

        public async Task<WorkflowInstance> AddMergeApprovalWorkflow(long iswcId, string submitterAgencyId, string preferredIswc, Bdo.Work.WorkflowType workflowType)
        {
            var now = DateTime.UtcNow;
            var webUserId = GetRequestSourceUserId();

            var workInfos = await workRepository.FindManyAsyncOptimized(x => x.IswcId == iswcId && x.Status,
               i => i.WorkflowInstance);
            var parentIswcWorkInfos = (await workRepository.FindManyAsyncOptimized(x => x.Iswc.Iswc1 == preferredIswc && x.Status && workInfos.All(y => y.AgencyId != x.AgencyId), i => i.Iswc, i => i.WorkflowInstance));

            workInfos = workInfos.Concat(parentIswcWorkInfos).ToList().DistinctBy(x => x.AgencyId);

            if (workInfos != null && workInfos.Any())
            {
                var fromIswc = workInfos?.FirstOrDefault()?.Iswc.Iswc1;

                var mergeRequest = new MergeRequest
                {
                    AgencyId = submitterAgencyId,
                    LastModifiedUserId = webUserId,
                    LastModifiedDate = now,
                    CreatedDate = now,
                    Status = true,
                    MergeStatus = (int)Bdo.Work.MergeStatus.Pending
                };

                var linkedTo = new IswclinkedTo
                {
                    CreatedDate = now,
                    LastModifiedDate = now,
                    LastModifiedUserId = webUserId,
                    Status = true,
                    LinkedToIswc = preferredIswc,
                    IswcId = iswcId
                };

                var workflowInstance = new WorkflowInstance
                {
                    WorkflowType = (int)workflowType,
                    CreatedDate = now,
                    LastModifiedDate = now,
                    LastModifiedUserId = webUserId,
                    InstanceStatus = (int)InstanceStatus.Outstanding,
                    IsDeleted = false,
                };

                List<CsnNotifications> csnRecords = new List<CsnNotifications>();
                if (workInfos != null)
                {
                    foreach (var workInfo in workInfos)
                    {
                        csnRecords.Add(new CsnNotifications(workInfo, submitterAgencyId, now, TransactionType.MER, fromIswc, preferredIswc));

                        if (workInfo.IswcEligible && workInfo.AgencyId != submitterAgencyId)
                        {
                            if (workflowInstance.WorkflowTask.Any(x => x.AssignedAgencyId == workInfo.AgencyId) || workInfo.AgencyId == submitterAgencyId)
                                continue;

                            workflowInstance.WorkflowTask.Add(new Data.DataModels.WorkflowTask
                            {
                                AssignedAgencyId = workInfo.AgencyId,
                                LastModifiedDate = now,
                                IsDeleted = false,
                                TaskStatus = (int)InstanceStatus.Outstanding,
                                LastModifiedUserId = webUserId
                            });
                        }
                    }
                }
                
                if (workflowInstance.WorkflowTask.Any())
                {
                    mergeRequest.IswclinkedTo.Add(linkedTo);
                    mergeRequest.WorkflowInstance.Add(workflowInstance);
                    var result = await mergeRequestRepository.AddAsync(mergeRequest);
                    await mergeRequestRepository.UnitOfWork.Save();
                }

                await notificationService.AddCsnNotifications(csnRecords, workflowInstance);

                return workflowInstance;
            }

            return new WorkflowInstance();
        }

        public async Task AddSubmissionAsync(Submission submission)
        {
            await searchService.AddSubmission(submission);
        }

        public async Task UpdateLabelSubmissionsAsync(Submission submission)
        {
            var unitOfWork = workRepository.UnitOfWork;
            var now = DateTime.UtcNow;

            var iswc = await iswcRepository.FindAsyncOptimizedByPath(x => x.Iswc1 == submission.Model.PreferredIswc && x.Status,
                    "Title", "Creator", "Publisher", "WorkInfo", "WorkInfo.AdditionalIdentifier", "WorkInfo.AdditionalIdentifier.Recording", "WorkInfo.WorkInfoPerformer", "WorkInfo.WorkFlowInstance",
            "WorkInfo.WorkFlowInstance.WorkflowTask", "WorkInfo.DerivedFrom");

            iswc.IswcStatusId = submission.IswcModel.IswcStatusId ?? 1;

            await searchService.UpdateWorksIswcStatus(iswc);

            foreach (var work in iswc.WorkInfo.Where(x => x.Status && x.AgencyWorkCode.StartsWith("PRS_"))) 
            { 
                work.LastModifiedDate = now; 
            }

            await unitOfWork.Save();
        }

        public async Task<IEnumerable<Bdo.Work.AdditionalIdentifier>> GetPublisherSubmitterInfo(IEnumerable<Bdo.Work.AdditionalIdentifier> additionalIdentifiers)
        {
            foreach (var identifier in additionalIdentifiers)
            {
                if (identifier.SubmitterDPID != null)
                {
                    var publisher = await publisherCodeRespository.FindAsync(x => x.Code == identifier.SubmitterDPID);
                    if (publisher != null)
                    {
                        identifier.SubmitterCode = publisher.Code;
                    }
                    else
                    {
                        identifier.SubmitterCode = identifier.SubmitterDPID;
                        await publisherCodeRespository.UpdateAsync(new PublisherSubmitterCode { Code = identifier.SubmitterCode, Publisher = identifier.SubmitterCode });
                        await publisherCodeRespository.UnitOfWork.Save();
                    }
                }
                else if (identifier.NameNumber != null)
                {
                    var publisher = await publisherCodeRespository.FindAsync(x => x.IpnameNumber == identifier.NameNumber);
                    if (publisher != null)
                    {
                        identifier.SubmitterCode = publisher.Code;
                    }
                    else if (identifier.SubmitterCode != null)
                    {
                        await publisherCodeRespository.UpdateAsync(new PublisherSubmitterCode { Code = identifier.SubmitterCode, Publisher = identifier.SubmitterCode, IpnameNumber = identifier.NameNumber });
                        await publisherCodeRespository.UnitOfWork.Save();
                    }
                }
                else
                {
                    identifier.SubmitterCode = "ISRC";
                }

                if (identifier.SubmitterCode != null)
                {
                    identifier.NumberTypeId = (await numberTypeRepository.FindAsync(x => x.Code == identifier.SubmitterCode))?.NumberTypeId;
                    if (identifier.NumberTypeId == null)
                    {
                        var numberType = await numberTypeRepository.UpdateAsync(new NumberType { Code = identifier.SubmitterCode, Description = identifier.SubmitterCode });
                        await numberTypeRepository.UnitOfWork.Save();
                        identifier.NumberTypeId = numberType.NumberTypeId;
                    }
                }
            }
            additionalIdentifiers = additionalIdentifiers.Where(x => x.NumberTypeId != null);
            return additionalIdentifiers;
        }

        public async Task<SubmissionChecksum> GetChecksum(string agency, string agencyWorkCode) => mapper.Map<SubmissionChecksum>(await checksumService.GetChecksum(agency, agencyWorkCode));

        public async Task UpsertChecksum(string agency, string agencyWorkCode, string hash)
        {
            var checksum = mapper.Map<SubmissionChecksums>(await GetChecksum(agency, agencyWorkCode));
            if (checksum == null) checksum = new SubmissionChecksums(agency, agencyWorkCode, hash);
            else checksum.Hash = hash;

            await checksumService.AddChecksum(checksum);
        }


        async Task UndoReplaceChildWorks(ICollection<WorkInfo> childIswcWorks, ICollection<WorkInfo>? parentIswcWorks)
        {
            if (childIswcWorks == null) return;

            var now = DateTime.UtcNow;
            foreach (var work in childIswcWorks.Where(x => x.Status))
            {
                work.IsReplaced = false;
                work.LastModifiedDate = now;

                await workRepository.UpdateAsync(work);
            }

            await workRepository.UnitOfWork.Save();
        }

        async Task DeleteWorksOnParent(ICollection<WorkInfo> parentIswcWorks)
        {
            if (parentIswcWorks == null) return;

            var now = DateTime.UtcNow;
            foreach (var work in parentIswcWorks)
            {
                work.IsReplaced = false;
                work.LastModifiedDate = now;
                work.ArchivedIswc = null;
                work.Status = false;
                work.LastModifiedDate = now;
                foreach (var title in work.Title) { title.Status = false; title.LastModifiedDate = now; }
                foreach (var creator in work.Creator) { creator.Status = false; creator.LastModifiedDate = now; }
                foreach (var p in work.Publisher) { p.Status = false; p.LastModifiedDate = now; }
                foreach (var p in work.WorkInfoPerformer) { p.Status = false; p.LastModifiedDate = now; }
                foreach (var df in work.DerivedFrom) { df.Status = false; df.LastModifiedDate = now; }

                await workRepository.UpdateAsync(work);
                await searchService.DeleteByWorkCode(work.WorkInfoId);
            }
            await workRepository.UnitOfWork.Save();
        }
        public async Task<IswcModel?> GetCacheIswcs(string iswc)
        {
            var cacheIswc = await cacheIswcService.GetCacheIswcs(iswc);

            var mappedSub = mapper.Map<Submission>(cacheIswc);

            return mappedSub?.SearchedIswcModels.FirstOrDefault();
        }
    }
}

