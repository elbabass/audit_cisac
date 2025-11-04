using SpanishPoint.Azure.Iswc.Data.Services.AgentRun.CosmosDb.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Data.Services.AgentRun
{
	public interface IAgentRunService
	{
		Task<AgentRuns> GetAgentRun(string runId);
		Task<IEnumerable<AgentRuns>> GetAgentRuns(int count);
		Task UpsertAgentRun(AgentRuns agentRun);
		Task UpdateAgentRuns(IEnumerable<AgentRuns> agentRuns);
	}
}
