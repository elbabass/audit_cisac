using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpanishPoint.Azure.Iswc.Bdo.Audit;
using SpanishPoint.Azure.Iswc.Business.Managers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Portal.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class AuditController : ControllerBase
    {
        private readonly IAuditManager auditManager;

        public AuditController(IAuditManager auditManager)
        {
            this.auditManager = auditManager;
        }

        [HttpGet, Authorize(AuthenticationSchemes = "Cookies")]
        public async Task<ActionResult<IEnumerable<AuditHistoryResult>>> Search([Required][FromQuery] string preferredIswc)
        {
            var response = await auditManager.Search(preferredIswc);
            if (response == null || !response.Any()) return NotFound($"No Audit records found for {preferredIswc}");
            return Ok(response);
        }
    }
}
