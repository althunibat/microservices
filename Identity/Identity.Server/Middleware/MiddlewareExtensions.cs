using System;
using Identity.Server.Options;
using Microsoft.AspNetCore.Builder;

namespace Identity.Server.Middleware {
    public static class MiddlewareExtensions {
        public static IApplicationBuilder UseRemoteIpAddressLoggingMiddleware(this IApplicationBuilder builder) {
            return builder.UseMiddleware<RemoteIpAddressLoggingMiddleware>();
        }
        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app) {
            if (app == null) {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<CorrelationIdMiddleware>();
        }

        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app, string header) {
            if (app == null) {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseCorrelationId(new CorrelationIdOptions {
                Header = header
            });
        }

        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app, CorrelationIdOptions options) {
            if (app == null) {
                throw new ArgumentNullException(nameof(app));
            }
            if (options == null) {
                throw new ArgumentNullException(nameof(options));
            }
            return app.UseMiddleware<CorrelationIdMiddleware>(Microsoft.Extensions.Options.Options.Create(options));
        }
    }
}