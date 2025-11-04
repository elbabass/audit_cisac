using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.ProcessingComponent.Processing
{
    public class AS_03 : IProcessingSubComponent
    {
        private readonly IWorkManager workManager;
        private readonly IAS_10 as_10;

        public AS_03(IWorkManager workManager, IAS_10 as_10)
        {
            this.workManager = workManager;
            this.as_10 = as_10;
        }

        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR };

        public bool? IsEligible => true;

        public PreferedIswcType PreferedIswcType => PreferedIswcType.New;

        public string Identifier => nameof(AS_03);
        public string PipelineComponentVersion => typeof(ProcessingComponent).GetComponentVersion();
        public IEnumerable<RequestType> ValidRequestTypes => new List<RequestType> { RequestType.Agency, RequestType.Label, RequestType.Publisher, RequestType.ThirdParty };

        async Task<IswcModel> IProcessingSubComponent.ProcessSubmission(Submission submission)
        {
            submission = await as_10.RecaculateAuthoritativeFlag(submission);

            var workinfoId = await workManager.AddWorkInfoAsync(submission, getNewIswc: true);
            submission.IswcModel = (await workManager.FindManyAsync(new long[] { workinfoId }, detailLevel: submission.DetailLevel)).First();

            return submission.IswcModel;
        }
    }
}
