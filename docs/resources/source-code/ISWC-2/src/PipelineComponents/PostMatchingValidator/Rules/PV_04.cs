using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules
{
    public class PV_04 : IRule, IAlwaysOnRule
    {
        private readonly IMessagingManager messagingManager;
        private readonly IRulesManager rulesManager;

        public PV_04(IMessagingManager messagingManager, IRulesManager rulesManager)
        {
            this.messagingManager = messagingManager;
            this.rulesManager = rulesManager;
        }

        public string Identifier => nameof(PV_04);

        public string ParameterName => "EnablePVTitleStandardization";

        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.FSQ };

        public ValidatorType ValidatorType => ValidatorType.PostMatchingValidator;
        public string PipelineComponentVersion => typeof(PostMatchingValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {

            var paramValue = await rulesManager.GetParameterValue<bool>(ParameterName);
            RuleConfiguration = paramValue.ToString();

            if (!submission.Model.PreviewDisambiguation && submission.MatchedResult != null && submission.MatchedResult.Matches.Any() && (submission.MatchedResult.Matches.Count() == 1
                || MultipleMatchedWorksButOneIswc()))
            {
                var matches = submission.MatchedResult.Matches.Where(m => m.Numbers.Any(n => n.Type == "ISWC" && n?.Number == submission.Model?.PreferredIswc));

                if (!matches.Any()) return (true, submission);

                var titleExclusions = await rulesManager.GetParameterValueEnumerable<TitleType>("ExcludeTitleTypes");

                if (!(submission.TransactionType == TransactionType.FSQ && matches.Any(x => x.MatchType == Bdo.MatchingEngine.MatchSource.MatchedByNumber)) &&
                    matches.Any(m => m.Contributors.Where(x => !string.IsNullOrWhiteSpace(x.IpBaseNumber) && x.ContributorType == ContributorType.Creator)
                .Any(t => !submission.Model.InterestedParties.Where(ip => t.IpBaseNumber == ip.IpBaseNumber).Any()))
                    && submission.RequestType != RequestType.Label)
                {
                    submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._247);
                    return (false, submission);
                }

                if (NoTitleMatches())
                {
                    submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._127);
                    return (false, submission);
                }

                bool NoTitleMatches()
                {
                    if (submission.IsEligible && submission.Model.PreferredIswc != submission.MatchedResult.Matches?.First().Numbers.FirstOrDefault(x => x.Type == "ISWC")?.Number)
                        return true;

                    if (paramValue && matches.Any(m => StringExtensions.StringComparisonExact(m.StandardizedTitle, submission.MatchedResult.StandardizedName)))
                        return false;


                    if (matches.Any(m => m.Titles.Any() && m.Titles.Where(x => !titleExclusions.Any(t => t == x.Type))
                        .Any(t => submission.Model.Titles.Where(ip => (StringExtensions.StringComparisonExact(t.Name, ip.Name) || StringExtensions.StringComparisonExact(t.Name, ip.StandardizedName)
                        || StringExtensions.StringComparisonExactSanitised(t.Name, ip.Name)) && t.Type == ip.Type).Any())))
                        return false;



                    return true;
                }
            }

            bool MultipleMatchedWorksButOneIswc()
            {
                var matches = submission.MatchedResult.Matches;

                if (matches.Any())
                {
                    return matches.Select(mw => mw.Numbers.FirstOrDefault(n => n.Type == "ISWC")).All(x => x?.Number == submission.Model.PreferredIswc);
                }

                return false;
            }

            return (true, submission);

        }
    }
}
