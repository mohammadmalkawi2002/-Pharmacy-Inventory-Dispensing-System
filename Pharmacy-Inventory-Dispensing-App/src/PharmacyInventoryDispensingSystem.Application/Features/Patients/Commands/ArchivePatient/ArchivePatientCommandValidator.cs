using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Patients.Commands.ArchivePatient
{
    public sealed class ArchivePatientCommandValidator:AbstractValidator<ArchivePatientCommand>
    {
        public ArchivePatientCommandValidator()
        {
            RuleFor(command => command.PatientId)
                .NotEmpty()
                .WithMessage("Patient ID is required.");
        }
    }
}
