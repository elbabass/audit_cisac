namespace SpanishPoint.Azure.Iswc.Data.Services.Matching.MatchingEngine.Entities
{
    internal class Performer
    {
        public string FirstName { get; set; }
        public string Name { get; set; }
        public MatchSource? Matched { get; set; }
    }
}
