using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Api.ThirdParty.Managers
{
    internal interface IPipelineManager
    {
        Task<IEnumerable<Submission>> RunPipelines(IEnumerable<Submission> submissions);
        Task<Submission> RunPipelines(Submission submission);
    }
}
