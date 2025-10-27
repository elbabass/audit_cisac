using System.Collections.Generic;
using System.Threading.Tasks;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.MetadataStandardizationValidator.Rules
{
    public class MD_18 : IRule
    {
        private readonly IMessagingManager messagingManager;
        private readonly IWorkManager workManager;
        private readonly IRulesManager rulesManager;
        private readonly IAgencyManager agencyManager;

        public MD_18(IMessagingManager messagingManager, IWorkManager workManager, IRulesManager rulesManager, IAgencyManager agencyManager)
        {
            this.messagingManager = messagingManager;
            this.workManager = workManager;
            this.rulesManager = rulesManager;
            this.agencyManager = agencyManager;
        }

        public string Identifier => nameof(MD_18);
        public string ParameterName => string.Empty;
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.CUR };
        public ValidatorType ValidatorType => ValidatorType.MetadataStandardizationValidator;
        public string PipelineComponentVersion => typeof(MetadataStandardizationValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }
        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var paramValue = await rulesManager.GetParameterValue<bool>("EnableChecksumValidation");

            if (!paramValue)
                return (true, submission);

            var model = submission.Model;
            if (model.PreviewDisambiguation) return (true, submission);

            if (!await agencyManager.ChecksumEnabled(model.Agency))
                return (true, submission);

            var checksum = await workManager.GetChecksum(model.WorkNumber.Type, model.WorkNumber.Number);
            if (checksum != null && checksum.Hash == model.HashCode)
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._174);
                return (false, submission);
            }

            return (true, submission);
        }
    }
}
