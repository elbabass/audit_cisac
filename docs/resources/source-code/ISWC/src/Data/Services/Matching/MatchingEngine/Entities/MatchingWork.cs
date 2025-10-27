using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.Services.Matching.MatchingEngine.Entities
{
    internal class MatchingWork
    {
        public MatchingWork()
        {
            Titles = new List<Name>();
            Contributors = new List<WorkContributor>();
            Artists = new List<Performer>();
            Numbers = new List<WorkNumber>();
        }
        public long Id { get; set; }
        public MatchSource MatchType { get; set; }
        public IEnumerable<Name> Titles { get; set; }
        public IEnumerable<WorkContributor> Contributors { get; set; }
        public IEnumerable<Performer> Artists { get; set; }
        public IEnumerable<WorkNumber> Numbers { get; set; }
        public int? IswcStatusID { get; set; }
        public bool IsDefinite { get; set; }
        public bool SkipContributorCountRules { get; set; }
        public bool ExcludeMatchesOTBelowSimilarity { get; set; }
        public bool ExcludeInEligibleWorks { get; set; }
        public string StandardizedTitle { get; set; }
        public bool SkipStandardizedTitleSearch { get; set; }
    }
}
