using System;
using Newtonsoft.Json;

namespace Framework.Services.Contracts.Http
{
    public class ByIdRequest<TId>: DefaultRequest where TId: IEquatable<TId> {
        [JsonConstructor]
        public ByIdRequest(TId id) {
            Id = id;
        }
        public TId Id { get; }
    }
}