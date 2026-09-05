using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Prescriptions.Queries.LookupPrescription
{
    public sealed class LookupPrescriptionQueryValidator
    : AbstractValidator<LookupPrescriptionQuery>
    {
        public LookupPrescriptionQueryValidator()
        {
            RuleFor(query => query.PrescriptionNumber)
                .NotEmpty()
                .Matches(@"^RX-\d{6}$");

            RuleFor(query => query.DocumentId)
                .NotEmpty()
                .Matches(@"^[12][0-9]{9}$");
        }
    }
}
