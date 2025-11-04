using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Bdo.Rules
{
    public interface IBatchRule : IBaseRule
    {
        Task<IEnumerable<Submission>> IsValid(IEnumerable<Submission> submissions);
    }
}
