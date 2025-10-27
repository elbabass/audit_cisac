using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Hosting;
using SpanishPoint.Azure.Iswc.Framework.Configuration.KeyVault;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SpanishPoint.Azure.Iswc.Api.Publisher
{
    [ExcludeFromCodeCoverage]
    internal static class Program
    {
        public static void Main()
        {
            CreateHostBuilder().Build().Run();
        }

        public static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureAppConfiguration((context, config) =>
            {
                var builtConfig = config.Build();

                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var keyVaultClient = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(
                        azureServiceTokenProvider.KeyVaultTokenCallback));

                config.AddAzureKeyVault(new AzureKeyVaultConfigurationOptions
                {
                    Vault = builtConfig["AzureKeyVaultBaseURL"],
                    ReloadInterval = TimeSpan.FromMinutes(30),
                    Client = keyVaultClient,
                    Manager = new PrefixKeyVaultSecretManager("AzureKeyVaultSecret-ISWC")
                });
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}
