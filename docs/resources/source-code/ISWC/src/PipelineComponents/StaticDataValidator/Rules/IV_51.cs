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
    public class IV_51 : IRule
    {
        private readonly IMessagingManager messagingManager;
        private readonly ILookupManager lookupManager;
        private readonly IWorkManager workManager;

        public IV_51(IMessagingManager messagingManager, ILookupManager lookupManager, IWorkManager workManager)
        {
            this.messagingManager = messagingManager;
            this.lookupManager = lookupManager;
            this.workManager = workManager;
        }

        public string Identifier => nameof(IV_51);
        public string ParameterName => string.Empty;
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CUR };
        public ValidatorType ValidatorType => ValidatorType.StaticValidator;
        public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var model = submission.Model;
            var preferredIswc = !string.IsNullOrWhiteSpace(submission.ExistingWork?.Iswc) ? submission.ExistingWork.Iswc : model.PreferredIswc;

            if (!model.DisambiguateFrom.Any() && (await workManager.ExistsWithDisambiguation(preferredIswc, model.WorkNumber)))
            {
                var sources = await lookupManager.GetAgencyDisallowDisambiguationOverwrite(model.Agency);

                if (!string.IsNullOrWhiteSpace(sources))
                {
                    if (sources.SplitWordOnSeparators().Contains(submission.RequestSource.ToFriendlyString()))
                    {
                        submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._167);
                        return (false, submission);
                    }
                }
            }

            return (true, submission);
        }
    }
}
