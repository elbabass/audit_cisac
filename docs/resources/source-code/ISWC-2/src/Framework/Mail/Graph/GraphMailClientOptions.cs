using System.Diagnostics.CodeAnalysis;

namespace SpanishPoint.Azure.Iswc.Framework.Mail.Graph
{
    [ExcludeFromCodeCoverage]
    public class GraphMailClientOptions
    {
        public string ClientID { get; set; } = string.Empty;
        public string Tenant { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;
    }
}
