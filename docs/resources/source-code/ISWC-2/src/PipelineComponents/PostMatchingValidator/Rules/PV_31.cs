using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Data.Repositories;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator.Rules
{
    /// <summary>
    /// Prevent ISWCLinkedTo self-referential chain.
    /// https://dev.azure.com/spanishpoint/CISAC/_workitems/edit/2446
    /// </summary>
    public class PV_31 : IRule, IAlwaysOnRule
    {
        private readonly IIswcLinkedToRepository iswcLinkedToRepository;
        private readonly IIswcRepository iswcRepository;
        private readonly IMessagingManager messagingManager;

        public PV_31(IIswcLinkedToRepository iswcLinkedToRepository, IIswcRepository iswcRepository, IMessagingManager messagingManager)
        {
            this.iswcLinkedToRepository = iswcLinkedToRepository;
            this.iswcRepository = iswcRepository;
            this.messagingManager = messagingManager;
        }

        public string Identifier => nameof(PV_31);

        public string ParameterName => string.Empty;

        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.MER };

        public ValidatorType ValidatorType => ValidatorType.PostMatchingValidator;

        public string PipelineComponentVersion => typeof(PostMatchingValidator).GetComponentVersion();

        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var model = submission.Model;

            foreach (var iswcToMerge in model.IswcsToMerge)
            {
                var parentIswc = (await iswcRepository.FindAsyncOptimized(x => x.Iswc1 == iswcToMerge, i => i.IswclinkedTo))?
                    .IswclinkedTo.FirstOrDefault(x => x.Status)?.LinkedToIswc;

                if (parentIswc != null)
                {
                    submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._157);
                    return (false, submission);
                }

                var iswc = await iswcLinkedToRepository.GetLinkedToTree(new IswcModel { Iswc = iswcToMerge });

                if (iswc.LinkedIswcTree.FindTreeNode(x => x.Data.Iswc == model.PreferredIswc) != null)
                {
                    submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._156);
                    return (false, submission);
                }
            }

            return (true, submission);
        }
    }
}
