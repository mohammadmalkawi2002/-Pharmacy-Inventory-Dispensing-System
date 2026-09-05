using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Users.Queries.GetUserById
{
    public sealed class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
    {
        public GetUserByIdQueryValidator()
        {
            RuleFor(q => q.UserId)
                .NotEmpty()
                .WithMessage("User ID is required.");
        }
    }
}
