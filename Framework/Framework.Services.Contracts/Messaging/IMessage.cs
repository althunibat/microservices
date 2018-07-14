using System;

namespace Framework.Services.Contracts.Messaging
{
    public interface  IMessage
    {
        string CorrelationId { get; }
        Guid Id { get; }
        DateTimeOffset TimeStamp { get; }
        int Version { get; }
        string RemoteIp { get; }
    }
}
