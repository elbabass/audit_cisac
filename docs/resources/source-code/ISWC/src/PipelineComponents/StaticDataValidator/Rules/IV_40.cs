using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules
{
    public class IV_40 : IRule
    {
        private readonly IMessagingManager messagingManager;
		private readonly IRulesManager rulesManager;
		private readonly ILookupManager lookupManager;

        public IV_40(IMessagingManager messagingManager, IRulesManager rulesManager, ILookupManager lookupManager)
        {
            this.messagingManager = messagingManager;
			this.rulesManager = rulesManager;
			this.lookupManager = lookupManager;
        }

        public string Identifier => nameof(IV_40);
        public string ParameterName => "ValidateDisambiguationInfo";
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.FSQ };
        public ValidatorType ValidatorType => ValidatorType.StaticValidator;
		public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();
		public string? RuleConfiguration { get; private set; }

		public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
			var paramValue = await rulesManager.GetParameterValue<bool>("ValidateDisambiguationInfo");
			RuleConfiguration = paramValue.ToString();

			if (!paramValue)
				return (true, submission);

            var model = submission.Model;

            if (model.Disambiguation)
            {
				if(model.DisambiguateFrom == null || model.DisambiguateFrom.Count == 0)
				{
					submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._124);
					return (false, submission);
				}
				else if (model.Performers != null && model.Performers.Any(p => string.IsNullOrWhiteSpace(p.LastName)))
				{
					submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._126);
					return (false, submission);
				}
				else if (model.DisambiguationReason == null || !Enum.IsDefined(typeof(DisambiguationReason), model.DisambiguationReason)){
					submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._123);
					return (false, submission);
				}
				else if(model.BVLTR != null && !Enum.IsDefined(typeof(BVLTR), model.BVLTR))
				{
					submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._125);
					return (false, submission);
				}
				else if (model.Instrumentation != null && model.Instrumentation.Count() != 0 && !await IsValidInstrumentationCode(model.Instrumentation))
				{
					submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._146);
					return (false, submission);
				}
				else 
				{
					foreach (DisambiguateFrom df in model.DisambiguateFrom)
					{
						if (!string.IsNullOrEmpty(df.Iswc) && df.Iswc.IsValidIswcPattern())
						{
							return (true, submission);
						}
					}
					submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._122);
					return (false, submission);
				}
			}
            else return (true, submission);
        }

		private async Task<bool> IsValidInstrumentationCode(IEnumerable<Instrumentation> instrumentations)
		{
			foreach(Instrumentation i in instrumentations)
			{
				if (await lookupManager.GetInstrumentationByCodeAsync(i.Code) == null)
					return false;
			}
			return true;
		}
    }
}
