using System.Diagnostics.CodeAnalysis;

namespace SpanishPoint.Azure.Iswc.Framework.Databricks
{
    [ExcludeFromCodeCoverage]
    public class DatabricksClientOptions
    {
        public long JobID { get; set; }
        public string BearerToken { get; set; } = string.Empty;
    }
}
