using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Identity.Server.Middleware {
    public class RemoteIpAddressLoggingMiddleware {
        public RemoteIpAddressLoggingMiddleware(RequestDelegate next) {
            _next = next;
        }

        private readonly RequestDelegate _next;

        public async Task Invoke(HttpContext context) {
            using (LogContext.PushProperty("Address", context.Connection.RemoteIpAddress)) {
                await _next.Invoke(context).ConfigureAwait(false);
            }
        }

    }
}