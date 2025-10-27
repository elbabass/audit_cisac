using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace SpanishPoint.Azure.Iswc.Framework.MaintenanceMode.Middleware
{
    [ExcludeFromCodeCoverage]
    public static class MaintenanceModeExtensions
    {
        public static IApplicationBuilder UseMaintenanceMode(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MaintenanceMode>();
        }
    }
}
