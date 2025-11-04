using AutoMapper;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Bdo.MatchingEngine;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.Repositories;
using SpanishPoint.Azure.Iswc.Data.Services.Matching;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Business.Managers
{
    public interface IMatchingManager
    {
        Task<IEnumerable<Submission>> MatchAsync(IEnumerable<Submission> submissions, string source = "Eligible");
        Task<IEnumerable<Submission>> MatchIsrcsAsync(IEnumerable<Submission> submissions);
        Task<IEnumerable<Submission>> RankMatches(IEnumerable<Submission> submissions);
    }

    public class MatchingManager : IMatchingManager
    {
        private readonly IMatchingService matchingService;
        private readonly IWorkManager workManager;
        private readonly IIswcRepository iswcRepository;
        private readonly IMapper mapper;
        private readonly IRulesManager rulesManager;
        private readonly IWorkRepository workRepository;
        private readonly IAdditionalIdentifierManager additionalIdentifierManager;

        public MatchingManager(
            IMatchingService matchingService,
            IWorkManager workManager,
            IIswcRepository iswcRepository,
            IMapper mapper,
            IRulesManager rulesManager,
            IWorkRepository workRepository,
            IAdditionalIdentifierManager additionalIdentifierManager)
        {
            this.matchingService = matchingService;
            this.workManager = workManager;
            this.iswcRepository = iswcRepository;
            this.mapper = mapper;
            this.rulesManager = rulesManager;
            this.workRepository = workRepository;
            this.additionalIdentifierManager = additionalIdentifierManager;
        }

        public async Task<IEnumerable<Submission>> MatchAsync(IEnumerable<Submission> submissions, string source = "Eligible")
        {
            await CheckIfArchivedLegacy();

            await matchingService.MatchAsync(submissions, source);

            return (source == "Search") ? await RankMatches(submissions) : submissions;

            async Task CheckIfArchivedLegacy()
            {
                foreach (var submission in submissions
                    .Where(x => (x.TransactionType.Equals(TransactionType.CAR) || x.TransactionType.Equals(TransactionType.CIQ)
                    || x.TransactionType.Equals(TransactionType.CMQ)) && x?.Rejection == null && x?.Model != null
                    && !string.IsNullOrEmpty(x.Model.PreferredIswc) && string.IsNullOrEmpty(x.Model.Iswc)))
                {
                    if (await workManager.CheckIfArchivedIswcAsync(submission.Model.PreferredIswc))
                    {
                        submission.Model.Iswc = submission.Model.PreferredIswc;
                        submission.Model.PreferredIswc = string.Empty;
                    }
                }
            }
        }

        public async Task<IEnumerable<Submission>> MatchIsrcsAsync(IEnumerable<Submission> submissions)
        {
            var isrcsToSearch = new List<string>();
            var sw = Stopwatch.StartNew();
            foreach (var submission in submissions)
            {
                var workCodesWithMatches = new List<string>();

                var additionalIdentifiers = submission.Model?.AdditionalIdentifiers != null
                    ? submission.Model.AdditionalIdentifiers.Where(ai => !string.IsNullOrEmpty(ai.WorkCode))
                    : Enumerable.Empty<Bdo.Work.AdditionalIdentifier>();

                foreach (var identifier in additionalIdentifiers)
                {
                    var workCode = identifier.WorkCode;
                    if (workCode != null)
                    {
                        isrcsToSearch.Add(workCode);

                    }
                }

            }

            var matchingIsrcIdentifiers = await additionalIdentifierManager.FindManyAsync(isrcsToSearch, 1);
            var matchingWorkInfos = await workRepository.FindManyWorkInfosAsync(matchingIsrcIdentifiers.Select(m => m.WorkInfoId));

            foreach (var submission in submissions)
            {
                if (submission.Model == null || submission.Model.AdditionalIdentifiers == null || !submission.Model.AdditionalIdentifiers.Any())
                    continue;
                var matches = new List<MatchingWork>();
                var workCodesWithMatches = new List<string>();
                var additionalIdentifiers = submission.Model.AdditionalIdentifiers.Where(ai => !string.IsNullOrEmpty(ai.WorkCode));
                foreach (var identifier in additionalIdentifiers)
                {
                    var workCode = identifier.WorkCode;
                    if (workCode != null)
                    {
                        var matchingWorkInfosPerSubmission = matchingWorkInfos.Where(w => w.AdditionalIdentifier.Any(ai => ai.WorkIdentifier == workCode && ai.NumberTypeId == 1));
                        if (matchingWorkInfosPerSubmission.Any())
                        {
                            workCodesWithMatches.Add(workCode);
                            matchingWorkInfosPerSubmission = matchingWorkInfosPerSubmission.DistinctBy(w => w.Iswc.Iswc1).ToList();
                            matchingWorkInfosPerSubmission.OrderBy(w => w.Iswc.CreatedDate);
                            foreach (var matchingWork in matchingWorkInfosPerSubmission)
                            {
                                var work = mapper.Map<SubmissionModel>(matchingWork);
                                matches.Add(new MatchingWork
                                {
                                    Id = matchingWork.WorkInfoId,
                                    Contributors = work.InterestedParties,
                                    MatchType = MatchSource.MatchedByIsrc,
                                    IsDefinite = true,
                                    Artists = work.Performers,
                                    Titles = work.Titles,
                                    Numbers = new List<WorkNumber>()
                                    {
                                        new WorkNumber()
                                        {
                                            Number = work.Iswc,
                                            Type = "ISWC",
                                        }
                                    },
                                    IswcStatus = matchingWork.Iswc.IswcStatusId
                                });
                            }
                        }
                    }
                }

                if (matches.Any(m => m.IswcStatus == 1))
                {
                    submission.IsrcMatchedResult = new MatchResult()
                    {
                        InputWorkId = submission.SubmissionId,
                        Matches = matches,
                        MatchTime = sw.Elapsed
                    };

                    submission.HasAlternateIswcMatches = hasAlternateIswcMatches(submission);

                    if (submission.RequestType == RequestType.Label && submission.TransactionType != TransactionType.CUR)
                    {
                        var isrcMatch = matches.FirstOrDefault(m => m.IswcStatus == 1);
                        var firstNumber = isrcMatch?.Numbers?.FirstOrDefault()?.Number;

                        if (!string.IsNullOrWhiteSpace(firstNumber))
                        {
                            submission.Model.PreferredIswc = firstNumber;
                        }
                    }
                    if (submission.HasAlternateIswcMatches && submission.RequestType != RequestType.Label)
                    {
                        submission.Model.AdditionalIdentifiers = submission.Model.AdditionalIdentifiers
                            .Where(ai => !string.IsNullOrEmpty(ai.WorkCode) && !workCodesWithMatches.Contains(ai.WorkCode));
                    }
                }
            }

            return submissions;

            bool hasAlternateIswcMatches(Submission submission)
            {
                if (submission.IsrcMatchedResult.Matches.Count() > 1)
                {
                    return true;
                }

                var firstIsrcNumber = submission.IsrcMatchedResult?.Matches?
                    .FirstOrDefault()?.Numbers?.FirstOrDefault()?.Number;

                if (string.IsNullOrEmpty(firstIsrcNumber))
                    return false;

                var matches = submission.MatchedResult?.Matches;
                if (matches == null || !matches.Any())
                    return false;

                foreach (var match in matches)
                {
                    var hasSameNumber = match?.Numbers != null &&
                                        match.Numbers.Any(n => n?.Number == firstIsrcNumber);
                    if (hasSameNumber)
                        return false;
                }

                return true;
            }
        }

        public async Task<IEnumerable<Submission>> RankMatches(IEnumerable<Submission> submissions)
        {
            var rolledUpRollsForMatching = await rulesManager.GetParameterValue<string>("CalculateRolledUpRole");

            var iswcMatches = submissions.SelectMany(x => x.MatchedResult.Matches.SelectMany(x => x.Numbers).Where(x => x.Type == "ISWC")).Select(x => x.Number).Distinct();
            var workInfos = (await workRepository.FindManyAsyncOptimizedByPath(x => iswcMatches.Contains(x.Iswc.Iswc1), new string[] { "Iswc", "Iswc.IswclinkedTo" })).ToList();
            var iswcs = workInfos.Select(x => x.Iswc).DistinctBy(x => x.Iswc1);

            foreach (var submission in submissions.Where(x => string.IsNullOrEmpty(x.MatchedResult.ErrorMessage)))
            {
                if (submission.MatchedResult?.Matches == null)
                    continue;

                submission.MatchedResult.Matches = submission.Model.DerivedFrom.Any() ?
                    submission.MatchedResult.Matches.Where(x => !x.Numbers.Any(n => submission.Model.DerivedFrom.Any(d => d.Iswc == n.Number))) : submission.MatchedResult.Matches;

                var matches = submission.MatchedResult.Matches;

                if (!matches.Any())
                    continue;

                var roles = GetRoleMappings();

                foreach (var match in matches)
                {
                    if (match.Titles.Any(x => submission?.Model != null && submission.Model.Titles.Any(y => StringExtensions.StringComparisonExact(y.Name, x.Name)
                    || StringExtensions.StringComparisonExact(y.StandardizedName, x.Name)
                    || StringExtensions.StringComparisonExactSanitised(y.Name, x.Name)))
                    || StringExtensions.StringComparisonExactSanitised(match.StandardizedTitle, submission.MatchedResult.StandardizedName)) // Exact Match
                    {
                        match.RankScore = 100 + (CalculateRankScore());
                    }
                    else // Fuzzy Match
                    {
                        match.RankScore = (90 + (CalculateRankScore() ?? 0));
                    }

                    int? CalculateRankScore()
                    {
                        if (!submission.Model.InterestedParties.Any()) return 10;

                        var submittedIps = submission.Model.InterestedParties.Count();

                        if (match.Contributors == null || !match.Contributors.Any()) return 0;

                        var matchingContributors = submission.TransactionType == TransactionType.CIQ ?
                            match?.Contributors.Where(x => x.ContributorType == ContributorType.Creator)
                            : match?.Contributors;

                        if (matchingContributors == null || !matchingContributors.Any()) return 0;

                        var totalMatchContributors = matchingContributors.GroupBy(x => x.IpBaseNumber, z => z.Type).Count();

                        if (!matchingContributors.Any())
                            return 0;

                        var totalMatched = (matchingContributors
                             .Where(x => submission.Model.InterestedParties
                             .Any(y => (x.IpBaseNumber == y.IpBaseNumber || 
                                ((submission.TransactionType == TransactionType.CIQ || submission.RequestType == RequestType.Label) && 
                                    !string.IsNullOrEmpty(x?.LastName) && 
                                    !string.IsNullOrWhiteSpace(y.LastName) && 
                                    x.LastName.ToUpper().Contains(y.LastName.ToUpper()))) 
                                    && ContributorRoleMatching(x.Type, y.Type))))
                             .Count();

                        if (totalMatched == 0 || totalMatchContributors == 0)
                            return 0;

                        if (totalMatched == submittedIps)
                            return 10 * totalMatched;

                        if (totalMatched > totalMatchContributors)
                            return 10 * (totalMatched / totalMatchContributors);

                        return 10 - ((totalMatchContributors - totalMatched) < 0 ? 1 : ((totalMatchContributors - totalMatched)));

                    }

                    bool ContributorRoleMatching(InterestedPartyType? role1, InterestedPartyType? role2)
                    {
                        if (role1 == null || role2 == null)
                            return true;

                        if (roles.Any())
                        {
                            var rolledUpRole1 = roles.FirstOrDefault(x => x.Value.Any(v => v.Equals(role1)));
                            var rolledUpRole2 = roles.FirstOrDefault(x => x.Value.Any(v => v.Equals(role2)));

                            if (rolledUpRole1.Key.Equals(rolledUpRole2.Key))
                                return true;
                        }
                        else if (role1.Equals(role2))
                            return true;

                        return false;
                    }
                }

                if (matches.Where(x => x.RankScore == matches.Max(y => y.RankScore))?.Count() > 1)
                {
                    var multipleMatches = matches.Where(y => y.RankScore.Equals(matches.Max(x => x.RankScore))).SelectMany(x => x.Numbers);
                    var oldestMatch = GetOldestIswc(multipleMatches, submission.Model.Iswc);

                    if (!string.IsNullOrEmpty(oldestMatch))
                    {
                        var top = matches
                            .OrderByDescending(z => z.RankScore)
                            .FirstOrDefault(m =>
                                (m.Numbers?.Any(n =>
                                    !string.IsNullOrEmpty(n?.Number) &&
                                    n.Number!.Equals(oldestMatch) &&
                                    string.Equals(n.Type, "ISWC", StringComparison.OrdinalIgnoreCase)) ?? false));

                        if (top != null)
                            top.RankScore++;
                    }

                    string GetOldestIswc(IEnumerable<Bdo.Work.WorkNumber> workNumbers, string? archivedIswc)
                    {
                        var works = new List<Data.DataModels.Iswc>() { };

                        if (submission.TransactionType == TransactionType.CUR && submission.IsEligible)
                        {
                            var iswcNumbers = workNumbers.Where(x => x.Type == "ISWC" && x.Number != archivedIswc && x.Number != submission.ExistingWork?.Iswc).Select(x => x.Number).Distinct();
                            works.AddRange(iswcs.Where(x => iswcNumbers.Contains(x.Iswc1)));
                            return works.OrderBy(x => x.CreatedDate).FirstOrDefault()?.Iswc1 ?? string.Empty;
                        }

                        var iswcNums = workNumbers.Where(x => x.Type == "ISWC" && x.Number != archivedIswc).Select(x => x.Number).Distinct();
                        works.AddRange(iswcs.Where(x => iswcNums.Contains(x.Iswc1)));
                        return works.OrderBy(x => x.CreatedDate).FirstOrDefault()?.Iswc1 ?? string.Empty;
                    }
                }

                submission.MatchedResult.Matches = matches.OrderByDescending(x => x.RankScore).Select(x => x);

                if (true)
                {
                    if (new List<TransactionType> { TransactionType.CAR, TransactionType.CUR, TransactionType.FSQ }.Contains(submission.TransactionType))
                        submission.MatchedResult.Matches = await RemoveReplacedWorksFromMatches();

                    var preferredIswc = submission.Model.PreferredIswc;
                    var isChild = false;
                    var topMatch = submission.MatchedResult.Matches.FirstOrDefault();


                    if (!string.IsNullOrWhiteSpace(preferredIswc) && topMatch != null)
                    {
                        if (topMatch.Numbers.Any(x => x.Type == "ISWC" && x.Number == preferredIswc))
                        {
                            if (new List<TransactionType> { TransactionType.CAR, TransactionType.CUR }.Contains(submission.TransactionType)
                                && topMatch.Numbers.Any(x => x.Number.Length == 20 && x.Number.StartsWith("AS")))
                                submission.Model.RelatedSubmissionIncludedIswc = true;

                            isChild = true;
                        }
                    }

                    if (submission.MatchedResult.Matches.Any())
                        await FollowLinkedToChain(submission.MatchedResult.Matches.First());

                    if (isChild)
                        submission.Model!.PreferredIswc = submission.MatchedResult?.Matches?.FirstOrDefault()?
                            .Numbers?.FirstOrDefault(x => x.Type.Equals("ISWC"))?.Number ?? string.Empty;
                }

                if ((string.IsNullOrWhiteSpace(submission.Model.PreferredIswc)
                    && (new List<TransactionType> { TransactionType.CAR, TransactionType.CUR, TransactionType.FSQ }.Contains(submission.TransactionType))))
                {

                    if (submission.MatchedResult?.Matches?.Any() == true)
                        submission.Model.PreferredIswc = AssignPreferredIswc(submission);
                }

               
                async Task<IEnumerable<MatchingWork>> RemoveReplacedWorksFromMatches()
                {
                    var updatedMatches = new List<MatchingWork>();
                    if (!matches.Any() || !matches.Any(x => x.Numbers.Any())) return submission.MatchedResult.Matches;

                    var works = await workManager.FindManyWorksAsync(submission.MatchedResult.Matches.Select(x => x.Id), excludeDeletedWorks: false, excludeRelaceWorks: false, detailLevel: DetailLevel.Minimal);
                    
                    if (!works.Any()) return submission.MatchedResult.Matches;

                    var dict = works
                        .GroupBy(x => (x.WorkNumber.Type, x.WorkNumber.Number, x.Iswc))
                        .ToDictionary(x => x.Key, y => y.First());

                    foreach (var match in submission.MatchedResult.Matches)
                    {
                        var workNumber = match.Numbers.FirstOrDefault(x => x.Type != "ISWC");
                        var iswc = match.Numbers.FirstOrDefault(x => x.Type == "ISWC")?.Number;

                        if (workNumber != null && iswc != null && dict.TryGetValue((workNumber.Type, workNumber.Number, iswc), out var work) && work.IsReplaced)
                            continue;

                        else
                            updatedMatches.Add(match);
                    }

                    return updatedMatches;
                }
            }

            return await Task.FromResult(submissions);

            string AssignPreferredIswc(Submission submission)
            {
                var matches = submission?.MatchedResult?.Matches;

                if (matches == null || submission == null) return string.Empty;

                if (submission.TransactionType.Equals(TransactionType.CAR) || submission.TransactionType.Equals(TransactionType.FSQ))
                    return matches.FirstOrDefault()?.Numbers.FirstOrDefault(n => !string.IsNullOrEmpty(n.Type) && n.Type.Equals("ISWC")
                    && !string.IsNullOrEmpty(n.Number) && !n.Number.Equals(submission.Model.Iswc))?.Number ?? string.Empty;

                else
                {
                    if (submission.IsEligible)
                    {
                        var workNumber = submission.Model?.AdditionalAgencyWorkNumbers?.Any() == true
                            ? submission.Model.AdditionalAgencyWorkNumbers.FirstOrDefault(x => x.IsEligible)?.WorkNumber?.Number
                            : submission.Model?.WorkNumber?.Number;

                        var numbers = matches.FirstOrDefault(x => x.Numbers.Any(n => !string.IsNullOrWhiteSpace(n.Number)
                        && n.Number.Equals(workNumber)))?.Numbers;

                        return numbers?.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Type) && x.Type.Equals("ISWC")
                        && !string.IsNullOrWhiteSpace(x.Number) && !x.Number.Equals(submission?.Model?.Iswc))?.Number ?? string.Empty;
                    }
                    else
                        return matches?.FirstOrDefault()?.Numbers?.FirstOrDefault(n => n.Type == "ISWC")?.Number ?? string.Empty;
                }

            }

            IDictionary<CisacInterestedPartyType, IEnumerable<InterestedPartyType>> GetRoleMappings()
            {
                IDictionary<CisacInterestedPartyType, IEnumerable<InterestedPartyType>> roleDictionary =
                new Dictionary<CisacInterestedPartyType, IEnumerable<InterestedPartyType>>();

                if (!string.IsNullOrWhiteSpace(rolledUpRollsForMatching))
                {
                    foreach (var roleMapping in rolledUpRollsForMatching.Split(";"))
                    {
                        if (roleMapping.Length > 0)
                        {
                            var roles = Regex.Replace(roleMapping, "[(),]", ";").Split(";").Where(x => !string.IsNullOrWhiteSpace(x));

                            roleDictionary.Add(
                                key: Enum.Parse<CisacInterestedPartyType>(roles.First()),
                                value: roles.Skip(1).Select(x => Enum.TryParse(x.Trim(), out InterestedPartyType value)
                                ? value : throw new Exception($"Couldn't convert '{x}' to InterestedPartyType.")));
                        }
                    }
                }
                return roleDictionary;
            }

            async Task<MatchingWork> FollowLinkedToChain(MatchingWork matchingWork)
            {
                var iswc = mapper.Map<IswcModel>(workInfos.FirstOrDefault(x => x.WorkInfoId == matchingWork.Id && x.Status)?.Iswc);

                if (iswc == null) return matchingWork;

                bool firstTime = true;

                async Task<MatchingWork> checkParentIswc(IswcModel iswcModel)
                {
                    var parentIswc = workInfos.FirstOrDefault(x => x.Iswc.Iswc1 == iswcModel.Iswc)?.Iswc.IswclinkedTo.FirstOrDefault(s => s.Status)?.LinkedToIswc;

                    if (!firstTime)
                        parentIswc = (await iswcRepository.FindAsyncOptimized(x => x.Iswc1 == iswcModel.Iswc, i => i.IswclinkedTo))?.IswclinkedTo.FirstOrDefault(s => s.Status)?.LinkedToIswc;

                    if (parentIswc != null)
                        iswcModel.ParentIswc ??= mapper.Map<IswcModel>(await iswcRepository.FindAsync(x => x.Iswc1 == parentIswc));

                    firstTime = false;

                    if (iswcModel.ParentIswc == null || iswcModel.ParentIswc.Iswc == iswc.Iswc)
                    {
                        if (string.IsNullOrEmpty(matchingWork.Numbers.FirstOrDefault(x => x.Type == "ISWC")?.Number))
                            return matchingWork;

                        matchingWork.Numbers.First(x => x.Type == "ISWC").Number = iswcModel.Iswc;
                        return matchingWork;
                    }
                    else
                    {
                        return await checkParentIswc(iswcModel.ParentIswc);
                    }
                }

                return await checkParentIswc(iswc);
            }

        }
    }
}
