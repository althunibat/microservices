using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Elasticsearch.Net;
using GreenPipes;
using Identity.Server.Data;
using Identity.Server.Middleware;
using Identity.Server.Models;
using Identity.Server.Options;
using Identity.Server.Services;
using IdentityServer4.Validation;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Options;
using Nest;
using Serilog;

namespace Identity.Server
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        private IHostingEnvironment Environment { get; }

        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            Environment = env;
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<RabbitMqOptions>(Configuration.GetSection("RabbitMqOptions"));
            services.Configure<ElasticSearchOptions>(Configuration.GetSection("ElasticSearchOptions"));
            services.Configure<ConsulOptions>(Configuration.GetSection("ConsulOptions"));
            Console.WriteLine($"cert.location:{Configuration["cert.location"]}");
            var certificate = new X509Certificate2(Path.GetFullPath(Configuration["cert.location"]), Configuration["cert.password"]);
            var connectionString = Configuration.GetConnectionString("identity-db");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
        
            services
                .AddDataProtection(opt => opt.ApplicationDiscriminator = "Sso-srv")
                .PersistKeysToFileSystem(new DirectoryInfo(Path.GetFullPath("cert")))
                .ProtectKeysWithCertificate(certificate);

            services.AddDbContextPool<ApplicationDbContext>(cfg =>
            {
                cfg.UseSqlServer(connectionString, opt => {
                    opt.MigrationsAssembly(migrationsAssembly);
                    opt.EnableRetryOnFailure(10, TimeSpan.FromMilliseconds(100), null);
                });
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                })
                .AddAspNetIdentity<ApplicationUser>()
                .AddSigningCredential(certificate)
                .AddSecretParser<JwtBearerClientAssertionSecretParser>()
                .AddSecretValidator<PrivateKeyJwtSecretValidator>()
                // this adds the config data from DB (clients, resources)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(connectionString, opt => {
                            opt.MigrationsAssembly(migrationsAssembly);
                            opt.EnableRetryOnFailure(10, TimeSpan.FromMilliseconds(100), null);
                        });
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(connectionString, opt => {
                            opt.MigrationsAssembly(migrationsAssembly);
                            opt.EnableRetryOnFailure(10, TimeSpan.FromMilliseconds(100), null);
                        });

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    // options.TokenCleanupInterval = 15; // frequency in seconds to cleanup stale grants. 15 is useful during debugging
                });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddHealthChecks(ConfigureHealthCheck);
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
            services.AddHostedService<MasstransitHostedService>();
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
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            appLifetime.ApplicationStopping.Register(Program.ConsulConfigCancellationTokenSource.Cancel);
            appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);

            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseCookiePolicy();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseRemoteIpAddressLoggingMiddleware();
            app.UseCorrelationId();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
           // SeedData.EnsureSeedData(app.ApplicationServices);
        }

        private static void ConfigureMasstransit(IServiceCollectionConfigurator cfg)
        {
        }

        private static void ConfigureHealthCheck(HealthCheckBuilder checks)
        {
            checks.AddValueTaskCheck("HTTP Endpoint",
                () => new ValueTask<IHealthCheckResult>(HealthCheckResult.Healthy("The Service is running")));
        }

        private static void ConfigureElasticSearch(ConnectionSettings conn)
        {
        }
    }
}
