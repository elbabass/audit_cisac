using Autofac;
using AutoMapper;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Converters;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using SpanishPoint.Azure.Iswc.Framework.Http.Middleware;
using SpanishPoint.Azure.Iswc.Framework.MaintenanceMode.Middleware;
using SpanishPoint.Azure.Iswc.Framework.Security;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SpanishPoint.Azure.Iswc.Api.Agency
{
    [ExcludeFromCodeCoverage]
    internal class Startup
    {
        public IConfigurationRoot Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            Environment = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .AddConfiguration(configuration);

            if (env.IsDevelopment())
                builder.AddUserSecrets<Startup>();

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddApplicationInsightsTelemetry()
                .ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) => { module.EnableSqlCommandTextInstrumentation = true; })
                .AddAutoMapper(AppDomain.CurrentDomain.GetAllLoadedAssemblies())
				.AddControllers()
                .AddNewtonsoftJson(opt => { opt.SerializerSettings.Converters.Add(new StringEnumConverter()); });

            services
                .AddAuditing(Configuration)
                .AddIpiScheduledSync(Configuration)
                .AddControllerImplementations()
                .AddDataAccess(Configuration)
                .AddMatchingApi(Configuration)
                .AddPipelines()
                .AddRequestLocalization()
                .AddSwagger()
                .AddAzureSearchServices(Configuration)
                .AddApiAuthentication(Configuration, Environment);

            services.AddHealthChecks();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var assemblies = AppDomain.CurrentDomain.GetAllLoadedAssemblies().ToArray();
            builder.RegisterAssemblyModules(assemblies);
            builder.RegisterAssemblyTypes(assemblies).Where(t => typeof(IBaseRule).IsAssignableFrom(t)).AsSelf().InstancePerLifetimeScope();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwaggerUI(options =>
                {
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }
                });
            }
            else
                app.UseHsts();

            app
                .UseIdentityServer()
                .UseAuthentication();

            app
                .UseSwagger()
                .UseRequestLocalization()
                .UseRouting()
                .UseCors(builder =>
                    builder.WithOrigins(
                        Configuration["BaseAddress-IswcPortal"],
                        Configuration["BaseAddress-IswcPublic"])
                   .AllowAnyHeader()
                   .AllowAnyMethod())
                .UseAuthorization()
                .UseMaintenanceMode()
                .UseJsonBodyTelemetry()
                .UseRequestFilterMiddleware()
                .UseEndpoints(endpoints =>
                {
                    var builder = endpoints.MapControllers();
                    if (!env.IsDevelopment())
                        builder.RequireAuthorization();
                    endpoints.MapHealthChecks("/api/health");
                });

            app.Run(async context =>
            {
                if (context.Request.Path == "/")
                    await context.Response.WriteAsync("ok.");
                else
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
            });
        }
    }
}
