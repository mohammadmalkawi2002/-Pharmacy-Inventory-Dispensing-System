using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Patients.Commands.UpdatePatient
{
    public sealed class UpdatePatientCommandValidator
        :AbstractValidator<UpdatePatientCommand>
    {
        public UpdatePatientCommandValidator()
        {
            RuleFor(command => command.PatientId)
            .NotEmpty()
            .WithMessage("Patient ID is required.");

            RuleFor(command => command.DocumentId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Document ID is required.")
                .Matches(@"^[12][0-9]{9}$")
                .WithMessage(
                    "Document ID must consist of 10 digits and start with 1 or 2.");

            RuleFor(command => command.FullName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Full name is required.")
                .MaximumLength(200)
                .WithMessage(
                    "Full name must not exceed 200 characters.");

            RuleFor(command => command.DateOfBirth)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Date of birth is required.")
                .Must(dateOfBirth =>
                    dateOfBirth.Date <= DateTime.UtcNow.Date)
                .WithMessage(
                    "Date of birth cannot be in the future.");

            RuleFor(command => command.PhoneNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .MaximumLength(16)
                .WithMessage(
                    "Phone number must not exceed 16 characters.")
                .Matches(@"^\+?[0-9]{9,15}$")
                .WithMessage(
                    "Phone number must contain between 9 and 15 digits and may start with '+'.");
        }
    }
}
