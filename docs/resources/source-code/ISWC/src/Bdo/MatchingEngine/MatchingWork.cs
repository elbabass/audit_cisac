using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Bdo.MatchingEngine
{
    public class MatchingWork
    {
        public MatchingWork()
        {
            Titles = new List<Title>();
            Contributors = new List<InterestedPartyModel>();
            Artists = new List<Performer>();
            Numbers = new List<WorkNumber>();
        }
        public long Id { get; set; }
        public MatchSource? MatchType { get; set; }
        public IEnumerable<Title> Titles { get; set; }
        public IEnumerable<InterestedPartyModel> Contributors { get; set; }
        public IEnumerable<Performer> Artists { get; set; }
        public IEnumerable<WorkNumber> Numbers { get; set; }
        public bool IsDefinite { get; set; }
        public int? RankScore { get; set; }
        public string? StandardizedTitle { get; set; }
        public int? IswcStatus { get; set; }
    }
}
