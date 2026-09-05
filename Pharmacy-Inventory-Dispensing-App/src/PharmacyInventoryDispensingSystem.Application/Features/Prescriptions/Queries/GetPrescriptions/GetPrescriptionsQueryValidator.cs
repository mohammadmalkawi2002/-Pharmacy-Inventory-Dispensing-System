using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Queries.GetPrescriptions
{
    public sealed class GetPrescriptionsQueryValidator
      : AbstractValidator<GetPrescriptionsQuery>
    {

        public GetPrescriptionsQueryValidator()
        {
            RuleFor(query => query.PageNumber)
           .GreaterThan(0);

            RuleFor(query => query.PageSize)
                .InclusiveBetween(1, 100);

            RuleFor(query => query.SortBy)
            .Must(BeValidSortField)
            .WithMessage(
                "SortBy must be one of: CreatedAtUtc, PrescriptionNumber, ValidFrom, ValidTo.");
        }


        private bool BeValidSortField(string? sortBy)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return true;
            }

            string normalizedSortBy = sortBy.Trim();

            return normalizedSortBy.Equals(
                       "CreatedAtUtc",
                       StringComparison.OrdinalIgnoreCase) ||
                   normalizedSortBy.Equals(
                       "PrescriptionNumber",
                       StringComparison.OrdinalIgnoreCase) ||
                   normalizedSortBy.Equals(
                       "ValidFrom",
                       StringComparison.OrdinalIgnoreCase) ||
                   normalizedSortBy.Equals(
                       "ValidTo",
                       StringComparison.OrdinalIgnoreCase);
        }
    }
}
