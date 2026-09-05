using MediatR;
using PharmacyInventoryDispensingSystem.Application.Common.Models;
using PharmacyInventoryDispensingSystem.Application.Features.Users.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Users.Queries.GetUsers
{
    public sealed record GetUsersQuery(
        string? SearchTerm = null,
        string? Role = null,
        int PageNumber = 1,
        int PageSize = 10)
        : IRequest<Result<PaginatedList<StaffUserDto>>>;
}
