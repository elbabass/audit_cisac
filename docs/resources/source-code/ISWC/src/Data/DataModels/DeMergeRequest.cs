using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class DeMergeRequest
    {
        public DeMergeRequest()
        {
            IswclinkedTo = new HashSet<IswclinkedTo>();
            WorkflowInstance = new HashSet<WorkflowInstance>();
        }

        public long DeMergeRequestId { get; set; }
        public string AgencyId { get; set; }
        public int DeMergeStatus { get; set; }
        public bool Status { get; set; }
        public byte[] Concurrency { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedUserId { get; set; }

        public virtual Agency Agency { get; set; }
        public virtual MergeStatus DeMergeStatusNavigation { get; set; }
        public virtual User LastModifiedUser { get; set; }
        public virtual ICollection<IswclinkedTo> IswclinkedTo { get; set; }
        public virtual ICollection<WorkflowInstance> WorkflowInstance { get; set; }
    }
}
