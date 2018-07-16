using System;

namespace Framework.Services.Contracts.Http {
    public class DefaultRequest {
        public string RequesterId { get; private set; }

        public void SetRequesterId(string requesterId) {
            if (string.IsNullOrWhiteSpace(requesterId))
                throw new ArgumentOutOfRangeException(nameof(requesterId));
            RequesterId = requesterId;
        }
    }
}