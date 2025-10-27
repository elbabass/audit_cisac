using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Bdo.MatchingEngine
{
    public class MatchResult
    {
        public MatchResult()
        {
            Matches = new List<MatchingWork>();
        }
        public long InputWorkId { get; set; }
        public TimeSpan? MatchTime { get; set; }
        public string? ErrorMessage { get; set; }
        public IEnumerable<MatchingWork> Matches { get; set; }
        public string? StandardizedName { get; set; }
    }
}
