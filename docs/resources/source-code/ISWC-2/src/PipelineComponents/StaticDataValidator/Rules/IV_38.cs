using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules
{
    public class IV_38 : IRule
    {
        private readonly IMessagingManager messagingManager;
        private readonly IWorkManager workManager;
        private readonly IRulesManager rulesManager;

        public IV_38(IWorkManager workManager, IRulesManager rulesManager, IMessagingManager messagingManager)
        {
            this.workManager = workManager;
            this.rulesManager = rulesManager;
            this.messagingManager = messagingManager;
        }

        public string Identifier => nameof(IV_38);
        public string ParameterName => "ValidateExcerpt";
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.CUR, TransactionType.FSQ };
        public ValidatorType ValidatorType => ValidatorType.StaticValidator;
        public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var paramValue = await rulesManager.GetParameterValue<bool>("ValidateExcerpt");
            RuleConfiguration = paramValue.ToString();

            if (!paramValue)
                return (true, submission);

            var model = submission.Model;

            if (model.DerivedWorkType == DerivedWorkType.Excerpt)
            {
                if (!model.DerivedFrom.Any())
                {
                    submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._147);
                    return (false, submission);
                }

                foreach (var der in model.DerivedFrom)
                {
                    if (string.IsNullOrWhiteSpace(der.Title) && string.IsNullOrWhiteSpace(der.Iswc))
                    {
                        submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._120);
                        return (false, submission);
                    }

                    else if ((!string.IsNullOrWhiteSpace(der.Title) || string.IsNullOrWhiteSpace(der.Title)) && !string.IsNullOrWhiteSpace(der.Iswc))
                    {
                        if (!der.Iswc.IsValidIswcPattern() || (await workManager.FindAsync(der.Iswc) == null))
                        {
                            submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._119);
                            return (false, submission);
                        }
                    }
                }
            }

            return (true, submission);
        }
    }
}
