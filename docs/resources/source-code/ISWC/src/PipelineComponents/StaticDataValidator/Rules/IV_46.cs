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
    public class IV_46 : IRule
    {
        private readonly IMessagingManager messagingManager;
        private readonly IWorkManager workManager;

        public IV_46(IMessagingManager messagingManager, IWorkManager workManager)
        {
            this.messagingManager = messagingManager;
            this.workManager = workManager;
        }

        public string Identifier => nameof(IV_46);
        public string ParameterName => string.Empty;
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CUR };
        public ValidatorType ValidatorType => ValidatorType.StaticValidator;
        public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var model = submission.Model;

            submission.ExistingWork = submission.Model.AdditionalAgencyWorkNumbers.Any() ?
                await workManager.FindWorkForValidationAsync(model.AdditionalAgencyWorkNumbers.FirstOrDefault().WorkNumber)
                : await workManager.FindWorkForValidationAsync(model.WorkNumber);

            if (!string.IsNullOrWhiteSpace(model.PreferredIswc) && !submission.IsPortalSubmissionFinalStep
                && (submission.ExistingWork != null && model.PreferredIswc != submission.ExistingWork.Iswc
                && model.WorkNumber.Number == submission.ExistingWork.WorkNumber.Number
                && model.WorkNumber.Type == submission.ExistingWork.WorkNumber.Type))
            {
                model.PreferredIswc = string.Empty;
            }

            return (true, submission);
        }
    }
}
