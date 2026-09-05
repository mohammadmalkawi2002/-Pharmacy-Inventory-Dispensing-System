using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Patients.Queries.LookupPatients
{
    public sealed class LookupPatientsQueryValidator
      : AbstractValidator<LookupPatientsQuery>
    {
        public LookupPatientsQueryValidator()
        {
            RuleFor(x => x.SearchTerm)
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(100);
        }
    }
}
