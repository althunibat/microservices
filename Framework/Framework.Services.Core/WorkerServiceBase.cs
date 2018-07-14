using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FluentValidation.Results;
using Framework.Common;
using Framework.Services.Contracts.Messaging;
using Microsoft.Extensions.Logging;

namespace Framework.Services.Core
{
    public abstract class WorkerServiceBase<TService>
    where TService: WorkerServiceBase<TService>
    {
        protected readonly ILogger Logger;

        protected WorkerServiceBase(ILogger<TService> logger)
        {
            Logger = logger;
        }

        protected static string GetCallingMember([CallerMemberName] string member = "") {
            return member;
        }

        protected string GetRemoteIp(IMessage msg) {
            return msg.RemoteIp;
        }
        protected string GetRequestId(IMessage msg)
        {
            return msg.CorrelationId;
        }
        
        protected IEnumerable<Error> HandleValidationErrors(ValidationResult result) {
            Logger.LogInformation(SharedResource.InvalidArgumentLog, GetCallingMember());
            return result.Errors
                .Select(e =>
                    new Error(1001, e.ErrorMessage)
                    {
                        Description = string.Concat(e.PropertyName, ":", e.AttemptedValue)
                    }).ToList();

        }

        protected IEnumerable<Error> HandleException(Exception e) {
            Logger.LogCritical(SharedResource.UnexpectedErrorLog,
                GetCallingMember(), e);
            var error = new Error(1001, SharedResource.UnexpectedError);
            return new List<Error> { error };
        }

    }
}