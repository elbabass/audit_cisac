using SpanishPoint.Azure.Iswc.Data.Services.AgentRun.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Framework.CosmosDb.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Data.Services.AgentRun.CosmosDb
{
	public class CosmosDbAgentRunService : IAgentRunService
	{
		private readonly ICosmosDbRepository<AgentRuns> repository;

		public CosmosDbAgentRunService(ICosmosDbRepository<AgentRuns> repository)
		{
			this.repository = repository;
		}

		public async Task<AgentRuns> GetAgentRun(string runId)
		{
			return await repository.GetItemAsync(runId, runId);
		}

		public Task<IEnumerable<AgentRuns>> GetAgentRuns(int count)
		{
			return repository.GetItemsAsync(x => !x.RuntimeChecked, count);
		}

		public async Task UpsertAgentRun(AgentRuns agentRun)
		{
			await repository.UpsertItemAsync(agentRun);
		}

		public async Task UpdateAgentRuns(IEnumerable<AgentRuns> agentRuns)
		{
			foreach(var agentRun in agentRuns)
			{
				await repository.UpsertItemAsync(agentRun);
			}
		}
	}
}
