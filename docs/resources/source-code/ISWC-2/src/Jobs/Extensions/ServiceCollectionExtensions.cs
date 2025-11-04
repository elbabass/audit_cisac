using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpanishPoint.Azure.Iswc.Data.DataModels.Context;
using SpanishPoint.Azure.Iswc.Framework.Caching;
using SpanishPoint.Azure.Iswc.Framework.Caching.InMemory;
using SpanishPoint.Azure.Iswc.Framework.CosmosDb;
using SpanishPoint.Azure.Iswc.Framework.Mail;
using SpanishPoint.Azure.Iswc.Framework.Mail.Graph;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text;

namespace SpanishPoint.Azure.Iswc.Jobs.Extensions
{
    [ExcludeFromCodeCoverage]
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddDbContextPool<CsiContext>(options =>
                {
                    options.UseSqlServer(
                        configuration["AzureKeyVaultSecret-ISWC-ConnectionString-ISWCAzureSqlDatabase"],
                        o => o.EnableRetryOnFailure());
                })
                .AddDbContextPool<CsiContextReadOnly>(options =>
                {
                    options.UseSqlServer(
                        configuration["AzureKeyVaultSecret-ISWC-ConnectionString-ISWCAzureSqlDatabase"],
                        o => o.EnableRetryOnFailure());
                })
                .AddDbContextPool<CsiContextMaintenance>(options =>
                {
                    options.UseSqlServer(
                        configuration["AzureKeyVaultSecret-ISWC-ConnectionString-ISWCAzureSqlDatabaseMaintenance"],
                        o =>
                        {
                            o.EnableRetryOnFailure();
                            o.CommandTimeout((int)TimeSpan.FromHours(24).TotalSeconds);
                        });
                });
        }

        public static IServiceCollection AddIpiScheduledSync(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddScoped<ICacheClient, InMemoryCacheClient>()
                .AddLazyCache()
                .AddHttpClient("SuisaIpiClient", opt =>
                {
                    opt.BaseAddress = new Uri(configuration["AzureKeyVaultSecret-ISWC-SuisaIpiClientUrl"]);
                    var basic = Convert.ToBase64String(
                        Encoding.ASCII.GetBytes($"{configuration["AzureKeyVaultSecret-ISWC-SuisaIpiUserId"]}:{configuration["AzureKeyVaultSecret-ISWC-SuisaIpiPassword"]}"));
                    opt.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basic);
                })
                .Services;
        }

        public static IServiceCollection AddAuditing(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .Configure<CosmosDbOptions>(x => x.DatabaseId = "ISWC")
                .AddSingleton(new CosmosClient(configuration["AzureKeyVaultSecret-ISWC-ConnectionString-ISWCCosmosDb"]));
        }

        public static IServiceCollection AddMail(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddScoped<IMailClient, GraphMailClient>()
                .AddOptions<GraphMailClientOptions>()
                .Configure(opt =>
                {
                    opt.ClientID = configuration["AzureKeyVaultSecret-ISWC-SMTP-ClientID"];
                    opt.Tenant = configuration["AzureKeyVaultSecret-ISWC-SMTP-Tenant"];
                    opt.Secret = configuration["AzureKeyVaultSecret-ISWC-SMTP-Secret"];
                })
                .Services;
        }

        public static IServiceCollection AddCsnNotifications(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddHttpClient("IswcApiClient", opt =>
                {
                    opt.BaseAddress = new Uri(configuration["AzureKeyVaultSecret-ISWC-BaseAddress-ApiManagement"]);
                    opt.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", configuration["AzureKeyVaultSecret-ISWC-Ocp-Apim-Subscription-Key"]);
                })
                .Services;
        }

        public static IServiceCollection AddCheckAgentRuns(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddHttpClient("MailClient", opt =>
                {
                    opt.BaseAddress = new Uri($"{configuration["AzureKeyVaultSecret-EDI-MailService-Uri"]}?code={configuration["AzureKeyVaultSecret-EDI-MailService-Key"]}");
                })
                .Services;
        }
    }
}
