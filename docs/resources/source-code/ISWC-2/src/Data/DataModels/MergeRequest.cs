using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class MergeRequest
    {
        public MergeRequest()
        {
            IswclinkedTo = new HashSet<IswclinkedTo>();
            WorkflowInstance = new HashSet<WorkflowInstance>();
        }

        public long MergeRequestId { get; set; }
        public string AgencyId { get; set; }
        public int MergeStatus { get; set; }
        public bool Status { get; set; }
        public byte[] Concurrency { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedUserId { get; set; }
        public int? MergeType { get; set; }

        public virtual Agency Agency { get; set; }
        public virtual User LastModifiedUser { get; set; }
        public virtual MergeStatus MergeStatusNavigation { get; set; }
        public virtual ICollection<IswclinkedTo> IswclinkedTo { get; set; }
        public virtual ICollection<WorkflowInstance> WorkflowInstance { get; set; }
    }
}
