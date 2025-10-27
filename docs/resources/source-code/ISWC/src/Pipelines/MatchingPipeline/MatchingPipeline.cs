using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.PipelineComponents.MatchingComponent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Pipelines.MatchingPipeline
{
    public class MatchingPipeline : IMatchingPipeline
    {
        private readonly IMatchingComponent matchingComponent;

        public MatchingPipeline(IMatchingComponent matchingComponent)
        {
            this.matchingComponent = matchingComponent;
        }

        public async Task<IEnumerable<Submission>> RunPipeline(IEnumerable<Submission> submissionBatch)
        {
            return await matchingComponent.ProcessBatch(submissionBatch);
        }
    }
}
