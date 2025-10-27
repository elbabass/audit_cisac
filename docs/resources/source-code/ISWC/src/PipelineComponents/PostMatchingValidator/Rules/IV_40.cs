using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules
{
	public class IV_40: IRule
	{
		private readonly IMessagingManager messagingManager;
		private readonly IRulesManager rulesManager;
		private readonly IWorkManager workManager;

		public IV_40(IRulesManager rulesManager, IWorkManager workManager, IMessagingManager messagingManager)
		{
			this.rulesManager = rulesManager;
			this.workManager = workManager;
			this.messagingManager = messagingManager;
		}

		public string Identifier => nameof(IV_40);
		public string ParameterName => "ValidateDisambiguationISWCs";
		public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.FSQ };
		public ValidatorType ValidatorType => ValidatorType.PostMatchingValidator;
		public string PipelineComponentVersion => typeof(PostMatchingValidator).GetComponentVersion();
		public string? RuleConfiguration { get; private set; }

		public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
		{
			var model = submission.Model;
			var paramValue = await rulesManager.GetParameterValue<bool>(ParameterName);
			RuleConfiguration = paramValue.ToString();

			if (!paramValue || model.DisambiguateFrom == null || !model.DisambiguateFrom.Any())
				return (true, submission);
			else
			{
				foreach(DisambiguateFrom df in model.DisambiguateFrom)
				{
					if(df.Iswc == null || await workManager.FindAsync(df.Iswc) == null)
					{
						submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._129);
						return (false, submission);
					}
				}
				return (true, submission);
			}
		}
	}
}
