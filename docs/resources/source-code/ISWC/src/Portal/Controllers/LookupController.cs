using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;

namespace SpanishPoint.Azure.Iswc.Portal.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class LookupController : ControllerBase
    {
        private readonly ILookupManager lookupManager;

        public LookupController(ILookupManager lookupManager)
        {
            this.lookupManager = lookupManager;
        }

        public async Task<ActionResult<IEnumerable<LookupData>>> GetLookupData()
        {
            return Ok(await lookupManager.GetLookups());
        }


        [HttpPost, Authorize(AuthenticationSchemes = "Cookies")]
        public async Task<ActionResult<IEnumerable<InterestedPartyModel>>> GetIps(InterestedPartySearchModel interestedPartySearchModel)
        {
            return Ok(await lookupManager.GetIps(interestedPartySearchModel));
        }
    }
}
