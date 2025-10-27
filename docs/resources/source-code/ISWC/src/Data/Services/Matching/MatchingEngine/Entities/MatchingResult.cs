using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.Services.Matching.MatchingEngine.Entities
{
    public class MatchingResult
    {
        public long ID { get; set; }
        public double TimeTaken { get; set; }
        public IEnumerable<MatchedEntity> MatchedEntities { get; set; }
        public Exception Error { get; set; }
    }

    public class MatchedEntity
    {
        public string MatchedBy { get; set; }
        public bool IsDefinite { get; set; }
        public long EntityID { get; set; }
        public int? TotalQGRam { get; set; }
        public int? SimilarityScore { get; set; }
    }
}
