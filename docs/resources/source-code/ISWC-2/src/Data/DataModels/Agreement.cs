using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class Agreement
    {
        public long AgreementId { get; set; }
        public DateTime AmendedDateTime { get; set; }
        public string CreationClass { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string EconomicRights { get; set; }
        public string Role { get; set; }
        public decimal SharePercentage { get; set; }
        public DateTime? SignedDate { get; set; }
        public string AgencyId { get; set; }
        public string IpbaseNumber { get; set; }

        public virtual Agency Agency { get; set; }
        public virtual InterestedParty IpbaseNumberNavigation { get; set; }
    }
}
