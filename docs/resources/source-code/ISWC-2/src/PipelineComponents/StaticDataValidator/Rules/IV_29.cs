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
    public class IV_29 : IRule
    {
        private readonly IMessagingManager messagingManager;
        private readonly IRulesManager rulesManager;
        private readonly IInterestedPartyManager interestedPartyManager;

        public IV_29(IRulesManager rulesManager, IInterestedPartyManager interestedPartyManager, IMessagingManager messagingManager)
        {
            this.rulesManager = rulesManager;
            this.interestedPartyManager = interestedPartyManager;
            this.messagingManager = messagingManager;
        }

        public string Identifier => nameof(IV_29);
        public string ParameterName => "RejectIPs";
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.CUR, TransactionType.FSQ };
        public ValidatorType ValidatorType => ValidatorType.StaticValidator;
        public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var model = submission.Model;
            var paramValue = await rulesManager.GetParameterValueEnumerable<string>(this.ParameterName);
            RuleConfiguration = await rulesManager.GetParameterValue<string>(ParameterName);
            IEnumerable<InterestedPartyModel> ipsToReject = await interestedPartyManager.FindManyByBaseNumber(paramValue);

            foreach (var interestedParty in model.InterestedParties)
            {
                var partyNames = interestedParty?.Names ?? Enumerable.Empty<NameModel>();

                var hasNameClash =
                    (ipsToReject ?? Enumerable.Empty<InterestedPartyModel>())
                        .SelectMany(x => x.Names ?? Enumerable.Empty<NameModel>())
                        .Any(i => partyNames.Any(x => x.IpNameNumber == i.IpNameNumber));

                var hasBaseNumberClash =
                    (ipsToReject ?? Enumerable.Empty<InterestedPartyModel>())
                        .Any(x => x.IpBaseNumber == interestedParty?.IpBaseNumber);

                if (hasNameClash || hasBaseNumberClash)
                {
                    submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._148);
                    return (false, submission);
                }
            }

            return (true, submission);
        }
    }
}
