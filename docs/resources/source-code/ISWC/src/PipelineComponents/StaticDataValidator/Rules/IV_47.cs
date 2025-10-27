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
    public class IV_47 : IRule
    {
        private readonly IMessagingManager messagingManager;

        public IV_47(IMessagingManager messagingManager)
        {
            this.messagingManager = messagingManager;
        }

        public string Identifier => nameof(IV_47);
        public string ParameterName => string.Empty;
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.CUR };
        public ValidatorType ValidatorType => ValidatorType.StaticValidator;
        public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var performers = submission.Model.Performers;
            if (performers.Any())
            {
                if (performers.Any(x => (!string.IsNullOrWhiteSpace(x.FirstName) && x.FirstName.Length > 50)
                 || (!string.IsNullOrWhiteSpace(x.LastName) && x.LastName.Length > 50)))
                {
                    submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._162);
                    return (false, submission);
                }
                else if (performers.Any(x => string.IsNullOrWhiteSpace(x.LastName)))
                {
                    submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._126);
                    return (false, submission);
                }

            }

            return (true, submission);
        }
    }
}
