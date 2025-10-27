using System;
using System.Collections.Generic;
using System.Text;

namespace SpanishPoint.Azure.Iswc.Bdo.Reports
{
    public class AuditReportSearchParameters
    {
        public DateTime FromDate { get; set; } = default;
        public DateTime ToDate { get; set; } = default;
        public StatusType Status { get; set; }
        public string? AgencyWorkCode { get; set; }
        public string? AgencyName { get; set; }
        public TransactionSource TransactionSource { get; set; }
        public string? SubmittingAgency { get; set; }
        public ReportType Report { get; set; }
        public DateTime AgreementFromDate { get; set; } = default;
        public DateTime AgreementToDate { get; set; } = default;
        public bool? MostRecentVersion { get; set; } = false;
        public int? Year { get; set; }
        public int? Month { get; set; }
        public string? Email { get; set; }
        public bool? ConsiderOriginalTitlesOnly { get; set; } = true;
        public bool? PotentialDuplicatesCreateExtractMode { get; set; }
        public string? CreatorNameNumber { get; set; }
        public string? CreatorBaseNumber { get; set; }
    }
}
