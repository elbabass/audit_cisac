using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.MatchingComponent.InitialMatching
{
    internal class MatchingForIswcNonEligibleSubmitter : IInitialMatchingComponent
    {
        private readonly IMatchingManager matchingManager;

        public MatchingForIswcNonEligibleSubmitter(IMatchingManager matchingManager)
        {
            this.matchingManager = matchingManager;
        }

        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.FSQ };

        public string Identifier => nameof(MatchingForIswcNonEligibleSubmitter);

        public bool? IsEligible => false;
		public string PipelineComponentVersion => typeof(MatchingComponent).GetComponentVersion();
        public IEnumerable<RequestType> ValidRequestTypes => new List<RequestType> { RequestType.Agency, RequestType.Label, RequestType.Publisher, RequestType.ThirdParty };


        public async Task<IEnumerable<Submission>> ProcessSubmissions(IEnumerable<Submission> submissions)
        {
            var matchingSource = submissions.Any(x => x.RequestType == RequestType.Label) ? "Label" : "InEligible";
            return await matchingManager.MatchAsync(submissions, matchingSource);
        }
    }
}
