using SpanishPoint.Azure.Iswc.Bdo.ComplexWork;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Bdo.Work
{
    public class WorkModel
    {
        public WorkModel()
        {
            PreferredIswc = string.Empty;
            Titles = new List<Title>();
            WorkNumbers = new List<AdditionalAgencyWorkNumber>();
        }
        public string PreferredIswc { get; set; }
        public ICollection<Title> Titles { get; set; }
        public ICollection<AdditionalAgencyWorkNumber> WorkNumbers { get; set; }
    }
}
