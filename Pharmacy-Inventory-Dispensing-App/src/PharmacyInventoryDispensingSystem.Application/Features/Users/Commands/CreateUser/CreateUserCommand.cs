using MediatR;
using PharmacyInventoryDispensingSystem.Application.Features.Users.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Users.Commands.CreateUser
{
    public sealed record CreateUserCommand(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        string Role)
        : IRequest<Result<StaffUserDto>>;
}
