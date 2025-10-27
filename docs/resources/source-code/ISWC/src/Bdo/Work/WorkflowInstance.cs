using System;

namespace SpanishPoint.Azure.Iswc.Bdo.Work
{
    public class WorkflowInstance
    {
        public long WorkflowInstanceId { get; set; }
        public WorkflowType WorkflowType { get; set; }
        public InstanceStatus InstanceStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public long? MergeRequestId { get; set; }
    }
}
