using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpanishPoint.Azure.Iswc.Bdo.Ipi
{
    public class InterestedPartyModel
    {
        public long? ContributorID { get; set; }
        public string? IpBaseNumber { get; set; }
        public InterestedPartyType? Type { get; set; }
        public CisacInterestedPartyType? CisacType { get; set; }
        public ICollection<NameModel>? Names { get; set; }
        public bool? IsAuthoritative { get; set; }
        public StatusModel? Status { get; set; }
        public string? Agency { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public long? IPNameNumber { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? DisplayName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Affiliation { get; set; }
        private long? _age;

        public long? Age
        {
            get
            {
                if (_age.HasValue)
                    return _age;

                if (BirthDate.HasValue)
                {
                    var today = DateTime.UtcNow;
                    var computed = today.Year - BirthDate.Value.Year;
                    if (BirthDate.Value.Date > today.AddYears(-computed)) computed--;
                    return computed;
                }

                return null;
            }
            set
            {
                _age = value;
            }
        }
        public long? AgeTolerance { get; set; }
        public ContributorType? ContributorType { get; set; }
        public bool IsWriter => new CisacInterestedPartyType[] { CisacInterestedPartyType.C, CisacInterestedPartyType.TA, CisacInterestedPartyType.MA }.Contains(CisacType.GetValueOrDefault());

        public bool IsExcludedFromIswc { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public LegalEntityType LegalEntityType { get; set; }
        public bool IsPseudonymGroupMember { get; set; }

        public bool IsCreatorInterestedPartyType() => Type != null && new InterestedPartyType?[] { InterestedPartyType.C, InterestedPartyType.CA, InterestedPartyType.A, InterestedPartyType.AR,
            InterestedPartyType.SR, InterestedPartyType.AD, InterestedPartyType.SA, InterestedPartyType.TR }.Contains(Type);

        public bool IsPublisherInterestedPartyType() => Type != null && new InterestedPartyType?[] { InterestedPartyType.E, InterestedPartyType.AM }.Contains(Type);
    }
}
