using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SpanishPoint.Azure.Iswc.Framework.Security
{
    [ExcludeFromCodeCoverage]
    public static class RequestFilterMiddlewareExtension
    {
        public static IApplicationBuilder UseRequestFilterMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestFilterMiddleware>();
        }
    }
}
