using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules
{
    public class IV_07 : IRule
    {
        private readonly IMessagingManager messagingManager;
        private readonly IRulesManager rulesManager;

        public IV_07(IRulesManager rulesManager, IMessagingManager messagingManager)
        {
            this.rulesManager = rulesManager;
            this.messagingManager = messagingManager;
        }

        public string Identifier => nameof(IV_07);
        public string ParameterName => "MinOfOneCreatorRole";
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.CUR, TransactionType.FSQ };
        public ValidatorType ValidatorType => ValidatorType.StaticValidator;
        public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var model = submission.Model;
            var paramValue = await rulesManager.GetParameterValueEnumerable<InterestedPartyType>(ParameterName);
            RuleConfiguration = await rulesManager.GetParameterValue<string>(ParameterName);

            if (model.InterestedParties == null || !model.InterestedParties.Any() || model.InterestedParties.Any(x => !x.Type.HasValue))
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._111);
                return (false, submission);
            }
            else
            {
                if (model.InterestedParties.Any(ip => paramValue.ToList().Contains(ip.Type ?? default)))
                    return (true, submission);
                else
                {
                    submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._111);
                    return (false, submission);
                }
            }
        }

    }
}