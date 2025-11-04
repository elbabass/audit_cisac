using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class NameReference
    {
        public long IpnameNumber { get; set; }
        public DateTime AmendedDateTime { get; set; }
        public string IpbaseNumber { get; set; }

        public virtual InterestedParty IpbaseNumberNavigation { get; set; }
        public virtual Name IpnameNumberNavigation { get; set; }
    }
}
