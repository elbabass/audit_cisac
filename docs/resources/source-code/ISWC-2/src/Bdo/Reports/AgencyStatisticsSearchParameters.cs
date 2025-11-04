using System;

namespace SpanishPoint.Azure.Iswc.Bdo.Reports
{
    public class AgencyStatisticsSearchParameters
    {
        public string? AgencyName { get; set; }
        public int? Year { get; set; } = DateTime.Now.Year;
        public int? Month { get; set; }
        public TimePeriod TimePeriod { get; set; }
        public TransactionSource TransactionSource { get; set; }
    }
}

