using System.Diagnostics.CodeAnalysis;
using Autofac;

namespace SpanishPoint.Azure.Iswc.Pipelines.ValidationPipeline
{
    [ExcludeFromCodeCoverage]
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ValidationPipeline>().As<IValidationPipeline>().InstancePerLifetimeScope();
        }
    }
}
