using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules
{
    public class PV_29 : IRule, IAlwaysOnRule
    {
        private readonly IMessagingManager messagingManager;

        public PV_29(IMessagingManager messagingManager)
        {
            this.messagingManager = messagingManager;
        }

        public string Identifier => nameof(PV_29);

        public string ParameterName => string.Empty;

        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType>() { TransactionType.CAR, TransactionType.FSQ };

        public ValidatorType ValidatorType => ValidatorType.PostMatchingValidator;
        public string PipelineComponentVersion => typeof(PostMatchingValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var model = submission.Model;

            if (string.IsNullOrEmpty(model.PreferredIswc) && (submission.IsEligible && !submission.Model.AdditionalAgencyWorkNumbers.Any()))
                return (true, submission);



            if ((!submission.IsEligible && !submission.MultipleAgencyWorkCodesChild) || (submission.Model.AdditionalAgencyWorkNumbers.Any() && !submission.Model.AdditionalAgencyWorkNumbers.Any(x=> x.IsEligible)))
            {
                if (submission.MatchedResult == null || !submission.MatchedResult.Matches.Any())
                {
                    if (submission.TransactionType == TransactionType.FSQ)
                        submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._163);
                    else
                        submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._140);
                    return (false, submission);
                }

                else if(submission.UpdateAllocatedIswc && !submission.IsEligible)
                {
                    submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._140);
                }

                var matchesCount = submission.IsPortalSubmissionFinalStep && submission.TransactionType != TransactionType.FSQ
                    ? submission.MatchedResult.Matches.GroupBy(
                  x => x.Numbers.FirstOrDefault(i => i.Type == "ISWC" && i.Number.Equals(model.PreferredIswc))?.Number)
                    .ToList()?.Where(x => x.Key != null).Count()
                    : submission.MatchedResult.Matches.GroupBy(
                  x => x.Numbers.FirstOrDefault(i => i.Type == "ISWC")?.Number)
                    .ToList()?.Where(x => x.Key != null).Count();

                if (matchesCount == 0)
                {
                    if (submission.TransactionType == TransactionType.FSQ)
                        submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._163);
                    else
                        submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._140);
                    return (false, submission);
                }
                else if (matchesCount != 1)
                {
                    if (submission.TransactionType == TransactionType.FSQ)
                        submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._164);
                    else
                        submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._160);
                    return (false, submission);
                }
            }
            return (true, submission);
        }
    }
}
