using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Medicines.Queries.LookupMedicines
{
    public sealed class LookupMedicinesQueryValidator
      : AbstractValidator<LookupMedicinesQuery>
    {
        public LookupMedicinesQueryValidator()
        {
            RuleFor(x => x.SearchTerm)
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(100);
        }
    }
}
