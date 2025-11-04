using System;
using System.Collections.Generic;
using System.Text;

namespace SpanishPoint.Azure.Iswc.Bdo.ComplexWork
{
    public class WorkContributor
    {
        public WorkContributor()
        {
            IPIBaseNumber = string.Empty;
            PersonFullName = string.Empty;
            LastName = string.Empty;
            RoleType = string.Empty;
        }
        public long PersonID { get; set; }
        public string IPIBaseNumber { get; set; }
        public DateTimeOffset IPICreatedDate { get; set; }
        public string PersonFullName { get; set; }
        public string LastName { get; set; }
        public int ContributorType { get; set; }
        public long? IPINumber { get; set; }
        public string RoleType { get; set; }
    }
}
