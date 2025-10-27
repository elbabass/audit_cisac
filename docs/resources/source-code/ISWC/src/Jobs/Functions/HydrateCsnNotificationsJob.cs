using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using SpanishPoint.Azure.Iswc.Api.Agency.V1;
using SpanishPoint.Azure.Iswc.Data.Repositories;
using SpanishPoint.Azure.Iswc.Data.Services.CacheIswcs;
using SpanishPoint.Azure.Iswc.Data.Services.CacheIswcs.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Data.Services.Notifications;
using SpanishPoint.Azure.Iswc.Data.Services.Notifications.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CacheIswcs = SpanishPoint.Azure.Iswc.Data.Services.CacheIswcs.CosmosDb.Models;

namespace SpanishPoint.Azure.Iswc.Jobs.Functions
{
    public class HydrateCsnNotificationsJob
    {
        private readonly INotificationService notificationService;
        private readonly ICacheIswcService cacheIswcService;
        private readonly IAgencyRepository agencyRepository;
        private readonly HttpClient client;

        public HydrateCsnNotificationsJob(INotificationService notificationService, IHttpClientFactory httpClientFactory, ICacheIswcService cacheIswcService, IAgencyRepository agencyRepository)
        {
            this.notificationService = notificationService;
            this.cacheIswcService = cacheIswcService;
            this.agencyRepository = agencyRepository;
            client = httpClientFactory.CreateClient("IswcApiClient");
            client.Timeout = TimeSpan.FromMinutes(5);
        }

        [FunctionName("HydrateCsnNotificationsJob")]
        public async Task RunAsync([TimerTrigger("%hydrateCsnNotificationsNcrontab%")] TimerInfo myTimer)
        {
            IEnumerable<CsnNotifications> pendingHydration = Enumerable.Empty<CsnNotifications>();

            var agencies = await agencyRepository.FindManyAsyncOptimized(x => x.AgencyId != null);

            foreach (var agency in agencies)
            {
                var csnWatermark = await notificationService.GetCsnNotificationHighWatermark(agency.AgencyId);

                if (csnWatermark == null || string.IsNullOrWhiteSpace(csnWatermark.HighWatermark))
                {
                    csnWatermark = new CsnNotificationsHighWatermark
                    {
                        ID = agency.AgencyId,
                        PartitionKey = agency.AgencyId,
                        HighWatermark = $"{DateTime.Now.AddDays(-20):yyyyMMdd}"
                    };
                }
                var currentHighwatermark = csnWatermark.HighWatermark;

                do
                {
                    pendingHydration = await GetCsnNotifications(currentHighwatermark, csnWatermark.PartitionKey);

                    var tasks = pendingHydration.Batch(10).Select(async batch =>
                    {
                        IList<CsnNotifications> hydrated = new List<CsnNotifications>();
                        IList<CacheIswcsModel> hydrateCacheIswcs = new List<CacheIswcsModel>();

                        var searchIswcs = batch.Select(x => new { iswc = x.ToIswc })
                        .Union(batch.Where(x => !string.IsNullOrWhiteSpace(x.FromIswc))
                        .Select(x => new { iswc = x.FromIswc }));

                        var response = await (await client
                        .PostAsJsonAsync("iswc/searchByIswc/batch", searchIswcs))
                        .EnsureSuccessStatusCodeAsync();

                        var iswcs = JsonConvert.DeserializeObject<ICollection<ISWCMetadataBatch>>(await response.Content.ReadAsStringAsync());

                foreach (var item in batch)
                {
                    var iswc = item.ToIswc;
                    item.HttpResponse = JsonConvert.SerializeObject(iswcs.FirstOrDefault(x => x.SearchResults?.FirstOrDefault()?.Iswc == iswc)?.SearchResults?.FirstOrDefault());

                            // CosmosDb document size limit of 2 MB - error code 413
                            if (Encoding.ASCII.GetByteCount(JsonConvert.SerializeObject(item)) > Math.Pow(2, 21))
                                item.HttpResponse = "null";

                            hydrated.Add(item);

                    // CacheIswcs
                    var iswcMetadata = iswcs.FirstOrDefault(x => x.SearchResults?.FirstOrDefault()?.Iswc == iswc)?.SearchResults.FirstOrDefault();
                    
                    foreach(var cachedIswcModel in CacheIswcs(iswcMetadata))
                    {
                        hydrateCacheIswcs.Add(cachedIswcModel);
                    }

                        }

                        await notificationService.UpdateCsnNotifications(hydrated);
                        await cacheIswcService.UpdateCacheIswcs(hydrateCacheIswcs);
                    });

                    await Task.WhenAll(tasks);

                    csnWatermark.HighWatermark = currentHighwatermark;
                    await notificationService.UpdateCsnNotificationHighWatermark(csnWatermark);

                } while (pendingHydration.Any());                

                async Task<IEnumerable<CsnNotifications>> GetCsnNotifications(string highWatermark, string agency)
                {
                    do
                    {
                        var result = await notificationService.GetCsnNotifications(200, $"{currentHighwatermark}{agency}");

                        if (result == null || !result.Any())
                        {
                            var date = DateTime.ParseExact(currentHighwatermark, "yyyyMMdd", CultureInfo.InvariantCulture);

                            if (date == DateTime.Today)
                                return result;

                            else if (date <= DateTime.Today)
                            {
                                date = date.AddDays(1);
                                currentHighwatermark = date.ToString("yyyyMMdd");
                            }
                        }
                        else
                            return result;

                    } while (true);
                }
            }
        }

        public static List<CacheIswcsModel> CacheIswcs(ISWCMetadata iswcMetadata)
        {
            var cacheIswcs = new List<CacheIswcsModel>();

            if (iswcMetadata != null)
            {
                var interestedParties = new List<CacheIswcs.InterestedParty>();

                var OtherTitles = new List<CacheIswcs.Title>();

                var allRecordings = new List<Recording>();

                iswcMetadata?.OtherTitles.ForEach(x => OtherTitles.Add(new CacheIswcs.Title
                    {
                        Title1 = x.Title1,
                        Type = x.Type.ToString(),
                        AdditionalProperties = x.AdditionalProperties
                    }
                ));

                iswcMetadata?.InterestedParties.ForEach(x => interestedParties.Add(new CacheIswcs.InterestedParty
                    {
                        IPNameNumber = x.NameNumber,
                        LastName = x.LastName,
                        LegalEntityType = (Bdo.Ipi.LegalEntityType)x.LegalEntityType,
                        Name = x.Name,
                        Role = x.Role.ToString(),
                        Affiliation = x.Affiliation
                    }
                ));

                foreach (var work in iswcMetadata?.Works)
                {
                    var recordings = new List<Recording>();
                    foreach (var recording in work.AdditionalIdentifiers.Recordings)
                    {
                        var recordingToAdd = new Recording
                        {
                            Isrc = recording.Isrc,
                            RecordingTitle = recording.RecordingTitle,
                            SubTitle = recording.SubTitle,
                            LabelName = recording.LabelName,
                            ReleaseEmbargoDate = recording.ReleaseEmbargoDate,
                            Performers = recording.Performers != null && recording.Performers.Count() > 0 ? recording.Performers.Select(x => new CacheIswcs.Performer {
                                Isni = x.Isni,
                                Ipn = x.Ipn,
                                FirstName = x.FirstName,
                                LastName = x.LastName,
                                Designation = x.Designation?.ToDesignationString()
                            }).ToList() : null
                            
                        };

                        recordings.Add(recordingToAdd);
                        allRecordings.Add(recordingToAdd);
                    }

                    cacheIswcs.Add(new CacheIswcsModel
                    {
                        ID = string.Concat(work.Agency, work.Workcode),
                        PartitionKey = String.Concat(work.Agency + work.Workcode),
                        IswcMetadata = new CacheIswcMetadata
                        {
                            Iswc = iswcMetadata.Iswc,
                            IswcStatus = iswcMetadata.IswcStatus,
                            ParentISWC = iswcMetadata.ParentISWC,
                            OverallParentISWC = iswcMetadata.OverallParentISWC,
                            OtherTitles = OtherTitles,
                            Agency = iswcMetadata.Agency,
                            CreatedDate = iswcMetadata.CreatedDate,
                            InterestedParties = interestedParties,
                            Recordings = recordings,
                            LastModifiedDate = iswcMetadata.LastModifiedDate,
                            LinkedISWC = iswcMetadata.LinkedISWC,
                            OriginalTitle = iswcMetadata.OriginalTitle,
                        }
                    });
                }

                cacheIswcs.Add(new CacheIswcsModel
                {
                    ID = iswcMetadata.Iswc,
                    PartitionKey = iswcMetadata.Iswc,
                    IswcMetadata = new CacheIswcMetadata
                    {
                        Iswc = iswcMetadata.Iswc,
                        IswcStatus = iswcMetadata.IswcStatus,
                        ParentISWC = iswcMetadata.ParentISWC,
                        OverallParentISWC = iswcMetadata.OverallParentISWC,
                        OtherTitles = OtherTitles,
                        Agency = iswcMetadata.Agency,
                        CreatedDate = iswcMetadata.CreatedDate,
                        InterestedParties = interestedParties,
                        Recordings = allRecordings,
                        LastModifiedDate = iswcMetadata.LastModifiedDate,
                        LinkedISWC = iswcMetadata.LinkedISWC,
                        OriginalTitle = iswcMetadata.OriginalTitle,
                    }
                });
            }

            return cacheIswcs;
        }
    }
}
