using System;

namespace SpanishPoint.Azure.Iswc.Data.Services.Search.Azure.Models
{
    public class WorkNameContributorPerformerIndexModel
    {
        public string GeneratedID { get; set; }
        public long WorkID { get; set; }
        public string WorkName { get; set; }
        public int? WorkNameType { get; set; }
        public long? PersonID { get; set; }
        public string PersonFullName { get; set; }
        public int? ContributorType { get; set; }
        public string IPIBaseNumber { get; set; }
        public long? IPINumber { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTimeOffset IPICreatedDate { get; set; }
        public int? WorkType { get; set; }
        public int? PersonType { get; set; }

    }
}
