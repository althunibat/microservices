using System;

namespace Framework.Services.Contracts.Http {
    public class SuccessResponse : IResponse {
        public SuccessResponse() {
            TimeStamp = DateTimeOffset.Now;
        }

        public DateTimeOffset TimeStamp { get; }
        public bool Success => true;
    }
}