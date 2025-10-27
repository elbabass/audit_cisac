using System.Diagnostics.CodeAnalysis;

namespace SpanishPoint.Azure.Iswc.Framework.Mail.Models
{
    [ExcludeFromCodeCoverage]
    public class MailMessageModel
    {
        public string Recipient { get; set; } = string.Empty;
        public string ReportType { get; set; } = string.Empty;
        public AgentRunInformation? AgentRunInformation { get; set; }
    }
}
