using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.Commands.ResetPassword
{
    public sealed class ResetPasswordCommandValidator:AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {

            RuleFor(x => x.Email)
           .NotEmpty()
          .WithMessage("Email is required")
          .EmailAddress()
          .WithMessage("Invalid email");

            RuleFor(x => x.Token)
                .NotEmpty()
                .WithMessage("Token is required");

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage("NewPassword is required")
                .MinimumLength(8);

        }
    }
}
