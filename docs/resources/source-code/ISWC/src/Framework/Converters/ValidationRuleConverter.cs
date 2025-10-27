using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SpanishPoint.Azure.Iswc.Framework.Converters
{
    public static class ValidationRuleConverter
    {
        public static IEnumerable<string> GetValues_IncludeAgenciesInEligibilityCheck(string parameterValue, string agencyCode)
        {
            foreach (var agencyGroup in parameterValue.Split(';'))
            {
                if (agencyGroup.Length < 3) continue;

                var agency = agencyGroup.Substring(0, 3);
                var childAgencies = Regex.Replace(agencyGroup.Substring(3), "[() ]", string.Empty).Split(',');

                if (agency == agencyCode)
                    return childAgencies.Concat(new string[] { agency });
            }
            return new List<string> { agencyCode };
        }
    }
}
