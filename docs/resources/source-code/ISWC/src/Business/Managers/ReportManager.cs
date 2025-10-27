using AutoMapper;
using SpanishPoint.Azure.Iswc.Bdo.Reports;
using SpanishPoint.Azure.Iswc.Data.Services.Audit;
using SpanishPoint.Azure.Iswc.Framework.Databricks;
using SpanishPoint.Azure.Iswc.Framework.Databricks.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Business.Managers
{
    public interface IReportManager
    {
        Task<IEnumerable<AuditReportResult>> ReportSearch(AuditReportSearchParameters searchParameters);
        Task<IEnumerable<FileAuditReportResult>> FileAuditReportSearch(AuditReportSearchParameters searchParameters);
        Task FtpReportExtract(AuditReportSearchParameters searchParameters);
        Task<IEnumerable<AgencyStatisticsResult>> GetAgencyStatistics(AgencyStatisticsSearchParameters searchParameters);
    }

    public class ReportManager : IReportManager
    {
        private readonly IAuditService auditService;
        private readonly IDatabricksClient databricksClient;
        private readonly IMapper mapper;

        public ReportManager(IAuditService auditService, IDatabricksClient databricksClient, IMapper mapper)
        {
            this.databricksClient = databricksClient;
            this.mapper = mapper;
            this.auditService = auditService;
        }

        public async Task<IEnumerable<AuditReportResult>> ReportSearch(AuditReportSearchParameters searchParameters)
        {
            return await auditService.Search(searchParameters);
        }

        public async Task<IEnumerable<AgencyStatisticsResult>> GetAgencyStatistics(AgencyStatisticsSearchParameters searchParameters)
        {
            return await auditService.GetAgencyStatistics(searchParameters);
        }

        public Task<IEnumerable<FileAuditReportResult>> FileAuditReportSearch(AuditReportSearchParameters searchParameters)
        {
            return auditService.SearchFileAudit(searchParameters);
        }

        public async Task FtpReportExtract(AuditReportSearchParameters searchParameters)
        {
            var requestModel = new SubmitJobRequestModel
            {
                NoteBookParameters = mapper.Map<NotebookParameters>(searchParameters)
            };

            var activeRuns = await databricksClient.GetActiveRuns();

            if (!activeRuns
                .Any(x =>
                    x.OverridingParameters.NotebookParameters.Equals(requestModel.NoteBookParameters) ||
                    x.Task.NotebookTask.BaseParameters.Equals(requestModel.NoteBookParameters)))
                await databricksClient.SubmitJob(requestModel);
        }
    }
}
