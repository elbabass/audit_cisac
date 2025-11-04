using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.MatchingComponent.PostMatching
{
    public class RankingComponent : IPostMatchingComponent
    {
        private readonly IMatchingManager matchingManager;

        public RankingComponent(IMatchingManager matchingManager)
        {
            this.matchingManager = matchingManager;
        }

        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.CUR, TransactionType.FSQ };
        public string Identifier => nameof(RankingComponent);
        public bool? IsEligible => null;
		public string PipelineComponentVersion => typeof(MatchingComponent).GetComponentVersion();
        public IEnumerable<RequestType> ValidRequestTypes => new List<RequestType> { RequestType.Agency, RequestType.Label, RequestType.Publisher, RequestType.ThirdParty };

        public Task<IEnumerable<Submission>> ProcessSubmissions(IEnumerable<Submission> submissions)
        {
            return matchingManager.RankMatches(submissions);
        }
    }
}
