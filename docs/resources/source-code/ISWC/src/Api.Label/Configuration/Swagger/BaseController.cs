using Microsoft.AspNetCore.Mvc;
using SpanishPoint.Azure.Iswc.Framework.Http.Responses;

namespace SpanishPoint.Azure.Iswc.Api.Label.Configuration.Swagger
{
    /// <summary>
    /// Base Controller
    /// </summary>
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        internal const string isPublicRequestContextItemKey = "isPublicRequest";
        internal const string agentVersionContextItemKey = "agentVersion";
        internal const string requestSourceContextItemKey = "requestSource";
        internal ActionResult MultiStatus(object value)
        {
            return new MultiStatusObjectResult(value);
        }
    }
}
