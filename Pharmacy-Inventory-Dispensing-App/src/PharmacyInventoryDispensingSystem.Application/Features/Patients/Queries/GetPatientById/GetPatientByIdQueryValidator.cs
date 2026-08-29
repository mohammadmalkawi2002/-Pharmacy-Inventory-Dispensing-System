using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Patients.Queries.GetPatientById
{
    public sealed class GetPatientByIdQueryValidator:AbstractValidator<GetPatientByIdQuery>
    {
        public GetPatientByIdQueryValidator()
        {
            RuleFor(x => x.PatientId)
                .NotEmpty()
                .WithMessage("Patient Id is required.");
                    

        }
    }
}
