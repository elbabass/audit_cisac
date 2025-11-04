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
    public class AS_01 : IProcessingSubComponent
    {
        private readonly IWorkManager workManager;
        private readonly IAS_10 as_10;
        private readonly IMapper mapper;

        public AS_01(IWorkManager workManager, IAS_10 as_10, IMapper mapper)
        {
            this.workManager = workManager;
            this.as_10 = as_10;
            this.mapper = mapper;
        }

        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR } ;

        public bool? IsEligible => true;

        public PreferedIswcType PreferedIswcType => PreferedIswcType.Existing;

        public string Identifier => nameof(AS_01);
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
                }

                return submission.IswcModel;
            }

            var workinfoId = await workManager.AddWorkInfoAsync(submission, getNewIswc: false);
            submission.IswcModel = (await workManager.FindManyAsync(new long[] { workinfoId }, detailLevel: submission.DetailLevel)).First();

            if (submission.Model?.AdditionalIdentifiers != null && submission.Model.AdditionalIdentifiers.Any())
            {
                foreach (var verifiedSubmission in submission.IswcModel.VerifiedSubmissions.Where(x => x.AdditionalIdentifiers != null && x.AdditionalIdentifiers.Any()))
                {
                    foreach (var additionalIdentifier in submission.Model.AdditionalIdentifiers.Where(x => x.NameNumber != null))
                    {
                        var existingAdditionalIdentifier = verifiedSubmission.AdditionalIdentifiers?.FirstOrDefault(x => x.WorkCode == additionalIdentifier.WorkCode);
                        if (existingAdditionalIdentifier == null)
                        {
                            continue;
                        }
                        else
                        {
                            existingAdditionalIdentifier.NameNumber = additionalIdentifier.NameNumber;
                        }
                    }
                }
            }

            return submission.IswcModel;
        }
    }
}
