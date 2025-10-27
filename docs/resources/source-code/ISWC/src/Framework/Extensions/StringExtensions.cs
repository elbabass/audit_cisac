using Newtonsoft.Json;
using SpanishPoint.Azure.Iswc.Framework.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpanishPoint.Azure.Iswc.Framework.Extensions
{
    public static class StringExtensions
    {
        private static readonly Regex iswcPattern = new Regex("[T][0-9]{10}", RegexOptions.Compiled);
        public static bool IsValidIswcPattern(this string iswc) => iswcPattern.Match(iswc).Success;

        public static T Deserialize<T>(this string @param) => TConverter.ChangeType<T>(param);

        public static IEnumerable<T> DeserializeEnumerable<T>(this string @param)
        {
            foreach (var item in param.Replace(" ", "").Split(','))
            {
                yield return Deserialize<T>(item);
            }
        }

        private static readonly IEnumerable<string> charactersToReplace = new List<string>
        {
            "\u0001","\u0002","\u0003","\u0004","\u0005","\u0006","\u0007","\u0008","\u0009","\u000A",
            "\u000B","\u000C","\u000D","\u000E","\u000F","\u0010","\u0011","\u0012","\u0013","\u0014",
            "\u0015","\u0016","\u0017","\u0018","\u0019","\u0009","\u001A","\u001B","\u001C","\u001D",
            "\u001E","\u001F",
        };

        public static string ReplaceUnicode(this string @string)
        {
            foreach (var character in charactersToReplace)
            {
                @string = @string.Replace(character, "");
            }
            return @string;
        }

        public static string IncrementIswc(this string iswc)
        {
            var nextnum = GetIswcNumber(iswc) + 1;
            var nextNumZeroFilled = nextnum.ToString("D9");
            return $"T{nextNumZeroFilled}{GetCheckDigit(nextNumZeroFilled)}";

            static int GetIswcNumber(string @str)
            {
                return int.Parse(@str.Substring(1, @str.Length - 2));
            }

            static int GetCheckDigit(string @str)
            {
                var sum = 1 + str.Select((num, idx) => ToInt(num) * (idx + 1)).Sum();
                var checkDigit = (10 - sum % 10) % 10;
                return checkDigit;
            }

            static int ToInt(char c)
            {
                return c - '0';
            }
        }

        public static string RemoveAlphaNumericChars(string str)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9]");
            str = rgx.Replace(str, "");

            return str;
        }

        public static bool StringComparisonExact(string? s1, string? s2)
        {
            if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
                return false;

            return (s1.ToUpperInvariant().Trim().Equals(s2.ToUpperInvariant().Trim()));
        }

        public static bool StringComparisonExactSanitised(string? s1, string? s2)
        {
            if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
                return false;

            return (Sanitise(s1).ToUpperInvariant().Trim().Equals(Sanitise(s2).ToUpperInvariant().Trim()));
        }

        private static readonly Regex searchQuerySpecialCharacters = new Regex(@"[""#%()\-*+,/;<=>?@\\^_`{|}~'\][]", RegexOptions.Compiled);

        public static string Sanitise(this string text)
            => searchQuerySpecialCharacters.Replace(text, " ");

        public static string AddPropertyToJson(this string text, string propertyName, string propertyValue)
        {
            dynamic json = JsonConvert.DeserializeObject(text);
            json[propertyName] = propertyValue;
            return json.ToString();
        }

        private static readonly Regex wordSeperators = new Regex(@"([., !?¿<>£$%^&*|:;+()'\-^@#~{}\[\]])", RegexOptions.Compiled);
        public static string[] SplitWordOnSeparators(this string text) => wordSeperators.Split(text);

        public static TEnum StringToEnum<TEnum>(this string? str) where TEnum : struct
        {
            return Enum.TryParse(str, out TEnum @enum) ? @enum : default;
        }

        /// <summary>
        /// Source: https://stackoverflow.com/a/4335913/2509703
        /// </summary>
        /// <param name="target"></param>
        /// <param name="trimString"></param>
        /// <returns></returns>
        public static string TrimEnd(this string target, string trimString)
        {
            if (string.IsNullOrEmpty(trimString)) return target;

            string result = target;
            while (result.EndsWith(trimString))
            {
                result = result.Substring(0, result.Length - trimString.Length);
            }

            return result;
        }

        public static string CreateSemiColonSeperatedString(IEnumerable<string> values)
        {
            StringBuilder sb = new StringBuilder();

            if (values == null || !values.Any())
                return string.Empty;

            sb.AppendJoin(';', values);

            return sb.ToString();
        }

        public static DateTime? TryParseDateTimeExact(this string dateTime, string format)
        {
            return DateTime.TryParseExact(dateTime, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt) ? dt : (DateTime?)null;
        }
    }
}