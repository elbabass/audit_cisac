using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpanishPoint.Azure.Iswc.Api.Publisher.Configuration.Swagger;
using SpanishPoint.Azure.Iswc.Api.Publisher.Managers;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using SpanishPoint.Azure.Iswc.Framework.Http.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Api.Publisher.V1.Controllers
{
    internal class SearchController : BaseController, IISWC_SearchController
    {
        private readonly IPipelineManager pipelineManager;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor contextAccessor;

        public SearchController(IPipelineManager pipelineManager, IMapper mapper, IHttpContextAccessor contextAccessor) : base()
        {
            this.pipelineManager = pipelineManager;
            this.mapper = mapper;
            this.contextAccessor = contextAccessor;
        }

        public async Task<ActionResult<IPContextSearchResponse>> ContextSearchAsync(IPContextSearchModel body)
        {
            var response = await pipelineManager.RunPipelines(
                new Bdo.Submissions.Submission
                {
                    Model = mapper.Map<SubmissionModel>(body),
                    SearchWorks = mapper.Map<ICollection<WorkModel>>(body.Works),
                    AdditionalIPNames = body?.AdditionalIPNames != null ? body.AdditionalIPNames.GetValueOrDefault(): false,
                    SocietyWorkCodes = body?.SocietyWorkCodes != null ? body.SocietyWorkCodes.GetValueOrDefault() : false,
                    TransactionType = TransactionType.CIQ,
                    IsPublicRequest = contextAccessor.GetRequestItem<bool>(isPublicRequestContextItemKey),
                    RequestSource = contextAccessor.GetRequestItem<RequestSource>(requestSourceContextItemKey),
                    DetailLevel = DetailLevel.Core,
                    RequestType = RequestType.Publisher
                });

            if (response.Rejection != null)
            {
                if (response.Rejection.Code == Bdo.Rules.ErrorCode._180) return NotFound("IP not found.");
                else return BadRequest(response.Rejection);
            }

            return Ok(mapper.Map<IPContextSearchResponse>(response));
        }

        public async Task<ActionResult<ICollection<IPContextSearchResponseBatch>>> ContextSearchBatchAsync(IEnumerable<IPContextSearchModel> body)
        {
            var response = await pipelineManager.RunPipelines(body.Select((x, idx) => new Bdo.Submissions.Submission
            {
                SubmissionId = idx + 1,
                Model = mapper.Map<SubmissionModel>(x),
                SearchWorks = mapper.Map<ICollection<WorkModel>>(x.Works),
                AdditionalIPNames = x?.AdditionalIPNames != null ? x.AdditionalIPNames.GetValueOrDefault() : false,
                SocietyWorkCodes = x?.SocietyWorkCodes != null ? x.SocietyWorkCodes.GetValueOrDefault() : false,
                TransactionType = TransactionType.CIQ,
                IsPublicRequest = contextAccessor.GetRequestItem<bool>(isPublicRequestContextItemKey),
                RequestSource = this.contextAccessor.GetRequestItem<RequestSource>(requestSourceContextItemKey),
                DetailLevel = DetailLevel.Core,
                RequestType = RequestType.Publisher
            }));


            return MultiStatus(mapper.Map<ICollection<IPContextSearchResponseBatch>>(response));
        }
    }
}
