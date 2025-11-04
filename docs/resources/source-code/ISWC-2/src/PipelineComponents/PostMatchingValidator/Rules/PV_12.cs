using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules
{
    public class PV_12 : IRule, IAlwaysOnRule
    {
        private readonly IMessagingManager messagingManager;
        private readonly ILinkedToManager linkedToManager;

        public PV_12(IMessagingManager messagingManager, ILinkedToManager linkedToManager)
        {
            this.messagingManager = messagingManager;
            this.linkedToManager = linkedToManager;
        }

        public string Identifier => nameof(PV_12);

        public string ParameterName => string.Empty;

        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.DMR };

        public ValidatorType ValidatorType => ValidatorType.PostMatchingValidator;

        public string PipelineComponentVersion => typeof(PostMatchingValidator).GetComponentVersion();

        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var model = submission.Model;
            if (!await linkedToManager.MergeExists(model.PreferredIswc, model.WorkNumber))
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._172);
                return (false, submission);
            }

            return (true, submission);
        }
    }
}
