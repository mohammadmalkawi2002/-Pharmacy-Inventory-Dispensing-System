using FluentValidation;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.Commands.Register
{
    public sealed class RegisterUserCommandValidator:AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
                 RuleFor(x => x.Email)
                .NotEmpty()
               .WithMessage("Email is required")
               .EmailAddress()
               .WithMessage("Invalid email");


            RuleFor(x => x.Password)
               .NotEmpty()
               .WithMessage("Password is required")
               .MinimumLength(8);

            RuleFor(x => x.FirstName)
               .NotEmpty()
               .WithMessage("FirstName is required");


            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("LastName is required");
                

            RuleFor(x => x.Role)
                .NotEmpty()
                .WithMessage("Role is required");

            RuleFor(x => x.Role)
                .Must(role => RoleNames.All.Contains(role))
                    .WithMessage("Invalid role.");



        }
    }
}
