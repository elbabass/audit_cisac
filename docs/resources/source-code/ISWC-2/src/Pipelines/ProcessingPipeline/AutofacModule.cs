using System.Diagnostics.CodeAnalysis;
using Autofac;


namespace SpanishPoint.Azure.Iswc.Pipelines.ProcessingPipeline
{
    [ExcludeFromCodeCoverage]
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProcessingPipeline>().As<IProcessingPipeline>().InstancePerLifetimeScope();
        }
    }
}
