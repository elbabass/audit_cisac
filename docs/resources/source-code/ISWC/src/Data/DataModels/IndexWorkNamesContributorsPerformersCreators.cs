using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class IndexWorkNamesContributorsPerformersCreators
    {
        public string GeneratedId { get; set; }
        public long WorkId { get; set; }
        public string WorkName { get; set; }
        public int WorkNameType { get; set; }
        public long? PersonId { get; set; }
        public string IpibaseNumber { get; set; }
        public DateTime IpicreatedDate { get; set; }
        public int? WorkType { get; set; }
        public string PersonFullName { get; set; }
        public string LastName { get; set; }
        public int PersonType { get; set; }
        public int ContributorType { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime Concurrency { get; set; }
        public long? Ipinumber { get; set; }
        public bool? IsMaintained { get; set; }
        public int? SocietyAccountNumber { get; set; }
        public string RoleType { get; set; }
    }
}
