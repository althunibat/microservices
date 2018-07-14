using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Framework.Api.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Winton.Extensions.Configuration.Consul;

namespace Framework.Api
{
    public static class ProgramBase<TStart> where TStart : StartupBase<TStart>
    {
        public static readonly CancellationTokenSource ConsulConfigCancellationTokenSource = new CancellationTokenSource();
        public static int Run(string[] args)
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
                .UseKestrel(options => options.AddServerHeader = false)
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
                    config.AddDockerSecrets(cfg => { cfg.Optional = true;});
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
                .UseDefaultServiceProvider((ctx, opt) => { })
                .UseHealthChecks("/hc")
                .UseStartup<TStart>()
                .UseSerilog();

            return builder.Build();
        }

        private static void InitializeLogs(IConfiguration config)
        {
            var format = "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level}] [" + Environment.MachineName +
                         "] {SourceContext}  [{Address}] [{RequestId}] {Message:lj}{NewLine}{Exception}{NewLine}";
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: format)
                .CreateLogger();
        }
    }
}