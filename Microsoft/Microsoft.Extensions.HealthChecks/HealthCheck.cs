// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.HealthChecks {
    public class HealthCheck : IHealthCheck {
        protected HealthCheck(Func<CancellationToken, ValueTask<IHealthCheckResult>> check) {
            Guard.ArgumentNotNull(nameof(check), check);

            Check = check;
        }

        protected Func<CancellationToken, ValueTask<IHealthCheckResult>> Check { get; }

        public ValueTask<IHealthCheckResult> CheckAsync(
            CancellationToken cancellationToken = default(CancellationToken)) {
            return Check(cancellationToken);
        }

        public static HealthCheck FromCheck(Func<IHealthCheckResult> check) {
            return new HealthCheck(token => new ValueTask<IHealthCheckResult>(check()));
        }

        public static HealthCheck FromCheck(Func<CancellationToken, IHealthCheckResult> check) {
            return new HealthCheck(token => new ValueTask<IHealthCheckResult>(check(token)));
        }

        public static HealthCheck FromTaskCheck(Func<Task<IHealthCheckResult>> check) {
            return new HealthCheck(token => new ValueTask<IHealthCheckResult>(check()));
        }

        public static HealthCheck FromTaskCheck(Func<CancellationToken, Task<IHealthCheckResult>> check) {
            return new HealthCheck(token => new ValueTask<IHealthCheckResult>(check(token)));
        }

        public static HealthCheck FromValueTaskCheck(Func<ValueTask<IHealthCheckResult>> check) {
            return new HealthCheck(token => check());
        }

        public static HealthCheck FromValueTaskCheck(Func<CancellationToken, ValueTask<IHealthCheckResult>> check) {
            return new HealthCheck(check);
        }
    }
}