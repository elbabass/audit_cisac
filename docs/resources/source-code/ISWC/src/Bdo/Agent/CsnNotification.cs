using SpanishPoint.Azure.Iswc.Bdo.Edi;
using System;

namespace SpanishPoint.Azure.Iswc.Bdo.Agent
{
    public class CsnNotification
    {
        public string SubmittingAgencyCode { get; set; } = string.Empty;
        public string ReceivingAgencyCode { get; set; } = string.Empty;
        public string ToIswc { get; set; } = string.Empty;
        public string FromIswc { get; set; } = string.Empty;
        public DateTime ProcessingDate { get; set; }
        public TransactionType TransactionType { get; set; }
        public string WorkflowTaskID { get; set; } = string.Empty;
        public string WorkflowStatus { get; set; } = string.Empty;
        public string ReceivingAgencyWorkCode { get; set; } = string.Empty;
        public string PartitionKey { get; set; } = string.Empty;
        public DateTime? ProcessedOnDate { get; }
        public string IswcMetaData { get; set; } = string.Empty;
        public string WorkflowMessage { get; set; } = string.Empty;
    } 
}
