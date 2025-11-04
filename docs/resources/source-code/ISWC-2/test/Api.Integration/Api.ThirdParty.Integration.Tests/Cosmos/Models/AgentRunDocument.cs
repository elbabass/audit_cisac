using Newtonsoft.Json;
using System;

namespace SpanishPoint.Azure.Iswc.Api.ThirdParty.Integration.Tests.Cosmos.Models
{
    public class AgentRunDocument
    {
		[JsonProperty(PropertyName = "id")]
		public string RunId { get; set; }
		public string AgentVersion { get; set; }
		public string AgencyCode { get; set; }
		public int UpdateRecordCount { get; set; }
		public int NewRecordCount { get; set; }
		public int OverallRejected { get; set; }
		public int OverallSent { get; set; }
		public int SuccessfulCount { get; set; }
		public int BusinessRejectionCount { get; set; }
		public int TechnicalRejectionCount { get; set; }
		public bool RunCompleted { get; set; }
		public DateTime? RunStartDate { get; set; }
		public DateTime? RunEndDate { get; set; }
		public string PartitionKey { get; set; }
	}
}
