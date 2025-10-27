using System.Diagnostics.CodeAnalysis;
using Autofac;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator
{
    [ExcludeFromCodeCoverage]
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<StaticDataValidator>().As<IStaticDataValidator>().InstancePerLifetimeScope();
        }
    }
}
