using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules
{
    public class PV_13: IRule, IAlwaysOnRule
    {
        private readonly IMessagingManager messagingManager;

        public PV_13(IMessagingManager messagingManager)
        {
            this.messagingManager = messagingManager;
        }

        public string Identifier => nameof(PV_13);
        public string ParameterName => string.Empty;
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CDR };
        public ValidatorType ValidatorType => ValidatorType.PostMatchingValidator;
		public string PipelineComponentVersion => typeof(PostMatchingValidator).GetComponentVersion();
		public string? RuleConfiguration { get; private set; }

		public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var model = submission.Model;

            if (string.IsNullOrWhiteSpace(model.ReasonCode))
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._133);
                return (false, submission);
            }
            else
                return (true, submission);
        }
    }
}
