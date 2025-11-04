namespace SpanishPoint.Azure.Iswc.Data.Services.Matching.MatchingEngine.Entities
{
    internal class WorkNumber
    {
        public string Number { get; set; }
        public string Type { get; set; }
        public MatchSource? Matched { get; set; }
    }
}
