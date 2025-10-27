using System.Diagnostics.CodeAnalysis;
using Autofac;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator
{
    [ExcludeFromCodeCoverage]
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PostMatchingValidator>().As<IPostMatchingValidator>().InstancePerLifetimeScope();
        }
    }
}
