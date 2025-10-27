using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.MatchingComponent.PostMatching
{
    internal class MatchingForIswcRelated : IPostMatchingComponent
    {
        private readonly IMatchingManager matchingManager;
        private readonly IMessagingManager messagingManager;

        public MatchingForIswcRelated(IMatchingManager matchingManager, IMessagingManager messagingManager)
        {
            this.matchingManager = matchingManager;
            this.messagingManager = messagingManager;
        }

        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.CUR, TransactionType.MER, TransactionType.FSQ };

        public string Identifier => nameof(MatchingForIswcRelated);

        public bool? IsEligible => null;
        public string PipelineComponentVersion => typeof(MatchingComponent).GetComponentVersion();
        public IEnumerable<RequestType> ValidRequestTypes => new List<RequestType> { RequestType.Agency, RequestType.Label, RequestType.Publisher, RequestType.ThirdParty };

        public async Task<IEnumerable<Submission>> ProcessSubmissions(IEnumerable<Submission> submissions)
        {
            foreach (var submission in submissions.ToList())
            {
                // DerivedFrom
                if ((submission.TransactionType == TransactionType.CAR || submission.TransactionType == TransactionType.CUR)
                    && (!string.IsNullOrEmpty(submission.Model.DerivedWorkType.ToString()))
                    && (submission.Model.DerivedFrom.Count > 0) && submission.Rejection == null && !submission.SkipProcessing)
                {
                    var derivedFromIswcs = submission.Model.DerivedFrom.Select(i => i.Iswc).Where(x => !string.IsNullOrWhiteSpace(x));
                    var submissionsToMatch = new List<Submission>() { };

                    foreach (var item in derivedFromIswcs.Cast<string>().Select((iswc, i) => (iswc, i)))
                    {
                        submissionsToMatch.Add(new Submission
                        {
                            SubmissionId = item.i,
                            Model = new SubmissionModel { WorkNumber = new WorkNumber { Type = "ISWC", Number = item.iswc } }
                        });
                    };

                    var matchedSubmissions = await matchingManager.MatchAsync(submissionsToMatch, source: "Search");

                    if (matchedSubmissions.Any(a => a.MatchedResult.Matches.Count() == 0))
                        submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._201);

                    if (matchedSubmissions.Any(r => r.MatchedResult.Matches.Any(
                        x => x.Numbers.Where(y => y.Type != null && y.Type.Equals("ISWC")).Where(
                            z => z.Number != null && submission.Model.DerivedFrom.Select(i => i.Iswc).Contains(z.Number)).Count() != 1)))
                        submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._201);
                }

                // Merges
                if ((submission.TransactionType == TransactionType.MER)
                    && (submission.Model.IswcsToMerge?.Count() > 0) && (submission.Rejection == null))
                {
                    var matchedSubmission = await matchingManager.MatchAsync(new List<Submission>() { new Submission() { TransactionType = TransactionType.MER,
                        SubmissionId = submission.SubmissionId, IsEligible = submission.IsEligible,
                        Model = new SubmissionModel { IswcsToMerge = submission.Model.IswcsToMerge, SourceDb = submission.Model.SourceDb } } }
                    );

                    if (!matchedSubmission.First().MatchedResult.Matches.Any())
                        submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._202);

                    if (matchedSubmission.Any(r => r.MatchedResult.Matches.Any(
                        x => x.Numbers.Where(y => y.Type != null && y.Type.Equals("ISWC")).Where(
                            z => z.Number != null && submission.Model.IswcsToMerge.Contains(z.Number)).Count() != 1)))
                        submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._202);
                }

                if ((submission.TransactionType == TransactionType.MER)
                    && (submission.Model.WorkNumbersToMerge?.Count() > 0) && (submission.Rejection == null))
                {
                    var matchedSubmission = await matchingManager.MatchAsync(new List<Submission>() { new Submission() { TransactionType = TransactionType.MER,
                        SubmissionId = submission.SubmissionId, IsEligible = submission.IsEligible,
                        Model = new SubmissionModel { WorkNumbersToMerge = submission.Model.WorkNumbersToMerge, SourceDb = submission.Model.SourceDb } } }
                    );

                    if (!matchedSubmission.First().MatchedResult.Matches.Any())
                        submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._203);

                    if (matchedSubmission.Any(r => r.MatchedResult.Matches.Any(
                        x => x.Numbers.Where(z => z.Number != null && submission.Model.WorkNumbersToMerge.Select(i => i.Number).Contains(z.Number)
                            && submission.Model.WorkNumbersToMerge.Select(i => i.Type).Contains(z.Type)).Count() != 1)))
                        submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._203);
                }
            }
            return submissions;
        }
    }
}
