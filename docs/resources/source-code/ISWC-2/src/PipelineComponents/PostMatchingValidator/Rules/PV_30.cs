using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules
{
    public class PV_30 : IRule, IAlwaysOnRule
	{
		private readonly IMessagingManager messagingManager;
		private readonly IWorkManager workManager;

		public PV_30(IWorkManager workManager, IMessagingManager messagingManager)
		{
			this.workManager = workManager;
			this.messagingManager = messagingManager;
		}

		public string Identifier => nameof(PV_30);
		public string ParameterName => string.Empty;
		public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.FSQ };
		public ValidatorType ValidatorType => ValidatorType.PostMatchingValidator;
		public string PipelineComponentVersion => typeof(PostMatchingValidator).GetComponentVersion();
		public string? RuleConfiguration { get; private set; }

		public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
		{
			if(submission.Model.DisambiguateFrom != null && submission.Model.DisambiguateFrom.Count >= 1 && submission.Model.Disambiguation)
            {
                foreach (DisambiguateFrom df in submission.Model.DisambiguateFrom)
                {
                    if (string.IsNullOrWhiteSpace(df.Iswc) || await workManager.FindAsync(df.Iswc) == null)
                    {
                        submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._128);
                        return (false, submission);
                    }
                }
            }
			
			return (true, submission);
		}
	}
}
