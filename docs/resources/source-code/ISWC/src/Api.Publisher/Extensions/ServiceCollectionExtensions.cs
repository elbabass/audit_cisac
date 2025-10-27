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
using SpanishPoint.Azure.Iswc.Api.Publisher.Configuration.Swagger;
using SpanishPoint.Azure.Iswc.Api.Publisher.Managers;
using SpanishPoint.Azure.Iswc.Api.Publisher.V1;
using SpanishPoint.Azure.Iswc.Api.Publisher.V1.Controllers;
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
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace SpanishPoint.Azure.Iswc.Api.Publisher
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
                .AddScoped<IISWC_SearchController, SearchController>();

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

            builder.AddSigningCredential(new X509Certificate2(
                Convert.FromBase64String(configuration["Certificate-IswcApi"]), string.Empty, X509KeyStorageFlags.MachineKeySet));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = configuration["BaseAddress-IswcApiPublisher"];
                    options.RequireHttpsMetadata = true;
                    options.Audience = "iswcapi";
                });

            services.AddTransient<ICustomTokenRequestValidator, AgentClaimsValidator>();

            return services;
        }
    }
}
