using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SpanishPoint.Azure.Iswc.Framework.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class HttpContextExtensions
    {
        public static T GetRequestItem<T>(this IHttpContextAccessor contextAccessor, string key) where T : struct
        {
            if (contextAccessor.HttpContext != null &&
                contextAccessor.HttpContext.Items.TryGetValue(key, out var value) &&
                value is string str && !string.IsNullOrEmpty(str))
            {
                return str.Deserialize<T>();
            }

            return default;
        }

        public static string? GetRequestItem(this IHttpContextAccessor contextAccessor, string key)
        {
            if (contextAccessor.HttpContext != null &&
                contextAccessor.HttpContext.Items.TryGetValue(key, out var value) &&
                value is string str && !string.IsNullOrEmpty(str))
            {
                return str;
            }

            return default!;
        }
    }
}
