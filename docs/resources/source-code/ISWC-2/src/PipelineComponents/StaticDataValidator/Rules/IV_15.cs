using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules
{
    public class IV_15 : IRule
    {
        private readonly IMessagingManager messagingManager;
        private readonly IRulesManager rulesManager;

        public IV_15(IMessagingManager messagingManager, IRulesManager rulesManager)
        {
            this.messagingManager = messagingManager;
            this.rulesManager = rulesManager;
        }

        public string Identifier => nameof(IV_15);
        public string ParameterName => "ValidateISWCCheckDigit";
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> {TransactionType.CAR, TransactionType.CUR,
            TransactionType.CDR, TransactionType.CMQ, TransactionType.CIQ, TransactionType.MER, TransactionType.DMR, TransactionType.FSQ };
        public ValidatorType ValidatorType => ValidatorType.StaticValidator;
        public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var paramValue = await rulesManager.GetParameterValue<bool>("ValidateISWCCheckDigit");
            RuleConfiguration = paramValue.ToString();

            if (!paramValue)
                return (true, submission);

            var model = submission.Model;

            if (!string.IsNullOrWhiteSpace(model.PreferredIswc))
            {
                if (!model.PreferredIswc.IsValidIswcPattern())
                {
                    submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._113);
                    return (false, submission);
                }

                if (CalculateSum(model.PreferredIswc) % 10 != 0)
                {
                    submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._141);
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

                if (CalculateSum(model.Iswc) % 10 != 0)
                {
                    submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._141);
                    return (false, submission);
                }
            }

            return (true, submission);
        }

        private int CalculateSum(string iswc)
        {
            var sum = 0;

            for (var i = 0; i < iswc.Length - 1; i++)
            {
                var iswcCharToInt = iswc[i] - '0';
                if (i == 0)
                    sum += 1;
                else if (i == 1)
                    sum += (1 * iswcCharToInt);
                else if (i > 1 && i < 10)
                    sum += (i * iswcCharToInt);
            }

            return sum + (iswc[10] - '0');
        }
    }
}
