namespace SpanishPoint.Azure.Iswc.Bdo.Ipi
{
    public class InterestedPartySearchModel
    {
        public string? IpBaseNumber { get; set; }
        public InterestedPartyType? Type { get; set; }
        public bool? IsAuthoritative { get; set; }
        public string? Affiliatiion { get; set; }
        public long? IPNameNumber { get; set; }
        public string? Name { get; set; }
        public ContributorType ContributorType { get; set; }
    }
}
