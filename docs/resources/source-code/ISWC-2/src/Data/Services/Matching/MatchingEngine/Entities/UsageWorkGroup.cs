using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.Services.Matching.MatchingEngine.Entities
{
    public class UsageWorkGroup
    {
        public long ID { get; set; }
        public string DistributionCode { get; set; }
        public string FullTitle { get; set; }
        public string FullComposer { get; set; }
        public string FullPerformer { get; set; }
        public string SourceMajor { get; set; }
        public string SourceMinor { get; set; }
        public IEnumerable<WorkName> Titles { get; set; }
        public IEnumerable<Person> Composers { get; set; }
        public IEnumerable<Person> Performers { get; set; }
        public IEnumerable<PerformanceNumber> PerformanceNumbers { get; set; }
        public IEnumerable<PerformanceForeign> Foreigns { get; set; }
    }
}
