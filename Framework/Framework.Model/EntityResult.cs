using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Common;

namespace Framework.Model {
    public class EntityResult {
        private EntityResult(params Error[] errors) {
            if(errors is null || !errors.Any())
                throw new ArgumentOutOfRangeException(nameof(errors));
            Errors = errors;
        }

        private EntityResult() {
            Succeeded = true;
        }

        /// <summary>
        ///     Flag indicating whether if the operation succeeded or not.
        /// </summary>
        /// <value>True if the operation succeeded, otherwise false.</value>
        public bool Succeeded { get; }

        public IEnumerable<Error> Errors { get; }

        public static EntityResult Success { get; } = new EntityResult();

        public static EntityResult Failed(params Error[] errors) {
            return new EntityResult(errors);
        }
    }
}