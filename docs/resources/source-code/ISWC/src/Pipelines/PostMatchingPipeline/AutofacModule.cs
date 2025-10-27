using System.Diagnostics.CodeAnalysis;
using Autofac;

namespace SpanishPoint.Azure.Iswc.Pipelines.PostMatchingPipeline
{
    [ExcludeFromCodeCoverage]
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PostMatchingPipeline>().As<IPostMatchingPipeline>().InstancePerLifetimeScope();
        }
    }
}
