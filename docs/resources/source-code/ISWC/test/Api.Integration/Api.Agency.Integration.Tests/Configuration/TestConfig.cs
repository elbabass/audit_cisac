
namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.Configuration
{
    public class TestConfig
    {
        public string IswcApiUrl { get; set; }
        public string IswcAgencyApiUrl { get; set; }
        public string MatchingEngineUrl { get; set; }
        public string MatchingEngineSecret { get; set; }
        public string IswcSecret { get; set; }
        public string CosmosConnectionString { get; set; }
        public string SearchServiceKey { get; set; }
        public string SearchServiceName { get; set; }
        public string WorkNumbersIndexer { get; set; }
        public string WorkNameContributorsPerformersIndexer { get; set; }

    }
}
