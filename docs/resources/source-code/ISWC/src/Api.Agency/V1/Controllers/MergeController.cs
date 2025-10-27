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
    internal class MergeController : BaseController, IISWC_MergeController
    {
        private readonly IPipelineManager pipelineManager;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor contextAccessor;

        public MergeController(IPipelineManager pipelineManager, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            this.pipelineManager = pipelineManager;
            this.mapper = mapper;
            this.contextAccessor = contextAccessor;
        }

        public async Task<IActionResult> DemergeISWCMetadataAsync(string preferredIswc, string agency, string workcode)
        {
            var agencyModel = agency;
            var model = new SubmissionModel()
            {
                PreferredIswc = preferredIswc,
                Agency = agencyModel,
                WorkNumber = new Bdo.Work.WorkNumber() { Type = agencyModel, Number = workcode },
                SourceDb = int.TryParse(agency, out int i) ? i : 0,
                ReasonCode = "DemergeIswcMetadata"
            };

            var response = await pipelineManager.RunPipelines(new Bdo.Submissions.Submission
            {
                RequestSource = contextAccessor.GetRequestItem<RequestSource>(requestSourceContextItemKey),
                Model = model,
                TransactionType = TransactionType.DMR,
                RequestType = RequestType.Agency
            });

            if (response.Rejection != null) return BadRequest(response.Rejection);
            if (response.IswcModel == null) return NotFound("Iswc not found");

            return NoContent();
        }

        public async Task<IActionResult> MergeISWCMetadataAsync(string preferredIswc, string agency, Body body)
        {
            var model = new SubmissionModel()
            {
                PreferredIswc = preferredIswc,
                IswcsToMerge = body.Iswcs ?? new List<string>(),
                WorkNumbersToMerge = body.AgencyWorks?.Select(aw => mapper.Map<Bdo.Work.WorkNumber>(aw)).ToList() ?? new List<Bdo.Work.WorkNumber>(),
                Agency = agency,
                SourceDb = int.TryParse(agency, out int i) ? i : default
            };

            var response = await pipelineManager.RunPipelines(new Bdo.Submissions.Submission
            {
                RequestSource = contextAccessor.GetRequestItem<RequestSource>(requestSourceContextItemKey),
                Model = model,
                TransactionType = TransactionType.MER,
                RequestType = RequestType.Agency
            });

            if (response.Rejection != null) return BadRequest(response.Rejection);
            if (response.IswcModel == null) return NotFound("Iswc not found");

            return NoContent();
        }
    }
}
