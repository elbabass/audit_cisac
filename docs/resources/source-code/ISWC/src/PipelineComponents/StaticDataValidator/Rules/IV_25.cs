using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules
{
    public class IV_25 : IRule
    {
        private readonly IMessagingManager messagingManager;
        private readonly IRulesManager rulesManager;
        private readonly IInterestedPartyManager interestedPartyManager;

        public IV_25(IRulesManager rulesManager, IInterestedPartyManager interestedPartyManager, IMessagingManager messagingManager)
        {
            this.rulesManager = rulesManager;
            this.interestedPartyManager = interestedPartyManager;
            this.messagingManager = messagingManager;
        }
        public string Identifier => nameof(IV_25);
        public string ParameterName => "IgnoreIPsForMatchingAndEligibility";
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.CUR, TransactionType.FSQ };
        public ValidatorType ValidatorType => ValidatorType.StaticValidator;
        public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var model = submission.Model;
            var paramValue = await rulesManager.GetParameterValueEnumerable<string>(this.ParameterName);
            IEnumerable<InterestedPartyModel> ipsToIgnore = await interestedPartyManager.FindManyByBaseNumber(paramValue);

            foreach (var interestedParty in model.InterestedParties.ToList())
            {
                if (interestedParty.Names.Any(i => ipsToIgnore.SelectMany(x => x.Names).Select(x => x.IpNameNumber).Contains(i.IpNameNumber)))
                {
                    model.InterestedParties.Remove(interestedParty);
                }
            }
            return (true, submission);
        }

    }
}
