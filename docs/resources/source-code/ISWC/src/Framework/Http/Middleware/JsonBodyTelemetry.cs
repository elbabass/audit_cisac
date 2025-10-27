using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;

namespace SpanishPoint.Azure.Iswc.Framework.Http.Middleware
{
    [ExcludeFromCodeCoverage]
    internal class JsonBodyTelemetry
    {
        private readonly RequestDelegate next;

        public JsonBodyTelemetry(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if ((context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put || context.Request.Method == HttpMethods.Patch) && context.Request.Body.CanRead)
            {
                var requestTelemetry = context.Features.Get<RequestTelemetry>();
                if (requestTelemetry != null)
                {
                    requestTelemetry.Properties.Add("RequestBody", await GetRequestBodyForTelemetry(context));

                    Stream originalBody = context.Response.Body;

                    try
                    {
                        using var memStream = new MemoryStream();
                        context.Response.Body = memStream;

                        await next(context);

                        memStream.Position = 0;
                        requestTelemetry.Properties.Add("ResponseBody", new StreamReader(memStream).ReadToEnd());

                        memStream.Position = 0;
                        await memStream.CopyToAsync(originalBody);
                    }
                    finally
                    {
                        context.Response.Body = originalBody;
                    }
                }
                else
                    await next(context);
            }
            else
                await next(context);
        }

        private async Task<string> GetRequestBodyForTelemetry(HttpContext context)
        {
            context.Request.EnableBuffering();
            using var stream = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await stream.ReadToEndAsync();
            context.Request.Body.Position = 0;
            return body;
        }
    }
}
