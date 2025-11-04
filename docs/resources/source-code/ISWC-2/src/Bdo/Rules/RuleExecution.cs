using System;

namespace SpanishPoint.Azure.Iswc.Bdo.Rules
{
    public class RuleExecution
    {
        public string? RuleName { get; set; }
        public string? RuleVersion { get; set; }
		public string? RuleConfiguration { get; set; }
        public TimeSpan TimeTaken { get; set; }
    }
}
