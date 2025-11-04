using SpanishPoint.Azure.Iswc.Bdo.Work;
using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Bdo.Submissions
{
    public class VerifiedSubmissionModel : SubmissionModel
    {
        public VerifiedSubmissionModel()
        {
            WorkflowInstance = new List<WorkflowInstance>();
        }
        public long WorkInfoID { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? IswcStatus { get; set; }
        public string? LastModifiedUser { get; set; }
        public bool IswcEligible { get; set; }
        public string? ArchivedIswc { get; set; }
        public ICollection<WorkflowInstance> WorkflowInstance { get; set; }
        public string? LinkedTo { get; set; }
    }
}