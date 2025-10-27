using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SpanishPoint.Azure.Iswc.PipelineComponents.IswcEligibilityValidator.Rules
{
    public class EL_02 : IRule
    {
        private readonly IAgreementManager agreementManager;
        private readonly IRulesManager rulesManager;
        private readonly IMessagingManager messagingManager;

        public EL_02(IAgreementManager agreementManager, IRulesManager rulesManager, IMessagingManager messagingManager)
        {
            this.agreementManager = agreementManager;
            this.rulesManager = rulesManager;
            this.messagingManager = messagingManager;
        }

        public string Identifier => nameof(EL_02);
        public string ParameterName => "AllowNonAffiliatedSubmissions";
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR };
        public ValidatorType ValidatorType => ValidatorType.IswcEligibiltyValidator;
        public string PipelineComponentVersion => typeof(IswcEligibilityValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var paramValue = await rulesManager.GetParameterValue<bool>(ParameterName);
            RuleConfiguration = paramValue.ToString();

            if (submission.IsEligible)
                return (true, submission);

            if (submission.Model.InterestedParties == null || submission.Model.InterestedParties.Count == 0)
                return (false, submission);

            var creatorTypes = new List<CisacInterestedPartyType>() { CisacInterestedPartyType.C, CisacInterestedPartyType.TA, CisacInterestedPartyType.MA };

            var affiliatedAgencies = (await agreementManager.FindManyAsync(submission.Model.InterestedParties
            .Where(x => x.CisacType != null && creatorTypes.Contains((CisacInterestedPartyType)x.CisacType))
            .Select(y => y.IpBaseNumber!))).Select(z => z.AgencyId).Distinct();

            if (!paramValue && !affiliatedAgencies.Any())
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._154);
                return (false, submission);
            }

            if (affiliatedAgencies != null && affiliatedAgencies.Any())
            {
                if (affiliatedAgencies.Any(x => !x.Equals(submission.UpdateAllocatedIswc ? submission.AgencyToReplaceIasAgency : submission.Model.Agency) && submission.RequestType != RequestType.Label))
                    submission.IsEligible = false;
                else
                    submission.IsEligible = true;

            }
            else
                submission.IsEligible = true;


            return (true, submission);
        }
    }
}
