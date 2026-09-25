using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Queries.ExportPrescriptionPdf
{
    public sealed class ExportPrescriptionPdfQueryValidator
        :AbstractValidator<ExportPrescriptionPdfQuery>
    {
        public ExportPrescriptionPdfQueryValidator()
        {
            RuleFor(p => p.PrescriptionId)
                .NotEmpty()
                .WithMessage("PrescriptionId is required");
        }
    }
}
