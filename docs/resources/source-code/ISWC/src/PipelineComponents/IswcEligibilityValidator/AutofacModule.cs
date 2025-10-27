using System.Diagnostics.CodeAnalysis;
using Autofac;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.IswcEligibilityValidator
{
    [ExcludeFromCodeCoverage]
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<IswcEligibilityValidator>().As<IIswcEligibilityValidator>().InstancePerLifetimeScope();
        }
    }
}
