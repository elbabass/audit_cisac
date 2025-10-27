using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Data.Services.UpdateWorkflowHistory
{
    public interface  IUpdateWorkflowHistoryService
    {
        Task SaveModel(Submission submission, long workinfoId);
        Task<Submission> GetModel(string preferrdIswc, long? workinfoId);
    }
}
