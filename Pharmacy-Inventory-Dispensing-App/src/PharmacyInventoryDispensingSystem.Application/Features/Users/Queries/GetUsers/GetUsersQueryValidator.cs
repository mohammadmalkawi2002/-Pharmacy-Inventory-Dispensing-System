using FluentValidation;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Users.Queries.GetUsers
{
    public sealed class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
    {
        public GetUsersQueryValidator()
        {
            RuleFor(q => q.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page number must be at least 1.");

            RuleFor(q => q.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("Page size must be between 1 and 100.");

            RuleFor(q => q.Role)
                .Must(role => role is null || IsValidStaffRole(role))
                .WithMessage(
                    $"Role must be one of: {RoleNames.Receptionist}, {RoleNames.Doctor}, {RoleNames.Pharmacist}.");
        }

        private static bool IsValidStaffRole(string role) =>
            role == RoleNames.Receptionist ||
            role == RoleNames.Doctor ||
            role == RoleNames.Pharmacist;
    }
}
