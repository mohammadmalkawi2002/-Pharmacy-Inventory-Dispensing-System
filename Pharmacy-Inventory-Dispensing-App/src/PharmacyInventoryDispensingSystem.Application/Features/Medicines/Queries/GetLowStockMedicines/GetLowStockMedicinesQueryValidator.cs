using FluentValidation;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Queries.GetLowStockMedicines
{
    public sealed class GetLowStockMedicinesQueryValidator : AbstractValidator<GetLowStockMedicinesQuery>
    {
        public GetLowStockMedicinesQueryValidator()
        {
            RuleFor(query => query.PageNumber)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than 0.");

            RuleFor(query => query.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("Page size must be between 1 and 100.");
        }
    }
}
