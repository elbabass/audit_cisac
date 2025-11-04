using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class WorkflowTask
    {
        public long WorkflowTaskId { get; set; }
        public long WorkflowInstanceId { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string AssignedAgencyId { get; set; }
        public int LastModifiedUserId { get; set; }
        public int TaskStatus { get; set; }
        public bool IsDeleted { get; set; }
        public string Message { get; set; }

        public virtual Agency AssignedAgency { get; set; }
        public virtual User LastModifiedUser { get; set; }
        public virtual WorkflowTaskStatus TaskStatusNavigation { get; set; }
        public virtual WorkflowInstance WorkflowInstance { get; set; }
    }
}
