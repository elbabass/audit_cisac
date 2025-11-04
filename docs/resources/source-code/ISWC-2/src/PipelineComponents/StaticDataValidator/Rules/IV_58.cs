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
    public class IV_58 : IRule
    {
        private readonly IMessagingManager messagingManager;
        private readonly IRulesManager rulesManager;

        public IV_58(IMessagingManager messagingManager, IRulesManager rulesManager)
        {
            this.messagingManager = messagingManager;
            this.rulesManager = rulesManager;
        }

        public string Identifier => nameof(IV_58);
        public string ParameterName => "MustHaveOneIP";
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.CUR };
        public ValidatorType ValidatorType => ValidatorType.StaticValidator;
        public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {

            if (submission.Model.PreviewDisambiguation && submission.RequestSource != Framework.Http.Requests.RequestSource.PORTAL)
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._181);
                return (false, submission);
            }
            else
                return (true, submission);
        }
    }
}
