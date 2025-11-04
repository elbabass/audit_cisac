using System;

namespace SpanishPoint.Azure.Iswc.Bdo.Work
{
    public class AdditionalAgencyWorkNumber
    {
        public AdditionalAgencyWorkNumber()
        {
            WorkNumber = new WorkNumber();
            AuditRequestId = Guid.NewGuid();

        }

        public WorkNumber WorkNumber { get; set; }
        public bool IsEligible { get; set; }
        public string CurrentIswc { get; set; } = string.Empty;
        public long? WorkId { get; set; }
        public Guid AuditRequestId { get; set; }
    }
}
