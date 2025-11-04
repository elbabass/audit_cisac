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
    public class IV_10 : IRule
    {
        private readonly IMessagingManager messagingManager;
        private readonly IAgencyManager agencyManager;
        private readonly IRulesManager rulesManager;

        public IV_10(IAgencyManager agencyManager, IRulesManager rulesManager, IMessagingManager messagingManager)
        {
            this.agencyManager = agencyManager;
            this.rulesManager = rulesManager;
            this.messagingManager = messagingManager;
        }

        public string Identifier => nameof(IV_10);
        public string ParameterName => "ValidateAgencyCode";
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> {
            TransactionType.CAR, TransactionType.CUR, TransactionType.CDR, TransactionType.FSQ
        };
        public ValidatorType ValidatorType => ValidatorType.StaticValidator;
        public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var paramValue = await rulesManager.GetParameterValue<bool>("ValidateAgencyCode");
            RuleConfiguration = paramValue.ToString();

            if (!paramValue)
                return (true, submission);

            var model = submission.Model;

            if (model.Agency != null && await agencyManager.FindAsync(model.Agency) == null)
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._103);
                return (false, submission);
            }

            return (true, submission);
        }

    }
}