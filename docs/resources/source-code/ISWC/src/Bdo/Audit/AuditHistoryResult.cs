using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Bdo.Audit
{
    public class AuditHistoryResult
    {
        public AuditHistoryResult()
        {
            Titles = new List<Title>();
            Creators = new List<InterestedPartyModel>();
        }
        public DateTime SubmittedDate { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public TransactionType TransactionType { get; set; }
        public string? SubmittingAgency { get; set; }
        public string? WorkNumber { get; set; }
        public string? LastModifiedUser { get; set; }
        public ICollection<InterestedPartyModel> Creators { get; set; }
        public ICollection<Title> Titles { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public InstanceStatus Status { get; set; }
        public long? WorkCode { get; set; }
        public long? WorkflowInstanceId { get; set; }
        public bool UpdateAllocatedIswc { get; set; }
        public string WorkNumberToReplaceIasWorkNumber { get; set; } = string.Empty;
    }
}
