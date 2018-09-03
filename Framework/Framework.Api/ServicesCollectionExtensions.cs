// Godwit  - Framework.Api
// 2018.09.03
// A

using Elasticsearch.Net;
using Framework.Api.Options;
using Framework.Api.Services;
using GreenPipes;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using System;
using Framework.Api.Middleware;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Framework.Api {
    public static class ServicesCollectionExtensions {
        public static void AddMassTransit(this IServiceCollection services, IConfiguration config, Action<IServiceCollectionConfigurator> configureMasstransit) {
            services.AddSingleton(cp => {
                RabbitMqOptions options = cp.GetRequiredService<IOptionsSnapshot<RabbitMqOptions>>().Value;
                IBusControl bus = Bus.Factory.CreateUsingRabbitMq(cfg => {
                    cfg.Host(new Uri(options.Address), h => {
                        h.Username(config["rabbit.username"]);
                        h.Password(config["rabbit.password"]);
                    });
                    cfg.ReceiveEndpoint(options.QueueName, opt => opt.LoadFrom(cp));
                    cfg.UseExtensionsLogging();
                    cfg.UseRetry(configurator => {
                        configurator.Incremental(10, TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(10));
                    });
                });

                return bus;
            });

            services.AddSingleton<IPublishEndpoint>(cp => cp.GetService<IBusControl>());
            services.AddSingleton<ISendEndpointProvider>(cp => cp.GetService<IBusControl>());
            services.AddSingleton<IBus>(cp => cp.GetService<IBusControl>());
            services.AddSingleton<IHostedService, MasstransitHostedService>();
            services.AddMassTransit(configureMasstransit);
        }
        public static void AddCoreFunctionality(this IServiceCollection services, IConfiguration config,
            Action<HealthCheckBuilder> configureHealthCheck, Func<ApiVersionDescription, Info> createInfoForApiVersion,
            string xmlCommentsFilePath) {
            services.AddOptions();
            // setup options
            services.Configure<RabbitMqOptions>(config.GetSection("RabbitMqOptions"));
            services.Configure<ElasticSearchOptions>(config.GetSection("ElasticSearchOptions"));
            services.Configure<ConsulOptions>(config.GetSection("ConsulOptions"));

            services.AddResponseCompression();
            services.AddCors();
            services.AddHealthChecks(configureHealthCheck);


            services.AddMvcCore()
                // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                .AddVersionedApiExplorer(o => o.GroupNameFormat = "'v'VVV");

            services.AddMvc()
                .AddJsonOptions(options => {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddMetrics();
            services.AddApiVersioning(o => o.ReportApiVersions = true);
            services.AddSwaggerGen(options => {
                // resolve the IApiVersionDescriptionProvider service
                // note: that we have to build a temporary service provider here because one has not been created yet
                IApiVersionDescriptionProvider provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                // add a swagger document for each discovered API version
                // note: you might choose to skip or document deprecated API versions differently
                foreach (ApiVersionDescription description in provider.ApiVersionDescriptions)
                    options.SwaggerDoc(description.GroupName, createInfoForApiVersion(description));

                // add a custom operation filter which sets default values
                options.OperationFilter<SwaggerDefaultValues>();

                // integrate xml comments
                options.IncludeXmlComments(xmlCommentsFilePath);
            });
            services.AddHttpContextAccessor();
        }

        public static void AddElasticSearch(this IServiceCollection services, Action<ConnectionSettings> configureElasticSearch) {
            services.AddSingleton(cp => {
                ElasticSearchOptions options = cp.GetRequiredService<IOptionsSnapshot<ElasticSearchOptions>>().Value;
                ConnectionSettings connectionSettings =

                    new ConnectionSettings(new StaticConnectionPool(options.Uris));
                configureElasticSearch(connectionSettings);
                return connectionSettings;
            });
            services.AddScoped(c => new ElasticClient(c.GetService<ConnectionSettings>()));
        }

        public static void UseCoreFunctionality(this IApplicationBuilder app, IApiVersionDescriptionProvider provider,
            Action<SwaggerUIOptions, IApiVersionDescriptionProvider> configureSwagger) {
            app.UseCors(opt => {
                opt.AllowAnyHeader();
                opt.AllowAnyMethod();
                opt.AllowAnyOrigin();
            });
            app.UseCorrelationId();
            app.UseResponseCompression();
            app.UseMvc();
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(opt => configureSwagger(opt, provider));
        }
    }
}