using MediatR;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.DTOs;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.Queries
{
    public sealed class GetCurrentUserQueryHandler(IIdentityService identityService) : IRequestHandler<GetCurrentUserQuery, Result<CurrentUserResponse>>
    {
        public Task<Result<CurrentUserResponse>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            return identityService.GetCurrentUserAsync(cancellationToken);
        }
    }
}
