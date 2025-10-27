using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpanishPoint.Azure.Iswc.Framework.Converters;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using System.Text.RegularExpressions;
using System;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.IswcEligibilityValidator.Rules
{
    public class EL_04 : IRule
    {
        private readonly IAgreementManager agreementManager;
        private readonly IMessagingManager messagingManager;
        private readonly IInterestedPartyManager interestedPartyManager;
        private readonly IRulesManager rulesManager;

        public EL_04(IAgreementManager agreementManager, IMessagingManager messagingManager, IInterestedPartyManager interestedPartyManager, IRulesManager rulesManager)
        {
            this.agreementManager = agreementManager;
            this.messagingManager = messagingManager;
            this.interestedPartyManager = interestedPartyManager;
            this.rulesManager = rulesManager;
        }

        public string Identifier => nameof(EL_04);
        public string ParameterName => "ISWCEligibleRoles";
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR };
        public ValidatorType ValidatorType => ValidatorType.IswcEligibiltyValidator;
        public string PipelineComponentVersion => typeof(IswcEligibilityValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }


        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            if (submission.Model.AdditionalIdentifiers == null || !submission.Model.AdditionalIdentifiers.Any(x => x.NameNumber != null) || submission.RequestType == RequestType.Label)
                return (true, submission);

            var submittingPublisher = (await interestedPartyManager.FindManyByNameNumber(new List<long> { submission.Model.AdditionalIdentifiers.First(x => x.NameNumber != null).NameNumber ?? 0 })).FirstOrDefault();

            var paramValue = await rulesManager.GetParameterValue<string>(this.ParameterName);
            RuleConfiguration = paramValue;

            var paramRoles = (Regex.Split(paramValue, @"[(, )]")).Where(x => !string.IsNullOrEmpty(x))
                .Select(a => (CisacInterestedPartyType)Enum.Parse(typeof(CisacInterestedPartyType), a)).ToList();
            var eligibleAgencies = ValidationRuleConverter.GetValues_IncludeAgenciesInEligibilityCheck(
                 await rulesManager.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck"), submission.Model.Agency);


            if (submittingPublisher != null || submittingPublisher?.IpBaseNumber != null)
            {
                if (eligibleAgencies.Any() && submission.Model.InterestedParties.Any())
                {

                    if (!await CheckAgreementsByIPs(submission.Model.InterestedParties))
                    {
                        submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._161);
                        return (false, submission);
                    }

                }

                if (!string.IsNullOrWhiteSpace(submittingPublisher.IpBaseNumber) && !(await CheckAgreementsByPublisher(submittingPublisher.IpBaseNumber)))
                {
                    submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._161);
                    return (false, submission);
                }

                return (true, submission);

            }
            else
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._161);
                return (false, submission);
            }




            async Task<bool> CheckAgreementsByPublisher(string ipBaseNumber)
            {
                var agreements = await agreementManager.FindManyAsync(new List<string> { ipBaseNumber });

                foreach (var agreement in agreements)
                {
                    if (eligibleAgencies.Contains(agreement.Agency.AgencyId) && agreement.IpbaseNumber == ipBaseNumber)
                        return true;
                }

                return false;
            }

            async Task<bool> CheckAgreementsByIPs(IEnumerable<InterestedPartyModel> interestedParties)
            {

                var agreements = await agreementManager.FindManyAsync(interestedParties

                .Select(x => x.IpBaseNumber!));

                foreach (var agreement in agreements)
                {
                    if (eligibleAgencies.Contains(agreement.Agency.AgencyId) && interestedParties.Any(x => agreement.IpbaseNumber == x.IpBaseNumber))
                        return true;
                }

                return false;
            }
        }
    }
}
