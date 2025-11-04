using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpanishPoint.Azure.Iswc.Api.Agency.Configuration.Swagger;
using SpanishPoint.Azure.Iswc.Api.Agency.Managers;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Framework.Extensions;

namespace SpanishPoint.Azure.Iswc.Api.Agency.V1.Controllers
{
    [ApiController]
    internal class WorkflowTasksController : BaseController, IISWC_Workflow_TasksController
    {
        private readonly IPipelineManager pipelineManager;
        private readonly IMapper mapper;
        private readonly ILogger<WorkflowTasksController> _logger;

        public WorkflowTasksController(IPipelineManager pipelineManager, IMapper mapper, ILogger<WorkflowTasksController> logger)
        {
            this.pipelineManager = pipelineManager;
            this.mapper = mapper;
            _logger = logger;
        }

        public async Task<ActionResult<ICollection<WorkflowTask>>> FindWorkflowTasksAsync(string agency,
            ShowWorkflows showWorkflows, WorkflowType? workflowType,
            IEnumerable<WorkflowStatus> status, int? startIndex, int? pageLength,
            string fromDate, string toDate, string iswc, string agencyWorkCodes, string originatingAgency)
        {
            var dateFormat = "yyyy-MM-dd";

            var response = await pipelineManager.RunPipelines(
                new Bdo.Submissions.Submission
                {
                    WorkflowSearchModel = new WorkflowSearchModel
                    {
                        Agency = agency,
                        StartIndex = startIndex,
                        PageLength = pageLength,
                        ShowWorkflows = (Bdo.Work.ShowWorkflows)showWorkflows,
                        Statuses = status.Any() ? mapper.Map<IEnumerable<InstanceStatus>>(status) : Enum.GetValues(typeof(InstanceStatus)).Cast<InstanceStatus>(),
                        FromDate = fromDate.TryParseDateTimeExact(dateFormat),
                        ToDate = toDate.TryParseDateTimeExact(dateFormat),
                        Iswc = iswc,
                        AgencyWorkCodes = !string.IsNullOrWhiteSpace(agencyWorkCodes) ? agencyWorkCodes.SplitWordOnSeparators() : Enumerable.Empty<string>(),
                        OriginatingAgency = originatingAgency,
                        WorkflowType = workflowType != null ? (Bdo.Work.WorkflowType?)Enum.Parse(typeof(Bdo.Work.WorkflowType), workflowType.ToString()!) : null
                    },
                    TransactionType = TransactionType.COR,
                    RequestType = RequestType.Agency
                });

            if (response.Rejection != null)
                return NotFound(response.Rejection);

            return Ok(mapper.Map<ICollection<WorkflowTask>>(response.Model.WorkflowTasks));
        }

        public async Task<ActionResult<ICollection<WorkflowTask>>> UpdateWorkflowTaskAsync(string agency, IEnumerable<WorkflowTaskUpdate> body)
        {
            try
            {
                var response = await pipelineManager.RunPipelines(
                    new Bdo.Submissions.Submission
                    {
                        Model = new SubmissionModel()
                        {
                            Agency = agency,
                            WorkflowTasks = mapper.Map<ICollection<Bdo.Work.WorkflowTask>>(body)
                        },
                        TransactionType = TransactionType.COA,
                        RequestType = RequestType.Agency
                    });

                if (response.Rejection != null)
                {
                    _logger.LogError(response.Rejection.Message, $"Work flow update has been rejected in {nameof(WorkflowTasksController)} - {nameof(UpdateWorkflowTaskAsync)}");
                    return NotFound(response.Rejection);
                }

                return MultiStatus(mapper.Map<ICollection<WorkflowTask>>(response.Model.WorkflowTasks));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, $"Failed to add update work flow tasks in {nameof(WorkflowTasksController)} - {nameof(UpdateWorkflowTaskAsync)}");
                return BadRequest();
            }
        }
    }
}
