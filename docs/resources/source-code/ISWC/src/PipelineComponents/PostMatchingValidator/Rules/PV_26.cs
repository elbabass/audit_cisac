using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using SpanishPoint.Azure.Iswc.Bdo.MatchingEngine;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules
{
    public class PV_26 : IRule, IAlwaysOnRule
    {
        private readonly IMessagingManager messagingManager;
        private readonly IRulesManager rulesManager;

        public PV_26(IMessagingManager messagingManager, IRulesManager rulesManager)
        {
            this.messagingManager = messagingManager;
            this.rulesManager = rulesManager;
        }

        public string Identifier => nameof(PV_26);

        public string ParameterName => "EnablePVTitleStandardization";

        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType>() { TransactionType.CUR };

        public ValidatorType ValidatorType => ValidatorType.PostMatchingValidator;
        public string PipelineComponentVersion => typeof(PostMatchingValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var paramValue = await rulesManager.GetParameterValue<bool>(ParameterName);
            RuleConfiguration = paramValue.ToString();

            var model = submission.Model;
            var workNumber = submission.Model.WorkNumber;

            var topMatchedIswc = GetTopMatchedIswc();

            var isExactMatch = GetExactMatch();

            if (string.IsNullOrWhiteSpace(model.PreferredIswc) || workNumber == null ||
                string.IsNullOrWhiteSpace(workNumber.Number) || workNumber.Type == null ||
                string.IsNullOrWhiteSpace(workNumber.Type))
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._116);
                return (false, submission);
            }
            else if (model.PreferredIswc != topMatchedIswc && !isExactMatch && submission.RequestType != RequestType.Label)
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._116);
                return (false, submission);
            }
            else if (submission.ExistingWork == null)
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._116);
                return (false, submission);
            }

            return (true, submission);

            string GetTopMatchedIswc()
            {
                if (submission.MatchedResult == null || !submission.MatchedResult.Matches.Any())
                    return string.Empty;

                var topMatch = submission.IsPortalSubmissionFinalStep ?
                    submission.MatchedResult?.Matches
                    .FirstOrDefault(x => x.Numbers.Any(n => !string.IsNullOrEmpty(n.Number) && !n.Number.Equals(model.Iswc)
                    && !string.IsNullOrEmpty(n.Type) && n.Type.Equals("ISWC") && n.Number.Equals(model.PreferredIswc)))?.Numbers
                    : submission.MatchedResult?.Matches
                    .FirstOrDefault(x => x.Numbers.Any(n => !string.IsNullOrEmpty(n.Number) && !n.Number.Equals(model.Iswc)
                    && !string.IsNullOrEmpty(n.Type) && n.Type.Equals("ISWC")))?.Numbers;

                if (topMatch == null || !topMatch.Any())
                    return string.Empty;


                return topMatch.FirstOrDefault(n => !string.IsNullOrEmpty(n.Type) && n.Type.Equals("ISWC")
                && !string.IsNullOrEmpty(n.Number) && !n.Number.Equals(model.Iswc))?.Number ?? string.Empty;
            }

            bool GetExactMatch()
            {
                if (string.IsNullOrEmpty(topMatchedIswc)) return false;

                foreach (var match in submission.MatchedResult.Matches.Where(x => x.Numbers.Any(n => !string.IsNullOrEmpty(n.Number)
                && n.Number.Equals(topMatchedIswc))))
                {
                    if (checkIpsMatch(match) && TitlesMatch(match))
                        return true;
                }

                return false;
            }

            bool TitlesMatch(MatchingWork matchingWork)
            {
                if (submission.IsEligible) return true;

                if (paramValue)
                    return StringExtensions.StringComparisonExact(matchingWork.StandardizedTitle, submission.MatchedResult.StandardizedName);

                else
                    return matchingWork.Titles.All(t => model.Titles.Any(x => (StringExtensions.StringComparisonExact(t.Name, x.Name)
                             || StringExtensions.StringComparisonExact(t.Name, x.StandardizedName)
                             || StringExtensions.StringComparisonExactSanitised(t.Name, x.Name))));
            }

            bool checkIpsMatch(MatchingWork matchingWork)
            {
                var x = matchingWork.Contributors.Where(x => x.ContributorType == Bdo.Ipi.ContributorType.Creator).Count().Equals(model.InterestedParties.Where(x => x.IsWriter).Count())
                   && matchingWork.Contributors.Where(x => x.ContributorType == Bdo.Ipi.ContributorType.Creator).All(c => model.InterestedParties.Where(x => x.IsWriter).Any(ip => !string.IsNullOrEmpty(ip.IpBaseNumber)
                    && ip.IpBaseNumber.Equals(c.IpBaseNumber)));

                return x || matchingWork.IswcStatus == 2;
            }
        }
    }
}
