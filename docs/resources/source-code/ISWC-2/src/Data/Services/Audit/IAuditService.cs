using SpanishPoint.Azure.Iswc.Bdo.Audit;
using SpanishPoint.Azure.Iswc.Bdo.Reports;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Data.Services.Audit
{
    public interface IAuditService
    {
        Task LogSubmissions(IEnumerable<Submission> submissions);
        Task<IEnumerable<AuditHistoryResult>> Search(string preferrdIswc, IEnumerable<WorkInfo> submissions);
        Task<IEnumerable<AuditReportResult>> Search(AuditReportSearchParameters searchParameters);
        Task<IEnumerable<FileAuditReportResult>> SearchFileAudit(AuditReportSearchParameters searchParameters);
        Task<IEnumerable<AgencyStatisticsResult>> GetAgencyStatistics(AgencyStatisticsSearchParameters searchParameters);
        Task GenerateAgencyStatistics(HighWatermark highWatermark);
        Task GenerateWorkflowStatistics();
    }
}
