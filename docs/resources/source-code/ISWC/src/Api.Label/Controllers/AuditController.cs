using Microsoft.AspNetCore.Mvc;
using SpanishPoint.Azure.Iswc.Api.Label.Configuration.Swagger;
using SpanishPoint.Azure.Iswc.Bdo.Audit;
using SpanishPoint.Azure.Iswc.Business.Managers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Api.Label.Controllers
{
    /// <summary>
    /// Audit Controller
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuditController : BaseController
    {
        private readonly IAuditManager auditManager;

        /// <summary>
        /// Audit Controller
        /// </summary>
        /// <param name="auditManager"></param>
        public AuditController(IAuditManager auditManager)
        {
            this.auditManager = auditManager;
        }

        /// <summary>Search Audit History by Preferred Iswc</summary>
        /// <returns>List of logged submissions from Audit History</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditHistoryResult>>> Search([Required][FromQuery]string preferredIswc)
        {
            var response = await auditManager.Search(preferredIswc);

            if (response == null || !response.Any()) return NotFound($"No Audit records found for {preferredIswc}");

            return Ok(response);
        }
    }
}
