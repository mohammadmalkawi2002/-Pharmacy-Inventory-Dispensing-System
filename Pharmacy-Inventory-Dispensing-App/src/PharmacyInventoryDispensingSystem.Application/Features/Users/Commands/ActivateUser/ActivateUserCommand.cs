using MediatR;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Users.Commands.ActivateUser
{
    public sealed record ActivateUserCommand(string UserId)
        : IRequest<Result<Updated>>;
}
