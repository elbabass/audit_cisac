using System;
using System.Collections.Generic;
using System.Text;

namespace SpanishPoint.Azure.Iswc.Bdo.Agent
{
	public class AgentRun
	{
		public AgentRun()
		{
			AgentVersion = string.Empty;
			AgencyCode = string.Empty;
			RunId = string.Empty;
		}

		public string AgentVersion { get; set; }
		public string AgencyCode { get; set; }
		public string RunId { get; set; }
		public int UpdateRecordCount { get; set; }
		public int NewRecordCount { get; set; }
		public int OverallRejected { get; set; }
		public int OverallSent { get; set; }
		public int SuccessfulCount { get; set; }
		public int BusinessRejectionCount { get; set; }
		public int TechnicalRejectionCount { get; set; }
	}
}
