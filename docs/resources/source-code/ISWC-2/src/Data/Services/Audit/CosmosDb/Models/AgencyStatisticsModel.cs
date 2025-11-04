using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SpanishPoint.Azure.Iswc.Bdo.Reports;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Framework.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpanishPoint.Azure.Iswc.Data.Services.Audit.CosmosDb.Models
{
    public class AgencyStatisticsModel : BaseModel
    {
        public AgencyStatisticsModel()
        {
        }

        internal AgencyStatisticsModel(IEnumerable<AuditRequestModel> agencyAuditData)
        {
            ID = Guid.NewGuid();
            AgencyCode = agencyAuditData.FirstOrDefault().AgencyCode;
            Year = agencyAuditData.FirstOrDefault().CreatedDate.Year;
            Month = agencyAuditData.FirstOrDefault().CreatedDate.Month;
            Day = agencyAuditData.FirstOrDefault().CreatedDate.Day;
            TransactionSource = agencyAuditData.FirstOrDefault().TransactionSource == TransactionSource.Publisher ?
                TransactionSource.Publisher : TransactionSource.Agency;

            PartitionKey = $"{AgencyCode}_{Month}_{Year}_{TransactionSource.ToFriendlyString().ToLower()}";
            WorkflowTasksAssignedPending = 1;

            GetValidSubmissionsTotal(agencyAuditData);
            GetInvalidSubmissionsTotal(agencyAuditData);
            GetEligibleIswcSubmissionsTotal(agencyAuditData);
            GetEligibleIswcSubmissionsWithDisambiguationTotal(agencyAuditData);
            GetInEligibleIswcSubmissionsTotal(agencyAuditData);
            GetInEligibleIswcSubmissionsWithDisambiguationTotal(agencyAuditData);

        }

        [JsonProperty(PropertyName = "id")]
        public Guid ID { get; internal set; }
        [JsonProperty]
        public int Day { get; internal set; }
        [JsonProperty]
        public int Month { get; internal set; }
        [JsonProperty]
        public int Year { get; internal set; }
        [JsonProperty]
        public string AgencyCode { get; internal set; }
        [JsonProperty]
        public int ValidSubmissions { get; internal set; }
        [JsonProperty]
        public int InvalidSubmissions { get; internal set; }
        [JsonProperty]
        public int EligibleIswcSubmissions { get; internal set; }
        [JsonProperty]
        public int EligibleIswcSubmissionsWithDisambiguation { get; internal set; }
        [JsonProperty]
        public int InEligibleIswcSubmissions { get; internal set; }
        [JsonProperty]
        public int InEligibleIswcSubmissionsWithDisambiguation { get; internal set; }
        [JsonProperty]
        public int WorkflowTasksAssigned { get; internal set; }
        [JsonProperty]
        public int WorkflowTasksAssignedApproved { get; internal set; }
        [JsonProperty]
        public int WorkflowTasksAssignedPending { get; internal set; }
        [JsonProperty]
        public int WorkflowTasksAssignedRejected { get; internal set; }
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty]
        public TransactionSource TransactionSource { get; internal set; } = TransactionSource.Agency;
        [JsonProperty]
        public SubmissionModel Work { get; internal set; }
        [JsonProperty]
        public string PartitionKey { get; internal set; }

        internal void GetValidSubmissionsTotal(IEnumerable<AuditRequestModel> agencyAuditData)
        {
            ValidSubmissions += agencyAuditData.Where(x => x.IsProcessingFinished && !x.IsProcessingError).Count(); ;
        }

        internal void GetInvalidSubmissionsTotal(IEnumerable<AuditRequestModel> agencyAuditData)
        {
            InvalidSubmissions += agencyAuditData.Where(x => x.IsProcessingFinished && x.IsProcessingError).Count(); ;
        }

        internal void GetEligibleIswcSubmissionsTotal(IEnumerable<AuditRequestModel> agencyAuditData)
        {
            EligibleIswcSubmissions += agencyAuditData.Where(x => x.IsProcessingFinished && !x.IsProcessingError && !x.Work.Disambiguation
            && (x.IsEligible || (x.RulesApplied.Any(x => IsEligible(x.RuleName))))).Count();
        }

        internal void GetEligibleIswcSubmissionsWithDisambiguationTotal(IEnumerable<AuditRequestModel> agencyAuditData)
        {
            EligibleIswcSubmissionsWithDisambiguation += agencyAuditData.Where(x => x.IsProcessingFinished && !x.IsProcessingError && (x.IsEligible
            || x.RulesApplied.Any(x => IsEligible(x.RuleName))) && x.Work.Disambiguation).Count();
        }

        internal void GetInEligibleIswcSubmissionsTotal(IEnumerable<AuditRequestModel> agencyAuditData)
        {
            InEligibleIswcSubmissions = agencyAuditData.Where(x => x.IsProcessingFinished && !x.IsProcessingError && !x.IsEligible && !x.RulesApplied.Any(x => IsEligible(x.RuleName)) && !x.Work.Disambiguation).Count(); ;
        }

        internal void GetInEligibleIswcSubmissionsWithDisambiguationTotal(IEnumerable<AuditRequestModel> agencyAuditData)
        {
            InEligibleIswcSubmissionsWithDisambiguation = agencyAuditData.Where(x => x.IsProcessingFinished && !x.IsProcessingError
            && !x.IsEligible && !x.RulesApplied.Any(x => IsEligible(x.RuleName)) && x.Work.Disambiguation).Count(); ;
        }

        private static readonly List<string> eligibleRules = new List<string> { "AS_01", "AS_03", "AS_04", "AS_05", "AS_11", "AS_12" };

        bool IsEligible(string ruleName) => eligibleRules.Contains(ruleName);
    }
}
