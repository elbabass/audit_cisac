using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules
{
    public class PV_24 : IRule, IAlwaysOnRule
    {
        private readonly IMessagingManager messagingManager;
        private readonly IWorkManager workManager;

        public PV_24(IWorkManager workManager, IMessagingManager messagingManager)
        {
            this.workManager = workManager;
            this.messagingManager = messagingManager;
        }
        public string Identifier => nameof(PV_24);
        public string ParameterName => string.Empty;
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.FSQ };
        public ValidatorType ValidatorType => ValidatorType.PostMatchingValidator;
        public string PipelineComponentVersion => typeof(PostMatchingValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            if (!string.IsNullOrEmpty(submission.Model.PreferredIswc) && !await workManager.Exists(submission.Model.PreferredIswc))
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._117);
                return (false, submission);
            }
            if(!string.IsNullOrEmpty(submission.Model.Iswc) && submission.Model.AllowProvidedIswc && await workManager.Exists(submission.Model.Iswc))
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._169);
                return (false, submission);
            }
            if (!string.IsNullOrEmpty(submission.Model.Iswc) && !submission.Model.AllowProvidedIswc && submission.Model.PreferredIswc != submission.Model.Iswc)
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._117);
                return (false, submission);
            }
            else
                return (true, submission);
        }
    }
}
