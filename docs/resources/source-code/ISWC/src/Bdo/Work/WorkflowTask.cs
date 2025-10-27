using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Bdo.Work
{
	public class WorkflowTask
	{
		public long TaskId { get; set; }
		public InstanceStatus TaskStatus { get; set; }
		public string? AssignedAgencyId { get; set; }
		public WorkflowType WorkflowTaskType { get; set; }
		public IswcModel? IswcMetadata { get; set; }
		public Rejection? Rejection { get; set; }
		public string? OriginatingSociety { get; set; }
		public DateTime? CreatedDate { get; set; }
		public ShowWorkflows ShowWorkflows { get; set; }
		public ICollection<InstanceStatus>? Statuses { get; set; }
		public string? Message { get; set; }
	}
}
