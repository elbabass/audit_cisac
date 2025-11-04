using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpanishPoint.Azure.Iswc.Api.Agency.Configuration.Swagger;
using SpanishPoint.Azure.Iswc.Business.Managers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Controllers
{
    /// <summary>
    /// Ipi Synchronisation Controller
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class IpiSynchronisationController : BaseController
    {
        private readonly ISynchronisationManager synchronisationManager;
        private readonly ILogger<IpiSynchronisationController> logger;

        /// <summary>
        /// Ipi Synchronisation Controller
        /// </summary>
        /// <param name="synchronisationManager"></param>
        /// <param name="logger"></param>
        public IpiSynchronisationController(
            ISynchronisationManager synchronisationManager,
            ILogger<IpiSynchronisationController> logger)
        {
            this.synchronisationManager = synchronisationManager;
            this.logger = logger;
        }

        /// <summary>AddSpecifiedIps</summary>
        /// <returns>No content</returns>
        [HttpPost]
        public async Task<IActionResult> AddSpecifiedIps(List<string> baseNumbers)
        {
            try
            {
                await synchronisationManager.AddSpecifiedIps(baseNumbers, logger);
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while adding specified IPs.");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
    }
}