using FluentValidation;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Users.Commands.UpdateUser
{
    public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(c => c.UserId)
                .NotEmpty()
                .WithMessage("User ID is required.");

            RuleFor(c => c.FirstName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("First name is required.")
                .MaximumLength(100)
                .WithMessage("First name must not exceed 100 characters.");

            RuleFor(c => c.LastName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Last name is required.")
                .MaximumLength(100)
                .WithMessage("Last name must not exceed 100 characters.");

            RuleFor(c => c.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("A valid email address is required.")
                .MaximumLength(256)
                .WithMessage("Email must not exceed 256 characters.");

            RuleFor(c => c.Role)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Role is required.")
                .Must(IsValidStaffRole)
                .WithMessage(
                    $"Role must be one of: {RoleNames.Receptionist}, {RoleNames.Doctor}, {RoleNames.Pharmacist}.");
        }

        private static bool IsValidStaffRole(string role) =>
            role == RoleNames.Receptionist ||
            role == RoleNames.Doctor ||
            role == RoleNames.Pharmacist;
    }
}
