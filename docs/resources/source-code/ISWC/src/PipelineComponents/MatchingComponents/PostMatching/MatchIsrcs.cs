using Microsoft.EntityFrameworkCore.Internal;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.MatchingComponent.PostMatching
{
    internal class MatchIsrcs : IPostMatchingComponent
    {
        private readonly IAdditionalIdentifierManager additionalIdentifierManager;
        private readonly IMatchingManager matchingManager;
        private readonly IWorkManager workManager;

        public MatchIsrcs(IAdditionalIdentifierManager additionalIdentifierManager, IMatchingManager matchingManager, IWorkManager workManager)
        {
            this.additionalIdentifierManager = additionalIdentifierManager;
            this.matchingManager = matchingManager;
            this.workManager = workManager;
        }

        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.CUR, TransactionType.CIQ };
        public string Identifier => nameof(MatchIsrcs);
        public bool? IsEligible => null;
        public string PipelineComponentVersion => typeof(MatchingComponent).GetComponentVersion();
        public IEnumerable<RequestType> ValidRequestTypes => new List<RequestType> { RequestType.Label, RequestType.Agency, RequestType.Publisher };

        public async Task<IEnumerable<Submission>> ProcessSubmissions(IEnumerable<Submission> submissions)
        {
            if (!submissions.Any(s => s.Model?.AdditionalIdentifiers != null && s.Model.AdditionalIdentifiers.Any()))
                return submissions;

            return await matchingManager.MatchIsrcsAsync(submissions);
        }
    }
}