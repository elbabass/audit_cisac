using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules
{
    public class IV_53 : IRule
    {
        private readonly IMessagingManager messagingManager;

        public IV_53(IMessagingManager messagingManager)
        {
            this.messagingManager = messagingManager;
        }

        public string Identifier => nameof(IV_53);
        public string ParameterName => string.Empty;
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.CUR };
        public ValidatorType ValidatorType => ValidatorType.StaticValidator;
        public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            if (submission.Model.AllowProvidedIswc && submission.Model.Iswc != null && submission.Model.Iswc.StartsWith("T3"))
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._171);
                return (false, submission);
            }

            return (true, submission);
        }
    }
}
