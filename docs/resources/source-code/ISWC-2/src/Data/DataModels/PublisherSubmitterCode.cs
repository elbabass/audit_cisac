using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class PublisherSubmitterCode
    {
        public int PublisherSubmitterCodeId { get; set; }
        public string Code { get; set; }
        public string Publisher { get; set; }
        public long? IpnameNumber { get; set; }
    }
}
