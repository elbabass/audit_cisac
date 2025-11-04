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
    public class EL_03 : IRule
    {
        private readonly IRulesManager rulesManager;
        private readonly IAgreementManager agreementManager;
        private readonly IWorkManager workManager;

        public EL_03(IRulesManager rulesManager, IAgreementManager agreementManager, IWorkManager workManager)
        {
            this.rulesManager = rulesManager;
            this.agreementManager = agreementManager;
            this.workManager = workManager;
        }

        public string Identifier => nameof(EL_03);
        public string ParameterName => "ISWCEligibleRoles";
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CUR, TransactionType.MER, TransactionType.DMR, TransactionType.CDR };
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



            submission.IsEligible = await CheckAgreements(submission.Model.InterestedParties.ToList());

            if (allowPDWorkSubmissions && !submission.IsEligible)
            {
                var creatorTypes = new List<CisacInterestedPartyType>() { CisacInterestedPartyType.C, CisacInterestedPartyType.TA, CisacInterestedPartyType.MA };
                var now = DateTime.UtcNow;

                submission.IsEligible = submission.Model.InterestedParties.Where(
                   x => x.CisacType != null && creatorTypes.Contains((CisacInterestedPartyType)x.CisacType))
                   .All(y => (CommonIPs.PublicDomainIps.Contains(y.IpBaseNumber) || y.DeathDate < now.AddYears(-80)) && paramRoles.Contains(y.CisacType ?? default));

                if (submission.TransactionType == TransactionType.CUR && submission.IsEligible)
                    submission.Model.AdditionalAgencyWorkNumbers.Select(x => { x.IsEligible = true; return x; });
            }

            if (!submission.IsEligible && submission.TransactionType == TransactionType.CUR)
            {
                var workinfoRecord = submission.Model.WorkNumber != null ? await workManager.FindAsync(submission.Model.WorkNumber) : null;

                if (workinfoRecord != null && workinfoRecord.Iswc == submission.Model.PreferredIswc)
                {
                    var isEligibleForDeletingIps = await CheckAgreements(workinfoRecord.InterestedParties.ToList());

                    if (!submission.IsEligible && isEligibleForDeletingIps)
                        submission.IsEligibileOnlyForDeletingIps = true;

                    submission.IsEligible = isEligibleForDeletingIps;
                }
            }

            async Task<bool> CheckAgreements(List<InterestedPartyModel> interestedParties)
            {
                var agreements = await agreementManager.FindManyAsync(interestedParties
                .Where(x => x.CisacType != null && paramRoles.Contains((CisacInterestedPartyType)x.CisacType))
                .Select(x => x.IpBaseNumber!));

                bool isEligible = false;
                foreach (var agreement in agreements)
                {
                    if (eligibleAgencies.Contains(agreement.Agency.AgencyId) && interestedParties.Any(x => agreement.IpbaseNumber == x.IpBaseNumber))
                    {
                        isEligible = true;
                        break;
                    }
                }


                return isEligible;
            }

            return (true, submission);
        }
    }
}
