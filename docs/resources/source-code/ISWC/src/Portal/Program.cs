using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SpanishPoint.Azure.Iswc.Framework.Configuration.KeyVault;
using System.Diagnostics.CodeAnalysis;

namespace SpanishPoint.Azure.Iswc.Portal
{
    [ExcludeFromCodeCoverage]
    internal static class Program
    {
        public static void Main()
        {
            CreateWebHostBuilder().Build().Run();
        }

        public static IHostBuilder CreateWebHostBuilder() =>
            Host.CreateDefaultBuilder()
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureAppConfiguration((context, config) =>
            {
                if (context.HostingEnvironment.IsProduction())
                {
                    var builtConfig = config.Build();

                    var azureServiceTokenProvider = new AzureServiceTokenProvider();
                    var keyVaultClient = new KeyVaultClient(
                        new KeyVaultClient.AuthenticationCallback(
                            azureServiceTokenProvider.KeyVaultTokenCallback));

                    config.AddAzureKeyVault(
                        builtConfig["AzureKeyVaultBaseURL"],
                        keyVaultClient,
                        new PrefixKeyVaultSecretManager("AzureKeyVaultSecret-ISWC"));
                }
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}
