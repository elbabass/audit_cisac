using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules
{
    public class IV_45 : IRule
    {
        private readonly IMessagingManager messagingManager;

        public IV_45(IMessagingManager messagingManager)
        {
            this.messagingManager = messagingManager;
        }

        public string Identifier => nameof(IV_45);
        public string ParameterName => string.Empty;
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.FSQ };
        public ValidatorType ValidatorType => ValidatorType.StaticValidator;
        public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var model = submission.Model;

            if ((model.WorkNumber != null && model.WorkNumber.Number?.Length > 20 && !model.AdditionalAgencyWorkNumbers.Any()) ||
                (model.AdditionalAgencyWorkNumbers.Any() && model.AdditionalAgencyWorkNumbers.Any(x => x.WorkNumber.Number.Length > 20)))
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._158);
                return (false, submission);
            }

            return (true, submission);
        }
    }
}
