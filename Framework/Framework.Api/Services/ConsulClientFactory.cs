using System;
using Consul;

namespace Framework.Api.Services {
    public class ConsulClientFactory {
        private static readonly object Lock = new object();
        private static IConsulClient _instance;

        private ConsulClientFactory() { }

        public static IConsulClient Instance(Action<ConsulClientConfiguration> cfg) {
            lock (Lock) {
                return _instance ?? (_instance = new ConsulClient(cfg));
            }
        }
    }
}