using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using SpanishPoint.Azure.Iswc.Data.DataModels.Context;
using SpanishPoint.Azure.Iswc.Framework.Caching;
using SpanishPoint.Azure.Iswc.Framework.Caching.InMemory;
using SpanishPoint.Azure.Iswc.Framework.CosmosDb;
using SpanishPoint.Azure.Iswc.Framework.Databricks;
using SpanishPoint.Azure.Iswc.Framework.Http.Client;
using SpanishPoint.Azure.Iswc.Framework.Http.Client.Options;
using SpanishPoint.Azure.Iswc.Framework.Http.Handler;
using SpanishPoint.Azure.Iswc.Framework.Http.Requests;
using SpanishPoint.Azure.Iswc.Portal.Configuration.Options;
using SpanishPoint.Azure.Iswc.Portal.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net.Http;

namespace SpanishPoint.Azure.Iswc.Portal.Extensions
{
    [ExcludeFromCodeCoverage]
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRequestLocalization(this IServiceCollection services)
        {
            services.AddLocalization().Configure<RequestLocalizationOptions>(opts =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en"),
                    new CultureInfo("fr"),
                    new CultureInfo("es")
                };

                opts.DefaultRequestCulture = new RequestCulture(supportedCultures[0]);
                opts.SupportedCultures = supportedCultures;
                opts.SupportedUICultures = supportedCultures;
                opts.SetDefaultCulture(supportedCultures[0].Name);
            });

            return services;
        }

        public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<ClientAppOptions>().Configure(opt =>
            {
                opt.LoginRedirectUri = new Uri(configuration["Uri-LoginRedirect"]);
                opt.RecaptchaPublicKey = configuration["Recaptcha-PublicKey"];
                opt.ApplicationInsightsKey = configuration["APPINSIGHTS_INSTRUMENTATIONKEY"];
                opt.IswcApiManagementUri = new Uri(configuration["BaseAddress-ApiManagement"]);
            });

            return services;
        }

        public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient("FastTrackAuthenticationService", opt => { opt.BaseAddress = new Uri(configuration["BaseAddress-AuthenticationService"]); });
            services.AddTransient<IFastTrackAuthenticationService, FastTrackAuthenticationService>();

            services
                .Configure<RestApiClientCredentialsOptions>(opt =>
                {
                    opt.Address = new Uri(new Uri(configuration["BaseAddress-IswcApi"]), "connect/token").AbsoluteUri;
                    opt.ClientId = "iswcportal";
                    opt.ClientSecret = configuration["Secret-IswcApi"];
                    opt.Scope = "iswcapi";
                })
                .AddHttpClient<IRestApiIdentityServerClient, IdentityServerClient<RestApiClientCredentialsOptions>>(client =>
                {
                    client.BaseAddress = new Uri(configuration["BaseAddress-IswcApi"]);
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                });

            services.AddAuthentication("Bearer");
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();


            return services;
        }

        public static IServiceCollection AddReCaptcha(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient("GoogleReCaptcha", opt =>
            {
                opt.BaseAddress = new Uri(configuration["Uri-Verify-Recaptcha"]);
            });

            return services;
        }
        public static IServiceCollection AddDatabricksClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DatabricksClientOptions>(x =>
            {
                x.JobID = long.Parse(configuration["Databricks-ReportsJobId"]);
                x.BearerToken = configuration["Databricks-ReportsJobBearerToken"];
            });

            services.AddHttpClient("DatabricksJobApi", opt =>
            {
                opt.BaseAddress = new Uri(configuration["Databricks-ReportsJobUri"]);
            });

            return services;
        }

        public static IServiceCollection AddAuditing(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CosmosDbOptions>(x =>
            {
                x.DatabaseId = "ISWC";
            });

            services.AddSingleton(new CosmosClient(configuration["ConnectionString-ISWCCosmosDb"]));

            return services;
        }

        public static IServiceCollection AddReporting(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddSingleton(x =>
                {
                    return CloudStorageAccount.Parse(configuration["ConnectionString-AzureStorage"]).CreateCloudBlobClient();
                });

            return services;
        }
        public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddMemoryCache()
                .AddScoped<ICacheClient, InMemoryCacheClient>()
                .AddLazyCache();

            services
                .AddDbContextPool<CsiContext>(options =>
                {
                    options.UseSqlServer(
                        configuration["ConnectionString-ISWCAzureSqlDatabase"],
                        o =>
                        {
                            o.EnableRetryOnFailure();
                            o.CommandTimeout((int)TimeSpan.FromMinutes(3).TotalSeconds);
                        });
                });

            return services;
        }

        public static IServiceCollection AddMatchingApi(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddTransient<ProtectedApiBearerTokenHandler>()
                .Configure<MatchingEngineClientCredentialsOptions>(opt =>
                {
                    opt.Address = new Uri(new Uri(configuration["BaseAddress-SpanishPointMatchingEngine"]) + "connect/token").AbsoluteUri;
                    opt.ClientId = configuration["MicrosoftEntraID-ClientID"];
                    opt.ClientSecret = configuration["Secret-MicrosoftEntraIDClient"];
                    opt.Scope = $"{configuration["MicrosoftEntraID-Scope"]}/.default";
                })
                .AddHttpClient<IMatchingEngineIdentityServerClient, IdentityServerClient<MatchingEngineClientCredentialsOptions>>(opt =>
                {
                    opt.BaseAddress = new Uri(configuration["BaseAddress-SpanishPointMatchingEngine"]);
                });

            services
                .AddHttpClient("MatchingApi", opt =>
                {
                    opt.BaseAddress = new Uri(configuration["BaseAddress-SpanishPointMatchingEngine"]);
                })
                .AddHttpMessageHandler<ProtectedApiBearerTokenHandler>()
                .AddPolicyHandler(
                    HttpPolicyExtensions
                        .HandleTransientHttpError().Or<TimeoutRejectedException>()
                        .WaitAndRetryAsync(new[] {
                            TimeSpan.FromSeconds(10),
                        }))
                .AddPolicyHandler(
                    Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(80)));
            return services;
        }
    }
}
