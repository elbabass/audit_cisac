using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpanishPoint.Azure.Iswc.Api.Agency.Configuration.Swagger;
using SpanishPoint.Azure.Iswc.Api.Agency.Managers;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using SpanishPoint.Azure.Iswc.Framework.Http.Requests;

namespace SpanishPoint.Azure.Iswc.Api.Agency.V1.Controllers
{
    [ApiController]
    internal class SubmissionController : BaseController, IISWC_SubmissionController
    {
        private readonly IPipelineManager pipelineManager;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly ILogger<SubmissionController> _logger;

        public SubmissionController(IPipelineManager pipelineManager, IMapper mapper, IHttpContextAccessor contextAccessor, ILogger<SubmissionController> logger)
        {
            this.pipelineManager = pipelineManager;
            this.mapper = mapper;
            this.contextAccessor = contextAccessor;
            _logger = logger;
        }

        public async Task<ActionResult<SubmissionResponse>> AddSubmissionAsync(Submission body)
        {
            var submission = new Bdo.Submissions.Submission
            {
                Model = mapper.Map<SubmissionModel>(body),
                TransactionType = TransactionType.CAR,
                DetailLevel = body?.DetailLevel != null ? StringExtensions.StringToEnum<Bdo.Submissions.DetailLevel>(body.DetailLevel.ToString()) : default,
                RequestType = RequestType.Agency
            };

            submission.IsPortalSubmissionFinalStep = !(submission.Model?.PreviewDisambiguation ?? false)
                && contextAccessor.GetRequestItem<bool>(isPublicRequestContextItemKey);
            submission.RequestSource = contextAccessor.GetRequestItem<RequestSource>(requestSourceContextItemKey);
            submission.AgentVersion = contextAccessor.GetRequestItem(agentVersionContextItemKey);

            var response = await pipelineManager.RunPipelines(submission);

            if (response.Rejection != null) return BadRequest(response.Rejection);

            var x = mapper.Map<SubmissionResponse>(response);
            return Created(string.Empty, x);
        }

        public async Task<ActionResult<ICollection<VerifiedSubmissionBatch>>> AddSubmissionBatchAsync(IEnumerable<SubmissionBatch> body)
        {
            try
            {
                var response = await pipelineManager.RunPipelines(body.Select((x, idx) => new Bdo.Submissions.Submission
                {
                    SubmissionId = x.SubmissionId ?? idx + 1,
                    Model = mapper.Map<SubmissionModel>(x.Submission),
                    DetailLevel = x.Submission?.DetailLevel != null ? StringExtensions.StringToEnum<Bdo.Submissions.DetailLevel>(x.Submission.DetailLevel.ToString()) : default,
                    TransactionType = TransactionType.CAR,
                    Rejection = mapper.Map<Bdo.Submissions.Rejection>(x.Rejection),
                    AgentVersion = contextAccessor.GetRequestItem(agentVersionContextItemKey),
                    RequestSource = contextAccessor.GetRequestItem<RequestSource>(requestSourceContextItemKey),
                    MultipleAgencyWorkCodes = mapper.Map<ICollection<Bdo.Work.MultipleAgencyWorkCodes>>(x.MultipleAgencyWorkCodes),
                    RequestType = RequestType.Agency
                }).ToList());


                if (response.Any(x => x.MultipleAgencyWorkCodes.Any()))
                    response = ConsolidateMultipleAgecnyWorkCodes(response);

                return MultiStatus(mapper.Map<ICollection<VerifiedSubmissionBatch>>(response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, $"Failed to add submission batch in {nameof(SubmissionController)} - {nameof(AddSubmissionBatchAsync)}");
                return BadRequest();
            }
        }

        public async Task<IActionResult> DeleteSubmissionAsync(string preferredIswc, string agency, string workcode, int sourceDb, string reasonCode)
        {
            var agencyModel = agency;
            var model = new SubmissionModel()
            {
                PreferredIswc = preferredIswc,
                Agency = agencyModel,
                WorkNumber = new Bdo.Work.WorkNumber() { Type = agencyModel, Number = workcode },
                SourceDb = sourceDb,
                ReasonCode = reasonCode
            };

            var response = await pipelineManager.RunPipelines(new Bdo.Submissions.Submission
            {
                Model = model,
                TransactionType = TransactionType.CDR,
                AgentVersion = contextAccessor.GetRequestItem(agentVersionContextItemKey),
                RequestSource = contextAccessor.GetRequestItem<RequestSource>(requestSourceContextItemKey),
                RequestType = RequestType.Agency
            });

            if (response.Rejection != null) return BadRequest(response.Rejection);
            if (response.IswcModel == null) return NotFound("Iswc not found");

            return NoContent();
        }

        public async Task<ActionResult<SubmissionResponse>> UpdateSubmissionAsync(string preferredIswc, Submission body)
        {
            var submission = new Bdo.Submissions.Submission
            {
                Model = mapper.Map<SubmissionModel>(body),
                TransactionType = TransactionType.CUR,
                AgentVersion = contextAccessor.GetRequestItem(agentVersionContextItemKey),
                RequestSource = contextAccessor.GetRequestItem<RequestSource>(requestSourceContextItemKey),
                DetailLevel = body?.DetailLevel != null ? StringExtensions.StringToEnum<Bdo.Submissions.DetailLevel>(body.DetailLevel.ToString()) : default,
                RequestType = RequestType.Agency
            };

            if (submission.Model != null && string.IsNullOrWhiteSpace(submission.Model.PreferredIswc))
                submission.Model.PreferredIswc = preferredIswc;

            submission.IsPortalSubmissionFinalStep = submission.Model?.PreviewDisambiguation ?? false ? false
                : contextAccessor.GetRequestItem<bool>(isPublicRequestContextItemKey);

            var response = await pipelineManager.RunPipelines(submission);

            if (response.Rejection != null) return BadRequest(response.Rejection);

            return Ok(mapper.Map<SubmissionResponse>(response));
        }

        public async Task<ActionResult<ICollection<VerifiedSubmissionBatch>>> UpdateSubmissionBatchAsync(IEnumerable<SubmissionBatch> body)
        {
            var response = await pipelineManager.RunPipelines(body.Select((x, idx) => new Bdo.Submissions.Submission
            {
                SubmissionId = x.SubmissionId ?? idx + 1,
                Model = mapper.Map<SubmissionModel>(x.Submission),
                TransactionType = TransactionType.CUR,
                RequestSource = contextAccessor.GetRequestItem<RequestSource>(requestSourceContextItemKey),
                AgentVersion = contextAccessor.GetRequestItem(agentVersionContextItemKey),
                Rejection = mapper.Map<Bdo.Submissions.Rejection>(x.Rejection),
                DetailLevel = x.Submission?.DetailLevel != null ? StringExtensions.StringToEnum<Bdo.Submissions.DetailLevel>(x.Submission.DetailLevel.ToString()) : default,
                MultipleAgencyWorkCodes = mapper.Map<ICollection<Bdo.Work.MultipleAgencyWorkCodes>>(x.MultipleAgencyWorkCodes),
                RequestType = RequestType.Agency
            }).ToList());

            if (response.Any(x => x.MultipleAgencyWorkCodes.Any()))
                response = ConsolidateMultipleAgecnyWorkCodes(response);

            var res =  MultiStatus(mapper.Map<ICollection<VerifiedSubmissionBatch>>(response));

            return res;
        }

        IEnumerable<Bdo.Submissions.Submission> ConsolidateMultipleAgecnyWorkCodes(IEnumerable<Bdo.Submissions.Submission> response)
        {
            var groupedChildren = response.Where(x => x.MultipleAgencyWorkCodesChild).GroupBy(x => x.SubmissionParentId);
            var removedExplodedSubs = new List<int>();
            foreach (var item in groupedChildren)
            {
                var parent = response.FirstOrDefault(x => x.SubmissionId == item.FirstOrDefault().SubmissionParentId);
                var multipleWorkCodesConsolidated = new List<Bdo.Work.MultipleAgencyWorkCodes>();
                foreach (var x in item)
                {
                    if (x != null)
                    {
                        var mac = new Bdo.Work.MultipleAgencyWorkCodes
                        {
                            Agency = x.Model.Agency,
                            WorkCode = x.Model.WorkNumber.Number

                        };

                        if (parent.Rejection == null)
                            mac.Rejection = x?.Rejection ?? null;
                        else
                            mac.Rejection = null;

                        multipleWorkCodesConsolidated.Add(mac);

                        if (x != null && !string.IsNullOrEmpty(x.SubmissionId.ToString()))
                            removedExplodedSubs.Add(x.SubmissionId);
                    }
                }

                response.FirstOrDefault(y => y.SubmissionId == item.FirstOrDefault().SubmissionParentId).MultipleAgencyWorkCodes = multipleWorkCodesConsolidated;
                response = response.Where(res => !removedExplodedSubs.Contains(res.SubmissionId));

            }

            return response;
        }
    }
}
