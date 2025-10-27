using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Pipelines.PostMatchingPipeline
{
    public interface IPostMatchingPipeline
    {
        Task<IEnumerable<Submission>> RunPipeline(IEnumerable<Submission> submissions);
    }
}
