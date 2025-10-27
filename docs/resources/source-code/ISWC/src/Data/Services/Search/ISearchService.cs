using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Data.Services.Search
{
    public interface ISearchService
    {
        Task DeleteByWorkCode(long workCode);
        Task AddSubmission(Submission submission);
        Task UpdateWorksIswcStatus(Data.DataModels.Iswc iswc);
    }
}
