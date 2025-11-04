using System.Diagnostics.CodeAnalysis;
using Autofac;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.MetadataStandardizationValidator
{
    [ExcludeFromCodeCoverage]
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MetadataStandardizationValidator>().As<IMetadataStandardizationValidator>().InstancePerLifetimeScope();
        }
    }
}
