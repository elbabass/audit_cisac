using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Converters;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.IswcEligibilityValidator.Rules
{
    public class EL_01 : IRule
    {
        private readonly IRulesManager rulesManager;
        private readonly IAgreementManager agreementManager;

        public EL_01(IRulesManager rulesManager, IAgreementManager agreementManager)
        {
            this.rulesManager = rulesManager;
            this.agreementManager = agreementManager;
        }

        public string Identifier => nameof(EL_01);
        public string ParameterName => "ISWCEligibleRoles";
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR };
        public ValidatorType ValidatorType => ValidatorType.IswcEligibiltyValidator;

        public string PipelineComponentVersion => typeof(IswcEligibilityValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            if (submission.IsEligible)
                return (true, submission);

            if (submission.Model.InterestedParties == null || submission.Model.InterestedParties.Count == 0)
                return (false, submission);

            var paramValue = await rulesManager.GetParameterValue<string>(this.ParameterName);
            RuleConfiguration = paramValue;

            var allowPDWorkSubmissions = await rulesManager.GetParameterValue<bool>("AllowPDWorkSubmissions");
            var paramRoles = (Regex.Split(paramValue, @"[(, )]")).Where(x => !string.IsNullOrEmpty(x))
                .Select(a => (CisacInterestedPartyType)Enum.Parse(typeof(CisacInterestedPartyType), a)).ToList();

            var eligibleAgencies = ValidationRuleConverter.GetValues_IncludeAgenciesInEligibilityCheck(
                await rulesManager.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck"), submission.UpdateAllocatedIswc ? submission.AgencyToReplaceIasAgency : submission.Model.Agency);

            if (submission.Model.AdditionalAgencyWorkNumbers.Any())
                eligibleAgencies.ToList().AddRange(submission.Model.AdditionalAgencyWorkNumbers.Select(x => x.WorkNumber.Type));

            var agreements = await agreementManager.FindManyAsync(submission.Model.InterestedParties
            .Where(x => x.CisacType != null && paramRoles.Contains((CisacInterestedPartyType)x.CisacType))
            .Select(x => x.IpBaseNumber!));

            foreach (var agreement in agreements)
            {
                if (eligibleAgencies.Distinct().Contains(agreement.Agency.AgencyId) && submission.Model.InterestedParties.Any(
                    x => agreement.IpbaseNumber == x.IpBaseNumber))
                {
                    submission.IsEligible = true;
                    break;
                }
            }

         

            if (allowPDWorkSubmissions && !submission.IsEligible)
            {
                var creatorTypes = new List<CisacInterestedPartyType>() { CisacInterestedPartyType.C, CisacInterestedPartyType.TA, CisacInterestedPartyType.MA };
                var now = DateTime.UtcNow;

                submission.IsEligible = submission.Model.InterestedParties.Where(
                    x => x.CisacType != null && creatorTypes.Contains((CisacInterestedPartyType)x.CisacType))
                            .All(y => (CommonIPs.PublicDomainIps.Contains(y.IpBaseNumber) || y.DeathDate < now.AddYears(-80)) && paramRoles.Contains(y.CisacType ?? default));

                if (submission.IsEligible && submission.Model.AdditionalAgencyWorkNumbers.Any())
                    submission.Model.AdditionalAgencyWorkNumbers.Select(x => { x.IsEligible = true; return x; });
            }

            return (true, submission);
        }
    }
}
