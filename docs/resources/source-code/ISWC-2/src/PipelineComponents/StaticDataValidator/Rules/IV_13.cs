using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules
{
    public class IV_13 : IRule
    {
        private readonly IMessagingManager messagingManager;
		private readonly IRulesManager rulesManager;

        public IV_13(IMessagingManager messagingManager, IRulesManager rulesManager)
        {
			this.rulesManager = rulesManager;
            this.messagingManager = messagingManager;
        }

        public string Identifier => nameof(IV_13);
        public string ParameterName => "PreferredISWCRequiredforUpdate";
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CMQ, TransactionType.CDR, TransactionType.CMQ };
        public ValidatorType ValidatorType => ValidatorType.StaticValidator;
		public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();
		public string? RuleConfiguration { get; private set; }

		public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
			var paramValue = await rulesManager.GetParameterValue<bool>("PreferredISWCRequiredforUpdate");
			RuleConfiguration = paramValue.ToString();

			if (!paramValue)
				return (true, submission);

            var model = submission.Model;

            if (string.IsNullOrWhiteSpace(model.PreferredIswc))
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._107);
                return (false, submission);
            }
            else
                return (true, submission);
        }

    }
}
