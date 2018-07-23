using System;
using System.Reflection;
using Framework.Model.Data;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Nest;
using SampleApi.options;
using SampleApi.V1.Data;
using SampleApi.V1.Services;
using SampleApi.V1.Services.Events;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace SampleApi {
    public class Startup : Framework.Api.StartupBase<Startup> {
        public Startup(IHostingEnvironment env, IConfiguration configuration) : base(env, configuration) { }

        public override void ConfigureServices(IServiceCollection services) {
            services.Configure<DataOptions>(Configuration.GetSection("DataOptions"));
            services.AddDbContextPool<SampleDbContext>(cfg => {
                cfg.UseSqlServer(Configuration.GetConnectionString("sample-db"), opt => {
                    opt.MigrationsAssembly(typeof(SampleDbContext).GetTypeInfo().Assembly.GetName().Name);
                    opt.EnableRetryOnFailure(10, TimeSpan.FromMilliseconds(100), null);
                });
            });
            services.AddScoped(typeof(IReadOnlyRepository<,>), typeof(Repository<,>));
            services.AddScoped<IPersonService, PersonService>();
            base.ConfigureServices(services);
        }

        protected override void ConfigureElasticSearch(ConnectionSettings conn) {
            conn.DefaultMappingFor<IPersonRetrieved>(i => i
                .IndexName("audit-person-retrieved")
                .IdProperty(e => e.Id)
                .TypeName("person-retrieved"));
        }

        protected override void ConfigureHealthCheck(HealthCheckBuilder checks) {
            base.ConfigureHealthCheck(checks);
            checks.AddSqlCheck("db", Configuration.GetConnectionString("sample-db"), TimeSpan.FromSeconds(10));
        }

        protected override void ConfigureSwagger(SwaggerUIOptions options, IApiVersionDescriptionProvider provider) {
            options.RoutePrefix = string.Empty;
            // build a swagger endpoint for each discovered API version
            foreach (var description in provider.ApiVersionDescriptions)
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToUpperInvariant());
        }

        protected override void ConfigureMasstransit(IServiceCollectionConfigurator cfg) {
            cfg.AddConsumer<AuditorService>();
        }

        protected override Info CreateInfoForApiVersion(ApiVersionDescription description) {
            var info = base.CreateInfoForApiVersion(description);
            info.Title = $"Sample Microservices API {description.ApiVersion}";
            return info;
        }
    }
}