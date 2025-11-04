using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.ProcessingComponent.Processing
{
    public class AS_09 : IProcessingSubComponent
    {
        private readonly IWorkManager workManager;

        public AS_09(IWorkManager workManager)
        {
            this.workManager = workManager;
        }

        public string Identifier => nameof(AS_09);

        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CDR };

        public bool? IsEligible => null;

        public PreferedIswcType PreferedIswcType => PreferedIswcType.Existing;
        public string PipelineComponentVersion => typeof(ProcessingComponent).GetComponentVersion();
        public IEnumerable<RequestType> ValidRequestTypes => new List<RequestType> { RequestType.Agency, RequestType.Label, RequestType.Publisher, RequestType.ThirdParty };

        public async Task<IswcModel> ProcessSubmission(Submission submission)
        {
            await workManager.DeleteAsync(submission);
            return new IswcModel();
        }
    }
}
