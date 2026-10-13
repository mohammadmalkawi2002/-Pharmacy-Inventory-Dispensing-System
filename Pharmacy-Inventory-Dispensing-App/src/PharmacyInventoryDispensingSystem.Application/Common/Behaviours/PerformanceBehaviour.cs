using MediatR;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Common.Behaviours
{
    public sealed class PerformanceBehaviour<TRequest, TResponse>(ILogger<TRequest> logger,ICurrentUser currentUser)
        : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private const long SlowRequestThresholdMilliseconds = 500;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var timer = new Stopwatch();
            timer.Start();

            var resposne = await next();

            timer.Stop();

            var elapsedMilliseconds = timer.ElapsedMilliseconds;

            if(elapsedMilliseconds > SlowRequestThresholdMilliseconds) 
            {
                var requestName = typeof(TRequest).Name;
                var userId = currentUser.Id;
                var email = currentUser.Email;

                logger.LogWarning(
                    "Long Running Request: {RequestName} ({ElapsedMilliseconds} ms) UserId: {UserId} Email: {Email}",
                    requestName,
                    elapsedMilliseconds,
                    userId,
                    email);

            }

            return resposne;

        }
    }
}
