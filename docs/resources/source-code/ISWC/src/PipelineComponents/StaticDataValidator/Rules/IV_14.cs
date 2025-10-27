using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules
{
    public class IV_14 : IRule
    {
        private readonly IMessagingManager messagingManager;

        public IV_14(IMessagingManager messagingManager)
        {
            this.messagingManager = messagingManager;
        }

        public string Identifier => nameof(IV_14);
        public string ParameterName => "ValidateISWCFormat";
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> {
            TransactionType.CAR, TransactionType.CUR, TransactionType.CDR, TransactionType.CMQ,
            TransactionType.CIQ, TransactionType.MER, TransactionType.DMR, TransactionType.FSQ
        };
        public ValidatorType ValidatorType => ValidatorType.StaticValidator;
        public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var model = submission.Model;

            if (!string.IsNullOrWhiteSpace(model.PreferredIswc))
            {
                if (!model.PreferredIswc.IsValidIswcPattern())
                {
                    submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._113);
                    return (false, submission);
                }
            }

            if (!string.IsNullOrWhiteSpace(model.Iswc))
            {
                if (!model.Iswc.IsValidIswcPattern())
                {
                    submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._113);
                    return (false, submission);
                }
            }

            return (true, submission);
        }
    }
}
