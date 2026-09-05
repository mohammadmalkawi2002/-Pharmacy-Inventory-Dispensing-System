using FluentValidation;
using PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Commands.CreatePrescription;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Commands.UpdatePrescription
{
    public sealed class UpdatePrescriptionCommandValidator
    : AbstractValidator<UpdatePrescriptionCommand>

    {
        public UpdatePrescriptionCommandValidator()
        {
            RuleFor(command => command.PrescriptionId)
            .NotEmpty()
            .WithMessage("Prescription ID must not be empty.");

            RuleFor(command => command.ValidFrom)
               .NotEmpty()
               .WithMessage("ValidFrom date is required.");

            RuleFor(command => command.ValidTo)
                .NotEmpty()
                .WithMessage("ValidTo date is required.")
                .GreaterThanOrEqualTo(command => command.ValidFrom)
                .WithMessage("ValidTo must be on or after ValidFrom.");


            RuleFor(command => command.Notes)
            .MaximumLength(500);

            RuleFor(command => command.Items)
                .NotEmpty()
                .WithMessage("A prescription must contain at least one medicine.");


            // Ensure that each medicine appears only once in the prescription:
            RuleFor(command => command.Items)
                .Must(items => items.GroupBy(item => item.MedicineId)
                .All(group => group.Count() == 1)
                )
                .WithMessage("Each medicine can only appear once in a prescription.");


            RuleForEach(command => command.Items)
                .SetValidator(new UpdatePrescriptionItemCommandValidator());
        }
    }
}
