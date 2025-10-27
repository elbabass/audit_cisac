using System.Diagnostics.CodeAnalysis;

namespace SpanishPoint.Azure.Iswc.Framework.Search.Azure
{
    [ExcludeFromCodeCoverage]
    public abstract class AzureSearchOptions
    {
        public string? SearchServiceName { get; set; }
        public string? ApiKey { get; set; }
        public string? IndexName { get; set; }
    }    
}
