using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules
{
    public class IV_54 : IRule
    {
        private readonly IMessagingManager messagingManager;
        private readonly IRulesManager rulesManager;
        private readonly IAgentManager agentVersionManager;

        public IV_54(IMessagingManager messagingManager, IRulesManager rulesManager, IAgentManager agentVersionManager)
        {
            this.messagingManager = messagingManager;
            this.rulesManager = rulesManager;
            this.agentVersionManager = agentVersionManager;
        }

        public string Identifier => nameof(IV_54);

        public string ParameterName => "ValidateAgentVersion";

        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.CUR, TransactionType.CDR };
        public ValidatorType ValidatorType => ValidatorType.StaticValidator;
        public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var paramValue = await rulesManager.GetParameterValue<bool>("ValidateAgentVersion");
            RuleConfiguration = paramValue.ToString();

            if (paramValue && !string.IsNullOrWhiteSpace(submission.AgentVersion) && !await agentVersionManager.CheckAgentVersionAsync(submission.AgentVersion))
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._173);
                return (false, submission);
            }

            return (true, submission);
        }
    }
}
