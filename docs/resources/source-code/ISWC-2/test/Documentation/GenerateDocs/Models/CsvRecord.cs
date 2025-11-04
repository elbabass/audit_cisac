using System.Diagnostics.CodeAnalysis;

namespace SpanishPoint.Azure.Iswc.Api.Tests.Documentation.GenerateDocs.Models
{
    [ExcludeFromCodeCoverage]
    class CsvRecord
    {
        public string Component { get; set; }
        public string Rule { get; set; }
        public string Test { get; set; }
        public string Description { get; set; }
    }
}
