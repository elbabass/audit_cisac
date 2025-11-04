using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage.Blob;
using SpanishPoint.Azure.Iswc.Bdo.Reports;
using SpanishPoint.Azure.Iswc.Business.Managers;


namespace SpanishPoint.Azure.Iswc.Portal.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportManager reportManager;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly CloudBlobContainer container;

        public ReportController(IReportManager reportManager, CloudBlobClient cloudBlobClient, IHttpContextAccessor contextAccessor)
        {
            this.reportManager = reportManager;
            this.contextAccessor = contextAccessor;
            this.container = cloudBlobClient.GetContainerReference("reporting");
        }

        [HttpPost, Authorize(AuthenticationSchemes = "Cookies")]
        public async Task<ActionResult<IEnumerable<AuditReportResult>>> SearchSubmissionAudit([FromBody] AuditReportSearchParameters searchParameters)
        {
            return Ok(await reportManager.ReportSearch(searchParameters));
        }

        [HttpPost, Authorize(AuthenticationSchemes = "Cookies")]
        public async Task<ActionResult<IEnumerable<FileAuditReportResult>>> SearchFileSubmissionAudit([FromBody] AuditReportSearchParameters searchParameters)
        {
            return Ok(await reportManager.FileAuditReportSearch(searchParameters));
        }

        [HttpPost, Authorize(AuthenticationSchemes = "Cookies")]
        public async Task<ActionResult<IEnumerable<AgencyStatisticsResult>>> GetAgencyStatistics([FromBody] AgencyStatisticsSearchParameters searchParameters)
        {
            return Ok(await reportManager.GetAgencyStatistics(searchParameters));
        }

        [HttpPost, Authorize(AuthenticationSchemes = "Cookies")]
        public async Task<ActionResult> ExtractFtp([FromBody] AuditReportSearchParameters searchParameters)
        {
            await reportManager.FtpReportExtract(searchParameters);
            return Ok();
        }

        [HttpGet, Authorize(AuthenticationSchemes = "Cookies")]
        public ActionResult<DateTimeOffset> GetDateOfCachedReport(string folder)
        {
            var blobs = container.ListBlobs(folder, useFlatBlobListing: true).OfType<CloudBlockBlob>();

            if (!blobs.Any())
            {
                var parts = folder.Split('/');
                if (parts.Length > 1)
                    blobs = container.ListBlobs(parts[0], useFlatBlobListing: true)
                        .OfType<CloudBlockBlob>()
                        .Where(x => x.Name.Contains($"_{parts[1]}_"));
            }

            return Ok(blobs.Max(x => x.Properties.Created));
        }
    }
}