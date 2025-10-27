using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class IndexContributors
    {
        public string GeneratedId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Type { get; set; }
        public long Ipinumber { get; set; }
        public int? Ipcode { get; set; }
        public string IpbaseNumber { get; set; }
        public DateTime Concurrency { get; set; }
        public bool? IsDeleted { get; set; }
        public string LegalEntityType { get; set; }
    }
}
