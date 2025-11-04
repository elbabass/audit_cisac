using System;

namespace SpanishPoint.Azure.Iswc.Bdo.Ipi
{
    public class NameModel
    {
        public long IpNameNumber { get; set; }
        public DateTime AmendedDateTime { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime CreatedDate { get; set; }
        public NameType TypeCode { get; set; }
        public long? ForwardingNameNumber { get; set; }
        public string? Agency { get; set; }
    }
}
