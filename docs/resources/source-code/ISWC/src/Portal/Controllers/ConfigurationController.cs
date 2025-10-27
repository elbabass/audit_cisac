using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using SpanishPoint.Azure.Iswc.Bdo.Portal;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Caching.Configuration;
using SpanishPoint.Azure.Iswc.Portal.Configuration;
using SpanishPoint.Azure.Iswc.Portal.Configuration.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Controllers
{
    [Route("[controller]/[action]")]
    public class ConfigurationController : Controller
    {
        private readonly IStringLocalizer localizer;
        private readonly IOptions<ClientAppOptions> clientAppOptions;
        private readonly IPortalMessageManager portalMessageManager;

        public ConfigurationController(
            IStringLocalizer<PortalStrings> localizer,
            IOptions<ClientAppOptions> clientAppOptions,
            IPortalMessageManager portalMessageManager
            )
        {
            this.localizer = localizer;
            this.clientAppOptions = clientAppOptions;
            this.portalMessageManager = portalMessageManager;
        }

        [HttpGet]
        [ResponseCache(Duration = CacheDuration.TwentyFourHours)]
        public IEnumerable<LocalizedString> GetLocalizedStrings()
        {
            return localizer.GetAllStrings();
        }

        [HttpGet]
        [ResponseCache(Duration = CacheDuration.TwentyFourHours)]
        public ClientAppOptions GetClientAppConfiguration()
        {
            return clientAppOptions.Value;
        }

        [HttpGet]
        [ResponseCache(Duration = CacheDuration.TwentyFourHours)]
        public async Task<ActionResult<IEnumerable<Message>>> GetPortalMessages(string env, string culture = "en")
        {
            return Ok(await portalMessageManager.GetPortalMessages(env, culture));
        }
    }
}