using MediatR;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Application.Features.Users.Dtos;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.Users.Queries.GetUserById
{
    public sealed class GetUserByIdQueryHandler(IStaffUserService staffUserService)
        : IRequestHandler<GetUserByIdQuery, Result<StaffUserDto>>
    {
        public Task<Result<StaffUserDto>> Handle(
            GetUserByIdQuery request,
            CancellationToken cancellationToken)
        {
            return staffUserService.GetByIdAsync(request.UserId, cancellationToken);
        }
    }
}
