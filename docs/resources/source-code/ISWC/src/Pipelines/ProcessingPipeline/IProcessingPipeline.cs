using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Pipelines.ProcessingPipeline
{
    public interface IProcessingPipeline
    {
        Task<IEnumerable<Submission>> RunPipeline(IEnumerable<Submission> submission);
    }
}
