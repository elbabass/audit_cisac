using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules
{
    public class PV_34 : IRule, IAlwaysOnRule
    {
        private readonly IMessagingManager messagingManager;

        public PV_34(IMessagingManager messagingManager)
        {
            this.messagingManager = messagingManager;
        }


        public string Identifier => nameof(PV_34);

        public string ParameterName => String.Empty;

        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR };

        public ValidatorType ValidatorType => ValidatorType.PostMatchingValidator;

        public string PipelineComponentVersion => typeof(PostMatchingValidator).GetComponentVersion();

        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            if (submission.IsrcMatchedResult.Matches.Any() && submission.MatchedResult.Matches.Any())
            {
                var metadataMatch = submission.MatchedResult.Matches.SelectMany(m => m.Numbers ?? Enumerable.Empty<WorkNumber>())
                    .FirstOrDefault(n => string.Equals(n?.Type, "ISWC", StringComparison.OrdinalIgnoreCase))?.Number; ;
                var isrcMatch = submission.IsrcMatchedResult.Matches.SelectMany(m => m.Numbers ?? Enumerable.Empty<WorkNumber>())
                    .FirstOrDefault(n => string.Equals(n?.Type, "ISWC", StringComparison.OrdinalIgnoreCase))?.Number;
                if (metadataMatch != null && isrcMatch != null && !metadataMatch.Equals(isrcMatch))
                {
                    submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._249);
                    return (false, submission);
                }
            }

            return (true, submission);
        }
    }
}
