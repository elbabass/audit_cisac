using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpanishPoint.Azure.Iswc.Api.Label.Configuration.Swagger;
using SpanishPoint.Azure.Iswc.Api.Label.Managers;
using SpanishPoint.Azure.Iswc.Api.Label.V1;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using SpanishPoint.Azure.Iswc.Framework.Http.Requests;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Api.Label.V1.Controllers
{
    internal class LabelSubmissionController : BaseController, IISWC_Label_SubmissionController
    {
        private readonly IPipelineManager pipelineManager;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor contextAccessor;

        public LabelSubmissionController(IPipelineManager pipelineManager, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            this.pipelineManager = pipelineManager;
            this.mapper = mapper;
            this.contextAccessor = contextAccessor;
        }

        public async Task<ActionResult<ICollection<VerifiedSubmissionBatch>>> AddLabelSubmissionBatchAsync(IEnumerable<SubmissionBatch> body)
        {
            var response = await pipelineManager.RunPipelines(body.Select((x, idx) => new Bdo.Submissions.Submission
            {
                SubmissionId = x.SubmissionId ?? idx + 1,
                Model = mapper.Map<SubmissionModel>(x.Submission),
                TransactionType = TransactionType.CAR,
                Rejection = mapper.Map<Bdo.Submissions.Rejection>(x.Rejection),
                RequestSource = contextAccessor.GetRequestItem<RequestSource>(requestSourceContextItemKey),
                RequestType = RequestType.Label
            }).ToList());

            return MultiStatus(mapper.Map<ICollection<VerifiedSubmissionBatch>>(response));
        }
    }
}
