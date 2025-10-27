using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules
{
    public class PV_14 : IRule, IAlwaysOnRule
    {
        private readonly IWorkManager workManager;
        private readonly IMessagingManager messagingManager;

        public PV_14(IWorkManager workManager, IMessagingManager messagingManager)
        {
            this.workManager = workManager;
            this.messagingManager = messagingManager;
        }

        public string Identifier => nameof(PV_14);
        public string ParameterName => string.Empty;
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CDR };
        public ValidatorType ValidatorType => ValidatorType.PostMatchingValidator;
		public string PipelineComponentVersion => typeof(PostMatchingValidator).GetComponentVersion();
		public string? RuleConfiguration { get; private set; }

		public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            SubmissionModel model = submission.Model;

            if (!string.IsNullOrEmpty(model.PreferredIswc) && !await workManager.Exists(model.PreferredIswc))
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._117);
                return (false, submission);
            }
  
            var iswc = (await workManager.FindIswcModelAsync(model.PreferredIswc, detailLevel: DetailLevel.Core));

            if (iswc.VerifiedSubmissions.Any())
            {
                var agencySubmissions = iswc.VerifiedSubmissions.Select(x => x.Agency);
                model.ApproveWorkflowTasks = submission.IsEligible && agencySubmissions.GroupBy(x => x)
                    .Select(y => y.FirstOrDefault()).Where(z => z != model.Agency).Count() > 0;
            }

            return (true, submission);
        }
    }
}
