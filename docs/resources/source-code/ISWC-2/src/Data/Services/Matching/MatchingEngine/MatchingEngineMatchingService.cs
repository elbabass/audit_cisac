using AutoMapper;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Data.Services.Matching.MatchingEngine.Entities;
using SpanishPoint.Azure.Iswc.Data.Services.Matching.MatchingEngine.SearchModels;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Data.Services.Matching.MatchingEngine
{
    internal class MatchingEngineMatchingService : IMatchingService
    {
        private readonly HttpClient httpClient;
        private readonly IMapper mapper;

        public MatchingEngineMatchingService(IHttpClientFactory httpClientFactory, IMapper mapper)
        {
            httpClient = httpClientFactory.CreateClient("MatchingApi");
            this.mapper = mapper;
        }

        public async Task<IEnumerable<Submission>> MatchAsync(IEnumerable<Submission> submissions, string source)
        {
            if (!submissions.Any()) return submissions;

            var dictionary = submissions.ToDictionary(x => x.SubmissionId);

            if (source != "Usage")
            {
                var submissionsForMatching = mapper.Map<IEnumerable<Submission>, IEnumerable<InputWorkInfo>>(
                    submissions,
                    opt => opt.AfterMap((src, dest) => { foreach (var x in dest) { x.SubSource = source; } }));

                var matchResults = await GetMatches(submissionsForMatching);
                matchResults.ToList().ForEach(x => x.Matches = x.Matches?.Where(m => !(m.IswcStatusID == 1 && m.Numbers.Any(n => n.Number.StartsWith("PRS_")))));

                if ((matchResults.Any(x => x.Matches == null || x.Matches.Count() == 0) || submissions.Any(x => x.TransactionType == TransactionType.CUR))
                    && submissions.Any(x => x.RequestType != RequestType.Label))
                {
                    var labelSubmissions = submissions.DeepCopy();
                    
                    submissionsForMatching = mapper.Map<IEnumerable<Submission>, IEnumerable<InputWorkInfo>>(
                    labelSubmissions.Select(x =>
                    {
                        if (x.Model.InterestedParties.All(y => !string.IsNullOrEmpty(y.IpBaseNumber)))
                        {
                            x.Model.InterestedParties = x.Model.InterestedParties.DistinctBy(y => new { y.IpBaseNumber, y.CisacType })
                                                                        .Select(c => { c.IPNameNumber = null; c.IpBaseNumber = null; c.Name = null; return c; }).ToList();
                        }
                        else
                        {
                            x.Model.InterestedParties = x.Model.InterestedParties
                                                                        .Select(c => { c.IPNameNumber = null; c.IpBaseNumber = null; c.Name = null; return c; }).ToList();
                        }

                        return x;
                    }),
                    opt => opt.AfterMap((src, dest) => { foreach (var x in dest) { x.SubSource = "Label"; x.SkipContributorCountRules = false; } }));

                    var matchesList = matchResults.ToList();
                    var provisonalMatchesList = (await GetMatches(submissionsForMatching)).ToList();
                    var submissionsList = submissions.ToList();

                    for (int i = 0; i < provisonalMatchesList.Count; i++)
                    {
                        if (matchesList[i].Matches == null)
                            matchesList[i].Matches = new List<MatchingWork>();

                        if (matchesList[i].Matches.Count() == 0 || submissionsList[i].TransactionType == TransactionType.CUR)
                        {
                            if (provisonalMatchesList[i].Matches != null && provisonalMatchesList[i].Matches.Count() > 0)
                                matchesList[i].Matches = matchesList[i].Matches.Concat(provisonalMatchesList[i].Matches.Where(x => x.IsDefinite && x.IswcStatusID == 2));
                        }
                    }
                    matchResults = matchesList;
                }

                if (matchResults.Any() && submissions.Any(x => x.TransactionType == TransactionType.FSQ))
                {
                    foreach (var matchResult in matchResults.Where(m => string.IsNullOrEmpty(m.ErrorMessage) && m.Matches != null && m.Matches.Any()))
                    {
                        var matchesList = matchResult.Matches.ToList();
                        for (int i = 0; i < matchesList.Count; i++)
                        {
                            if (matchesList[i].IswcStatusID != null && matchesList[i].IswcStatusID == 2)
                            {
                                matchesList.RemoveAt(i);
                                i--;
                            }
                        }
                        matchResult.Matches = matchesList;
                    };
                }

                if (matchResults.Any() && source != "Search")
                {
                    foreach (var matchResult in matchResults.Where(m => string.IsNullOrEmpty(m.ErrorMessage) && m.Matches != null && m.Matches.Any()))
                    {
                        var matchesList = matchResult.Matches.ToList();
                        for (int i = 0; i < matchesList.Count; i++)
                        {
                            if (!matchesList[i].SkipContributorCountRules && !matchesList[i].IsDefinite)
                            {
                                matchesList.RemoveAt(i);
                                i--;
                            }
                        }
                        matchResult.Matches = matchesList;
                    };
                }

                if (matchResults.Any() && source != "Search" && source != "Usage")
                {
                    foreach (var matchResult in matchResults.Where(m => string.IsNullOrEmpty(m.ErrorMessage) && m.Matches != null && m.Matches.Any()))
                    {
                        var matchesList = matchResult.Matches.ToList();
                        for (int i = 0; i < matchesList.Count; i++)
                        {
                            if (!matchesList[i].SkipContributorCountRules && !matchesList[i].IsDefinite)
                            {
                                matchesList.RemoveAt(i);
                                i--;
                            }
                        }
                        matchResult.Matches = matchesList;
                    };
                }

                foreach (var matchResult in matchResults)
                {
                    if (string.IsNullOrEmpty(matchResult.ErrorMessage) && matchResult.Matches != null && matchResult.Matches.Any())
                    {
                        var matches = mapper.Map<Bdo.MatchingEngine.MatchResult>(matchResult);

                        dictionary[(int)matchResult.InputWorkId].MatchedResult = matches;
                    }
                }

                dictionary = await GetMatchesForCUR();
            }
            else
            {
                var submissionsForUsageMatching = mapper.Map<IEnumerable<Submission>, IEnumerable<UsageWorkGroup>>(submissions);

                var usageMatchResults = await GetMatches(submissionsForUsageMatching);

                foreach (var usageMatchResult in usageMatchResults)
                {
                    if (usageMatchResult.Error == null && usageMatchResult.MatchedEntities != null && usageMatchResult.MatchedEntities.Any())
                    {
                        dictionary[(int)usageMatchResult.ID].MatchedResult = mapper.Map<Bdo.MatchingEngine.MatchResult>(usageMatchResult);
                    }
                }
            }

            return dictionary.Values;

            async Task<Dictionary<int, Submission>> GetMatchesForCUR()
            {
                var updates = submissions.Where(s => s.TransactionType.Equals(TransactionType.CUR));

                if (!updates.Any()) return dictionary;

                var updatesToMatch = mapper.Map<IEnumerable<Submission>, IEnumerable<InputWorkInfo>>(
                    updates,
                    opt => opt.AfterMap((src, dest) => { foreach (var x in dest) { x.SubSource = source; } }));

                updatesToMatch.Select(x => { x.Numbers = default; x.SkipContributorCountRules = false; x.ExcludeMatchesOTBelowSimilarity = true; return x; }).ToList();

                var updateMatches = await GetMatches(updatesToMatch);

                return GetUpdatedMatches(updateMatches);
            }

            Dictionary<int, Submission> GetUpdatedMatches(IEnumerable<MatchResult> additionalMatches)
            {
                foreach (var matchedUpdate in additionalMatches)
                {
                    var newMatches = mapper.Map<Bdo.MatchingEngine.MatchResult>(matchedUpdate);

                    if (newMatches != null && !newMatches.Matches.Any()) return dictionary;

                    dictionary.Where(x => x.Key.Equals((int)matchedUpdate.InputWorkId))
                        .Select(x =>
                        {
                            x.Value.MatchedResult.Matches = x.Value.MatchedResult.Matches.Concat(newMatches.Matches);
                            return x;
                        }).ToList();
                }

                return dictionary;
            }
        }

        public async Task<IEnumerable<MatchResult>> GetMatches(IEnumerable<InputWorkInfo> inputs)
        {
            var response = await (await httpClient.PostAsJsonAsync("Work/Match", inputs)).EnsureSuccessStatusCodeAsync();
            return (await response.Content.ReadAsAsync<IEnumerable<MatchResult>>())?.ToList();
        }

        public async Task<IEnumerable<MatchingResult>> GetMatches(IEnumerable<UsageWorkGroup> groups)
        {
            var response = await (await httpClient.PostAsJsonAsync("UsageWorks/Match", groups)).EnsureSuccessStatusCodeAsync();
            return (await response.Content.ReadAsAsync<IEnumerable<MatchingResult>>())?.ToList();
        }

        public async Task<IEnumerable<InterestedPartyModel>> MatchContributorsAsync(InterestedPartySearchModel searchModel)
        {
            var matchingModel = new ContributorsSearchModel
            {
                LastName = new SearchField { Type = SearchOperator.Contains, Value = searchModel.Name },
                IPINumber = searchModel.IPNameNumber.HasValue && searchModel.IPNameNumber.Value != 0
                    ? new SearchField { Type = SearchOperator.ExactMatch, Value = searchModel.IPNameNumber }
                    : null,
                IPBaseNumber = new SearchField { Type = SearchOperator.Contains, Value = searchModel.IpBaseNumber }
            };

            var response = await (await httpClient.PostAsJsonAsync("Contributor/Match", matchingModel)).EnsureSuccessStatusCodeAsync();
            return mapper.Map<IEnumerable<InterestedPartyModel>>(await response.Content.ReadAsAsync<IEnumerable<Contributor>>());
        }
    }
}
