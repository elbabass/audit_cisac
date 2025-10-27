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
            if (contextAccessor.HttpContext != null)
            {
                var requestItem = contextAccessor.HttpContext.Items.FirstOrDefault(x => x.Key.ToString() == key);

                if (!requestItem.Value.Equals(default))
                {
                    var result = requestItem.Value?.ToString();

                    if (!string.IsNullOrWhiteSpace(result))
                        return result.Deserialize<T>();
                }
            }

            return default!;
        }

        public static string? GetRequestItem(this IHttpContextAccessor contextAccessor, string key)
        {
            if (contextAccessor.HttpContext != null)
            {
                var requestItem = contextAccessor.HttpContext.Items.FirstOrDefault(x => x.Key.ToString() == key);

                if (!requestItem.Value.Equals(default))
                    return requestItem.Value.ToString();
            }

            return default!;
        }
    }
}
