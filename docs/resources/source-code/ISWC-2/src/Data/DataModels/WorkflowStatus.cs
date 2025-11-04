using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class WorkflowStatus
    {
        public WorkflowStatus()
        {
            WorkflowInstance = new HashSet<WorkflowInstance>();
        }

        public int WorkflowStatusId { get; set; }
        public string Code { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedUserId { get; set; }
        public string Description { get; set; }

        public virtual User LastModifiedUser { get; set; }
        public virtual ICollection<WorkflowInstance> WorkflowInstance { get; set; }
    }
}
