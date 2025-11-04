using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules
{
    public class IV_57 : IRule
    {
        private readonly IWorkManager workManager;
        private readonly IRulesManager rulesManager;
        private readonly IMessagingManager messagingManager;

        public IV_57(IWorkManager workManager, IRulesManager rulesManager, IMessagingManager messagingManager)
        {
            this.workManager = workManager;
            this.rulesManager = rulesManager;
            this.messagingManager = messagingManager;
        }

        public string Identifier => nameof(IV_57);

        public string ParameterName => string.Empty;

        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.CUR };

        public ValidatorType ValidatorType => ValidatorType.StaticValidator;

        public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();

        public string? RuleConfiguration => null;

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var model = submission.Model;

            if (model.AdditionalAgencyWorkNumbers.Any() && submission.TransactionSource != Bdo.Reports.TransactionSource.Publisher)
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._182);
                return (false, submission);
            }

            return (true, submission);

        }
    }
}
