using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class Status
    {
        public int StatusId { get; set; }
        public DateTime AmendedDateTime { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string ForwardingBaseNumber { get; set; }
        public int StatusCode { get; set; }
        public string IpbaseNumber { get; set; }

        public virtual InterestedParty IpbaseNumberNavigation { get; set; }
    }
}
