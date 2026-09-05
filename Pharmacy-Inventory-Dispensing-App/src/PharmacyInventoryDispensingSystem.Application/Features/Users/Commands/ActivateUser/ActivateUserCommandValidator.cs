using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Users.Commands.ActivateUser
{
    public sealed class ActivateUserCommandValidator : AbstractValidator<ActivateUserCommand>
    {
        public ActivateUserCommandValidator()
        {
            RuleFor(c => c.UserId)
                .NotEmpty()
                .WithMessage("User ID is required.");
        }
    }
}
