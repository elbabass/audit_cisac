using System;
using System.Collections.Generic;
using System.Linq;

namespace SpanishPoint.Azure.Iswc.Framework.Extensions
{
    public static class EnumExtensions
    {
        public static IEnumerable<T> AllValues<T>(this T source) where T : IConvertible
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static string ToFriendlyString(this Enum code)
        {
            return code.ToString().TrimStart('_');
        }

        public static string ToDesignationString(this Enum code)
        {
            return code.ToString().Replace('_', ' ');
        }
    }
}
