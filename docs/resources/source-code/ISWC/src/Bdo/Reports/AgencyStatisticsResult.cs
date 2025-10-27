namespace SpanishPoint.Azure.Iswc.Bdo.Reports
{
    public class AgencyStatisticsResult
    {
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string? AgencyCode { get; set; }
        public int ValidSubmissions { get; set; }
        public int InvalidSubmissions { get; set; }
        public int EligibleIswcSubmissions { get; set; }
        public int EligibleIswcSubmissionsWithDisambiguation { get; set; }
        public int InEligibleIswcSubmissions { get; set; }
        public int InEligibleIswcSubmissionsWithDisambiguation { get; set; }
        public int WorkflowTasksAssigned { get; set; }
        public int WorkflowTasksAssignedApproved { get; set; }
        public int WorkflowTasksAssignedPending { get; set; }
        public int WorkflowTasksAssignedRejected { get; set; }
    }
}
