using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace SpanishPoint.Azure.Iswc.Framework.Databricks.Models
{
    [ExcludeFromCodeCoverage]
    public class NotebookParameters
    {
        [JsonProperty("agency_work_code")]
        public string? AgencyWorkCode { get; set; }

        [JsonProperty("submitting_agency_code")]
        public string? SubmittingAgencyCode { get; set; }

        [JsonProperty("status")]
        public string? Status { get; set; } = string.Empty;

        [JsonProperty("agency_code")]
        public string? AgencyCode { get; set; }

        [JsonProperty("report_type")]
        public string? ReportType { get; set; }

        [JsonProperty("from_date")]
        public string? FromDate { get; set; }

        [JsonProperty("to_date")]
        public string? ToDate { get; set; }

        [JsonProperty("transaction_source")]
        public string? TransactionSource { get; set; }

        [JsonProperty("agreement_from_date")]
        public string? AgreementFromDate { get; set; } = string.Empty;

        [JsonProperty("agreement_to_date")]
        public string? AgreementToDate { get; set; } = string.Empty;

        [JsonProperty("most_recent_version")]
        public string? MostRecentVersion { get; set; } = string.Empty;

        [JsonProperty("email")]
        public string? Email { get; set; }

        [JsonProperty("consider_original_titles_only")]
        public string? ConsiderOriginalTitlesOnly { get; set; } = string.Empty;

        [JsonProperty("potential_duplicates_create_extract_mode")]
        public string? PotentialDuplicatesCreateExtractMode { get; set; } = string.Empty;

        [JsonProperty("creator_name_number")]
        public string? CreatorNameNumber { get; set; } = string.Empty;

        [JsonProperty("creator_base_number")]
        public string? CreatorBaseNumber { get; set; } = string.Empty;

        public override bool Equals(object? obj)
        {
            if (!(obj is NotebookParameters item))
            {
                return false;
            }

            return AgencyCode == item.AgencyCode && AgencyWorkCode == item.AgencyWorkCode && FromDate == item.FromDate && ReportType == item.ReportType
                && Status == item.Status && SubmittingAgencyCode == item.SubmittingAgencyCode && ToDate == item.ToDate && TransactionSource == item.TransactionSource
                && AgreementFromDate == item.AgreementFromDate && AgreementToDate == item.AgreementToDate && MostRecentVersion == item.MostRecentVersion
                && ConsiderOriginalTitlesOnly == item.ConsiderOriginalTitlesOnly && CreatorNameNumber == item.CreatorNameNumber && CreatorBaseNumber == item.CreatorBaseNumber;
        }

        public override int GetHashCode()
        {
            throw new System.NotImplementedException();
        }
    }
}
