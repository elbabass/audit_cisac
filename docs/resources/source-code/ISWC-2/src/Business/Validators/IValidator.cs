using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Business.Validators
{
    public interface IValidator
    {
        Task<IEnumerable<Submission>> ValidateBatch(IEnumerable<Submission> batch);
    }
}
