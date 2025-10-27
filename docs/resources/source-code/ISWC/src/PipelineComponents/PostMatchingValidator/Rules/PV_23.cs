using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules
{
    public class PV_23 : IRule, IAlwaysOnRule
    {
        private readonly IMessagingManager messagingManager;
        private readonly IWorkManager workManager;

        public PV_23(IMessagingManager messagingManager, IWorkManager workManager)
        {
            this.messagingManager = messagingManager;
            this.workManager = workManager;
        }

        public string Identifier => nameof(PV_23);
        public string ParameterName => string.Empty;
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CUR };
        public ValidatorType ValidatorType => ValidatorType.PostMatchingValidator;
        public string PipelineComponentVersion => typeof(PostMatchingValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var archivedIswc = (await workManager.FindVerifiedAsync(submission.Model.WorkNumber))?.ArchivedIswc;

            if (string.IsNullOrWhiteSpace(submission.Model.Iswc) ||
                (archivedIswc != null && archivedIswc == submission.Model.Iswc))
                return (true, submission);
            else
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._116);
                return (false, submission);
            }
        }
    }
}
