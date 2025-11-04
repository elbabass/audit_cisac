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
    public class IV_05 : IRule
    {
        private readonly IMessagingManager messagingManager;

        public IV_05(IMessagingManager messagingManager)
        {
            this.messagingManager = messagingManager;
        }

        public string Identifier => nameof(IV_05);

        public string ParameterName => string.Empty;

        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.CUR, TransactionType.FSQ };

        public ValidatorType ValidatorType => ValidatorType.StaticValidator;

        public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();

        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {

            if (!submission.Model.Titles.Any(t => t.Type == Bdo.Work.TitleType.OT && !string.IsNullOrWhiteSpace(t.Name)))
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._109);
                return (false, submission);
            }
            else
                return (true, submission);
        }
    }
}
