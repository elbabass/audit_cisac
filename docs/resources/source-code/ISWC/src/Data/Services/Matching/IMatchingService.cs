using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Data.Services.Matching.MatchingEngine.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Data.Services.Matching
{
    public interface IMatchingService
    {
        Task<IEnumerable<Submission>> MatchAsync(IEnumerable<Submission> submissions, string source);
        Task<IEnumerable<InterestedPartyModel>> MatchContributorsAsync(InterestedPartySearchModel searchModel);
    }
}
