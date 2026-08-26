using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.Commands.Login
{
    public sealed class LoginCommandValidator:AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {

            RuleFor(x => x.Email).NotEmpty()
          .WithMessage("Email is required")
          .EmailAddress()
          .WithMessage("Invalid email");


            RuleFor(x => x.Password)
               .NotEmpty()
              .WithMessage("Password is required")
             .MinimumLength(8);
        }
    }
}
