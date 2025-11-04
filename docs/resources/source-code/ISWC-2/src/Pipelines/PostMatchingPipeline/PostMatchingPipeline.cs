using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Pipelines.PostMatchingPipeline
{
    public class PostMatchingPipeline : IPostMatchingPipeline
    {
        private readonly IPostMatchingValidator postMatchingValidator;

        public PostMatchingPipeline(IPostMatchingValidator postMatchingValidator)
        {
            this.postMatchingValidator = postMatchingValidator;
        }

        public async Task<IEnumerable<Submission>> RunPipeline(IEnumerable<Submission> submissions)
        {
            return await postMatchingValidator.ValidateBatch(submissions);
        }
    }
}
