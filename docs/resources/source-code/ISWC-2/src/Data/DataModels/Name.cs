using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class Name
    {
        public Name()
        {
            Creator = new HashSet<Creator>();
            IpnameUsage = new HashSet<IpnameUsage>();
            NameReference = new HashSet<NameReference>();
            Publisher = new HashSet<Publisher>();
        }

        public long IpnameNumber { get; set; }
        public DateTime AmendedDateTime { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string TypeCode { get; set; }
        public long? ForwardingNameNumber { get; set; }
        public string AgencyId { get; set; }

        public virtual Agency Agency { get; set; }
        public virtual ICollection<Creator> Creator { get; set; }
        public virtual ICollection<IpnameUsage> IpnameUsage { get; set; }
        public virtual ICollection<NameReference> NameReference { get; set; }
        public virtual ICollection<Publisher> Publisher { get; set; }
    }
}
