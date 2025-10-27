using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace SpanishPoint.Azure.Iswc.Api.Tests.Documentation.GenerateDocs.Extensions
{
    [ExcludeFromCodeCoverage]
    internal static class StringExtensions
    {
        public static string GetStringAtIdx(this string @string, int idx)
        {
            var strings = @string.Split('.');
            return strings[idx];
        }

        public static string TrimEnd(this string @string, string trimString)
            => Regex.Replace(@string, $"{trimString}\\b", string.Empty);
    }
}
