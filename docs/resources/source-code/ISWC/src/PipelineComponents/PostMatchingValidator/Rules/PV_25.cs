using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules
{
    public class PV_25 : IRule, IAlwaysOnRule
    {
        private readonly IMessagingManager messagingManager;
        private readonly IWorkManager workManager;

        public PV_25(IWorkManager workManager, IMessagingManager messagingManager)
        {
            this.workManager = workManager;
            this.messagingManager = messagingManager;
        }

        public string Identifier => nameof(PV_25);
        public string ParameterName => string.Empty;
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CDR };
        public ValidatorType ValidatorType => ValidatorType.PostMatchingValidator;
        public string PipelineComponentVersion => typeof(PostMatchingValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var model = submission.Model;

            if (string.IsNullOrWhiteSpace(model.PreferredIswc) || model.WorkNumber == null ||
                model.WorkNumber.Type == null || string.IsNullOrWhiteSpace(model.WorkNumber.Type) ||
                string.IsNullOrWhiteSpace(model.WorkNumber.Number))
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._103);
                return (false, submission);
            }

            var workInfoByWorkNumber = (await workManager.FindVerifiedAsync(model.WorkNumber));
            if (workInfoByWorkNumber == null || workInfoByWorkNumber.Iswc == null || workInfoByWorkNumber.WorkNumber == null || workInfoByWorkNumber.WorkNumber.Type == null)
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._143);
                return (false, submission);
            }

            var workInfoByPreferredIswc = model.Agency != null ? (await workManager.FindVerifiedAsync(model.PreferredIswc, model.Agency)) : default;
            if (workInfoByPreferredIswc == null || workInfoByPreferredIswc.Iswc == null || workInfoByPreferredIswc.WorkNumber == null || workInfoByPreferredIswc.WorkNumber.Type == null)
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._117);
                return (false, submission);
            }
            else if (workInfoByWorkNumber.Iswc != workInfoByPreferredIswc.Iswc)
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._117);
                return (false, submission);
            }
            return (true, submission);
        }
    }
}
