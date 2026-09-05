using FluentValidation;
using System;
using System.Collections.Generic;

namespace PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Commands.CreatePrescription
{
    public sealed class CreatePrescriptionCommandValidator:AbstractValidator<CreatePrescriptionCommand>
    {

        public CreatePrescriptionCommandValidator() 
        {
            RuleFor(command => command.PatientId)
               .NotEmpty()
               .WithMessage("PatientId is required.");


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
                .Must(items=>items.GroupBy(item => item.MedicineId)
                .All(group => group.Count() == 1)
                )
                .WithMessage("Each medicine can only appear once in a prescription.");


            RuleForEach(command => command.Items)
                .SetValidator(new CreatePrescriptionItemCommandValidator());
        }
    }
}
