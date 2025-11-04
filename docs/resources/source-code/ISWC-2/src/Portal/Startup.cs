using Autofac;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Converters;
using SpanishPoint.Azure.Iswc.Framework.Caching.Configuration;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using SpanishPoint.Azure.Iswc.Portal.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace SpanishPoint.Azure.Iswc.Portal
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .AddEnvironmentVariables()
                .AddUserSecrets<Startup>();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllersWithViews()
                .AddNewtonsoftJson();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            services
                .AddResponseCompression(options => { options.EnableForHttps = true; })
                .AddApplicationInsightsTelemetry()
                .AddAutoMapper(cfg =>
                {
                    cfg.ShouldMapProperty = p =>
                        p.GetMethod != null &&
                        !p.GetMethod.IsStatic &&
                        p.GetIndexParameters().Length == 0;
                }, AppDomain.CurrentDomain.GetAllLoadedAssemblies())
                .AddDataAccess(Configuration)
                .AddRequestLocalization()
                .AddOptions(Configuration)
                .AddAuthentication(Configuration)
                .AddReCaptcha(Configuration)
                .AddAuditing(Configuration)
                .AddDatabricksClient(Configuration)
                .AddReporting(Configuration)
                .AddMatchingApi(Configuration);

            services.AddHealthChecks();
        }

        public static void ConfigureContainer(ContainerBuilder builder)
        {
            var assemblies = AppDomain.CurrentDomain.GetAllLoadedAssemblies().ToArray();
            builder.RegisterAssemblyModules(assemblies);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseResponseCompression();
            app.UseSpaStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse =
                r =>
                {
                    string extension = Path.GetExtension(r.File.PhysicalPath);
                    if (new string[] { ".css", ".js", ".gif", ".jpg", ".png", ".svg", ".ico", ".pdf" }.Contains(extension))
                    {
                        r.Context.Response.Headers.Append("Cache-Control", "max-age=" + CacheDuration.FifteenWeeks);
                    }
                }
            });

            app.UseRouting();

            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");

                endpoints.MapHealthChecks("/portal/health");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
