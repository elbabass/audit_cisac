namespace SpanishPoint.Azure.Iswc.Data.Services.Matching.MatchingEngine.Entities
{
    public class Contributor
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Type { get; set; }
        public long IPINumber { get; set; }
        public long? IPCode { get; set; }
        public string IPBaseNumber { get; set; }
        public string LegalEntityType { get; set; }
    }
}
