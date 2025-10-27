using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules
{
    public class PV_10 : IRule, IAlwaysOnRule
    {
        private readonly IMessagingManager messagingManager;
        private readonly IWorkManager workManager;

        public PV_10(IWorkManager workManager, IMessagingManager messagingManager)
        {
            this.workManager = workManager;
            this.messagingManager = messagingManager;
        }

        public string Identifier => nameof(PV_10);
        public string ParameterName => string.Empty;
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.MER, TransactionType.DMR };
        public ValidatorType ValidatorType => ValidatorType.PostMatchingValidator;
        public string PipelineComponentVersion => typeof(PostMatchingValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var iswcs = new List<string>();

            if (submission.MatchedResult.Matches.Count() > 0)
                iswcs.AddRange(submission.MatchedResult.Matches.Select(
                    x => x.Numbers.FirstOrDefault(y => y.Type != null && y.Type.Equals("ISWC")).Number!).Distinct());

            var submittersEligibleSubmissions = (await workManager.FindManyAsync(iswcs, true))?.Where(x => x.Agency == submission.Model.Agency).ToList();

            if (submittersEligibleSubmissions == null || !submittersEligibleSubmissions.Any())
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._150);
                return (false, submission);
            }

            submission.IsEligible = true;

            return (true, submission);
        }
    }
}
