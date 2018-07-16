// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.Extensions.HealthChecks {
    public class HealthCheckResult : IHealthCheckResult {
        private static readonly IReadOnlyDictionary<string, object> _emptyData = new Dictionary<string, object>();

        private HealthCheckResult(CheckStatus checkStatus, string description,
            IReadOnlyDictionary<string, object> data) {
            CheckStatus = checkStatus;
            Description = description;
            Data = data ?? _emptyData;
        }

        public CheckStatus CheckStatus { get; }
        public IReadOnlyDictionary<string, object> Data { get; }
        public string Description { get; }

        public static HealthCheckResult Unhealthy(string description) {
            return new HealthCheckResult(CheckStatus.Unhealthy, description, null);
        }

        public static HealthCheckResult Unhealthy(string description, IReadOnlyDictionary<string, object> data) {
            return new HealthCheckResult(CheckStatus.Unhealthy, description, data);
        }

        public static HealthCheckResult Healthy(string description) {
            return new HealthCheckResult(CheckStatus.Healthy, description, null);
        }

        public static HealthCheckResult Healthy(string description, IReadOnlyDictionary<string, object> data) {
            return new HealthCheckResult(CheckStatus.Healthy, description, data);
        }

        public static HealthCheckResult Warning(string description) {
            return new HealthCheckResult(CheckStatus.Warning, description, null);
        }

        public static HealthCheckResult Warning(string description, IReadOnlyDictionary<string, object> data) {
            return new HealthCheckResult(CheckStatus.Warning, description, data);
        }

        public static HealthCheckResult Unknown(string description) {
            return new HealthCheckResult(CheckStatus.Unknown, description, null);
        }

        public static HealthCheckResult Unknown(string description, IReadOnlyDictionary<string, object> data) {
            return new HealthCheckResult(CheckStatus.Unknown, description, data);
        }

        public static HealthCheckResult FromStatus(CheckStatus status, string description) {
            return new HealthCheckResult(status, description, null);
        }

        public static HealthCheckResult FromStatus(CheckStatus status, string description,
            IReadOnlyDictionary<string, object> data) {
            return new HealthCheckResult(status, description, data);
        }
    }
}