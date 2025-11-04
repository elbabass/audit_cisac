using SpanishPoint.Azure.Iswc.Framework.Databricks.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Framework.Databricks
{
    public interface IDatabricksClient
    {
        Task<IEnumerable<Run>> GetActiveRuns();
        Task SubmitJob(SubmitJobRequestModel jobParameters);
    }
}