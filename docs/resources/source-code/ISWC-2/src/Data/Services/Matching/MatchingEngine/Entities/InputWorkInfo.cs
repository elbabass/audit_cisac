using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.Services.Matching.MatchingEngine.Entities
{
    internal class InputWorkInfo : MatchingWork
    {
        public string Source => "ISWC Database";
        public string SubSource { get; set; }
        public IEnumerable<WorkNumber> DisambiguateFromNumbers { get; set; }
        public string WorkType { get; set; }
        public bool IndependentWorkNumberMatch { get; set; }
    }
}
