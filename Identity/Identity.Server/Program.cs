using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Identity.Server.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using Winton.Extensions.Configuration.Consul;

namespace Identity.Server
{
    public class Program
    {
        public static readonly CancellationTokenSource ConsulConfigCancellationTokenSource = new CancellationTokenSource();

        public static int Main(string[] args)
        {
            try
            {
                BuildWebHost(args).Run();
                return 0;
            }
            catch (Exception ex)
            {
                const string str = "Host terminated unexpectedly";
                Log.Fatal(ex, str);
                Log.CloseAndFlush();
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IWebHost BuildWebHost(string[] args)
        {
            var builder = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var hostingEnvironment = hostingContext.HostingEnvironment;
                    config.AddJsonFile("appsettings.json", true, true).AddJsonFile(
                        string.Format("appsettings.{0}.json", hostingEnvironment.EnvironmentName), true, true);

                    if (hostingEnvironment.IsDevelopment())
                    {
                        var assembly = Assembly.Load(new AssemblyName(hostingEnvironment.ApplicationName));
                        if (assembly != null)
                            config.AddUserSecrets(assembly, true);
                    }

                    config.AddEnvironmentVariables();
                    config.AddDockerSecrets(cfg => { cfg.Optional = true; });
                    if (args != null)
                        config.AddCommandLine(args);
                    var jsonCfg = config.Build();
                    config.AddConsul(jsonCfg["ConsulOptions:ServiceName"], ConsulConfigCancellationTokenSource.Token,
                        opt =>
                        {
                            opt.ReloadOnChange = true;
                            opt.Optional = true;
                            opt.ConsulConfigurationOptions = cfg =>
                            {
                                cfg.Address = new Uri(jsonCfg["ConsulOptions:HttpEndpoint"]);
                                cfg.Datacenter = jsonCfg["ConsulOptions:Datacenter"];
                            };
                        }
                    );
                    InitializeLogs(config.Build());
                })
                .UseKestrel((ctx,options) =>
                {
                    options.AddServerHeader = false;
                })
                .UseDefaultServiceProvider((ctx, opt) => { })
                .UseHealthChecks("/hc")
                .UseStartup<Startup>()
                .UseSerilog();

            return builder.Build();
        }

        private static void InitializeLogs(IConfiguration config)
        {
            var format = "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level}] [" + Environment.MachineName +
                         "] {SourceContext}  [{Address}] [{RequestId}] {Message:lj}{NewLine}{Exception}{NewLine}";
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Default", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Elasticsearch(
                    new ElasticsearchSinkOptions(config["ElasticSearchOptions:Urls"].Split(';')
                        .Select(url => new Uri(url)))
                    {
                        AutoRegisterTemplate = true,
                        BufferBaseFilename = $"logs/{config["ConsulOptions:ServiceName"]}-logs",
                        TypeName = config["ConsulOptions:ServiceName"]
                    })
                .WriteTo.Console(outputTemplate: format)
                .CreateLogger();
        }
    }
}
