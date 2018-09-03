using Framework.Api.Middleware;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Nest;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using IApplicationLifetime = Microsoft.AspNetCore.Hosting.IApplicationLifetime;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Framework.Api {
    public abstract class StartupBase<TStart> where TStart : StartupBase<TStart> {
        protected StartupBase(IHostingEnvironment env, IConfiguration configuration) {
            Environment = env;
            Configuration = configuration;
        }

        protected IConfiguration Configuration { get; }

        protected IHostingEnvironment Environment { get; }

        private static string XmlCommentsFilePath {
            get {
                var basePath = AppContext.BaseDirectory;
                var fileName = typeof(TStart).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine(basePath, fileName);
            }
        }

        public virtual void ConfigureServices(IServiceCollection services) {
            services.AddCoreFunctionality(Configuration, ConfigureHealthCheck, CreateInfoForApiVersion, XmlCommentsFilePath);
            services.AddMassTransit(Configuration, ConfigureMasstransit);
            services.AddElasticSearch(ConfigureElasticSearch);
        }

       

        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env,
            IApplicationLifetime appLifetime, IApiVersionDescriptionProvider provider) {
            app.UseForwardedHeaders(new ForwardedHeadersOptions {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            appLifetime.ApplicationStopping.Register(ProgramBase<TStart>.ConsulConfigCancellationTokenSource.Cancel);
            app.UseCoreFunctionality(provider, ConfigureSwagger);
        }

 

        protected virtual void ConfigureSwagger(SwaggerUIOptions options, IApiVersionDescriptionProvider provider) { }

        protected virtual void ConfigureMasstransit(IServiceCollectionConfigurator cfg) { }

        protected virtual void ConfigureHealthCheck(HealthCheckBuilder checks) {
            checks.AddValueTaskCheck("HTTP Endpoint",
                () => new ValueTask<IHealthCheckResult>(HealthCheckResult.Healthy("The Service is running")));
        }

        protected virtual void ConfigureElasticSearch(ConnectionSettings conn) { }

        protected virtual Info CreateInfoForApiVersion(ApiVersionDescription description) {
            var info = new Info {
                Title = $"Microservice API {description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
                Description = "A Microservice Api",
                Contact = new Contact { Name = "Hamza Althunibat", Email = "althunibat@outlook.com" },
                TermsOfService = "Private"
            };

            if (description.IsDeprecated) info.Description += "This API version has been deprecated.";

            return info;
        }
    }
}