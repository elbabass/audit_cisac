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
using AutoMapper;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Bdo.MatchingEngine;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules
{
    public class PV_20 : IRule, IAlwaysOnRule
    {
        private readonly IMessagingManager messagingManager;
        private readonly IWorkManager workManager;
        private readonly IRulesManager rulesManager;
        private readonly IMapper mapper;

        public PV_20(IMessagingManager messagingManager, IWorkManager workManager, IRulesManager rulesManager, IMapper mapper)
        {
            this.messagingManager = messagingManager;
            this.workManager = workManager;
            this.rulesManager = rulesManager;
            this.mapper = mapper;
        }

        public string Identifier => nameof(PV_20);
        public string ParameterName => "EnablePVTitleStandardization";
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CUR };
        public ValidatorType ValidatorType => ValidatorType.PostMatchingValidator;
        public string PipelineComponentVersion => typeof(PostMatchingValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var model = submission.Model;

            var paramValue = await rulesManager.GetParameterValue<bool>(ParameterName);
            RuleConfiguration = paramValue.ToString();

            if (!submission.IsEligible && model.WorkNumber != null && !submission.SkipProcessing)
            {
                var iswcModel = await workManager.FindIswcModelAsync(submission.Model.WorkNumber);
                var iswcMetadata = mapper.Map<ISWCMetadataModel>(iswcModel);
                var titleExclusions = await rulesManager.GetParameterValueEnumerable<TitleType>("ExcludeTitleTypes");

                if (!string.IsNullOrWhiteSpace(submission.Model.PreferredIswc) && !string.IsNullOrWhiteSpace(iswcModel.Iswc)
                    && submission.Model.PreferredIswc == iswcModel.Iswc && iswcModel.IswcStatusId != 2)
                {
                    if (!CheckIPs(submission.Model.InterestedParties.Where(x => x.IsWriter), iswcMetadata.InterestedParties.Where(x => x.IsWriter)))
                    {
                        submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._144);
                        return (false, submission);
                    }
                    else if (!CheckTitles(submission.Model.Titles, null, iswcMetadata, titleExclusions))
                    {
                        submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._144);
                        return (false, submission);
                    }

                    return (true, submission);
                }
                else
                {
                    if (submission.MatchedResult?.Matches == null || !submission.MatchedResult.Matches.Any())
                    {
                        submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._144);
                        return (false, submission);
                    }

                    var match = submission.MatchedResult.Matches.Where(x => x.Numbers?.FirstOrDefault(x => x.Type == "ISWC")?.Number == submission.Model.PreferredIswc)?.FirstOrDefault();

                    if (match == null)
                    {
                        submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._247);
                        return (false, submission);

                    }
                    if (!CheckIPs(submission.Model.InterestedParties.Where(x => x.IsWriter), match.Contributors.Where(x => x.ContributorType == ContributorType.Creator)))
                    {
                        submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._247);
                        return (false, submission);
                    }
                    else if (!CheckTitles(submission.Model.Titles, match, null, titleExclusions))
                    {
                        submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._127);
                        return (false, submission);
                    }
                }
                return (true, submission);
            }

            return (true, submission);

            bool CheckIPs(IEnumerable<InterestedPartyModel> subIps, IEnumerable<InterestedPartyModel> matchIps)
            {
                return matchIps.GroupBy(x => x.IpBaseNumber).ToList().Count().Equals(subIps.GroupBy(x => x.IpBaseNumber).ToList().Count())
                    && matchIps.All(c => subIps.Any(ip => !string.IsNullOrEmpty(ip.IpBaseNumber)
                    && ip.IpBaseNumber.Equals(c.IpBaseNumber)));
            }

            bool CheckTitles(IEnumerable<Title> subTitles, MatchingWork? match, ISWCMetadataModel? metadataModel, IEnumerable<TitleType> titleExclusions)
            {
                var matchTitles = match != null ? match?.Titles : metadataModel?.Titles;
                if (match == null && (metadataModel == null || !metadataModel.Titles.Any())) return false;


                else if (paramValue)
                {
                    return match != null ? StringExtensions.StringComparisonExact(submission.MatchedResult.StandardizedName, match?.StandardizedTitle)
                        : matchTitles.Any(x => (subTitles.Any(y => StringExtensions.StringComparisonExact(y.Name, x?.Name)
                         || StringExtensions.StringComparisonExactSanitised(x?.Name, y.Name))));
                }
                else
                {

                    if (subTitles.Where(x => !titleExclusions.Contains(x.Type)).Any(
                       t => !matchTitles.Where(ip => (StringExtensions.StringComparisonExact(t.Name, ip.Name)
                       || StringExtensions.StringComparisonExact(t.Name, ip.StandardizedName)
                       || StringExtensions.StringComparisonExactSanitised(t.Name, ip.Name))
                       && t.Type == ip.Type && !titleExclusions.Contains(ip.Type)).Any())) return false;

                    else return matchTitles.Where(x => !titleExclusions.Contains(x.Type)).Any(
                        t => subTitles.Where(ip => (StringExtensions.StringComparisonExact(t.Name, ip.Name)
                        || StringExtensions.StringComparisonExact(t.Name, ip.StandardizedName)
                        || StringExtensions.StringComparisonExactSanitised(t.Name, ip.Name))
                        && t.Type == ip.Type && !titleExclusions.Contains(ip.Type)).Any());

                }
            }
        }
    }
}
