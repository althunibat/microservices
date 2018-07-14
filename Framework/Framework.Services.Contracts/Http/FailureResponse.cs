using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Common;

namespace Framework.Services.Contracts.Http {
    public class FailureResponse : IResponse {
        public FailureResponse(ICollection<Error> errors) {
            if (errors == null || !errors.Any())
                throw new ArgumentOutOfRangeException(nameof(errors));
            Errors = errors;
            TimeStamp = DateTimeOffset.Now;
        }
        public DateTimeOffset TimeStamp { get; }

        public IEnumerable<Error> Errors { get; }

        public bool Success => false;
    }
}