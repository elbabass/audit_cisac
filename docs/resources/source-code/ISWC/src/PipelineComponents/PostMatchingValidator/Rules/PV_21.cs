using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.MatchingEngine;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Converters;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules
{
    public class PV_21 : IRule, IAlwaysOnRule
    {
        private readonly IInterestedPartyManager interestedPartyManager;
        private readonly IWorkManager workManager;
        private readonly IMessagingManager messagingManager;
        private readonly IRulesManager rulesManager;

        public PV_21(IInterestedPartyManager interestedPartyManager, IWorkManager workManager, IMessagingManager messagingManager, IRulesManager rulesManager)
        {
            this.interestedPartyManager = interestedPartyManager;
            this.workManager = workManager;
            this.messagingManager = messagingManager;
            this.rulesManager = rulesManager;
        }

        public string Identifier => nameof(PV_21);
        public string ParameterName => string.Empty;
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CUR };
        public ValidatorType ValidatorType => ValidatorType.PostMatchingValidator;
        public string PipelineComponentVersion => typeof(PostMatchingValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var model = submission.Model;
            var eligibleAgencies = ValidationRuleConverter.GetValues_IncludeAgenciesInEligibilityCheck(
               await rulesManager.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck"), submission.Model.Agency);

            if (submission.MatchedResult?.Matches == null || !submission.MatchedResult.Matches.Any())
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._144);
                return (false, submission);
            }
            else if (submission.UpdateAllocatedIswc && !submission.IsEligible)
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._144);
                return (false, submission);
            }
            else if (model.WorkNumber != null && await otherEligibleWorksOnIswc(submission))
            {
                if (!await CheckIPsIfDeletedAreAuthoritative())
                {
                    submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._145);
                    return (false, submission);
                }

                return (true, submission);
            }
            else
                return (true, submission);

            async Task<bool> CheckIPsIfDeletedAreAuthoritative()
            {
                var work = await workManager.FindAsync(model.PreferredIswc, iswcEligibleOnly: true, excludeLabelSubmissions: true);
                var deletedIps = work != null ? GetDeletedIps(work.InterestedParties, model.InterestedParties) : null;

                if (deletedIps != null && deletedIps.Any())
                {
                    var mostRecentSubmissionByAgency = await workManager.FindAsync(model.WorkNumber);
                    foreach (var interestedParty in deletedIps.Where(i => !submission.Model.InterestedParties.Any(x => x.IPNameNumber == i.IPNameNumber)))
                    {

                        var isPublicDomain = CommonIPs.PublicDomainIps.Contains(interestedParty.IpBaseNumber) || (interestedParty.DeathDate < DateTime.UtcNow.AddYears(-80));
                        if (mostRecentSubmissionByAgency.InterestedParties.Any(i => i.IpBaseNumber == interestedParty.IpBaseNumber) || isPublicDomain) continue;

                        else if (!(submission.Model?.Agency != null && await interestedPartyManager.IsAuthoritative(interestedParty, eligibleAgencies)))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            List<InterestedPartyModel> GetDeletedIps(IEnumerable<InterestedPartyModel> workinfoIps, ICollection<InterestedPartyModel> interestedParties)
            {
                var interestedPartiesIpBaseNumbers = interestedParties.Select(x => x.IpBaseNumber);
                return workinfoIps.Where(x => x.Type == InterestedPartyType.C && !interestedPartiesIpBaseNumbers.Contains(x.IpBaseNumber)).ToList();
            }

            async Task<bool> otherEligibleWorksOnIswc(Submission submission)
            {
                if (submission.Model.PreferredIswc == null) return false;
                return await workManager.HasOtherEligibleWorks(model.PreferredIswc, model.Agency, excludeLabelSubmissions: true);
            }
        }
    }
}
