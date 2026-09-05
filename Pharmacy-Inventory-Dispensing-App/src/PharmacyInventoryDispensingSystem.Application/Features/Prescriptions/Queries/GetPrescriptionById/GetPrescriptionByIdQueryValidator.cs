using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Queries.GetPrescriptionById
{
    public sealed class GetPrescriptionByIdQueryValidator
    : AbstractValidator<GetPrescriptionByIdQuery>
    {
        public GetPrescriptionByIdQueryValidator()
        {
            RuleFor(x => x.PrescriptionId)
                .NotEmpty()
                .WithMessage("Prescription ID must not be empty.");
        }
    }
}
