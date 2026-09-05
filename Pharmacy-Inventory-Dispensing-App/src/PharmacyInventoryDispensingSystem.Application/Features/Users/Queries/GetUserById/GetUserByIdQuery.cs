using MediatR;
using PharmacyInventoryDispensingSystem.Application.Features.Users.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Users.Queries.GetUserById
{
    public sealed record GetUserByIdQuery(string UserId)
        : IRequest<Result<StaffUserDto>>;
}
