using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using System;

namespace SpanishPoint.Azure.Iswc.Bdo.Reports
{
    public class AuditReportResult
    {
        public AuditReportResult()
        {

        }

        public string AuditRequestId { get; set; } = string.Empty;
        public string AuditId { get; set; } = string.Empty;
        public long? RecordId { get; set; } = 0;
        public string AgencyCode { get; set; } = string.Empty;
        public DateTimeOffset? CreatedDate { get; set; }
        public bool IsProcessingError { get; set; }
        public bool IsProcessingFinished { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        [JsonConverter(typeof(StringEnumConverter))]
        public TransactionType TransactionType { get; set; }
        public string? PreferredIswc { get; set; } = string.Empty;
        public string? AgencyWorkCode { get; set; } = string.Empty;
        public int? SourceDb { get; set; } = 0;
        public string OriginalTitle { get; set; } = string.Empty;
        [JsonConverter(typeof(StringEnumConverter))]
        public TransactionSource TransactionSource { get; set; }
        public string CreatorNames { get; set; } = string.Empty;
        public string CreatorNameNumbers { get; set; } = string.Empty;
        public long? PublisherNameNumber { get; set; } = 0;
        public string PublisherWorkNumber { get; set; } = string.Empty;
    }
}
