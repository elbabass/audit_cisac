using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace SpanishPoint.Azure.Iswc.Data.Services.Matching.MatchingEngine.Entities
{
    internal class WorkContributor
    {
        public string Name { get; set; }
		public string LastName { get; set; }
        public long? IPINumber { get; set; }
        public string IPIBaseNumber { get; set; }
        public DateTime? IPICreatedDate { get; set; }
        public ContributorType? TypeCode { get; set; }
        public ContributorRole? Role { get; set; }
        public ContributorMatchSource? Matched { get; set; }
    }

    public enum ContributorType
    {
        Publisher = 1,
        Writer = 2
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ContributorRole
    {
        SR = 1,
        SA = 2,
        TR = 3,
        PA = 4,
        ES = 5,
        CA = 6,
        AD = 7,
        AR = 8,
        SE = 9,
        C = 10,
        A = 11,
        E = 12,
        AQ = 13,
        AM = 14
    }

    public enum ContributorMatchSource
    {
        NumberAndText = 0,
        Number = 1,
        Text = 2
    }
}
