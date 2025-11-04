using System.Collections.Generic;
using System.Threading.Tasks;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.PipelineComponents.ProcessingComponent;

namespace SpanishPoint.Azure.Iswc.Pipelines.ProcessingPipeline
{
    public class ProcessingPipeline : IProcessingPipeline
    {
        private readonly IProcessingComponent processingComponent;

        public ProcessingPipeline(IProcessingComponent processingComponent)
        {
            this.processingComponent = processingComponent;
        }

        public async Task<IEnumerable<Submission>> RunPipeline(IEnumerable<Submission> submissions)
        {
            return await processingComponent.ProcessBatch(submissions);
        }
    }
}
