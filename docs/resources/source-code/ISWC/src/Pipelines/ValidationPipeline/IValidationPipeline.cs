using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Pipelines.ValidationPipeline
{
    public interface IValidationPipeline
    {
        Task<IEnumerable<Submission>> RunPipeline(IEnumerable<Submission> submissionBatch);
    }
}
