using System;
using System.Threading;
using System.Threading.Tasks;
using Framework.Common;
using Framework.Model.Data;
using Framework.Services.Contracts.Http;
using Framework.Services.Core;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SampleApi.V1.Data.Model;
using SampleApi.V1.Services.Events;
using SampleApi.V1.Services.Responses;
using SampleApi.V1.Services.Responses.Dto;

namespace SampleApi.V1.Services {
    public class PersonService : ServiceBase<PersonService>, IPersonService {
        private readonly IReadOnlyRepository<Person, long> _repository;

        public PersonService(ILogger<PersonService> logger, IBus bus, IHttpContextAccessor httpAccessor,
            IReadOnlyRepository<Person, long> repository) : base(logger, bus, httpAccessor) {
            _repository = repository;
        }

        public async Task<IResponse> GetById(ByIdRequest<long> request) {
            Logger.LogInformation(SharedResource.BeginMethodExecution,
                GetCallingMember());
            if (request == null || request.Id == default(long)) {
                var error = new Error(1001, SharedResource.InvalidArgument);
                Logger.LogWarning(SharedResource.InvalidArgumentLog, GetCallingMember());
                Logger.LogInformation(SharedResource.EndMethodExecution, GetCallingMember());
                return new FailureResponse(new[] {error});
            }

            try {
                var result = await _repository
                    .GetSingleBy(x => x.Id == request.Id, x => new PersonDto(x), includes: x => x.Addresses)
                    .ConfigureAwait(false);
                if (result == null) {
                    var error = new Error(1001, SharedResource.RecordNotFound) {
                        Description = string.Concat("Id", ":", request.Id)
                    };
                    Logger.LogWarning(SharedResource.RecordNotFoundLog, GetCallingMember());
                    Logger.LogInformation(SharedResource.EndMethodExecution, GetCallingMember());
                    return new FailureResponse(new[] {error});
                }

                await Bus.Publish<IPersonRetrieved>(
                    new PersonRetrieved(GetRequestId(), GetRemoteIp(), result.Name),
                    CancellationToken.None).ConfigureAwait(false);
                var response =
                    new PersonResponse(result);
                Logger.LogInformation(SharedResource.EndMethodExecution, GetCallingMember());
                return response;
            }
            catch (Exception e) {
                return HandleException(e);
            }
        }

        private class PersonRetrieved : IPersonRetrieved {
            public PersonRetrieved(string correlationId, string remoteIp, string name) {
                CorrelationId = correlationId;
                RemoteIp = remoteIp;
                Name = name;
                Id = NewId.NextGuid();
                TimeStamp = DateTimeOffset.UtcNow;
                Version = 1;
            }

            public string CorrelationId { get; }
            public Guid Id { get; }
            public DateTimeOffset TimeStamp { get; }
            public int Version { get; }
            public string RemoteIp { get; }
            public string Name { get; }
        }
    }
}