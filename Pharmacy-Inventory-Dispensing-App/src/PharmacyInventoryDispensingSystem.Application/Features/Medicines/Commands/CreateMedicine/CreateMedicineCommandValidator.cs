using FluentValidation;
using PharmacyInventoryDispensingSystem.Domain.Enums;
using System;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Commands.CreateMedicine
{
    public sealed class CreateMedicineCommandValidator : AbstractValidator<CreateMedicineCommand>
    {
        public CreateMedicineCommandValidator()
        {
            RuleFor(command => command.Code)
        .Cascade(CascadeMode.Stop)
             .NotEmpty()
            .WithMessage("Medicine code is required.")
            .MaximumLength(15)
            .WithMessage("Medicine code must not exceed 15 characters.")
            .Matches(@"^\d+$")
            .WithMessage("Medicine code must contain digits only.");

            RuleFor(command => command.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Medicine name is required.")
                .MaximumLength(100)
                .WithMessage("Medicine name must not exceed 100 characters.");

            RuleFor(command => command.Strength)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Medicine strength is required.")
                .MaximumLength(50)
                .WithMessage("Medicine strength must not exceed 50 characters.");

            RuleFor(command => command.Form)
                .IsInEnum()
                .WithMessage("The provided medicine form is invalid.");

            RuleFor(command => command.Form)
              .IsInEnum()
              .WithMessage("The provided medicine form is invalid.");

            RuleFor(command => command.StockUnit)
                .IsInEnum()
                .WithMessage("The provided stock unit is invalid.");

            RuleFor(command => command.PackageUnit)
                .IsInEnum()
                .WithMessage("The provided package unit is invalid.");

            RuleFor(command => command.UnitsPerPackage)
                .GreaterThan(0)
                .WithMessage("Units per package must be greater than zero.");

            RuleFor(command => command.ReorderLevel)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Reorder level cannot be negative.");
        }
    }
}
