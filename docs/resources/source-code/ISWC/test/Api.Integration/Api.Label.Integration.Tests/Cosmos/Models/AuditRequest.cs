using Newtonsoft.Json;
using SpanishPoint.Azure.Iswc.Api.Label.Integration.Tests.V1;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpanishPoint.Azure.Iswc.Api.Label.Integration.Tests.Cosmos.Models
{
    public class AuditRequestModel
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
        public string PartitionKey { get; internal set; }

        [JsonProperty]
        public long? WorkIdBefore { get; internal set; }
        [JsonProperty]
        public long? WorkIdAfter { get; internal set; }
        [JsonProperty]
        public Rejection TransactionError { get; internal set; }
        [JsonProperty]
        public SubmissionModel Work { get; internal set; }

        [JsonProperty]
        public string TransactionType { get; internal set; }
        [JsonProperty]
        public int RequestSource { get; internal set; }
        [JsonProperty]
        public long? WorkflowInstanceId { get; internal set; }

        [JsonProperty]
        public string TransactionSource { get; internal set; }
        [JsonProperty]
        public bool IsEligible { get; internal set; }

        [JsonProperty]
        public bool RelatedSubmissionIncludedIswc { get; internal set; }

    }

    public class SubmissionModel
    {
        [JsonProperty]
        public string PreferredIswc { get; set; }
        [JsonProperty]
        public int SourceDb { get; set; }
        [JsonProperty]
        public WorkNumber WorkNumber { get; set; }
        [JsonProperty]
        public bool Disambiguation { get; set; }
        [JsonProperty]
        public DisambiguationReason? DisambiguationReason { get; set; }
        [JsonProperty]
        public ICollection<DisambiguateFrom> DisambiguateFrom { get; set; }
        [JsonProperty]
        public DerivedWorkType? DerivedWorkType { get; set; }
        [JsonProperty]
        public ICollection<DerivedFrom> DerivedFrom { get; set; }
        [JsonProperty]
        public ICollection<Performer> Performers { get; set; }
        [JsonProperty]
        public ICollection<Instrumentation> Instrumentation { get; set; }
        [JsonProperty]
        public string Iswc { get; set; }
        [JsonProperty]
        public string Agency { get; set; }
        [JsonProperty]
        public ICollection<Title> Titles { get; set; }
        [JsonProperty]
        public ICollection<InterestedPartyModel> InterestedParties { get; set; }
        [JsonProperty]
        public bool IsReplaced { get; set; }
        [JsonProperty]
        public bool ApproveWorkflowTasks { get; set; }
        [JsonProperty]
        public int Category { get; set; }
        [JsonProperty]
        public ICollection<string> IswcsToMerge { get; set; }
        [JsonProperty]
        public ICollection<WorkNumber> WorkNumbersToMerge { get; set; }
        [JsonProperty]
        public bool PreviewDisambiguation { get; set; }
        [JsonProperty]
        public IEnumerable<AdditionalIdentifier> AdditionalIdentifiers { get; set; }
        [JsonProperty]
        public bool RelatedSubmissionIncludedIswc { get; set; }

    }

    public class WorkNumber
    {
        [JsonProperty]
        public string Type { get; set; }
        [JsonProperty]
        public string Number { get; set; }
    }

    public class InterestedPartyModel
    {
        [JsonProperty]
        public string IpBaseNumber { get; set; }
        [JsonProperty]
        public int Type { get; set; }
        [JsonProperty]
        public string Agency { get; set; }
        [JsonProperty]
        public long IPNameNumber { get; set; }
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public string LastName { get; set; }
        [JsonProperty]
        public string Affiliation { get; set; }
    }

    public class AdditionalIdentifier
    {
        [JsonProperty]
        public string WorkCode { get; set; }
        [JsonProperty]
        public string SubmitterCode { get; set; }
        [JsonProperty]
        public long? NameNumber { get; set; }
    }

    public class Title
    {
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public TitleType Type { get; set; }
    }
}
