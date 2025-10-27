using System.Diagnostics.CodeAnalysis;
using Autofac;
using SpanishPoint.Azure.Iswc.Business.Managers;

namespace SpanishPoint.Azure.Iswc.Business
{
    [ExcludeFromCodeCoverage]
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RulesManager>().As<IRulesManager>().InstancePerLifetimeScope();
            builder.RegisterType<WorkManager>().As<IWorkManager>().InstancePerLifetimeScope();
            builder.RegisterType<AdditionalIdentifierManager>().As<IAdditionalIdentifierManager>().InstancePerLifetimeScope();
            builder.RegisterType<NumberTypeManager>().As<INumberTypeManager>().InstancePerLifetimeScope();
            builder.RegisterType<AgencyManager>().As<IAgencyManager>().InstancePerLifetimeScope();
            builder.RegisterType<AgreementManager>().As<IAgreementManager>().InstancePerLifetimeScope();
            builder.RegisterType<SubmissionSourceManager>().As<ISubmissionSourceManager>().InstancePerLifetimeScope();
            builder.RegisterType<InterestedPartyManager>().As<IInterestedPartyManager>().InstancePerLifetimeScope();
            builder.RegisterType<MessagingManager>().As<IMessagingManager>().InstancePerLifetimeScope();
            builder.RegisterType<StandardizedTitleManager>().As<IStandardizedTitleManager>().InstancePerLifetimeScope();
            builder.RegisterType<MatchingManager>().As<IMatchingManager>().InstancePerLifetimeScope();
            builder.RegisterType<AuditManager>().As<IAuditManager>().InstancePerLifetimeScope();
            builder.RegisterType<LookupManager>().As<ILookupManager>().InstancePerLifetimeScope();
            builder.RegisterType<WorkflowManager>().As<IWorkflowManager>().InstancePerLifetimeScope();
            builder.RegisterType<SynchronisationManager>().As<ISynchronisationManager>().InstancePerLifetimeScope();
            builder.RegisterType<ReportManager>().As<IReportManager>().InstancePerLifetimeScope();
            builder.RegisterType<UpdateWorkflowsManager>().As<IUpdateWorkflowsManager>().InstancePerLifetimeScope();
            builder.RegisterType<DatabaseMaintenanceManager>().As<IDatabaseMaintenanceManager>().InstancePerLifetimeScope();
            builder.RegisterType<UserManager>().As<IUserManger>().InstancePerLifetimeScope();;
            builder.RegisterType<LinkedToManager>().As<ILinkedToManager>().InstancePerLifetimeScope();
            builder.RegisterType<AgentManager>().As<IAgentManager>().InstancePerLifetimeScope();
            builder.RegisterType<PortalMessageManager>().As<IPortalMessageManager>().InstancePerLifetimeScope();
            builder.RegisterType<AgentManager>().As<IAgentManager>().InstancePerLifetimeScope();
        }
    }
}
