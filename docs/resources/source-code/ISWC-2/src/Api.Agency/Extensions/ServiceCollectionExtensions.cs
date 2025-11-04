using IdentityModel.Client;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using SpanishPoint.Azure.Iswc.Api.Agency.Configuration.Swagger;
using SpanishPoint.Azure.Iswc.Api.Agency.Managers;
using SpanishPoint.Azure.Iswc.Api.Agency.V1;
using SpanishPoint.Azure.Iswc.Api.Agency.V1.Controllers;
using SpanishPoint.Azure.Iswc.Data.DataModels.Context;
using SpanishPoint.Azure.Iswc.Data.Services.Search.Azure.Options;
using SpanishPoint.Azure.Iswc.Framework.Authentication.IdentityServer;
using SpanishPoint.Azure.Iswc.Framework.Caching;
using SpanishPoint.Azure.Iswc.Framework.Caching.InMemory;
using SpanishPoint.Azure.Iswc.Framework.CosmosDb;
using SpanishPoint.Azure.Iswc.Framework.Http.Client;
using SpanishPoint.Azure.Iswc.Framework.Http.Client.Options;
using SpanishPoint.Azure.Iswc.Framework.Http.Handler;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SpanishPoint.Azure.Iswc.Api.Agency
{
    [ExcludeFromCodeCoverage]
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services
                .AddApiVersioning(opt =>
                {
                    opt.ApiVersionReader = new HeaderApiVersionReader("api-version");
                    opt.AssumeDefaultVersionWhenUnspecified = true;
                    opt.Conventions.Add(new VersionByNamespaceConvention());
                })
                .AddVersionedApiExplorer(opt => opt.GroupNameFormat = "'v'VVV")
                .AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>()
                .AddSwaggerGen(opt =>
                {
                    opt.IncludeXmlComments(XmlCommentsFilePath());
                    opt.CustomSchemaIds(x => x.FullName);
                });

            return services;

            static string XmlCommentsFilePath()
            {
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine(basePath, fileName);
            }
        }

        public static IServiceCollection AddControllerImplementations(this IServiceCollection services)
        {
            services
                .AddScoped<IISWC_MergeController, MergeController>()
                .AddScoped<IISWC_Workflow_TasksController, WorkflowTasksController>()
                .AddScoped<IISWC_SubmissionController, SubmissionController>()
                .AddScoped<IISWC_SearchController, SearchController>()
                .AddScoped<IISWC_AgentController, AgentController>()
                .AddScoped<IISWC_Allocation_and_ResolutionController, AllocationAndResolutionController>()
                .AddScoped<IISWC_Usage_SearchController, UsageController>();

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
                })
                .AddDbContextPool<CsiContextReadOnly>(options =>
                {
                    options.UseSqlServer(
                        configuration["ConnectionString-ISWCAzureSqlDatabase"] + "ApplicationIntent=readonly;",
                        o =>
                        {
                            o.EnableRetryOnFailure();
                            o.CommandTimeout((int)TimeSpan.FromMinutes(3).TotalSeconds);
                        });
                });

            return services;
        }

        public static IServiceCollection AddPipelines(this IServiceCollection services)
        {
            services
                .AddScoped<IPipelineManager, PipelineManager>();

            return services;
        }

        public static IServiceCollection AddMatchingApi(this IServiceCollection services, IConfiguration configuration)
        {
            var matchingEngineBaseAddress = configuration["BaseAddress-SpanishPointMatchingEngine"] ??
                throw new ArgumentNullException("BaseAddress-SpanishPointMatchingEngine", "Missing BaseAddress-SpanishPointMatchingEngine configuration value.");

            services
                .AddTransient<ProtectedApiBearerTokenHandler>()
                .Configure<MatchingEngineClientCredentialsOptions>(opt =>
                {
                    opt.Address = new Uri(new Uri(matchingEngineBaseAddress) + "connect/token").AbsoluteUri;
                    opt.ClientId = configuration["MicrosoftEntraID-ClientID"];
                    opt.ClientSecret = configuration["Secret-MicrosoftEntraIDClient"];
                    opt.Scope = $"{configuration["MicrosoftEntraID-Scope"]}/.default";
                })
                 .AddHttpClient<IMatchingEngineIdentityServerClient, IdentityServerClient<MatchingEngineClientCredentialsOptions>>(opt =>
                 {
                     opt.BaseAddress = new Uri(matchingEngineBaseAddress);
                 });

            services
                .AddHttpClient("MatchingApi", opt =>
                {
                    opt.BaseAddress = new Uri(matchingEngineBaseAddress);
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

        public static IServiceCollection AddRequestLocalization(this IServiceCollection services)
        {
            services.AddLocalization().Configure<RequestLocalizationOptions>(opts =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en")
                };

                opts.DefaultRequestCulture = new RequestCulture(supportedCultures[0]);
                opts.SupportedCultures = supportedCultures;
                opts.SupportedUICultures = supportedCultures;
                opts.SetDefaultCulture(supportedCultures[0].Name);
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

        public static IServiceCollection AddIpiScheduledSync(this IServiceCollection services, IConfigurationRoot configuration)
        {
            var suisaIpiClientUrl = configuration["SuisaIpiClientUrl"] ??
                throw new ArgumentNullException("SuisaIpiClientUrl", "Missing SuisaIpiClientUrl configuration value.");

            return services
                 .AddScoped<ICacheClient, InMemoryCacheClient>()
                 .AddLazyCache()
                 .AddHttpClient("SuisaIpiClient", opt =>
                 {
                     opt.BaseAddress = new Uri(suisaIpiClientUrl);
                     opt.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                        "Basic", Convert.ToBase64String(
                             Encoding.ASCII.GetBytes($"{configuration["SuisaIpiUserId"]}:{configuration["SuisaIpiPassword"]}")));
                 }).Services;
        }

        public static IServiceCollection AddAzureSearchServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .Configure<WorksOptions>(opt =>
                {
                    opt.IndexName = configuration["Name-WorksIndex"];
                    opt.SearchServiceName = configuration["Name-AzureSearch"];
                    opt.ApiKey = configuration["ApiKey-AzureSearch"];
                });

            return services;
        }

        public static IServiceCollection AddApiAuthentication(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            var builder = services.AddIdentityServer()
                     .AddInMemoryApiResources(IdentityServerConfiguration.Apis)
                     .AddInMemoryClients(IdentityServerConfiguration.Clients(configuration));

            var iswcApiCertificate = configuration["Certificate-IswcApi"] ??
                throw new ArgumentNullException("Certificate-IswcApi", "Missing Certificate-IswcApi configuration value.");

            builder.AddSigningCredential(new X509Certificate2(
                Convert.FromBase64String(iswcApiCertificate), string.Empty, X509KeyStorageFlags.MachineKeySet));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = configuration["BaseAddress-IswcApi"];
                    options.RequireHttpsMetadata = true;
                    options.Audience = "iswcapi";
                });

            services.AddTransient<ICustomTokenRequestValidator, AgentClaimsValidator>();

            return services;
        }
    }
}
