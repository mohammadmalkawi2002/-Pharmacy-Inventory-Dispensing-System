using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Patients.Commands.RestorePatient
{
    public sealed class RestorePatientCommandValidator
        :AbstractValidator<RestorePatientCommand>
    {
        public RestorePatientCommandValidator()
        {
            RuleFor(command => command.PatientId)
                .NotEmpty()
                .WithMessage("Patient ID is required.");



        }
    }
}
