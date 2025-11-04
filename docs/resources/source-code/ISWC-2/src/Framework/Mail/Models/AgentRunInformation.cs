using System.Diagnostics.CodeAnalysis;

namespace SpanishPoint.Azure.Iswc.Framework.Mail.Models
{
	[ExcludeFromCodeCoverage]
	public class AgentRunInformation
	{
		public string RunId { get; set; } = string.Empty;
		public string AgencyCode { get; set; } = string.Empty;
	}
}
