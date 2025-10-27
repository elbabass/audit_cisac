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
    public class AS_15 : IProcessingSubComponent
    {
        private readonly IWorkManager workManager;
        private readonly IMapper mapper;

        public AS_15(IWorkManager workManager, IMapper mapper)
        {
            this.workManager = workManager;
            this.mapper = mapper;
        }

        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.CUR };
        public IEnumerable<RequestType> ValidRequestTypes => new List<RequestType> { RequestType.Label };
        public bool? IsEligible => null;
        public PreferedIswcType PreferedIswcType => PreferedIswcType.Existing;
        public string Identifier => nameof(AS_15);
        public string PipelineComponentVersion => typeof(ProcessingComponent).GetComponentVersion();

        public async Task<IswcModel> ProcessSubmission(Submission submission)
        {
            if (submission.IsrcMatchedResult.Matches.Where(x => x.IswcStatus == 1).Any())
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

                var workinfoId = await workManager.UpdateAsync(submission);
                submission.IswcModel = (await workManager.FindManyAsync(new long[] { workinfoId })).FirstOrDefault();

                submission.Model.IswcsToMerge = new List<string>() { submission.Model.PreferredIswc };
                submission.Model.PreferredIswc = submission.IsrcMatchedResult.Matches.FirstOrDefault(x => x.IswcStatus == 1).Numbers.FirstOrDefault(x => x.Type == "ISWC").Number ?? string.Empty;
                submission.IswcModel.VerifiedSubmissions.FirstOrDefault().LinkedTo = submission.Model.PreferredIswc;
            }

            return submission.IswcModel;
        }
    }
}
