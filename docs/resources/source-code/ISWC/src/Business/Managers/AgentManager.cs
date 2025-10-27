using AutoMapper;
using SpanishPoint.Azure.Iswc.Bdo.Agent;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.Repositories;
using SpanishPoint.Azure.Iswc.Data.Services.Notifications;
using SpanishPoint.Azure.Iswc.Data.Services.AgentRun;
using SpanishPoint.Azure.Iswc.Data.Services.AgentRun.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Framework.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SpanishPoint.Azure.Iswc.Business.Managers
{
    public interface IAgentManager
    {
        Task<bool> CheckAgentVersionAsync(string version);
        Task<AgentNotification> GetNotification(string agency, TransactionType transactionType, DateTime fromDate, int pageNumber, string continuationToken);
        Task<string?> UpdateAgentRun(AgentRun agentRun);
    }

    public class AgentManager : IAgentManager
    {
        private readonly IAgentRepository agentVersionRepository;
        private readonly IAgentRunService agentRunService;
        private readonly IMapper mapper;
        private readonly ICacheClient cacheClient;
        private readonly INotificationService notificationService;
        private readonly IMessagingManager messagingManager;
        private readonly ILogger<AgencyManager> logger;

        public AgentManager(IAgentRepository agentVersionRepository, IAgentRunService agentRunService, IMapper mapper, ICacheClient cacheClient,
            INotificationService notificationService, IMessagingManager messagingManager, ILogger<AgencyManager> logger)
        {
            this.agentVersionRepository = agentVersionRepository;
            this.agentRunService = agentRunService;
            this.mapper = mapper;
            this.cacheClient = cacheClient;
            this.notificationService = notificationService;
            this.messagingManager = messagingManager;
            this.logger = logger;
        }

        public async Task<bool> CheckAgentVersionAsync(string version)
        {
            var agentVersions = await cacheClient.GetOrCreateAsync(async () =>
            {
                return mapper.Map<IEnumerable<AgentVersion>>(await agentVersionRepository.FindManyAsync(x => true));
            });

            return agentVersions.Any() && agentVersions.Any(x => x.Version == version);
        }

        public async Task<string?> UpdateAgentRun(AgentRun agentRun)
        {
            AgentRuns run = mapper.Map<AgentRuns>(agentRun);
            run.RuntimeChecked = false;

            if (!string.IsNullOrEmpty(agentRun.RunId))
            {
                var existingRun = await agentRunService.GetAgentRun(agentRun.RunId);
                if (existingRun == null || existingRun.AgencyCode != run.AgencyCode)
                    return null;

                run.RunStartDate = existingRun.RunStartDate;
                run.RunEndDate = DateTime.UtcNow;
                run.RunCompleted = true;
            }
            else
            {
                run.RunId = Guid.NewGuid().ToString();
                run.RunStartDate = DateTime.UtcNow;
                run.RunCompleted = false;
                run.PartitionKey = run.RunId;
            }
            await agentRunService.UpsertAgentRun(run);
            return run.RunId;
        }

        public async Task<AgentNotification> GetNotification(string agency, TransactionType transactionType, DateTime fromDate, int pageNumber, string continuationToken)
        {
            try
            {
                var result = await notificationService.GetCsnNotifications(agency, transactionType, fromDate, pageNumber, continuationToken);

                return new AgentNotification()
                {
                    ContinuationToken = result.continuationToken,
                    CsnNotifications = mapper.Map<IEnumerable<CsnNotification>>(result.Item2)
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return new AgentNotification()
                {
                    Rejection = await messagingManager.GetRejectionMessage(Bdo.Rules.ErrorCode._175)
                };
            }

        }
    }
}