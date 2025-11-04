using AutoMapper;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Data.Repositories;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.ProcessingComponent.Processing
{
    public class AS_14 : IProcessingSubComponent
    {
        private readonly IWorkManager workManager;
        private readonly IWorkRepository workRepository;
        private readonly IMapper mapper;
        private readonly IAS_10 as_10;

        public AS_14(IWorkManager workManager, IWorkRepository workRepository, IMapper mapper, IAS_10 as_10)
        {
            this.workManager = workManager;
            this.workRepository = workRepository;
            this.mapper = mapper;
            this.as_10 = as_10;
        }

        public string Identifier => nameof(AS_14);

        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CUR };

        public bool? IsEligible => false;

        public PreferedIswcType PreferedIswcType => PreferedIswcType.Existing;
        public string PipelineComponentVersion => typeof(ProcessingComponent).GetComponentVersion();
        public IEnumerable<RequestType> ValidRequestTypes => new List<RequestType> { RequestType.Agency, RequestType.Label, RequestType.Publisher, RequestType.ThirdParty };

        public async Task<IswcModel> ProcessSubmission(Submission submission)
        {
            var workinfo = await workRepository.FindAsync(w =>
                w.AgencyWorkCode == submission.Model.WorkNumber!.Number && w.AgencyId == submission.Model.WorkNumber.Type && w.Status);
            submission.IswcModel = (await workManager.FindManyAsync(new long[] { workinfo.WorkInfoId }, detailLevel: submission.DetailLevel)).First();

            return submission.IswcModel;
        }
    }
}
