using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SpanishPoint.Azure.Iswc.Framework.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class TypeExtensions
    {
        private static IDictionary<Type, string> TypeCache { get; } = new Dictionary<Type, string>();
        public static string GetComponentVersion(this Type type)
        {
            lock (TypeCache)
            {
                if (!TypeCache.TryGetValue(type, out string? version))
                {
                    TypeCache[type] = type.Assembly.GetName().Version!.ToString();
                }
                return version ?? "Version could not be found.";
            }
        }

        public static T? DeepCopy<T>(this T source)
        {
            if (source == null) return default;
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
        }
    }
}
