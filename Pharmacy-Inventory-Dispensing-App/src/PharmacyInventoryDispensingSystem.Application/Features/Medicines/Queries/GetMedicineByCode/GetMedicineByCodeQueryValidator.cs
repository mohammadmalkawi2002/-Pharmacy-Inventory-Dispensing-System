using FluentValidation;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Queries.GetMedicineByCode
{
    public sealed class GetMedicineByCodeQueryValidator : AbstractValidator<GetMedicineByCodeQuery>
    {
        public GetMedicineByCodeQueryValidator()
        {
            RuleFor(query => query.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Medicine code is required.")
                .MaximumLength(15)
                .WithMessage("Medicine code must not exceed 15 characters.");
        }
    }
}
