using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpanishPoint.Azure.Iswc.Api.ThirdParty.Configuration.Swagger;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Caching.Configuration;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Api.ThirdParty.Controllers
{
    /// <summary>
    /// Lookup Controller
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LookupController : BaseController
    {
        private readonly ILookupManager lookupMananger;
        private readonly IMessagingManager messagingManager;

        /// <summary>
        /// Lookup Controller
        /// </summary>
        /// <param name="lookupMananger"></param>
        /// <param name="messagingManager"></param>
        public LookupController(ILookupManager lookupMananger, IMessagingManager messagingManager)
        {
            this.lookupMananger = lookupMananger;
            this.messagingManager = messagingManager;
        }

        /// <summary>Get Lookup Data</summary>
        /// <returns>Lookup Data</returns>
        [HttpGet]
        [AllowAnonymous]
        [ResponseCache(Duration = CacheDuration.TwentyFourHours)]
        public async Task<ActionResult<LookupData>> GetLookupData()
        {
            return Ok(await lookupMananger.GetLookups());
        }

        /// <summary>Get IPs</summary>
        /// <returns>List creators or publishers</returns>
        [HttpPost]
        public async Task<ActionResult<IEnumerable<InterestedPartyModel>>> GetIps(InterestedPartySearchModel interestedPartySearchModel)
        {
            return Ok(await lookupMananger.GetIps(interestedPartySearchModel));
        }

        /// <summary>
        /// Get Error Codes
        /// </summary>
        /// <returns>List of error codes and their corresponding message.</returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rejection>>> GetErrorCodes() =>
            Ok(await Enum.GetValues(typeof(ErrorCode))
                .OfType<ErrorCode>()
                .Select(async x => await messagingManager.GetRejectionMessage(x))
                .WhenAll());
    }
}