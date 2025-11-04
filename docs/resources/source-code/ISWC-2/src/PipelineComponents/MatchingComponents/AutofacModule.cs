using System.Diagnostics.CodeAnalysis;
using Autofac;
using SpanishPoint.Azure.Iswc.PipelineComponents.MatchingComponent.InitialMatching;
using SpanishPoint.Azure.Iswc.PipelineComponents.MatchingComponent.PostMatching;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.MatchingComponent
{
    [ExcludeFromCodeCoverage]
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MatchingComponent>().As<IMatchingComponent>().InstancePerLifetimeScope();
            builder.RegisterType<MatchingForIswcEligibleSubmitter>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<MatchingForIswcNonEligibleSubmitter>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<MatchingForIswcEligibleExisting>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<MatchingForIswcNonEligibleExisting>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<RankingComponent>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<MatchingForIswcRelated>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<SearchComponent>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<MatchingAlterIps>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<MatchIsrcs>().AsSelf().InstancePerLifetimeScope();
        }
    }
}
