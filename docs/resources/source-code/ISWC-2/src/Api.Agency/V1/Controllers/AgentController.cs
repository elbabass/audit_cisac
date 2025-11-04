using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SpanishPoint.Azure.Iswc.Api.Agency.Configuration.Swagger;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Api.Agency.V1.Controllers
{
    internal class AgentController : BaseController, IISWC_AgentController
    {
        private readonly IAgentManager agentManager;
        private readonly IMapper mapper;

        public AgentController(IAgentManager agentManager, IMapper mapper)
        {
            this.agentManager = agentManager;
            this.mapper = mapper;
        }

        public async Task<ActionResult<AgentNotificationResponse>> GetNotificationsAsync(string agency, CsnTransactionType csnTransactionType, string fromDate, int pageLength, string continuationToken)
        {
            var transactionType = StringExtensions.StringToEnum<TransactionType>(csnTransactionType.ToString());
            DateTime.TryParse(fromDate, out var dateFrom);

            var response = await agentManager.GetNotification(agency, transactionType, dateFrom, pageLength, continuationToken);

            if (response?.Rejection != null) return BadRequest(response.Rejection);

            return Ok(mapper.Map<AgentNotificationResponse>(response));
        }

        public async Task<ActionResult<Response>> UpdateAgentRunAsync(AgentRun body)
        {
            var runId = await agentManager.UpdateAgentRun(mapper.Map<Bdo.Agent.AgentRun>(body));
            if (!string.IsNullOrEmpty(body.RunId) && string.IsNullOrEmpty(runId))
                return NotFound("Agent Run not found");

            return Ok(new Response { RunId = runId });
        }
    }
}
