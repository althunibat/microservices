using Framework.Services.Contracts.Messaging;

namespace SampleApi.V1.Services.Events
{
    public interface IPersonRetrieved:IEvent
    {
        string Name { get; }
    }
}