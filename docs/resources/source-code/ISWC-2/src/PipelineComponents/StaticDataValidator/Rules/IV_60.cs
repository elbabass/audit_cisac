using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules
{
    public class IV_60 : IRule
    {
        private readonly IMessagingManager messagingManager;
        private readonly IRulesManager rulesManager;

        public IV_60(IMessagingManager messagingManager, IRulesManager rulesManager)
        {
            this.messagingManager = messagingManager;
            this.rulesManager = rulesManager;
        }

        public string Identifier => nameof(IV_60);
        public string ParameterName => string.Empty;
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.CUR };
        public ValidatorType ValidatorType => ValidatorType.StaticValidator;
        public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var model = submission.Model;

            if (submission.RequestType == RequestType.Label && model.InterestedParties != null 
                && model.InterestedParties.Any(x => (string.IsNullOrEmpty(x.Name) || string.IsNullOrEmpty(x.LastName))))
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._251);
                return (false, submission);
            }
            
            return (true, submission);
        }
    }
}
