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
    public class IV_11 : IRule
    {
        private readonly IMessagingManager messagingManager;

        public IV_11(IMessagingManager messagingManager)
        {
            this.messagingManager = messagingManager;
        }

        public string Identifier => nameof(IV_11);

        public string ParameterName => string.Empty;

        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.CUR, TransactionType.CDR, TransactionType.FSQ };

        public ValidatorType ValidatorType => ValidatorType.StaticValidator;

        public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();

        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)

        {
            if (string.IsNullOrWhiteSpace(submission.Model.WorkNumber?.Number))
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._136);
                return (false, submission);
            }

            if (submission.TransactionSource == Bdo.Reports.TransactionSource.Publisher 
                && submission.Model.AdditionalAgencyWorkNumbers.Any() 
                && submission.Model.AdditionalAgencyWorkNumbers.Any(x => string.IsNullOrWhiteSpace(x.WorkNumber?.Number)))
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._136);
                return (false, submission);
            }

            if (submission.RequestType == RequestType.Agency
                && submission.TransactionSource != Bdo.Reports.TransactionSource.Publisher
                && submission.Model.WorkNumber.Number.Length == 20 && submission.Model.WorkNumber.Number.StartsWith("AS")
                && (submission.TransactionType == TransactionType.CAR || submission.TransactionType == TransactionType.CUR))
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._136);
                return (false, submission);
            }

            return (true, submission);
        }
    }
}
