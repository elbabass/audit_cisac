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

namespace SpanishPoint.Azure.Iswc.PipelineComponents.LookupDataValidator.Rules
{
    public class IV_24 : IRule
    {
        private readonly IRulesManager rulesManager;
        private readonly IInterestedPartyManager interestedPartyManager;
        private readonly IMessagingManager messagingManager;

        public IV_24(IRulesManager rulesManager, IInterestedPartyManager interestedPartyManager, IMessagingManager messagingManager)
        {
            this.rulesManager = rulesManager;
            this.interestedPartyManager = interestedPartyManager;
            this.messagingManager = messagingManager;
        }

        public string Identifier => nameof(IV_24);
        public string ParameterName => "AllowPDWorkSubmissions";
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.CUR, TransactionType.FSQ };
        public ValidatorType ValidatorType => ValidatorType.LookupDataValidator;
        public string PipelineComponentVersion => typeof(LookupDataValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var model = submission.Model;
            bool paramValue = await rulesManager.GetParameterValue<bool>(this.ParameterName);
            RuleConfiguration = paramValue.ToString();

            var allIps = await interestedPartyManager
               .FindManyByNameNumber(model.InterestedParties.SelectMany(ip => ip.Names.Select(x => x.IpNameNumber)));
            var now = DateTime.UtcNow;

            foreach (var interestedParty in allIps)
            {
                foreach (var ip in model.InterestedParties.Where(ip => ip.IpBaseNumber == interestedParty.IpBaseNumber))
                {
                    ip.DeathDate = interestedParty.DeathDate;
                }
            }

            var isFullyPublicDomain = allIps.Any() && allIps.All(x => CommonIPs.PublicDomainIps.Contains(x.IpBaseNumber) || (x.DeathDate < now.AddYears(-80)));

            if (isFullyPublicDomain)
            {
                if (paramValue)
                {
                    if (allIps.Select(x => x.IpBaseNumber).All(y => CommonIPs.PublicDomainIps.Contains(y)))
                    {
                        submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._108);
                        return (false, submission);
                    }
                    else
                        return (true, submission);
                }
                else
                {
                    submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._108);
                    return (false, submission);
                }
            }
            else
                return (true, submission);
        }
    }
}
