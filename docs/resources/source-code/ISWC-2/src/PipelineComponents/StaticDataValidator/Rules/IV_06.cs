using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules
{
    public class IV_06 : IRule
    {
        private readonly IMessagingManager messagingManager;
		private readonly IRulesManager rulesManager;

        public IV_06(IMessagingManager messagingManager, IRulesManager rulesManager)
        {
            this.messagingManager = messagingManager;
			this.rulesManager = rulesManager;
        }

        public string Identifier => nameof(IV_06);
        public string ParameterName => "OnlyOneOriginalTitle";
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> {
            TransactionType.CAR, TransactionType.CUR, TransactionType.CDR, TransactionType.FSQ
        };
        public ValidatorType ValidatorType => ValidatorType.StaticValidator;
		public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();
		public string? RuleConfiguration { get; private set; }

		public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
			var paramValue = await rulesManager.GetParameterValue<bool>("OnlyOneOriginalTitle");
			RuleConfiguration = paramValue.ToString();

			if (!paramValue)
				return (true, submission);

            var model = submission.Model;

            if (model.Titles != null && model.Titles.Count(title => title.Type == TitleType.OT) > 1)
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._110);
                return (false, submission);
            }
            else
                return (true, submission);
        }
    }
}
