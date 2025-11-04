using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Converters;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules
{
    public class IV_08 : IRule
    {
        private readonly ISubmissionSourceManager submissionSourceManager;
        private readonly IMessagingManager messagingManager;
        private readonly IRulesManager rulesManager;
        private readonly IAgencyManager agencyManager;

        public IV_08(ISubmissionSourceManager submissionSourceManager, IMessagingManager messagingManager, IRulesManager rulesManager, IAgencyManager agencyManager)
        {
            this.submissionSourceManager = submissionSourceManager;
            this.messagingManager = messagingManager;
            this.rulesManager = rulesManager;
            this.agencyManager = agencyManager;
        }

        public string Identifier => nameof(IV_08);
        public string ParameterName => "ValidateSubmittingAgency";
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> {
            TransactionType.CAR, TransactionType.CUR, TransactionType.CDR, TransactionType.MER, TransactionType.FSQ
        };
        public ValidatorType ValidatorType => ValidatorType.StaticValidator;
        public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var paramValue = await rulesManager.GetParameterValue<bool>("ValidateSubmittingAgency");
            RuleConfiguration = paramValue.ToString();

            if (!paramValue)
                return (true, submission);

            var model = submission.Model;

            if (model.Agency != null && await agencyManager.FindAsync(model.Agency) == null)
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._103);
                return (false, submission);
            }

            // Value is set to 0 if not provided in EDI submission
            if (model.SourceDb == 0)
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._138);
                return (false, submission);
            }

            if (submission.TransactionType != TransactionType.FSQ && (await submissionSourceManager.FindAsync(model.SourceDb.ToAgencyCode()) == null ||
               (model.Agency != null && await submissionSourceManager.FindAsync(model.Agency) == null)))
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._115);
                return (false, submission);
            }
            else
                return (true, submission);
        }
    }
}
