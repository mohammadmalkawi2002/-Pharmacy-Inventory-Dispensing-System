using MediatR;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Application.Common.Models;
using PharmacyInventoryDispensingSystem.Application.Features.Users.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Users.Queries.GetUsers
{
    public sealed class GetUsersQueryHandler(IStaffUserService staffUserService)
        : IRequestHandler<GetUsersQuery, Result<PaginatedList<StaffUserDto>>>
    {
        public Task<Result<PaginatedList<StaffUserDto>>> Handle(
            GetUsersQuery request,
            CancellationToken cancellationToken)
        {
            return staffUserService.GetPagedAsync(
                request.SearchTerm,
                request.Role,
                request.PageNumber,
                request.PageSize,
                cancellationToken);
        }
    }
}
