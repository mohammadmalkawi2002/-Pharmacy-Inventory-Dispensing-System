using MediatR;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Users.Commands.UpdateUser
{
    public sealed record UpdateUserCommand(
        string UserId,
        string FirstName,
        string LastName,
        string Email,
        string Role)
        : IRequest<Result<Updated>>;
}
