using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Reports;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Framework.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Framework.Http.Requests;
using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.Services.Audit.CosmosDb.Models
{
    /// <summary>
    /// Second level Audit record. Stores the Submission level detail.
    /// </summary>
    internal class AuditRequestModel : BaseModel
    {
        [JsonProperty(PropertyName = "id")]
        public Guid AuditRequestId { get; internal set; }
        [JsonProperty]
        public Guid AuditId { get; internal set; }
        [JsonProperty]
        public int RecordId { get; internal set; }
        [JsonProperty]
        public string AgencyCode { get; internal set; }
        [JsonProperty]
        public DateTime CreatedDate { get; internal set; }
        [JsonProperty]
        public bool IsProcessingFinished { get; internal set; }
        [JsonProperty]
        public bool IsProcessingError { get; internal set; }
        [JsonProperty]
        public DateTime ProcessingCompletionDate { get; internal set; }
        [JsonProperty]
        public ICollection<RuleExecution> RulesApplied { get; internal set; }
        [JsonProperty]
        public string PartitionKey { get; internal set; }

        [JsonProperty]
        public long? WorkIdBefore { get; internal set; }
        [JsonProperty]
        public long? WorkIdAfter { get; internal set; }
        [JsonProperty]
        public Rejection TransactionError { get; internal set; }
        [JsonProperty]
        public string IswcStatus { get; internal set; }
        [JsonProperty]
        public SubmissionModel Work { get; internal set; }
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty]
        public TransactionType TransactionType { get; internal set; }
        [JsonProperty]
        public long? WorkflowInstanceId { get; internal set; }
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty]
        public TransactionSource TransactionSource { get; internal set; } = TransactionSource.Agency;
        [JsonProperty]
        public bool IsEligible { get; internal set; }
        [JsonProperty]
        public bool RelatedSubmissionIncludedIswc { get; internal set; }
        [JsonProperty]
        public RequestSource RequestSource { get; internal set; }
        [JsonProperty]
        public string AgentVersion { get; internal set; }
        [JsonProperty]
        public bool UpdateAllocatedIswc { get; internal set; }
        [JsonProperty]
        public string WorkNumberToReplaceIasWorkNumber { get; internal set; }
        public ICollection<MultipleAgencyWorkCodes> MultipleAgencyWorkCodes { get; internal set; }
        public bool MultipleAgencyWorkCodesChild { get; internal set; }
    }
}
