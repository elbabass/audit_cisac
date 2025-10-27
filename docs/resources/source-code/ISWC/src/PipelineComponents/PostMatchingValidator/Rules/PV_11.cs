using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules
{
    public class PV_11 : IRule, IAlwaysOnRule
    {
        private readonly IMessagingManager messagingManager;
        private readonly IAgencyManager agencyManager;
        private readonly IWorkflowManager workflowManager;

        public PV_11(IAgencyManager agencyManager, IWorkflowManager workflowManager, IMessagingManager messagingManager)
        {
            this.agencyManager = agencyManager;
            this.workflowManager = workflowManager;
            this.messagingManager = messagingManager;
        }

        public string Identifier => nameof(PV_11);

        public string ParameterName => string.Empty;

        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.COA, TransactionType.COR };

        public ValidatorType ValidatorType => ValidatorType.PostMatchingValidator;

        public string PipelineComponentVersion => typeof(PostMatchingValidator).GetComponentVersion();

        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var model = submission.Model;
            var agency = submission.TransactionType == TransactionType.COR
                ? submission.WorkflowSearchModel.Agency
                : submission.Model.Agency;

            if (await agencyManager.FindAsync(agency) == null)
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._151);
                return (false, submission);
            }

            if (submission.TransactionType == TransactionType.COR)
            {
                model.WorkflowTasks = await workflowManager.FindWorkflows(submission.WorkflowSearchModel);
                return (true, submission);
            }
            else if (submission.TransactionType == TransactionType.COA)
            {
                model.WorkflowTasks = await workflowManager.UpdateWorkflowTasks(model.WorkflowTasks, agency);
                return (true, submission);
            }
            return (true, submission);
        }
    }
}
