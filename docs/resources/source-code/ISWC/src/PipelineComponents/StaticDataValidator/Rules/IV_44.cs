using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules
{
    public class IV_44 : IRule
    {
        private readonly IMessagingManager messagingManager;

        public IV_44(IMessagingManager messagingManager)
        {
            this.messagingManager = messagingManager;
        }

        public string Identifier => nameof(IV_44);
        public string ParameterName => "PreferredISWCRequired";
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CMQ };
        public ValidatorType ValidatorType => ValidatorType.StaticValidator;
		public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();
		public string? RuleConfiguration { get; private set; }

		public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
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