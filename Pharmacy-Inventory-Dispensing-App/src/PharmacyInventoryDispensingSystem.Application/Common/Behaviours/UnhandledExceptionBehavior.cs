using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Common.Behaviours
{
    public sealed class UnhandledExceptionBehavior<TRequest,TResponse>(ILogger<TRequest> logger)
        :IPipelineBehavior<TRequest,TResponse> where TRequest : notnull
    {

        private readonly ILogger<TRequest> _lgger = logger;

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            try
            {
               return await next(cancellationToken);
            }

            catch (Exception ex) 
            {
                var requestName = typeof(TRequest).Name;
                _lgger.LogError(
                         ex, "Unhandled exception for request {RequestName} {@Request}", 
                         requestName,
                         request);

                throw;
            
            }
        }
    }
}
