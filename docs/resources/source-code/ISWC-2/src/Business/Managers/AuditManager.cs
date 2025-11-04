using Azure.Storage.Blobs;
using SpanishPoint.Azure.Iswc.Bdo.Audit;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Data.Repositories;
using SpanishPoint.Azure.Iswc.Data.Services.Audit;
using SpanishPoint.Azure.Iswc.Data.Services.ReportingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Business.Managers
{
    public interface IAuditManager
    {
        Task LogSubmissions(IEnumerable<Submission> submissions);
        Task<IEnumerable<AuditHistoryResult>> Search(string preferredIswc);
        Task ProcessAuditChanges(BlobContainerClient container);
        Task GenerateAgencyStatistics();

    }

    public class AuditManager : IAuditManager
    {
        private readonly IAuditService auditService;
        private readonly IWorkRepository workRepository;
        private readonly INameRepository nameRepository;
        private readonly IReportingService reportingService;
        private readonly IHighWatermarkRepository highWatermarkRepository;
        private readonly IWorkflowInstanceRepository workflowInstanceRepository;

        public AuditManager(
            IAuditService auditService,
            IWorkRepository workRepository,
            INameRepository nameRepository,
            IReportingService reportingService,
            IHighWatermarkRepository highWatermarkRepository,
            IWorkflowInstanceRepository workflowInstanceRepository)
        {
            this.auditService = auditService;
            this.workRepository = workRepository;
            this.nameRepository = nameRepository;
            this.reportingService = reportingService;
            this.highWatermarkRepository = highWatermarkRepository;
            this.workflowInstanceRepository = workflowInstanceRepository;
        }

        public async Task GenerateAgencyStatistics()
        {
            var highWatermark = await highWatermarkRepository.FindAsync(x => x.Name == "AgencyStatisticsHighWaterMark");

            if (highWatermark is null || highWatermark.Value == default)
                throw new ArgumentException("AgencyStatsWatermark missing from configuration table.");

            await auditService.GenerateAgencyStatistics(highWatermark);
            await auditService.GenerateWorkflowStatistics();
        }

        public async Task LogSubmissions(IEnumerable<Submission> submissions) => await auditService.LogSubmissions(submissions);

        public async Task ProcessAuditChanges(BlobContainerClient container)
        {
            var highWatermark = await highWatermarkRepository.FindAsync(x => x.Name == "AuditHighWatermark");

            if (highWatermark is null || highWatermark.Value == default)
                throw new ArgumentException("AuditHighWatermark missing from configuration table.");

            await reportingService.ProcessAuditChanges(container, highWatermark);
        }

        public async Task<IEnumerable<AuditHistoryResult>> Search(string preferredIswc)
        {
            var submissions = await workRepository.FindManyAsyncOptimized(x => x.Iswc.Iswc1 == preferredIswc);

            var auditHistoryResults = await auditService.Search(preferredIswc, submissions);

            foreach (var res in auditHistoryResults)
            {
                res.Status = Bdo.Work.InstanceStatus.Approved;

                if (res.TransactionType != Bdo.Edi.TransactionType.CAR) res.Status = await GetWorkflowInstanceStatus(res.WorkflowInstanceId, res);

                var creators = new List<InterestedPartyModel>();
                foreach (var ip in res.Creators)
                {
                    if (creators.Any(x => x.IPNameNumber == ip.IPNameNumber && x.IpBaseNumber == ip.IpBaseNumber))
                        continue;

                    if (string.IsNullOrWhiteSpace(ip.Name))
                    {
                        var name = await nameRepository.FindAsyncOptimized(n => n.IpnameNumber.Equals(ip.IPNameNumber));

                        if (name != null)
                            ip.Name = name.LastName + ' ' + name.FirstName;
                    }

                    creators.Add(ip);
                }

                res.Creators = creators;
            }

            return auditHistoryResults.OrderBy(x => x.SubmittedDate);

            async Task<Bdo.Work.InstanceStatus> GetWorkflowInstanceStatus(long? workflowInstanceId, AuditHistoryResult auditHistoryResult)
            {
                if (workflowInstanceId != null)
                {
                    var workflowInstance = await workflowInstanceRepository.FindAsync(x => x.WorkflowInstanceId == workflowInstanceId);

                    if (workflowInstance != null)
                        return (Bdo.Work.InstanceStatus)workflowInstance.InstanceStatus;
                }

                return Bdo.Work.InstanceStatus.Approved;
            }
        }
    }
}