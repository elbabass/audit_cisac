using IdentityServer4.Extensions;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.MatchingComponent.InitialMatching
{
    internal class SearchComponent : IInitialMatchingComponent
    {
        private readonly IWorkManager workManager;
        private readonly IMatchingManager matchingManager;
        private readonly IRulesManager rulesManager;

        public SearchComponent(IWorkManager workManager, IMatchingManager matchingManager, IRulesManager rulesManager)
        {
            this.workManager = workManager;
            this.matchingManager = matchingManager;
            this.rulesManager = rulesManager;
        }

        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CMQ, TransactionType.CIQ };
        public string Identifier => nameof(SearchComponent);
        public bool? IsEligible => null;
        public string ParameterName => "EnableThirdPartyIpAffiliation";
        public string PipelineComponentVersion => typeof(MatchingComponent).GetComponentVersion();
        public IEnumerable<RequestType> ValidRequestTypes => new List<RequestType> { RequestType.Agency, RequestType.Label, RequestType.Publisher, RequestType.ThirdParty };

        public async Task<IEnumerable<Submission>> ProcessSubmissions(IEnumerable<Submission> submissions)
        {
            var submissionsWithIswcOrWorkCode = submissions
                .Where(x => !string.IsNullOrEmpty(x.Model.PreferredIswc) || !string.IsNullOrEmpty(x.Model.WorkNumber.Number));

            var paramValue = await rulesManager.GetParameterValue<bool>(ParameterName);

            foreach (var submission in submissionsWithIswcOrWorkCode)
            {
                if (submission.RequestType == RequestType.ThirdParty)
                {
                    if (!string.IsNullOrEmpty(submission.Model.PreferredIswc))
                    {
                        var cachedIswc = await workManager.GetCacheIswcs(submission.Model.PreferredIswc);
                        if (cachedIswc != null)
                        {
                            submission.SearchedIswcModels.Add(cachedIswc);
                            continue;
                        }
                    }

                    if (!string.IsNullOrEmpty(submission.Model.WorkNumber.Number))
                    {
                        var cachedIswc = await workManager.GetCacheIswcs(string.Concat(submission.Model.WorkNumber.Type, submission.Model.WorkNumber.Number));
                        if (cachedIswc != null)
                        {
                            submission.SearchedIswcModels.Add(cachedIswc);
                            continue;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(submission.Model.PreferredIswc))
                    if (await workManager.Exists(submission.Model.PreferredIswc))
                        submission.SearchedIswcModels.Add(await workManager.FindIswcModelAsync(submission.Model.PreferredIswc, readOnly: true, submission.DetailLevel));
                    else if (await workManager.CheckIfArchivedIswcAsync(submission.Model.PreferredIswc))
                        submission.SearchedIswcModels.Add(await workManager.FindArchivedIswcModelAsync(submission.Model.PreferredIswc, readOnly: true, submission.DetailLevel));

                if (!string.IsNullOrEmpty(submission.Model.WorkNumber.Number) && await workManager.Exists(submission.Model.WorkNumber))
                    submission.SearchedIswcModels.Add(await workManager.FindIswcModelAsync(submission.Model.WorkNumber, readOnly: true, submission.DetailLevel));

                if (submission.IsPublicRequest)
                    submission.SearchedIswcModels = submission.SearchedIswcModels.Where(x => x.IswcStatusId != 2).ToList();

                if (submission.SearchedIswcModels.Any())
                {
                    foreach(var iswcModel in submission.SearchedIswcModels)
                    {
                        if (iswcModel.VerifiedSubmissions.Any())
                        {
                            foreach (var v in iswcModel.VerifiedSubmissions)
                            {
                                if (v.AdditionalIdentifiers != null && v.AdditionalIdentifiers.Any())
                                {
                                    DateTimeOffset now = DateTimeOffset.Now;
                                    v.AdditionalIdentifiers = v.AdditionalIdentifiers.Where(ai => ai.ReleaseEmbargoDate == null || (ai.ReleaseEmbargoDate != null && ai.ReleaseEmbargoDate <= now));
                                }
                            }
                        }
                    }
                }
            }

            string matchingSource = (submissions.Any() && !submissions.FirstOrDefault().MatchingSource.IsNullOrEmpty()) ? submissions.FirstOrDefault().MatchingSource : "Search";
            submissions = await matchingManager.MatchAsync(submissions.Where(x => !submissionsWithIswcOrWorkCode.Any(y => y.SubmissionId == x.SubmissionId)), matchingSource);

            foreach (var submission in submissions)
            {
                var iswcModels = await workManager.FindManyAsync(submission.MatchedResult.Matches.Select(y => y.Id), readOnly: true, submission.DetailLevel);

                if (submission.IsPublicRequest)
                    iswcModels = iswcModels.Where(x => x.IswcStatusId != 2).ToList();

                if (iswcModels != null && iswcModels.Any())
                {
                    foreach (var iswc in iswcModels)
                    {
                        foreach (var x in submission.MatchedResult.Matches)
                        {
                            if (iswc.Iswc == x.Numbers.FirstOrDefault(n => n.Type == "ISWC")?.Number)
                                iswc.RankScore = x.RankScore;
                        };

                        if (iswc.VerifiedSubmissions.Any())
                        {
                            foreach (var v in iswc.VerifiedSubmissions)
                            {
                                if (submission.IsPublicRequest)
                                {
                                    v.IsPublicRequest = true;
                                    if (submission.RequestType == RequestType.ThirdParty && !paramValue)
                                        v.InterestedParties.Select(x => { x.Affiliation = null; return x; }).ToList();
                                }

                                if (v.AdditionalIdentifiers != null && v.AdditionalIdentifiers.Any())
                                {
                                    DateTimeOffset now = DateTimeOffset.Now;
                                    v.AdditionalIdentifiers = v.AdditionalIdentifiers.Where(ai => ai.ReleaseEmbargoDate == null || (ai.ReleaseEmbargoDate != null && ai.ReleaseEmbargoDate <= now));
                                }
                            }
                        }
                    }

                    if (submission.TransactionType == TransactionType.CIQ && submission.Model.Titles.Any())
                    {
                        var rankGroup = iswcModels.Where(x => x.RankScore >= 100 && x.VerifiedSubmissions.OrderBy(x => x.CreatedDate).Any(x => x.IswcEligible &&
                        StringExtensions.StringComparisonExact(submission.Model?.Titles?.FirstOrDefault(x => x?.Type == Bdo.Work.TitleType.OT)?.Name, x.Titles.FirstOrDefault(t => t?.Type == Bdo.Work.TitleType.OT)?.Name)));

                        if (rankGroup.Any())
                            foreach (var ranked in rankGroup.OrderByDescending(x => x.RankScore).GroupBy(x => x.RankScore))
                            {
                                var agencyWorksPerGroup = ranked.OrderByDescending(x => x.VerifiedSubmissions.Count()).GroupBy(x => x.VerifiedSubmissions.Count());

                                foreach (var item in agencyWorksPerGroup)
                                {
                                    foreach (var x in item)
                                    {
                                        var ips = x.VerifiedSubmissions.Select(ip => ip.InterestedParties);
                                        var ipCount = ips.Where(a => a.Any(x => new List<CisacInterestedPartyType> { CisacInterestedPartyType.TA, CisacInterestedPartyType.MA }.Contains(x.CisacType ?? CisacInterestedPartyType.C))).Count();

                                        if (ipCount > 0)
                                            iswcModels.Where(y => y.IswcId == x.IswcId).FirstOrDefault().RankScore -= (ipCount * 2);

                                       
                                    }

                                    iswcModels.Where(x => item.Select(y => y.IswcId).Contains(x.IswcId)).Select(x => { x.RankScore += item.Key; return x; }).ToList();
                                }
                            }
                    }

                    submission.SearchedIswcModels = iswcModels.OrderByDescending(x => x.RankScore).ToList();
                }
            }

            foreach (var submission in submissionsWithIswcOrWorkCode)
            {
                if (submission.IsPublicRequest)
                    submission.SearchedIswcModels = submission.SearchedIswcModels.Where(x => x.IswcStatusId != 2).ToList();

                if (submission.SearchedIswcModels != null && submission.SearchedIswcModels.Any())
                {
                    foreach (var iswc in submission.SearchedIswcModels)
                    {
                        if (iswc.VerifiedSubmissions.Any() && submission.IsPublicRequest)
                        {
                            foreach (var v in iswc.VerifiedSubmissions)
                            {
                                v.IsPublicRequest = true;

                                if (submission.RequestType == RequestType.ThirdParty && !paramValue)
                                    v.InterestedParties.Select(x => { x.Affiliation = null; return x; }).ToList();
                            }
                        }
                    }
                }
            }

            return submissions.Union(submissionsWithIswcOrWorkCode);
        }
    }
}
