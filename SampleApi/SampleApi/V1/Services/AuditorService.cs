using System.Threading.Tasks;
using Framework.Services.Core;
using MassTransit;
using Microsoft.Extensions.Logging;
using Nest;
using SampleApi.V1.Services.Events;

namespace SampleApi.V1.Services
{
    public class AuditorService: WorkerServiceBase<AuditorService>, IConsumer<IPersonRetrieved>
    {
        private readonly ElasticClient _client;
        public AuditorService(ILogger<AuditorService> logger, ElasticClient client) : base(logger)
        {
            _client = client;
        }

        public async Task Consume(ConsumeContext<IPersonRetrieved> context)
        {
            var msg = context.Message;
            Logger.LogInformation($"auditing msg {msg.Id} of type {typeof(IPersonRetrieved)}");
            await _client.IndexDocumentAsync(msg).ConfigureAwait(false);
        }
    }
}