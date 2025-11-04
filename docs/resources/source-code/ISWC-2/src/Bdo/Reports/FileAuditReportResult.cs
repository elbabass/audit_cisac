using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using System;

namespace SpanishPoint.Azure.Iswc.Bdo.Reports
{
    public class FileAuditReportResult
    {
        public FileAuditReportResult()
        {

        }

        public string AuditId { get; set; } = string.Empty;
        public string AgencyCode { get; set; } = string.Empty;
        public DateTimeOffset? DatePickedUp { get; set; }
        public DateTimeOffset? DateAckGenerated { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string AckFileName { get; set; } = string.Empty;
        public long? PublisherNameNumber { get; set; } = 0;
        public string Status { get; set; } = string.Empty;
    }
}
