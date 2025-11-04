using SpanishPoint.Azure.Iswc.Framework.Converters;
using System;
using System.Reflection;

namespace SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Extensions
{
    internal static class StringExtensions
    {
        public static T GetField<T>(this string @string, int start, int length, string dateTimeFormat = null)
        {
            var trimmedField = @string.Substring(start - 1, length).Trim();

            if (dateTimeFormat != null)
            {
                if (Type.GetTypeCode(typeof(T)) == TypeCode.DateTime)
                    return string.IsNullOrEmpty(trimmedField) ? default : (T)Convert.ChangeType(DateTime.ParseExact(trimmedField, dateTimeFormat, null), typeof(T));
                else
                    return string.IsNullOrEmpty(trimmedField) ? default : TConverter.ChangeType<T>(DateTime.ParseExact(trimmedField, dateTimeFormat, null));
            }
            else if (typeof(T).GetTypeInfo().IsEnum)
                return string.IsNullOrEmpty(trimmedField) ? default : (T)Enum.Parse(typeof(T), trimmedField);
            else
                return string.IsNullOrEmpty(trimmedField) ? default : TConverter.ChangeType<T>(trimmedField);
        }
    }
}

