using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class InterestedParty
    {
        public InterestedParty()
        {
            Agreement = new HashSet<Agreement>();
            Creator = new HashSet<Creator>();
            IpnameUsage = new HashSet<IpnameUsage>();
            NameReference = new HashSet<NameReference>();
            Publisher = new HashSet<Publisher>();
            Status = new HashSet<Status>();
        }

        public string IpbaseNumber { get; set; }
        public DateTime AmendedDateTime { get; set; }
        public DateTime? BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public string BirthState { get; set; }
        public DateTime? DeathDate { get; set; }
        public string Gender { get; set; }
        public string Type { get; set; }
        public string AgencyId { get; set; }

        public virtual Agency Agency { get; set; }
        public virtual ICollection<Agreement> Agreement { get; set; }
        public virtual ICollection<Creator> Creator { get; set; }
        public virtual ICollection<IpnameUsage> IpnameUsage { get; set; }
        public virtual ICollection<NameReference> NameReference { get; set; }
        public virtual ICollection<Publisher> Publisher { get; set; }
        public virtual ICollection<Status> Status { get; set; }
    }
}
