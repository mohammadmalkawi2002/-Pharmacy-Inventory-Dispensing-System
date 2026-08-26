using FluentValidation;
using MediatR;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.Domain.Common.Results.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Common.Behaviours
{

    public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators = null)
        : IPipelineBehavior<TRequest, TResponse>
            where TRequest : IRequest<TResponse>
            where TResponse : IResult
    {
        private readonly IEnumerable<IValidator<TRequest>>? _validators = validators;

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken ct)
        {
            //1 if you do not have any validator ==> stop do not complete

            if (!_validators.Any())
            {
                return await next(ct);
            }

          //here: means you have validator => encapsulate the req inside a context that fluent validation understand it :

            var context =new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
            _validators.Select(v =>
                v.ValidateAsync(context, ct)));

            var errors = validationResults
                .SelectMany(result => result.Errors)
                .Where(error => error is not null)
                .Select(error => Error.Validation(
                    code: error.PropertyName,
                    description: error.ErrorMessage))
                .ToList();

            if (errors.Count == 0)
            {
                return await next(ct);
            }

            return (dynamic)errors;

        }
    }
}
