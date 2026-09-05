using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Commands.UpdatePrescription
{
    public sealed class UpdatePrescriptionItemCommandValidator
        :AbstractValidator<UpdatePrescriptionItemCommand>
    {

        public UpdatePrescriptionItemCommandValidator()
        {

            RuleFor(item => item.MedicineId)
                .NotEmpty();

            RuleFor(item => item.QuantityPrescribed)
                .GreaterThan(0);

            RuleFor(item => item.MaxFillCount)
                .GreaterThan(0);

            RuleFor(item => item.DosageInstructions)
                .MaximumLength(500);

        }
    }
}
