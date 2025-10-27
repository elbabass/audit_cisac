using Autofac;
using Autofac.Extensions.DependencyInjection.AzureFunctions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpanishPoint.Azure.Iswc.Data;
using SpanishPoint.Azure.Iswc.Jobs.Extensions;
using SpanishPoint.Azure.Iswc.Jobs.Functions;
using System.Diagnostics.CodeAnalysis;

[assembly: FunctionsStartup(typeof(SpanishPoint.Azure.Iswc.Jobs.Startup))]
namespace SpanishPoint.Azure.Iswc.Jobs
{
    [ExcludeFromCodeCoverage]
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;
            var serviceProvider = services.BuildServiceProvider();

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(serviceProvider.GetService<IHostEnvironment>().ContentRootPath)
                .AddEnvironmentVariables();

            var config = configurationBuilder.Build();

            if (config["ASPNETCORE_ENVIRONMENT"] != "Development")
            {
                config = config.AddKeyVaultSecrets(configurationBuilder);
            }

            services
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddLocalization()
                .AddAutoMapper(typeof(MappingProfile))
                .AddIpiScheduledSync(config)
                .AddDataAccess(config)
                .AddSingleton<IConfiguration>(config)
                .AddAuditing(config)
                .AddMail(config)
                .AddCsnNotifications(config)
                .AddCheckAgentRuns(config);

            builder
                .UseAutofacServiceProviderFactory(ConfigureContainer);
        }

        private void ConfigureContainer(ContainerBuilder builder)
        {
            builder
                .RegisterAssemblyTypes(typeof(Startup).Assembly)
                .InNamespace(typeof(SendMail).Namespace)
                .AsSelf()
                .InstancePerTriggerRequest();

            builder.RegisterModule<Business.AutofacModule>();
            builder.RegisterModule<AutofacModule>();
        }
    }
}
