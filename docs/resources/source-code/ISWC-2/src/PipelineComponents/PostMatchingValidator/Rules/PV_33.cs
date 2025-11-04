using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules
{
    public class PV_33 : IRule, IAlwaysOnRule
    {
        private readonly IMessagingManager messagingManager;
        private readonly IWorkManager workManager;  
        private readonly IInterestedPartyManager interestedPartyManager;
        private readonly IAgreementManager agreementManager;

        public PV_33(IMessagingManager messagingManager, IWorkManager workManager,
            IInterestedPartyManager interestedPartyManager, IAgreementManager agreementManager)
        {
            this.messagingManager = messagingManager;
            this.workManager = workManager;
            this.interestedPartyManager = interestedPartyManager;
            this.agreementManager = agreementManager;
        }


        public string Identifier => nameof(PV_33);

        public string ParameterName => String.Empty;

        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CIQ, TransactionType.CMQ };

        public ValidatorType ValidatorType => ValidatorType.PostMatchingValidator;

        public string PipelineComponentVersion => typeof(PostMatchingValidator).GetComponentVersion();

        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            if (submission.SearchWorks.Any())
            {
                if (submission.IsrcMatchedResult != null 
                    && submission.IsrcMatchedResult.Matches != null && submission.IsrcMatchedResult.Matches.Any())
                {
                    var preferredIswcs = submission.IsrcMatchedResult.Matches
                        .SelectMany(m => m.Numbers.Where(n => n.Type == "ISWC"))
                        .Select(n => n.Number)
                        .Distinct()
                        .ToList();

                    if (preferredIswcs != null && preferredIswcs.Any())
                    {
                        var isrcWorks = await workManager.FindManyIswcModelsAsync(preferredIswcs, detailLevel: DetailLevel.Core);
                        if (isrcWorks != null && isrcWorks.Any())
                        {
                            foreach (var isrcWork in isrcWorks)
                                submission.SearchedIswcModels.Add(isrcWork);
                        }
                    }
                }

                var subIp = submission.Model.InterestedParties.FirstOrDefault();
                var matchingIps = GetBestMatchIps(submission.SearchedIswcModels, subIp);
                List<InterestedPartyModel> selectedIps = new List<InterestedPartyModel>();
                if (subIp != null && matchingIps.Any())
                {
                    var ipBaseNumbers = matchingIps
                        .Select(ip => ip.IpBaseNumber)
                        .Where(ipb => !string.IsNullOrEmpty(ipb))
                        .Distinct()
                        .Cast<string>() 
                        .ToList();

                    if (ipBaseNumbers.Any())
                    {
                        var interestedParties = await interestedPartyManager.FindManyByBaseNumber(ipBaseNumbers);
                        if (interestedParties != null && interestedParties.Any())
                        {
                            var firstName = subIp.Names?.FirstOrDefault()?.FirstName ?? string.Empty;
                            var filteredIps = interestedParties
                                .Where(ip =>
                                    (string.IsNullOrEmpty(firstName) ||
                                    (ip.Names?.Any(n =>
                                        !string.IsNullOrEmpty(n.FirstName) &&
                                        (n.FirstName.Contains(firstName, StringComparison.OrdinalIgnoreCase) ||
                                         firstName.Contains(n.FirstName, StringComparison.OrdinalIgnoreCase))
                                    ) ?? false)) && 
                                    (!subIp.BirthDate.HasValue || !ip.BirthDate.HasValue || ip.BirthDate == subIp.BirthDate) &&
                                    (!subIp.DeathDate.HasValue || !ip.DeathDate.HasValue || ip.DeathDate == subIp.DeathDate) &&
                                    (!subIp.Age.HasValue || !ip.Age.HasValue || !subIp.AgeTolerance.HasValue ||
                                        subIp.AgeTolerance.Value >= Math.Abs(ip.Age.Value - subIp.Age.Value))
                                )
                                .ToList();

                            if (submission.Model.Affiliations != null && submission.Model.Affiliations.Any())
                            {
                                var agreements = await agreementManager.FindManyAsync(ipBaseNumbers);

                                var affiliatedIpBaseNumbers = agreements
                                    .Where(a => !string.IsNullOrEmpty(a.AgencyId) && submission.Model.Affiliations.Contains(a.AgencyId))
                                    .Select(a => a.IpbaseNumber)
                                    .Where(ip => !string.IsNullOrEmpty(ip))
                                    .Distinct()
                                    .ToHashSet();

                                filteredIps = filteredIps
                                    .Where(ip => !string.IsNullOrEmpty(ip.IpBaseNumber) && affiliatedIpBaseNumbers.Contains(ip.IpBaseNumber))
                                    .ToList();
                            }

                            selectedIps = matchingIps
                                .Where(ip =>
                                    !string.IsNullOrEmpty(ip.IpBaseNumber) &&
                                    filteredIps.Any(f => f.IpBaseNumber == ip.IpBaseNumber))
                                .ToList();

                            if (selectedIps.Any())
                            {
                                foreach (var selectedIp in selectedIps)
                                {
                                    var nameSource = filteredIps.FirstOrDefault(ip => ip.IpBaseNumber == selectedIp.IpBaseNumber);

                                    if (nameSource?.Names != null && nameSource.Names.Any())
                                    {
                                        selectedIp.Names = nameSource.Names;
                                    }
                                }
                                
                                submission.Model.InterestedParties = selectedIps;

                                submission.SearchedIswcModels = submission.SearchedIswcModels
                                    .Where(iswc =>
                                        iswc.VerifiedSubmissions.Any(vs =>
                                            vs.InterestedParties.Any(ip => selectedIps.Any(sip => ip.IpBaseNumber == sip.IpBaseNumber))))
                                    .GroupBy(iswc => iswc.IswcId)
                                    .Select(g => g.First())
                                    .ToList();
                            }
                        }
                    }
                }

                if (!selectedIps.Any())
                {
                    submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._180);
                    return (false, submission);
                }
            }

            if (submission.SearchedIswcModels.Any())
                return (true, submission);

            submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._180);
            return (false, submission);

            IEnumerable<InterestedPartyModel> GetBestMatchIps(IEnumerable<IswcModel> searchedIswcModels, InterestedPartyModel? interestedParty)
            {
                if (interestedParty == null)
                    return Enumerable.Empty<InterestedPartyModel>();

                return searchedIswcModels
                    .SelectMany(iswc => iswc.VerifiedSubmissions
                        .SelectMany(sub => sub.InterestedParties
                            .Where(ip =>
                                (!string.IsNullOrEmpty(interestedParty.IpBaseNumber) &&
                                    ip.IpBaseNumber == interestedParty.IpBaseNumber) ||
                                (string.IsNullOrEmpty(interestedParty.IpBaseNumber) &&
                                    interestedParty.IPNameNumber.HasValue &&
                                    ip.IPNameNumber.HasValue &&
                                    ip.IPNameNumber.Value == interestedParty.IPNameNumber.Value) ||
                                (string.IsNullOrEmpty(interestedParty.IpBaseNumber) &&
                                    !interestedParty.IPNameNumber.HasValue &&
                                    !string.IsNullOrEmpty(interestedParty.LastName) &&
                                    !string.IsNullOrEmpty(ip.LastName) &&
                                    ip.LastName.Contains(interestedParty.LastName, StringComparison.OrdinalIgnoreCase))
                            )
                            .Select(ip => new { Party = ip, Rank = iswc.RankScore ?? 0 })
                        )
                    )
                    .OrderByDescending(x => x.Rank)
                    .GroupBy(x => x.Party.IpBaseNumber ?? x.Party.IPNameNumber?.ToString())
                    .Select(g => g.First().Party);
            }
        }
    }
}
