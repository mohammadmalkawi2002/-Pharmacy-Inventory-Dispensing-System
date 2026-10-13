using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Common.Behaviours
{
    /// <summary>
    /// before the request reach the handler log the request Name and who send it.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="logger"></param>
    /// <param name="currentUser"></param>
    public sealed class LoggingBehaviour<TRequest>(ILogger<TRequest> logger,ICurrentUser currentUser)
        : IRequestPreProcessor<TRequest> where TRequest : notnull
    {
        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var userId = currentUser.Id;
            var email = currentUser.Email;

            logger.LogInformation(
                "Request: {RequestName} UserId: {UserId} Email: {Email}",
                requestName,
                userId,
                email);

            return Task.CompletedTask;
        }
    }
}
