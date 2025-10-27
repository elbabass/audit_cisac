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
        public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfigurationRoot configuration)
        {
            return services
                .AddEntityFrameworkSqlServer()
                .AddDbContextPool<CsiContext>(options =>
                {
                    options.UseSqlServer(
                        configuration["ConnectionString-ISWCAzureSqlDatabase"],
                        o => o.EnableRetryOnFailure());
                })
                .AddDbContextPool<CsiContextReadOnly>(options =>
                {
                    options.UseSqlServer(
                        configuration["ConnectionString-ISWCAzureSqlDatabase"],
                        o => o.EnableRetryOnFailure());
                })
                .AddDbContextPool<CsiContextMaintenance>(options =>
                {
                    options.UseSqlServer(
                        configuration["ConnectionString-ISWCAzureSqlDatabaseMaintenance"],
                        o =>
                        {
                            o.EnableRetryOnFailure();
                            o.CommandTimeout((int)TimeSpan.FromHours(24).TotalSeconds);
                        });
                });
        }

        public static IServiceCollection AddIpiScheduledSync(this IServiceCollection services, IConfigurationRoot configuration)
        {
            return services
                 .AddScoped<ICacheClient, InMemoryCacheClient>()
                 .AddLazyCache()
                 .AddHttpClient("SuisaIpiClient", opt =>
                 {
                     opt.BaseAddress = new Uri(configuration["SuisaIpiClientUrl"]);
                     opt.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                        "Basic", Convert.ToBase64String(
                             Encoding.ASCII.GetBytes($"{configuration["SuisaIpiUserId"] }:{configuration["SuisaIpiPassword"]}")));
                 }).Services;
        }

        public static IServiceCollection AddAuditing(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                 .Configure<CosmosDbOptions>(x =>
                 {
                     x.DatabaseId = "ISWC";
                 })
                 .AddSingleton(new CosmosClient(configuration["ConnectionString-ISWCCosmosDb"]));
        }

        public static IServiceCollection AddMail(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddScoped<IMailClient, GraphMailClient>()
                .AddOptions<GraphMailClientOptions>().Configure(opt =>
                {
                    opt.ClientID = configuration["SMTP-ClientID"];
                    opt.Tenant = configuration["SMTP-Tenant"];
                    opt.Secret = configuration["SMTP-Secret"];
                }).Services;
        }

        public static IServiceCollection AddCsnNotifications(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddHttpClient("IswcApiClient", opt =>
                {
                    opt.BaseAddress = new Uri(configuration["BaseAddress-ApiManagement"]);
                    opt.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", configuration["Ocp-Apim-Subscription-Key"]);
                }).Services;
        }

        public static IServiceCollection AddCheckAgentRuns(this IServiceCollection services, IConfiguration configuration)
		{
            return services
                .AddHttpClient("MailClient", OptionsBuilderConfigurationExtensions =>
                {
                    OptionsBuilderConfigurationExtensions.BaseAddress = new Uri($"{configuration["MailServiceUri"]}?code={configuration["MailServiceKey"]}");
                }).Services;
		}
    }
}