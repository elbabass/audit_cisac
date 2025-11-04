using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Bdo.Rules
{
    public interface IRule : IBaseRule
    { 
        Task<(bool IsValid, Submission Submission)> IsValid(Submission submission);
    }
}
