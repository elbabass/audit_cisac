
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Data.Repositories;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.ProcessingComponent.Processing
{
    public class AS_11 : IProcessingSubComponent
    {
        private readonly IWorkManager workManager;
        private readonly IIswcRepository iswcRepository;

        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.DMR };

        public bool? IsEligible => true;
        public PreferedIswcType PreferedIswcType => PreferedIswcType.Existing;
        public string Identifier => nameof(AS_11);
        public string PipelineComponentVersion => typeof(ProcessingComponent).GetComponentVersion();
        public IEnumerable<RequestType> ValidRequestTypes => new List<RequestType> { RequestType.Agency, RequestType.Label, RequestType.Publisher, RequestType.ThirdParty };

        public AS_11(IWorkManager workManager, IIswcRepository iswcRepository)
        {
            this.workManager = workManager;
            this.iswcRepository = iswcRepository;
        }

        public async Task<IswcModel> ProcessSubmission(Submission submission)
        {
            await workManager.DeleteIswcLinkedTo(submission);

            var iswc = (await iswcRepository.FindAsync(x => x.Iswc1 == submission.Model.PreferredIswc)).IswcId;
            submission.IswcModel = await workManager.FindAsync(iswc);

            return submission.IswcModel;
        }
    }
}
