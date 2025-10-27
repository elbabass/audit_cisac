using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.MatchingComponent
{
    internal interface IMatchingSubComponent
    {
        string Identifier { get; }
        /// <summary>
        /// IsEligible eq null indicates that it applies to both Eligible and non-Eligible submissions
        /// </summary>
        bool? IsEligible { get; }
        IEnumerable<TransactionType> ValidTransactionTypes { get; }
		string PipelineComponentVersion { get; }
        IEnumerable<RequestType> ValidRequestTypes { get; }

        Task<IEnumerable<Submission>> ProcessSubmissions(IEnumerable<Submission> submissions);
    }
}
