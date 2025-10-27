namespace SpanishPoint.Azure.Iswc.Api.Label.Integration.Tests.Cosmos.Models
{
    public class CSN
    {
        public string id { get; set; }
        public string SubmittingAgencyCode { get; set; }
        public string ReceivingAgencyCode { get; set; }
        public string ToIswc { get; set; }
        public string FromIswc { get; set; }
        public string ProcessingDate { get; set; }
        public string TransactionType { get; set; }
        public string WorkflowTaskID { get; set; }
        public string WorkflowStatus { get; set; }
        public string ReceivingAgencyWorkCode { get; set; }
        public string PartitionKey { get; set; }
    }
}
