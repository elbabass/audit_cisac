using System.Diagnostics.CodeAnalysis;
using Autofac;


namespace SpanishPoint.Azure.Iswc.Pipelines.MatchingPipeline
{
    [ExcludeFromCodeCoverage]
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MatchingPipeline>().As<IMatchingPipeline>().InstancePerLifetimeScope();
        }
    }
}
