using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Framework.Api.Services
{
    public class MasstransitHostedService : Microsoft.Extensions.Hosting.IHostedService
    {
        private BusHandle _busHandle;
        private readonly IBusControl _busControl;
        private readonly ILogger _logger;
        public MasstransitHostedService(IBusControl busControl, ILoggerFactory factory)
        {
            _busControl = busControl;
            _logger = factory.CreateLogger<MasstransitHostedService>();
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Masstransit Esb Started Sucessfully");
            _busHandle = await _busControl.StartAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Masstransit Esb Stopped Sucessfully");
            await _busHandle.StopAsync(cancellationToken);
        }
    }
}