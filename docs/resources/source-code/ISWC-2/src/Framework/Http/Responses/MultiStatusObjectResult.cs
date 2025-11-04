using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace SpanishPoint.Azure.Iswc.Framework.Http.Responses
{
    public  class MultiStatusObjectResult : ObjectResult
    {
        public MultiStatusObjectResult(object value) : base(value)
        {
            StatusCode = (int)HttpStatusCode.MultiStatus;
        }
    }
}
