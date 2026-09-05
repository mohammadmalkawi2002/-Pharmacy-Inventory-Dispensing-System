using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Commands.CreatePrescription
{
    public sealed class CreatePrescriptionItemCommandValidator
        :AbstractValidator<CreatePrescriptionItemCommand>
    {
        public CreatePrescriptionItemCommandValidator()
        {
            RuleFor(command => command.MedicineId)
                .NotEmpty()
                .WithMessage("MedicineId is required.");

            RuleFor(command => command.QuantityPrescribed)
                .GreaterThan(0)
                .WithMessage("QuantityPrescribed must be greater than 0.");

            RuleFor(command => command.MaxFillCount)
                .GreaterThan(0)
                .WithMessage("MaxFillCount must be greater than 0.");


            RuleFor(command => command.DosageInstructions)
                .MaximumLength(500);
        }
    }
}
