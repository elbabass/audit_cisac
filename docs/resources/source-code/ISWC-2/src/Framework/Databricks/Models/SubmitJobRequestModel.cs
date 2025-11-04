using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace SpanishPoint.Azure.Iswc.Framework.Databricks.Models
{
    [ExcludeFromCodeCoverage]
    public class SubmitJobRequestModel
    {
        [JsonProperty("job_id")]
        public long JobID { get; set; }

        [JsonProperty("notebook_params")]
        public NotebookParameters? NoteBookParameters { get; set; }
    }
}
