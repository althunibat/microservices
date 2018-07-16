using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Framework.Api.Middleware;
using Framework.Api.Options;
using Framework.Api.Services;
using GreenPipes;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Options;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Framework.Api
{
    public abstract class StartupBase<TStart> where TStart : StartupBase<TStart>
    {
        protected IConfiguration Configuration { get; }

        protected IHostingEnvironment Environment { get; }

        protected StartupBase(IHostingEnvironment env, IConfiguration configuration)
        {
            Environment = env;
            Configuration = configuration;
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            // setup options
            services.Configure<RabbitMqOptions>(Configuration.GetSection("RabbitMqOptions"));
            services.Configure<ElasticSearchOptions>(Configuration.GetSection("ElasticSearchOptions"));
            services.Configure<ConsulOptions>(Configuration.GetSection("ConsulOptions"));

            services.AddResponseCompression();
            services.AddCors();
            services.AddHealthChecks(ConfigureHealthCheck);


            services.AddMvcCore()
                // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                .AddVersionedApiExplorer(o => o.GroupNameFormat = "'v'VVV");

            services.AddMvc()
                .AddJsonOptions(options => {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddMetrics();
            services.AddApiVersioning(o => o.ReportApiVersions = true);
            services.AddSwaggerGen(options =>
            {
                // resolve the IApiVersionDescriptionProvider service
                // note: that we have to build a temporary service provider here because one has not been created yet
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                // add a swagger document for each discovered API version
                // note: you might choose to skip or document deprecated API versions differently
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
                }

                // add a custom operation filter which sets default values
                options.OperationFilter<SwaggerDefaultValues>();

                // integrate xml comments
                options.IncludeXmlComments(XmlCommentsFilePath);
            });
            services.AddMassTransit(ConfigureMasstransit);
            services.AddSingleton(cp =>
            {
                var options = cp.GetRequiredService<IOptionsSnapshot<RabbitMqOptions>>().Value;
                var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.Host(new Uri(options.Address), h =>
                    {
                        h.Username(Configuration["rabbit.username"]);
                        h.Password(Configuration["rabbit.password"]);
                    });
                    cfg.ReceiveEndpoint(options.QueueName, opt => opt.LoadFrom(cp));
                    cfg.UseExtensionsLogging();
                    cfg.UseRetry(configurator =>
                        {
                            configurator.Incremental(10, TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(10));
                        });
                });

                return bus;
            });

            services.AddSingleton<IPublishEndpoint>(cp => cp.GetService<IBusControl>());
            services.AddSingleton<ISendEndpointProvider>(cp => cp.GetService<IBusControl>());
            services.AddSingleton<IBus>(cp => cp.GetService<IBusControl>());
            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, MasstransitHostedService>();

            services.AddSingleton(cp =>
            {
                var options = cp.GetRequiredService<IOptionsSnapshot<ElasticSearchOptions>>().Value;
                var connectionSettings =
                    new ConnectionSettings(new StaticConnectionPool(options.Uris));
                ConfigureElasticSearch(connectionSettings);
                return connectionSettings;
            });
            services.AddScoped(c => new ElasticClient(c.GetService<ConnectionSettings>()));
            services.AddHttpContextAccessor();
        }


        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env,
            IApplicationLifetime appLifetime, IApiVersionDescriptionProvider provider)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            appLifetime.ApplicationStopping.Register(ProgramBase<TStart>.ConsulConfigCancellationTokenSource.Cancel);
            app.UseCors(opt =>
            {
                opt.AllowAnyHeader();
                opt.AllowAnyMethod();
                opt.AllowAnyOrigin();
            });
            app.UseCorrelationId();
            app.UseResponseCompression();
            app.UseMvc();
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(opt=> ConfigureSwagger(opt,provider));
        }

        protected virtual void ConfigureSwagger(SwaggerUIOptions options, IApiVersionDescriptionProvider provider)
        {
        }

        protected virtual void ConfigureMasstransit(IServiceCollectionConfigurator cfg)
        {
        }

        protected virtual void ConfigureHealthCheck(HealthCheckBuilder checks)
        {
            checks.AddValueTaskCheck("HTTP Endpoint",
                () => new ValueTask<IHealthCheckResult>(HealthCheckResult.Healthy("The Service is running")));
        }

        protected virtual void ConfigureElasticSearch(ConnectionSettings conn)
        {
        }

        protected virtual Info CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new Info
            {
                Title = $"Microservice API {description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
                Description = "A Microservice Api",
                Contact = new Contact { Name = "Hamza Althunibat", Email = "althunibat@outlook.com" },
                TermsOfService = "Private"
            };

            if (description.IsDeprecated)
            {
                info.Description += "This API version has been deprecated.";
            }

            return info;
        }

        private static string XmlCommentsFilePath {
            get {
                var basePath = AppContext.BaseDirectory;
                var fileName = typeof(TStart).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine(basePath, fileName);
            }
        }
    }
}