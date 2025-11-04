using SpanishPoint.Azure.Iswc.Bdo.Work;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpanishPoint.Azure.Iswc.Bdo.Submissions
{
    public class WorkflowSearchModel
    {
        public WorkflowSearchModel()
        {
            Agency = string.Empty;
            Statuses = new List<InstanceStatus>();
            AgencyWorkCodes = new List<string>();
        }

        public string Agency { get; set; }
        public int? StartIndex { get; set; }
        public int? PageLength { get; set; }
        public ShowWorkflows ShowWorkflows { get; set; }
        public IEnumerable<InstanceStatus> Statuses { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Iswc { get; set; }
        public IEnumerable<string> AgencyWorkCodes { get; set; }
        public string? OriginatingAgency { get; set; }
        public WorkflowType? WorkflowType { get; set; }
    }
}
