using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class WorkflowInstance
    {
        public WorkflowInstance()
        {
            WorkflowTask = new HashSet<WorkflowTask>();
        }

        public long WorkflowInstanceId { get; set; }
        public int WorkflowType { get; set; }
        public long? WorkInfoId { get; set; }
        public long? MergeRequestId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedUserId { get; set; }
        public int InstanceStatus { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeMergeRequestId { get; set; }

        public virtual DeMergeRequest DeMergeRequest { get; set; }
        public virtual WorkflowStatus InstanceStatusNavigation { get; set; }
        public virtual User LastModifiedUser { get; set; }
        public virtual MergeRequest MergeRequest { get; set; }
        public virtual WorkInfo WorkInfo { get; set; }
        public virtual WorkflowType WorkflowTypeNavigation { get; set; }
        public virtual ICollection<WorkflowTask> WorkflowTask { get; set; }
    }
}
