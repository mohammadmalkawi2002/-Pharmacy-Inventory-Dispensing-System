using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Commands.CancelPrescription
{
    public sealed class CancelPrescriptionCommandValidator
     : AbstractValidator<CancelPrescriptionCommand>
    {
        public CancelPrescriptionCommandValidator()
        {
            RuleFor(command => command.PrescriptionId)
                .NotEmpty();
        }
    }
}
