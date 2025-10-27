using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SpanishPoint.Azure.Iswc.Data.Services.AgentRun;
using SpanishPoint.Azure.Iswc.Data.Services.AgentRun.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Framework.Mail;
using SpanishPoint.Azure.Iswc.Framework.Mail.Models;
using SpanishPoint.Azure.Iswc.Jobs.Functions;

namespace SpanishPoint.Azure.Iswc.Jobs.Functions
{
	public class CheckAgentRuns
	{

		private readonly IAgentRunService agentRunService;
		private readonly HttpClient httpClient;
		private readonly IConfiguration configuration;

		public CheckAgentRuns(IAgentRunService agentRunService, IHttpClientFactory httpClientFactory, IConfiguration configuration)
		{
			this.agentRunService = agentRunService;
			this.httpClient = httpClientFactory.CreateClient("MailClient");
			this.configuration = configuration;
		}

		[FunctionName("CheckAgentRuns")]
		public async Task Run([TimerTrigger("%checkAgentRunsNcrontab%")] TimerInfo myTimer, ILogger log)
		{
			if (bool.Parse(configuration["RunCheckAgentRuns"]))
			{
				var agentRuns = (await agentRunService.GetAgentRuns(50)).ToList();
				IList<AgentRuns> runs = new List<AgentRuns>();
				var mailModel = new MailMessageModel { Recipient = configuration["AgentRunsEmailRecipient"] };

				string agentRunsAlertTimeframe = configuration["AgentRunsAlertTimeframe"];
				TimeSpan timeSpan = TimeSpan.ParseExact(agentRunsAlertTimeframe, "g", null);
				var now = DateTime.UtcNow;

				for (int i = agentRuns.Count - 1; i >= 0; i--)
				{
					if (agentRuns[i].RunStartDate < now.AddHours(-timeSpan.TotalHours))
					{
						mailModel.AgentRunInformation = new AgentRunInformation { RunId = agentRuns[i].RunId, AgencyCode = agentRuns[i].AgencyCode };
						if (agentRuns[i].RunEndDate == null)
						{
							await httpClient.PostAsJsonAsync("", mailModel);
						}
						else
						{
							var differenceInHours = (agentRuns[i].RunEndDate - agentRuns[i].RunStartDate)?.TotalHours;
							if (differenceInHours > timeSpan.TotalHours)
							{
								await httpClient.PostAsJsonAsync("", mailModel);
							}
						}
						agentRuns[i].RuntimeChecked = true;
						runs.Add(agentRuns[i]);
					}

					agentRuns.Remove(agentRuns[i]);
				}

				await agentRunService.UpdateAgentRuns(runs);
			}
			
		}
	}
}
