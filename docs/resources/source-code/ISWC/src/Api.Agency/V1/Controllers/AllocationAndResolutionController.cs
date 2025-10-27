using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpanishPoint.Azure.Iswc.Api.Agency.Configuration.Swagger;
using SpanishPoint.Azure.Iswc.Api.Agency.Managers;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using SpanishPoint.Azure.Iswc.Framework.Http.Requests;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Api.Agency.V1.Controllers
{
    internal class AllocationAndResolutionController : BaseController, IISWC_Allocation_and_ResolutionController
    {
        private readonly IPipelineManager pipelineManager;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor contextAccessor;

        public AllocationAndResolutionController(IPipelineManager pipelineManager, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            this.pipelineManager = pipelineManager;
            this.mapper = mapper;
            this.contextAccessor = contextAccessor;
        }

        public async Task<ActionResult<ICollection<VerifiedSubmissionBatch>>> AddAllocationBatchAsync(IEnumerable<SubmissionBatch> body)
        {
            var response = await pipelineManager.RunPipelines(body.Select((x, idx) => new Bdo.Submissions.Submission
            {
                SubmissionId = x.SubmissionId ?? idx + 1,
                Model = mapper.Map<SubmissionModel>(x.Submission),
                TransactionType = TransactionType.CAR,
                TransactionSource = Bdo.Reports.TransactionSource.Publisher,
                Rejection = mapper.Map<Bdo.Submissions.Rejection>(x.Rejection),
                RequestSource = contextAccessor.GetRequestItem<RequestSource>(requestSourceContextItemKey),
                RequestType = RequestType.Publisher
            }).ToList());

            return MultiStatus(mapper.Map<ICollection<VerifiedSubmissionBatch>>(response));
        }

        public async Task<ActionResult<ICollection<VerifiedSubmissionBatch>>> AddResolutionBatchAsync(IEnumerable<SubmissionBatch> body)
        {
            var response = await pipelineManager.RunPipelines(body.Select((x, idx) => new Bdo.Submissions.Submission
            {
                SubmissionId = x.SubmissionId ?? idx + 1,
                Model = mapper.Map<SubmissionModel>(x.Submission),
                IsEligible = false,
                TransactionSource = Bdo.Reports.TransactionSource.Publisher,
                IsPortalSubmissionFinalStep = true,
                TransactionType = TransactionType.FSQ,
                Rejection = mapper.Map<Bdo.Submissions.Rejection>(x.Rejection),
                RequestSource = contextAccessor.GetRequestItem<RequestSource>(requestSourceContextItemKey),
                RequestType = RequestType.Publisher
            }).ToList());

            return MultiStatus(mapper.Map<ICollection<VerifiedSubmissionBatch>>(response));
        }
    }
}
