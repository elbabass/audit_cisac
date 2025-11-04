using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;

namespace SpanishPoint.Azure.Iswc.Framework.Http.Middleware
{
    [ExcludeFromCodeCoverage]
    public static class JsonBodyTelemetryExtensions
    {
        public static IApplicationBuilder UseJsonBodyTelemetry(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JsonBodyTelemetry>();
        }
    }
}
