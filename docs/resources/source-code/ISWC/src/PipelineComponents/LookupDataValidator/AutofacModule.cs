using System.Diagnostics.CodeAnalysis;
using Autofac;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.LookupDataValidator
{
    [ExcludeFromCodeCoverage]
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LookupDataValidator>().As<ILookupDataValidator>().InstancePerLifetimeScope();
        }
    }
}
