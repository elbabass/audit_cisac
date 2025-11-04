using SpanishPoint.Azure.Iswc.Bdo.ComplexWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpanishPoint.Azure.Iswc.Data.Services.Search.Azure.Models
{
    public class ComplexWorksIndexModel
    {
        public string GeneratedID { get; set; }
        public long WorkID { get; set; }
        public string MedleyType { get; set; }
        public string SocietyAccountNumber { get; set; }
        public bool? ExistsInDatabase { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsEligible { get; set; }
        public int? IswcStatusID { get; set; }
        public IEnumerable<Name> WorkNames { get; set; }
        public IEnumerable<WorkNumber> WorkNumbers { get; set; }
        public IEnumerable<WorkContributor> WorkContributors { get; set; }
        public IEnumerable<WorkPerformer> WorkPerformers { get; set; }
    }
}
