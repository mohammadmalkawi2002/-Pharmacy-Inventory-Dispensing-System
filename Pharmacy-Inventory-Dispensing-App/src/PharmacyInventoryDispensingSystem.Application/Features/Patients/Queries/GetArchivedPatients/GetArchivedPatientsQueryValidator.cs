using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Patients.Queries.GetArchivedPatients
{
    public sealed class GetArchivedPatientsQueryValidator
    : AbstractValidator<GetArchivedPatientsQuery>
    {
        public GetArchivedPatientsQueryValidator()
        {
          

            RuleFor(query => query.DocumentType)
                .Must(documentType =>
                    documentType is null ||
                    Enum.IsDefined(documentType.Value))
                .WithMessage(
                    "Document type must be Citizen or Resident.");
           
            RuleFor(query => query.SortBy)
                .Must(BeAValidSortField)
                .WithMessage(
                    "SortBy must be FullName or CreatedAtUtc.");
           
            RuleFor(query => query.PageNumber)
                .GreaterThan(0);

            RuleFor(query => query.PageSize)
                .InclusiveBetween(1, 100);
           
        }

        private static bool BeAValidSortField(string? sortBy)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return true;
               
            }

            string normalizedSortBy = sortBy.Trim();

            return normalizedSortBy.Equals(
                       "FullName",
                       StringComparison.OrdinalIgnoreCase) ||
                   normalizedSortBy.Equals(
                       "CreatedAtUtc",
                       StringComparison.OrdinalIgnoreCase);
        }
    }
}
