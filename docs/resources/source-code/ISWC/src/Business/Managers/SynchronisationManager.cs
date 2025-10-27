using Microsoft.Extensions.Logging;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.Repositories;
using SpanishPoint.Azure.Iswc.Data.Services.IpiService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Business.Managers
{
    public interface ISynchronisationManager
    {
        Task IpiScheduledSync(int scheduledSyncBatchSize, ILogger logger);
        Task AddSpecifiedIps(List<string> baseNumbers, ILogger logger);
    }

    public class SynchronisationManager : ISynchronisationManager
    {
        private readonly IIpiService ipiService;
        private readonly IHighWatermarkRepository highWatermarkRepository;
        private readonly IAgencyManager agencyManager;
        private readonly IInterestedPartyManager interestedPartyManager;

        public SynchronisationManager(IIpiService ipiService, IInterestedPartyManager interestedPartyManager,
            IHighWatermarkRepository highWatermarkRepository, IAgencyManager agencyManager)
        {
            this.ipiService = ipiService;
            this.interestedPartyManager = interestedPartyManager;
            this.highWatermarkRepository = highWatermarkRepository;
            this.agencyManager = agencyManager;
        }

        public async Task IpiScheduledSync(int scheduledSyncBatchSize, ILogger logger)
        {
            var currentIpiHighWatermark = await GetLatestHighWatermark();

            if (currentIpiHighWatermark.Value == null)
                throw new ArgumentException($"[ {DateTime.UtcNow} > : ERROR] Error retrieving high water mark date");

            var isfinished = false;
            var totalProcessedIPCount = 0;

            while (!isfinished)
            {
                isfinished = true;
                var batchProcessedIPCount = 0;

                try
                {
                    await foreach (var (interestedParty, watermark) in ipiService.GetChangedIpsSinceDate(currentIpiHighWatermark.Value, scheduledSyncBatchSize))
                    {
                        if (interestedParty != null)
                        {
                            batchProcessedIPCount += 1;
                            await AddInterestedParty(interestedParty);

                            if (!string.IsNullOrEmpty(watermark.ToString()))
                            {
                                currentIpiHighWatermark.Value = watermark;
                                isfinished = false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Response status code does not indicate success: 429 (429).")
                    {
                        logger.LogWarning($"[ {DateTime.UtcNow} > : WARNING] " + ex.Message);
                        isfinished = false;
                        await Task.Delay(TimeSpan.FromMinutes(3));
                    }
                    else
                        throw;
                }

                totalProcessedIPCount += batchProcessedIPCount;

                if (batchProcessedIPCount > 0)
                {
                    logger.LogInformation($"[ {DateTime.UtcNow} > : INFO] Successfully processed batch of {batchProcessedIPCount} IPs");
                }

                await UpdateWatermark();
            }

            logger.LogInformation($"[ {DateTime.UtcNow} > : INFO] All updates processed ({totalProcessedIPCount} IPs) and set new High Water mark to {currentIpiHighWatermark.Value}");

            async Task UpdateWatermark()
            {
                await highWatermarkRepository.UpdateAsync(currentIpiHighWatermark);
                await highWatermarkRepository.UnitOfWork.Save();
            }

            async Task<HighWatermark> GetLatestHighWatermark()
            {
                return await highWatermarkRepository.FindAsync(x => x.Name == "IpiHighWatermark");
            }
        }

        public async Task AddSpecifiedIps(List<string> baseNumbers, ILogger logger)
        {
            var isfinished = false;
            var totalProcessedIPCount = 0;

            while (!isfinished)
            {
                isfinished = true;
                var batchProcessedIPCount = 0;

                try
                {
                    await foreach (var interestedParty in ipiService.GetIps(baseNumbers))
                    {
                        if (interestedParty != null)
                        {
                            batchProcessedIPCount += 1;
                            await AddInterestedParty(interestedParty);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Response status code does not indicate success: 429 (429).")
                    {
                        logger.LogWarning($"[ {DateTime.UtcNow} > : WARNING] " + ex.Message);
                        isfinished = false;
                        await Task.Delay(TimeSpan.FromMinutes(3));
                    }
                    else
                        throw;
                }

                totalProcessedIPCount += batchProcessedIPCount;

                if (batchProcessedIPCount > 0)
                {
                    logger.LogInformation($"[ {DateTime.UtcNow} > : INFO] Successfully processed batch of {batchProcessedIPCount} IPs");
                }
            }

            logger.LogInformation($"[ {DateTime.UtcNow} > : INFO] All updates processed ({totalProcessedIPCount} IPs)");
        }

        private async Task AddInterestedParty(InterestedParty interestedParty)
        {
            if (!await agencyManager.Exists(interestedParty.AgencyId))
                await agencyManager.AddAgency(interestedParty.AgencyId);

            if (interestedParty.Agreement != null)
            {
                foreach (var agency in interestedParty.Agreement.Select(x => x.AgencyId).Distinct())
                {
                    if (!await agencyManager.Exists(agency))
                        await agencyManager.AddAgency(agency);
                }
            }

            await interestedPartyManager.UpsertInterestedPartyAsync(interestedParty);
        }
    }
}