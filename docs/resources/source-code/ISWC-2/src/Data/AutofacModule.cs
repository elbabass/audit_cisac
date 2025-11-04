using System.Diagnostics.CodeAnalysis;
using Autofac;
using SpanishPoint.Azure.Iswc.Data.Repositories;
using SpanishPoint.Azure.Iswc.Data.Services.AgentRun;
using SpanishPoint.Azure.Iswc.Data.Services.AgentRun.CosmosDb;
using SpanishPoint.Azure.Iswc.Data.Services.AgentRun.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Data.Services.Audit;
using SpanishPoint.Azure.Iswc.Data.Services.Audit.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Data.Services.CacheIswcs;
using SpanishPoint.Azure.Iswc.Data.Services.CacheIswcs.CosmosDb;
using SpanishPoint.Azure.Iswc.Data.Services.CacheIswcs.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Data.Services.Checksum;
using SpanishPoint.Azure.Iswc.Data.Services.Checksum.CosmosDb;
using SpanishPoint.Azure.Iswc.Data.Services.Checksum.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Data.Services.CosmosDb;
using SpanishPoint.Azure.Iswc.Data.Services.IpiService;
using SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi;
using SpanishPoint.Azure.Iswc.Data.Services.IswcService;
using SpanishPoint.Azure.Iswc.Data.Services.IswcService.CosmosDb;
using SpanishPoint.Azure.Iswc.Data.Services.IswcService.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Data.Services.Matching;
using SpanishPoint.Azure.Iswc.Data.Services.Matching.MatchingEngine;
using SpanishPoint.Azure.Iswc.Data.Services.Notifications;
using SpanishPoint.Azure.Iswc.Data.Services.Notifications.ComosDb;
using SpanishPoint.Azure.Iswc.Data.Services.Notifications.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Data.Services.ReportingService;
using SpanishPoint.Azure.Iswc.Data.Services.ReportingService.Parquet;
using SpanishPoint.Azure.Iswc.Data.Services.Rules;
using SpanishPoint.Azure.Iswc.Data.Services.Search;
using SpanishPoint.Azure.Iswc.Data.Services.Search.Azure;
using SpanishPoint.Azure.Iswc.Data.Services.UpdateWorkflowHistory;
using SpanishPoint.Azure.Iswc.Data.Services.UpdateWorkflowHistory.CosmosDb;
using SpanishPoint.Azure.Iswc.Data.Services.UpdateWorkflowHistory.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Framework.CosmosDb.Repositories;
using SpanishPoint.Azure.Iswc.Framework.Databricks;
using SpanishPoint.Azure.Iswc.Framework.Search;
using SpanishPoint.Azure.Iswc.Framework.Search.Azure;

namespace SpanishPoint.Azure.Iswc.Data
{
    [ExcludeFromCodeCoverage]
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AdditionalIdentifierRepository>().As<IAdditionalIdentifierRepository>().InstancePerLifetimeScope(); ;
            builder.RegisterType<AgencyRepository>().As<IAgencyRepository>().InstancePerLifetimeScope();
            builder.RegisterType<AgreementRepository>().As<IAgreementRepository>().InstancePerLifetimeScope();
            builder.RegisterType<AgreementRepository>().As<IAgreementRepository>().InstancePerLifetimeScope();
            builder.RegisterType<HighWatermarkRepository>().As<IHighWatermarkRepository>().InstancePerLifetimeScope();
            builder.RegisterType<InstrumentationRepository>().As<IInstrumentationRepository>().InstancePerLifetimeScope();
            builder.RegisterType<InterestedPartyRepository>().As<IInterestedPartyRepository>().InstancePerLifetimeScope();
            builder.RegisterType<IswcLinkedToRepository>().As<IIswcLinkedToRepository>().InstancePerLifetimeScope();
            builder.RegisterType<IswcRepository>().As<IIswcRepository>().InstancePerLifetimeScope();
            builder.RegisterType<LookupRepository>().As<ILookupRepository>().InstancePerLifetimeScope();
            builder.RegisterType<MatchingEngineMatchingService>().As<IMatchingService>().InstancePerLifetimeScope();
            builder.RegisterType<MergeRequestRepository>().As<IMergeRequestRepository>().InstancePerLifetimeScope();
            builder.RegisterType<NameRepository>().As<INameRepository>().InstancePerLifetimeScope();
            builder.RegisterType<RecordingRepository>().As<IRecordingRepository>().InstancePerLifetimeScope();
            builder.RegisterType<RulesService>().As<IRulesService>().InstancePerLifetimeScope();
            builder.RegisterType<StandardizedTitleRepository>().As<IStandardizedTitleRepository>().InstancePerLifetimeScope();
            builder.RegisterType<SubmissionSourceRepository>().As<ISubmissionSourceRepository>().InstancePerLifetimeScope();
            builder.RegisterType<WorkRepository>().As<IWorkRepository>().InstancePerLifetimeScope();
            builder.RegisterType<WorkflowInstanceRepository>().As<IWorkflowInstanceRepository>().InstancePerLifetimeScope();
            builder.RegisterType<WorkflowRepository>().As<IWorkflowRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ParquetReportingService>().As<IReportingService>().InstancePerLifetimeScope();
            builder.RegisterType<PerformerRepository>().As<IPerformerRepository>().InstancePerLifetimeScope();
            builder.RegisterType<PublisherCodeRepository>().As<IPublisherCodeRespository>().InstancePerLifetimeScope();
            builder.RegisterType<NumberTypeRepository>().As<INumberTypeRepository>().InstancePerLifetimeScope();
            builder.RegisterType<DatabaseMaintenanceRepository>().As<IDatabaseMaintenanceRepository>().InstancePerLifetimeScope();
            builder.RegisterType<WebUserRepository>().As<IWebUserRepository>().InstancePerLifetimeScope();
            builder.RegisterType<AgentRepository>().As<IAgentRepository>().InstancePerLifetimeScope();
            builder.RegisterType<MessageRepository>().As<IMessageRepository>().InstancePerLifetimeScope();

            builder.RegisterType<CosmosDbAuditService>().As<IAuditService>().InstancePerLifetimeScope();
            builder.RegisterType<CosmosDbIswcService>().As<IIswcService>().InstancePerLifetimeScope();
            builder.RegisterType<CosmosDbNotificationService>().As<INotificationService>().InstancePerLifetimeScope();
            builder.RegisterType<CosmosDbRepository<AuditModel>>().As<ICosmosDbRepository<AuditModel>>().InstancePerLifetimeScope();
            builder.RegisterType<CosmosDbRepository<AuditRequestModel>>().As<ICosmosDbRepository<AuditRequestModel>>().InstancePerLifetimeScope();
            builder.RegisterType<CosmosDbRepository<FileAuditModel>>().As<ICosmosDbRepository<FileAuditModel>>().InstancePerLifetimeScope();
            builder.RegisterType<CosmosDbRepository<CsnNotifications>>().As<ICosmosDbRepository<CsnNotifications>>().InstancePerLifetimeScope();
            builder.RegisterType<CosmosDbRepository<CsnNotificationsHighWatermark>>().As<ICosmosDbRepository<CsnNotificationsHighWatermark>>().InstancePerLifetimeScope();
            builder.RegisterType<CosmosDbRepository<IswcModel>>().As<ICosmosDbRepository<IswcModel>>().InstancePerLifetimeScope();
            builder.RegisterType<CosmosDbRepository<UpdateWorkflowHistoryModel>>().As<ICosmosDbRepository<UpdateWorkflowHistoryModel>>().InstancePerLifetimeScope();
            builder.RegisterType<CosmosDbRepository<AgencyStatisticsModel>>().As<ICosmosDbRepository<AgencyStatisticsModel>>().InstancePerLifetimeScope();
            builder.RegisterType<CosmosDbUpdateWorkflowHistoryService>().As<IUpdateWorkflowHistoryService>().InstancePerLifetimeScope();
            builder.RegisterType<CosmosDbCacheIswcService>().As<ICacheIswcService>().InstancePerLifetimeScope();
            builder.RegisterType<CosmosDbRepository<CacheIswcsModel>>().As<ICosmosDbRepository<CacheIswcsModel>>().InstancePerLifetimeScope();
            builder.RegisterType<SuisaIpiService>().As<IIpiService>().InstancePerLifetimeScope();            
            builder.RegisterType<DatabricksClient>().As<IDatabricksClient>().InstancePerLifetimeScope();
            builder.RegisterType<CosmosDbChecksumService>().As<IChecksumService>().InstancePerLifetimeScope();
            builder.RegisterType<CosmosDbRepository<SubmissionChecksums>>().As<ICosmosDbRepository<SubmissionChecksums>>().InstancePerLifetimeScope();
            builder.RegisterType<CosmosDbAgentRunService>().As<IAgentRunService>().InstancePerLifetimeScope();
            builder.RegisterType<CosmosDbRepository<AgentRuns>>().As<ICosmosDbRepository<AgentRuns>>().InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(AzureSearchClient<>)).As(typeof(ISearchClient<>)).SingleInstance();
            builder.RegisterType<AzureSearchService>().As<ISearchService>().InstancePerLifetimeScope();
        }
    }
}
