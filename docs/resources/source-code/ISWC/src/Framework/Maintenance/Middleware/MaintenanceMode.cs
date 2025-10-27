using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Framework.MaintenanceMode.Middleware
{
    [ExcludeFromCodeCoverage]
    public class MaintenanceMode
    {
        private readonly IConfiguration config;
        private readonly RequestDelegate next;
        private const string parameterName = "MaintenanceModeEnabled";

        public MaintenanceMode(IConfiguration config, RequestDelegate next)
        {
            this.config = config;
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (IsMaintenanceModeEnabled())
            {
                context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    code = "503",
                    message = "The system is currently undergoing scheduled maintenance."
                };

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response));
                await context.Response.Body.WriteAsync(body, 0, body.Length);

                return;
            }

            await next(context);
        }

        private bool IsMaintenanceModeEnabled()
        {
            var value = config[parameterName];
            return !string.IsNullOrWhiteSpace(value) && value.Deserialize<bool>();
        }
    }
}
