using AutoMapper;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.ProcessingComponent.Processing
{
    public class AS_02 : IProcessingSubComponent
    {
        private readonly IWorkManager workManager;
        private readonly IAS_10 as_10;
        private readonly IMapper mapper;

        public AS_02(IWorkManager workManager, IAS_10 as_10, IMapper mapper)
        {
            this.workManager = workManager;
            this.as_10 = as_10;
            this.mapper = mapper;
        }

        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR };

        public bool? IsEligible => false;

        public PreferedIswcType PreferedIswcType => PreferedIswcType.Existing;

        public string Identifier => nameof(AS_02);
        public string PipelineComponentVersion => typeof(ProcessingComponent).GetComponentVersion();
        public IEnumerable<RequestType> ValidRequestTypes => new List<RequestType> { RequestType.Agency, RequestType.Label, RequestType.Publisher, RequestType.ThirdParty };

        public async Task<IswcModel> ProcessSubmission(Submission submission)
        {
            submission = await as_10.RecaculateAuthoritativeFlag(submission);

            if (submission.Model.PreviewDisambiguation && submission.MatchedResult.Matches.Any())
            {
                var model = submission.Model;
                submission.IswcModel = new IswcModel
                {
                    Agency = model.Agency,
                    Iswc = "",
                    VerifiedSubmissions = new List<VerifiedSubmissionModel>()
                    {
                        mapper.Map<VerifiedSubmissionModel>(model)
                    }
                };

                foreach (var x in submission.IswcModel.VerifiedSubmissions)
                {
                    x.IswcEligible = submission.IsEligible;
                    x.Iswc = "";
                    x.WorkInfoID = 0;
                };

                return submission.IswcModel;
            }

            var workinfoId = await workManager.AddWorkInfoAsync(submission, getNewIswc: false);
            submission.IswcModel = (await workManager.FindManyAsync(new long[] { workinfoId }, detailLevel : submission.DetailLevel)).First();

            return submission.IswcModel;
        }
    }
}
