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
    public class PV_09 : IRule, IAlwaysOnRule
    {
        private readonly IMessagingManager messagingManager;
        private readonly IWorkManager workManager;

        public PV_09(IWorkManager workManager, IMessagingManager messagingManager)
        {
            this.workManager = workManager;
            this.messagingManager = messagingManager;
        }

        public string Identifier => nameof(PV_09);
        public string ParameterName => string.Empty;
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.MER, TransactionType.DMR };
        public ValidatorType ValidatorType => ValidatorType.PostMatchingValidator;
        public string PipelineComponentVersion => typeof(PostMatchingValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var model = submission.Model;

            if(submission.TransactionType == TransactionType.DMR)
            {
                var iswc = await workManager.FindAsync(model.PreferredIswc!);
                if (iswc == null || iswc.IsReplaced)
                {
                    submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._153);
                    return (false, submission);
                }

                return (true, submission);
            }

            if (await workManager.FindAsync(model.PreferredIswc!) == null || (!model.IswcsToMerge.Any() && !model.WorkNumbersToMerge.Any()))
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._132);
                return (false, submission);
            }
            else
            {
                foreach (var iswcToMerge in model.IswcsToMerge)
                {
                    var iswc = await workManager.FindAsync(iswcToMerge);
                    if (iswc == null || iswc.IsReplaced)
                    {
                        submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._132);
                        return (false, submission);
                    }
                }

                foreach (var workNumber in model.WorkNumbersToMerge)
                {
                    var workInfo = await workManager.FindAsync(workNumber);

                    if (workInfo == null || workInfo.IsReplaced)
                    {
                        submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._132);
                        return (false, submission);
                    }
                }

                return (true, submission);
            }
        }
    }
}
