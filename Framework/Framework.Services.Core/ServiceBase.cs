using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FluentValidation.Results;
using Framework.Common;
using Framework.Services.Contracts.Http;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Framework.Services.Core {
    public abstract class ServiceBase<TService>
        where TService : ServiceBase<TService> {
        protected readonly IBus Bus;
        protected readonly IHttpContextAccessor HttpAccessor;
        protected readonly ILogger Logger;

        protected ServiceBase(ILogger<TService> logger, IBus bus, IHttpContextAccessor httpAccessor) {
            Logger = logger;
            Bus = bus;
            HttpAccessor = httpAccessor;
        }

        protected static string GetCallingMember([CallerMemberName] string member = "") {
            return member;
        }

        protected string GetRemoteIp() {
            return HttpAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
        }

        protected string GetRequestId() {
            return HttpAccessor.HttpContext.TraceIdentifier;
        }

        protected FailureResponse HandleValidationErrors(ValidationResult result) {
            Logger.LogWarning(SharedResource.InvalidArgumentLog, GetCallingMember());
            Logger.LogInformation(SharedResource.EndMethodExecution, GetCallingMember());
            return new FailureResponse(result.Errors
                .Select(e =>
                    new Error(1001, e.ErrorMessage) {
                        Description = string.Concat(e.PropertyName, ":", e.AttemptedValue)
                    }).ToList());
        }

        protected FailureResponse HandleException(Exception e) {
            Logger.LogCritical(SharedResource.UnexpectedErrorLog,
                GetCallingMember(), e);
            Logger.LogInformation(SharedResource.EndMethodExecution, GetCallingMember());
            var error = new Error(1001, SharedResource.UnexpectedError);
            return new FailureResponse(new List<Error> {error});
        }
    }
}