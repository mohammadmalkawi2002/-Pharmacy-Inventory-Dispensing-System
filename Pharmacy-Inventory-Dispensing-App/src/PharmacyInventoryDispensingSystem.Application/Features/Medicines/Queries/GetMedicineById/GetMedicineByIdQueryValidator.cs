using FluentValidation;
using System;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Queries.GetMedicineById
{
    public sealed class GetMedicineByIdQueryValidator : AbstractValidator<GetMedicineByIdQuery>
    {
        public GetMedicineByIdQueryValidator()
        {
            RuleFor(query => query.MedicineId)
                .NotEmpty()
                .WithMessage("Medicine Id is required.");
        }
    }
}
