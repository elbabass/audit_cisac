using System.Diagnostics.CodeAnalysis;
using Autofac;
using SpanishPoint.Azure.Iswc.PipelineComponents.ProcessingComponent.Processing;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.ProcessingComponent
{
    [ExcludeFromCodeCoverage]
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AS_01>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<AS_02>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<AS_03>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<AS_04>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<AS_05>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<AS_08>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<AS_09>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<AS_10>().As<IAS_10>().InstancePerLifetimeScope();
            builder.RegisterType<AS_11>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<AS_12>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<AS_13>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<AS_14>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<AS_15>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ProcessingComponent>().As<IProcessingComponent>().InstancePerLifetimeScope();
        }
    }
}
