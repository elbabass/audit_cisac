using System;
using System.IO;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpanishPoint.Azure.Iswc.Data;
using SpanishPoint.Azure.Iswc.Jobs.Extensions;

try
{
    Console.WriteLine("ISWC Jobs starting up...");

    var host = new HostBuilder()
        .UseServiceProviderFactory(new AutofacServiceProviderFactory())
        .ConfigureFunctionsWorkerDefaults()
        .ConfigureAppConfiguration(builder =>
        {
            Console.WriteLine("Configuring application...");

            builder
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();

            try
            {
                var keyVaultUrl = builder.Build()["AzureKeyVaultBaseURL"];
                Console.WriteLine($"KeyVault URL: {keyVaultUrl ?? "(none)"}");

                if (!string.IsNullOrWhiteSpace(keyVaultUrl))
                {
                    var secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
                    builder.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
                    Console.WriteLine("KeyVault configuration added.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Startup:KeyVault] {ex}");
            }
        })
        .ConfigureServices((hostContext, services) =>
        {
            Console.WriteLine("Configuring services...");

            var config = hostContext.Configuration;
            services.AddHttpContextAccessor();

            services
                .AddLocalization()
                .AddAutoMapper(typeof(MappingProfile))
                .AddIpiScheduledSync(config)
                .AddDataAccess(config)
                .AddAuditing(config)
                .AddMail(config)
                .AddCsnNotifications(config)
                .AddCheckAgentRuns(config);
        })
        .ConfigureContainer<ContainerBuilder>((context, builder) =>
        {
            Console.WriteLine("Registering Autofac modules...");
            builder.RegisterModule(new SpanishPoint.Azure.Iswc.Business.AutofacModule());
            builder.RegisterModule(new SpanishPoint.Azure.Iswc.Data.AutofacModule());
        })
        .Build();

    Console.WriteLine("Starting host...");
    host.Run();
}
catch (Exception ex)
{
    Console.Error.WriteLine($"[Fatal startup error] {ex}");
    throw;
}
