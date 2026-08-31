using FluentValidation;
using System;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Queries.GetMedicines
{
    public sealed class GetMedicinesQueryValidator : AbstractValidator<GetMedicinesQuery>
    {
        public GetMedicinesQueryValidator()
        {
            RuleFor(query => query.PageNumber)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than 0.");

            RuleFor(query => query.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("Page size must be between 1 and 100.");

            RuleFor(query => query.SortBy)
                .Must(BeValidSortField)
                .WithMessage("SortBy must be CreatedAtUtc or QuantityInStock.");

            RuleFor(query => query.Form)
                .IsInEnum()
                .When(query=>query.Form.HasValue)
                .WithMessage("The provided medicine form is invalid.");


            RuleFor(query => query.StockUnit)
                 .IsInEnum()
                 .When(query => query.StockUnit.HasValue)
            .WithMessage("The provided stock unit is invalid.");
        }

        private static bool BeValidSortField(string? sortBy)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return true;
            }

            string normalizedSortBy = sortBy.Trim();

            return normalizedSortBy.Equals("CreatedAtUtc", StringComparison.OrdinalIgnoreCase) ||
                   normalizedSortBy.Equals("QuantityInStock", StringComparison.OrdinalIgnoreCase);
        }
    }
}
