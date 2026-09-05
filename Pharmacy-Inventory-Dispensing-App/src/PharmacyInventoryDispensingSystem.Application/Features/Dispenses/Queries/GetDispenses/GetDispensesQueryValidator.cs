using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Dispenses.Queries.GetDispenses
{
    public sealed class GetDispensesQueryValidator
    : AbstractValidator<GetDispensesQuery>
    {
        public GetDispensesQueryValidator()
        {
            RuleFor(query => query.PageNumber)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than 0.");

            RuleFor(query => query.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("Page size must be between 1 and 100.");

            RuleFor(query => query.SearchTerm)
                .MaximumLength(100)
                .When(query =>
                    !string.IsNullOrWhiteSpace(query.SearchTerm))
                .WithMessage(
                    "Search term must not exceed 100 characters.");

            RuleFor(query => query.ToDate)
                .GreaterThanOrEqualTo(query => query.FromDate)
                .When(query =>
                    query.FromDate.HasValue &&
                    query.ToDate.HasValue)
                .WithMessage(
                    "To date must be greater than or equal to from date.");
        }
    }
}