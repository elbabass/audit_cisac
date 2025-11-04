namespace SpanishPoint.Azure.Iswc.Data.Services.Matching.MatchingEngine.Entities
{
    internal class Name
    {
        public string Title { get; set; }
        public string Type { get; set; }
        public MatchSource? Matched { get; set; }
        public string StandardizedTitle { get; set; }
    }
}