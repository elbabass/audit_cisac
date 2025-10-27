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
    public class AS_05 : IProcessingSubComponent
    {
        private readonly IWorkManager workManager;
        private readonly IAS_10 as_10;
        private readonly IMapper mapper;

        public AS_05(IWorkManager workManager, IAS_10 as_10, IMapper mapper)
        {
            this.workManager = workManager;
            this.as_10 = as_10;
            this.mapper = mapper;
        }

        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CUR };

        public bool? IsEligible => true;

        public PreferedIswcType PreferedIswcType => PreferedIswcType.Different;
        public string Identifier => nameof(AS_05);
        public string PipelineComponentVersion => typeof(ProcessingComponent).GetComponentVersion();
        public IEnumerable<RequestType> ValidRequestTypes => new List<RequestType> { RequestType.Agency, RequestType.Label, RequestType.Publisher, RequestType.ThirdParty };

        public async Task<IswcModel> ProcessSubmission(Submission submission)
        {
            submission = await as_10.RecaculateAuthoritativeFlag(submission);

            var IswcToMerge = submission.ExistingWork?.PreferredIswc ?? string.Empty;

            if (submission.Model.PreviewDisambiguation)
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

            if (submission.IsEligibileOnlyForDeletingIps)
                submission.IsEligible = false;

            var workinfoId = await workManager.UpdateAsync(submission);
            submission.IswcModel = (await workManager.FindManyAsync(new long[] { workinfoId })).FirstOrDefault();

            if (submission.IsEligibileOnlyForDeletingIps &&
                !submission.IswcModel.VerifiedSubmissions.FirstOrDefault(x => x.WorkNumber.Number == submission.Model.WorkNumber.Number).IswcEligible)
                return submission.IswcModel;

            if (submission.IsPortalSubmissionFinalStep)
            {
                submission.Model.IswcsToMerge = new List<string>() { IswcToMerge };
                submission.Model.PreferredIswc = submission.MatchedResult.Matches.FirstOrDefault().Numbers.FirstOrDefault(x => x.Type == "ISWC"
                    && x.Number.Equals(submission.Model.PreferredIswc)).Number ?? string.Empty;
                submission.IswcModel.VerifiedSubmissions.FirstOrDefault().LinkedTo = IswcToMerge;
            }
            else
            {
                submission.Model.IswcsToMerge = new List<string>() { submission.Model.PreferredIswc };
                if(submission.MatchedResult.Matches.Any(x => x.IswcStatus == 2))
                {
                    submission.Model.IswcsToMerge = submission.MatchedResult.Matches.Where(x => x.IswcStatus == 2).Select(x => x.Numbers.FirstOrDefault(n => n.Type == "ISWC").Number).ToList();
                }

                if (submission.RequestType == RequestType.Label && submission.IsrcMatchedResult.Matches.Where((x => x.IswcStatus == 1)).Any())
                {
                    submission.Model.PreferredIswc = submission.IsrcMatchedResult.Matches.FirstOrDefault(x => x.IswcStatus == 1).Numbers.FirstOrDefault(x => x.Type == "ISWC").Number ?? string.Empty;
                }
                else
                {
                    submission.Model.PreferredIswc = submission.MatchedResult.Matches.FirstOrDefault().Numbers.FirstOrDefault(x => x.Type == "ISWC").Number ?? string.Empty;
                }

                submission.IswcModel.VerifiedSubmissions.FirstOrDefault().LinkedTo = submission.Model.PreferredIswc;
            }


            await workManager.AddIswcLinkedTo(submission);
            var newWorkInfoId = (await workManager.FindVerifiedAsync(submission.Model.WorkNumber)).WorkInfoID;

            return (await workManager.FindManyAsync(new long[] { newWorkInfoId }, detailLevel: submission.DetailLevel)).FirstOrDefault();

        }
    }
}
