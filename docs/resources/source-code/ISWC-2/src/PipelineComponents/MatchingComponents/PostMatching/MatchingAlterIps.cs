using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.MatchingComponent.PostMatching
{
    internal class MatchingAlterIps : IPostMatchingComponent
    {
        private readonly IMatchingManager matchingManager;

        public MatchingAlterIps(IMatchingManager matchingManager)
        {
            this.matchingManager = matchingManager;
        }

        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.FSQ };
        public string Identifier => nameof(MatchingForIswcRelated);
        public bool? IsEligible => null;
        public string PipelineComponentVersion => typeof(MatchingComponent).GetComponentVersion();
        public IEnumerable<RequestType> ValidRequestTypes => new List<RequestType> { RequestType.Agency, RequestType.Label, RequestType.Publisher, RequestType.ThirdParty };

        public async Task<IEnumerable<Submission>> ProcessSubmissions(IEnumerable<Submission> submissions)
        {
            if (submissions.Any(x => !x.MatchedResult.Matches.Any() && x.Rejection == null && !x.SkipProcessing))
            {
                var now = DateTime.UtcNow;
                List<Submission> submissionsWithNoMatchesAndPdIps =
                    (submissions.Where(x => (x.Model?.InterestedParties?.Any(
                        y => CommonIPs.PublicDomainIps.Contains(y.IpBaseNumber) || (y.DeathDate.HasValue && y.DeathDate.Value < now.AddYears(-80))) == true) && 
                        !(x.MatchedResult?.Matches?.Any() ?? false)).ToList().DeepCopy()) ?? new List<Submission>();

                List<Submission> submissionsToRunThroughMatching = new List<Submission>();

                foreach (var sub in submissionsWithNoMatchesAndPdIps)
                {
                    var isFullyPublicDomain = sub.Model.InterestedParties.All(x => CommonIPs.PublicDomainIps.Contains(x.IpBaseNumber)
                        || (x.DeathDate < now.AddYears(-80)));

                    if (!isFullyPublicDomain)
                    {
                        sub.Model.InterestedParties = sub.Model.InterestedParties.Where(
                            x => !CommonIPs.PublicDomainIps.Contains(x.IpBaseNumber) && !(x.DeathDate < now.AddYears(-80))).ToList();

                        submissionsToRunThroughMatching.Add(sub);
                    }

                }

                if (!submissionsToRunThroughMatching.Any())
                    return submissions;

                var dictionary = submissions.ToDictionary(x => x.SubmissionId);

                var submissionsWithNewMatchResult = await matchingManager.MatchAsync(submissionsToRunThroughMatching);

                foreach (var submission in submissionsWithNewMatchResult)
                {
                    dictionary[submission.SubmissionId].MatchedResult = submission.MatchedResult;
                }

                return submissions;
            }
            else
                return submissions;
        }
    }
}
