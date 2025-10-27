using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules
{
    public class PV_22 : IRule, IAlwaysOnRule
    {
        private readonly IMessagingManager messagingManager;
        private readonly IWorkManager workManager;

        public PV_22(IWorkManager workManager, IMessagingManager messagingManager)
        {
            this.workManager = workManager;
            this.messagingManager = messagingManager;
        }

        public string Identifier => nameof(PV_22);
        public string ParameterName => string.Empty;
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CDR, TransactionType.CUR };
        public ValidatorType ValidatorType => ValidatorType.PostMatchingValidator;
        public string PipelineComponentVersion => typeof(PostMatchingValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var model = submission.Model;

            if (!string.IsNullOrEmpty(model.PreferredIswc) && !await workManager.Exists(model.PreferredIswc))
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._117);
                return (false, submission);
            }
            else if (submission.TransactionType == TransactionType.CDR)
            {
                if (model.WorkNumber != null && (await workManager.FindVerifiedAsync(model.WorkNumber))?.Iswc == model.PreferredIswc)
                    return (true, submission);
                else
                {
                    submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._117);
                    return (false, submission);
                }
            }
            else return (true, submission);
        }
    }
}

