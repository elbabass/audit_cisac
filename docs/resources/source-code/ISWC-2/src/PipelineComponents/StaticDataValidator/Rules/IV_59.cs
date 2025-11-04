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
    public class IV_59 : IRule
    {
        private readonly IMessagingManager messagingManager;
        private readonly IRulesManager rulesManager;

        public IV_59(IMessagingManager messagingManager, IRulesManager rulesManager)
        {
            this.messagingManager = messagingManager;
            this.rulesManager = rulesManager;
        }

        public string Identifier => nameof(IV_59);
        public string ParameterName => string.Empty;
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.CUR };
        public ValidatorType ValidatorType => ValidatorType.StaticValidator;
        public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var model = submission.Model;

            if (submission.RequestType == RequestType.Label && model.AdditionalIdentifiers == null)
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._250);
                return (false, submission);
            }

            if (submission.RequestType == RequestType.Label && !(model.AdditionalIdentifiers?.Any(x => !string.IsNullOrEmpty(x.SubmitterDPID)) ?? false))
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._250);
                return (false, submission);
            }
            
            return (true, submission);
        }
    }
}
