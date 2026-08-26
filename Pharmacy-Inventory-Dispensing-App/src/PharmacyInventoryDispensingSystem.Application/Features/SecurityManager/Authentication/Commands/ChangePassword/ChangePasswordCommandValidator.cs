using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.Commands.ChangePassword
{
    public sealed class ChangePasswordCommandValidator:AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.CurrentPassword)
                  .NotEmpty()
                  .WithMessage("CurrentPassword is required");



            RuleFor(x => x.NewPassword)
                 .NotEmpty()
                 .WithMessage("NewPassword is required")
                 .MinimumLength(8)
                 .NotEqual(x=>x.CurrentPassword)
                 .WithMessage("New password must be different from the current password.");



        }
    }
}
