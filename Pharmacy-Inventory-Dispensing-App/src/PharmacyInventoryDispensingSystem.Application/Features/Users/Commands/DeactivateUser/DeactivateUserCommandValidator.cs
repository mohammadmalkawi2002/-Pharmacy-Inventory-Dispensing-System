using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Users.Commands.DeactivateUser
{
    public sealed class DeactivateUserCommandValidator : AbstractValidator<DeactivateUserCommand>
    {
        public DeactivateUserCommandValidator()
        {
            RuleFor(c => c.UserId)
                .NotEmpty()
                .WithMessage("User ID is required.");
        }
    }
}
